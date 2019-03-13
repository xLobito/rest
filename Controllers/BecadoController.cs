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
    public class BecadoController : ControllerBase
    {
        private readonly BecadoContext _context;

        public BecadoController(BecadoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Becado>> GetAll()
        {
            List<Becado> becado = new List<Becado>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[2];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM BECADO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                becado.Add(new Becado { RUT = objects[0].ToString(), CODIGO = objects[1].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return becado;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetBecado")]
        public ActionResult<List<Becado>> GetById(long id)
        {
            IEnumerable<Becado> becadosSelect = this.GetAll().Value.Where(l => l.RUT == id.ToString()).Select(l => new Becado { RUT = l.RUT, CODIGO = l.CODIGO });

            if(becadosSelect == null)
            {
                return NotFound();
            }
            return becadosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Becado item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Becado> becados = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO BECADO VALUES(" + "'" + item.RUT + "', '" + item.CODIGO + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Becados.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetBecado", new { Id = item.CODIGO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Becado> becados = this.GetAll().Value;
            var becado = becados.FirstOrDefault(l => l.CODIGO == id.ToString());

            if(becado == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < becados.Count; i++)
                        {
                            var l = becados[i];
                            _context.Becados.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM BECADOS WHERE RUT = '" + becado.RUT + "AND CODIGO = '" + becado.CODIGO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Becados.Remove(becado);
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