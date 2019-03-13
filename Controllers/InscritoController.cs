using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using RestAPI.Models;
using Oracle.ManagedDataAccess.Client;
using System;

namespace RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscritoController : ControllerBase
    {
        private readonly InscritoContext _context;

        public InscritoController(InscritoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Inscrito>> GetAll()
        {
            List<Inscrito> inscrito = new List<Inscrito>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[2];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM INSCRITO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                inscrito.Add(new Inscrito { RUT_ALUMNO = objects[0].ToString(), CODIGO_CURSO = objects[1].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return inscrito;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetInscrito")]
        public ActionResult<List<Inscrito>> GetById(long id)
        {
            IEnumerable<Inscrito> inscritosSelect = this.GetAll().Value.Where(l => l.RUT_ALUMNO == id.ToString()).Select(l => new Inscrito { RUT_ALUMNO = l.RUT_ALUMNO, CODIGO_CURSO = l.CODIGO_CURSO });

            if(inscritosSelect == null)
            {
                return NotFound();
            }
            return inscritosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Inscrito item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Inscrito> inscritos = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO INSCRITO VALUES(" + "'" + item.RUT_ALUMNO + "', '" + item.CODIGO_CURSO + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Inscritos.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetInscrito", new { Id = item.CODIGO_CURSO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Inscrito> inscritos = this.GetAll().Value;
            var inscrito = inscritos.FirstOrDefault(l => l.CODIGO_CURSO == id.ToString());

            if(inscrito == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < inscritos.Count; i++)
                        {
                            var l = inscritos[i];
                            _context.Inscritos.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM INSCRITOS WHERE RUT_ALUMNO = '" + inscrito.RUT_ALUMNO + "AND CODIGO_CURSO = '" + inscrito.CODIGO_CURSO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Inscritos.Remove(inscrito);
                        _context.SaveChanges();
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }
            return NoContent();
        }
    }
}