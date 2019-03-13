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
    public class ModuloController : ControllerBase
    {
        private readonly ModuloContext _context;

        public ModuloController(ModuloContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Modulo>> GetAll()
        {
            List<Modulo> modulo = new List<Modulo>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[4];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM MODULO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                modulo.Add(new Modulo { CODIGO_CURSO = objects[0].ToString(), DIA = objects[1].ToString(), MODULO= objects[2].ToString(), AUTORELLENO= objects[3].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return modulo;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetModulo")]
        public ActionResult<List<Modulo>> GetById(long id)
        {
            IEnumerable<Modulo> modulosSelect = this.GetAll().Value.Where(l => l.CODIGO_CURSO == id.ToString()).Select(l => new Modulo { DIA = l.DIA, MODULO = l.MODULO , AUTORELLENO = l.AUTORELLENO });

            if(modulosSelect == null)
            {
                return NotFound();
            }
            return modulosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Modulo item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Modulo> modulos = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO MODULO VALUES(" + "'" + item.CODIGO_CURSO + "', '" + item.DIA + "', '" + item.MODULO + "', '" + item.AUTORELLENO + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Modulos.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetModulo", new { Id = item.CODIGO_CURSO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Modulo> modulos = this.GetAll().Value;
            var modulo = modulos.FirstOrDefault(l => l.CODIGO_CURSO == id.ToString());

            if(modulo == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < modulos.Count; i++)
                        {
                            var l = modulos[i];
                            _context.Modulos.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM MODULO WHERE RUT = '" + modulo.CODIGO_CURSO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Modulos.Remove(modulo);
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