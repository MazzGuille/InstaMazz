using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;
using System.Data;
using InstaMazz.Models;
using InstaMazz.Datos;
using Microsoft.Data.SqlClient;
using System;


namespace InstaMazz.Controllers
{
    public class AccesoController : Controller
    {

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(UsuarioModel oUsuario)
        {
            bool registrado;
            string mensaje;
            bool email;

            email = oUsuario.email.Contains('.');

            if (email == false)
            {
                ViewData["email"] = "Formato de E-Mail invalido";

                return View();
            }

            if (oUsuario.Nombre.Length < 3 || oUsuario.Nombre.Length > 30)
            {
                ViewData["Nombre"] = "El nombre debe tener entre 3 y 20 caracteres";


                return View();
            }

            if (oUsuario.UserName.Length < 3 || oUsuario.UserName.Length > 10)
            {
                ViewData["NombreUsuario"] = "El nombre de usuario debe tener entre 3 y 10 caracteres";


                return View();
            }

            if (oUsuario.Contraseña == oUsuario.ConfirmarClave)
            {
                oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                var cmd = new SqlCommand("sp_RegistrarUsuario", conexion);
                cmd.Parameters.AddWithValue("Nombre", oUsuario.Nombre);
                cmd.Parameters.AddWithValue("UserName", oUsuario.UserName);
                cmd.Parameters.AddWithValue("email", oUsuario.email);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();

                cmd.ExecuteNonQuery();

                registrado = (bool)cmd.Parameters["Registrado"].Value;
                mensaje = (string)cmd.Parameters["Mensaje"].Value;
            }

            ViewData["Mensaje"] = mensaje;



            if (registrado)
            {

                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Login(UsuarioModel oUsuario)
        {
            oUsuario.Contraseña = ConvertirSHA256(oUsuario.Contraseña);

            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                var cmd = new SqlCommand("sp_ValidarUsuario", conexion);
                cmd.Parameters.AddWithValue("email", oUsuario.email);
                cmd.Parameters.AddWithValue("Contraseña", oUsuario.Contraseña);
                cmd.CommandType = CommandType.StoredProcedure;

                conexion.Open();


                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
                var result = oUsuario.IdUsuario;
            }




            if (oUsuario.IdUsuario != 0)
            {
                
                //ISession session = HttpContext.Session;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos";
                return View();
            }

        }




        public static string ConvertirSHA256(string text)
        {
            if (text == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(text));
                foreach (byte b in result)
                    sb.Append(b.ToString("x2"));
            }
            return sb.ToString();

        }
    }
}
