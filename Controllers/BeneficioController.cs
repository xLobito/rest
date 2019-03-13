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
    public class BeneficioController : ControllerBase
    {
        private readonly BeneficioContext _context;

        public BeneficioController(BeneficioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Beneficio>> GetAll()
        {
            List<Beneficio> beneficio = new List<Beneficio>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[2];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM BENEFICIO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                beneficio.Add(new Beneficio { CODIGO = objects[0].ToString(), NOMBRE = objects[1].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return beneficio;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetBeneficio")]
        public ActionResult<List<Beneficio>> GetById(long id)
        {
            IEnumerable<Beneficio> beneficiosSelect = this.GetAll().Value.Where(l => l.CODIGO == id.ToString()).Select(l => new Beneficio { CODIGO = l.CODIGO, NOMBRE = l.NOMBRE });

            if(beneficiosSelect == null)
            {
                return NotFound();
            }
            return beneficiosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Beneficio item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Beneficio> beneficios = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO BENEFICIO VALUES(" + "'" + item.CODIGO + "', '" + item.NOMBRE + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Beneficios.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetBeneficio", new { Id = item.NOMBRE }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Beneficio> beneficios = this.GetAll().Value;
            var beneficio = beneficios.FirstOrDefault(l => l.NOMBRE == id.ToString());

            if(beneficio == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < beneficios.Count; i++)
                        {
                            var l = beneficios[i];
                            _context.Beneficios.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM BENEFICIOS WHERE CODIGO = '" + beneficio.CODIGO + "AND NOMBRE = '" + beneficio.NOMBRE + "'";
                        cmd.ExecuteNonQuery();
                        _context.Beneficios.Remove(beneficio);
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