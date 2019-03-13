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
    public class CursoController : ControllerBase
    {
        private readonly CursoContext _context;

        public CursoController(CursoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Curso>> GetAll()
        {
            List<Curso> curso = new List<Curso>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[4];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM CURSO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                curso.Add(new Curso { CODIGO = objects[0].ToString(), NOMBRE = objects[1].ToString(), CARRERA = objects[2].ToString(), RUT_PROFESOR= objects[3].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return curso;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetCurso")]
        public ActionResult<List<Curso>> GetById(long id)
        {
            IEnumerable<Curso> cursosSelect = this.GetAll().Value.Where(l => l.CODIGO == id.ToString()).Select(l => new Curso { CODIGO = l.CODIGO, NOMBRE = l.NOMBRE, CARRERA = l.CARRERA, RUT_PROFESOR = l.RUT_PROFESOR });

            if(cursosSelect == null)
            {
                return NotFound();
            }
            return cursosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Curso item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Curso> cursos = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO CURSO VALUES(" + "'" + item.CODIGO + "', '" + item.NOMBRE + "', '" + item.CARRERA + "', '" + item.RUT_PROFESOR + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Cursos.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetCurso", new { Id = item.CODIGO }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Curso> cursos = this.GetAll().Value;
            var curso = cursos.FirstOrDefault(l => l.CODIGO == id.ToString());

            if(curso == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < cursos.Count; i++)
                        {
                            var l = cursos[i];
                            _context.Cursos.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM CURSOS WHERE CODIGO = '" + curso.CODIGO + "'";
                        cmd.ExecuteNonQuery();
                        _context.Cursos.Remove(curso);
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