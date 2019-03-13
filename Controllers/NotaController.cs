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
    public class NotaController : ControllerBase
    {
        private readonly NotaContext _context;

        public NotaController(NotaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Nota>> GetAll()
        {
            List<Nota> nota = new List<Nota>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[6];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM NOTA";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                nota.Add(new Nota { RUT_ALUMNO = objects[0].ToString(), CODIGO_CURSO = objects[1].ToString(), NUMERO = objects[2].ToString(), TIPO= objects[3].ToString(), NOTA= objects[4].ToString(), PONDERACION= objects[5].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return nota;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetNota")]
        public ActionResult<List<Nota>> GetById(long id)
        {
            IEnumerable<Nota> notasSelect = this.GetAll().Value.Where(l => l.RUT_ALUMNO == id.ToString()).Select(l => new Nota { RUT_ALUMNO = l.RUT_ALUMNO, CODIGO_CURSO = l.CODIGO_CURSO, NUMERO = l.NUMERO, TIPO = l.TIPO, NOTA = l.NOTA, PONDERACION = l.PONDERACION });

            if(notasSelect == null)
            {
                return NotFound();
            }
            return notasSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Nota item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Nota> notas = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO NOTA VALUES(" + "'" + item.RUT_ALUMNO + "', '" + item.CODIGO_CURSO + "', '" + item.NUMERO + "', '" + item.TIPO + "', '" + item.NOTA + "', '" + item.PONDERACION + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Notas.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetNota", new { Id = item.RUT_ALUMNO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Nota> notas = this.GetAll().Value;
            var nota = notas.FirstOrDefault(l => l.RUT_ALUMNO == id.ToString());

            if(nota == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < notas.Count; i++)
                        {
                            var l = notas[i];
                            _context.Notas.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM NOTAS WHERE RUT_ALUMNO = '" + nota.RUT_ALUMNO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Notas.Remove(nota);
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