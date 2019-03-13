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
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioContext _context;

        public UsuarioController(UsuarioContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<Usuario>> GetAll()
        {
            List<Usuario> usuario = new List<Usuario>();

            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        object[] objects = new object[5];
                        conn.Open();
                        cmd.CommandText = "SELECT * FROM USUARIO";
                        OracleDataReader reader = cmd.ExecuteReader();
                        while(reader.Read())
                        {
                            int ret = reader.GetValues(objects);
                            if(ret > 0)
                            {
                                usuario.Add(new Usuario { RUT = objects[0].ToString(), NOMBRE = objects[1].ToString(), TIPO = objects[2].ToString(), CONTRASENA= objects[3].ToString(), CARRERA= objects[4].ToString() });
                            }
                        }
                        reader.Dispose();
                        conn.Close();
                        return usuario;
                    }
                    catch(Exception)
                    {
                        return NotFound();
                    }
                }
            }   
        }

        [HttpGet("{id}", Name = "GetUsuario")]
        public ActionResult<List<Usuario>> GetById(long id)
        {
            IEnumerable<Usuario> usuariosSelect = this.GetAll().Value.Where(l => l.RUT == id.ToString()).Select(l => new Usuario { RUT = l.RUT, NOMBRE = l.NOMBRE, TIPO = l.TIPO, CONTRASENA = l.CONTRASENA, CARRERA = l.CARRERA });

            if(usuariosSelect == null)
            {
                return NotFound();
            }
            return usuariosSelect.ToList();
        }

        [HttpPost]
        public IActionResult Create(Usuario item)
        {
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        List<Usuario> usuarios = this.GetAll().Value;
                        conn.Open();
                        cmd.CommandText = "INSERT INTO USUARIO VALUES(" + "'" + item.RUT + "', '" + item.NOMBRE + "', '" + item.TIPO + "', '" + item.CONTRASENA + "', '" + item.CARRERA + "')";
                        cmd.ExecuteNonQuery();
                        //_context.Usuarios.Add(item);
                        //_context.SaveChanges();                        
                    }
                    catch(Exception ex)
                    {
                        return NotFound();
                    }
                }
            }
            return CreatedAtRoute("GetUsuario", new { Id = item.RUT }, item);
        }

        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            List<Usuario> usuarios = this.GetAll().Value;
            var usuario = usuarios.FirstOrDefault(l => l.RUT == id.ToString());

            if(usuario == null)
            {
                return NotFound();
            }
            using(OracleConnection conn = new OracleConnection(connectionString:"User Id=sinuois;Password=SINUOIS;Data Source=localhost:1521;"))
            {
                using(OracleCommand cmd = conn.CreateCommand())
                {
                    try 
                    {
                        for (int i = 0; i < usuarios.Count; i++)
                        {
                            var l = usuarios[i];
                            _context.Usuarios.Add(l);
                        }
                        conn.Open();
                        cmd.CommandText = "DELETE FROM USUARIOS WHERE RUT = '" + usuario.RUT + "'";
                        cmd.ExecuteNonQuery();
                        _context.Usuarios.Remove(usuario);
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