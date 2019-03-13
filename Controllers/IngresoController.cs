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
    public class IngresoController : ControllerBase
    {
        private readonly IngresoContext _context;

        public IngresoController(IngresoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Ingreso>> GetAll()
        {
            List<Ingreso> ingreso = new List<Ingreso>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[2];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM INGRESO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                ingreso.Add(new Ingreso { RUT = objects[0].ToString(), IMEI = objects[1].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return ingreso;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetIngreso")]
        public ActionResult<List<Ingreso>> GetById(long id)
        {
            IEnumerable<Ingreso> ingresosSelect = this.GetAll().Value.Where(l => l.RUT == id.ToString()).Select(l => new Ingreso { RUT = l.RUT, IMEI = l.IMEI });

            if(ingresosSelect == null)
            {
                return NotFound();
            }
            return ingresosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Ingreso item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Ingreso> ingresos = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO INGRESO VALUES(" + "'" + item.RUT + "', '" + item.IMEI + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Ingresos.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetIngreso", new { Id = item.IMEI }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Ingreso> ingresos = this.GetAll().Value;
            var ingreso = ingresos.FirstOrDefault(l => l.IMEI == id.ToString());

            if(ingreso == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < ingresos.Count; i++)
                        {
                            var l = ingresos[i];
                            _context.Ingresos.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM INGRESOS WHERE RUT = '" + ingreso.RUT + "AND IMEI = '" + ingreso.IMEI + "'";
                        cmd.ExecuteNonQuery();
                        _context.Ingresos.Remove(ingreso);
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