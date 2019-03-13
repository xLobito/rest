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
    public class CarreraController : ControllerBase
    {
        private readonly CarreraContext _context;

        public CarreraController(CarreraContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Carrera>> GetAll()
        {
            List<Carrera> carrera = new List<Carrera>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[3];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM CARRERA";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                carrera.Add(new Carrera { CODIGO = objects[0].ToString(), NOMBRE = objects[1].ToString(), ARANCEL = objects[2].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return carrera;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetCarrera")]
        public ActionResult<List<Carrera>> GetById(long id)
        {
            IEnumerable<Carrera> carrerasSelect = this.GetAll().Value.Where(l => l.CODIGO == id.ToString()).Select(l => new Carrera { CODIGO = l.CODIGO, NOMBRE = l.NOMBRE, ARANCEL = l.ARANCEL });

            if(carrerasSelect == null)
            {
                return NotFound();
            }
            return carrerasSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Carrera item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Carrera> carreras = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO CARRERA VALUES(" + "'" + item.CODIGO + "', '" + item.NOMBRE + "', '" + item.ARANCEL + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Carreras.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetCarrera", new { Id = item.CODIGO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Carrera> carreras = this.GetAll().Value;
            var carrera = carreras.FirstOrDefault(l => l.CODIGO == id.ToString());

            if(carrera == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < carreras.Count; i++)
                        {
                            var l = carreras[i];
                            _context.Carreras.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM CARRERAS WHERE CODIGO = '" + carrera.CODIGO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Carreras.Remove(carrera);
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