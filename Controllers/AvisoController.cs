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
    public class AvisoController : ControllerBase
    {
        private readonly AvisoContext _context;

        public AvisoController(AvisoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Aviso>> GetAll()
        {
            List<Aviso> aviso = new List<Aviso>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[3];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM AVISO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                aviso.Add(new Aviso { CODIGO_CURSO = objects[0].ToString(), NUMERO = objects[1].ToString(), TEXTO = objects[2].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return aviso;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetAviso")]
        public ActionResult<List<Aviso>> GetById(long id)
        {
            IEnumerable<Aviso> avisosSelect = this.GetAll().Value.Where(l => l.CODIGO_CURSO == id.ToString()).Select(l => new Aviso { CODIGO_CURSO = l.CODIGO_CURSO, NUMERO = l.NUMERO, TEXTO = l.TEXTO });

            if(avisosSelect == null)
            {
                return NotFound();
            }
            return avisosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Aviso item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Aviso> avisos = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO AVISO VALUES(" + "'" + item.CODIGO_CURSO + "', '" + item.NUMERO + "', '" + item.TEXTO + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Avisos.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetAviso", new { Id = item.CODIGO_CURSO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Aviso> avisos = this.GetAll().Value;
            var aviso = avisos.FirstOrDefault(l => l.CODIGO_CURSO == id.ToString());

            if(aviso == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < avisos.Count; i++)
                        {
                            var l = avisos[i];
                            _context.Avisos.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM AVISOS WHERE CODIGO_CURSO = '" + aviso.CODIGO_CURSO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Avisos.Remove(aviso);
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