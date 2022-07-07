using InstaMazz.Datos;
using InstaMazz.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InstaMazz.Controllers
{
    public class PostController : Controller
    {
        public IActionResult CrearPost()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CrearPost(Publicaciones oPublicaciones)
        {

            var cn = new Conexion();

            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("sp_CrearPost", conexion);
                cmd.Parameters.AddWithValue("IdUsuario", oPublicaciones.IdUsuario);
                cmd.Parameters.AddWithValue("UrlImg", oPublicaciones.UrlImg);
                cmd.Parameters.AddWithValue("Descripcion", oPublicaciones.Descripcion);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                conexion.Close();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult EliminarPost()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EliminarPost(Publicaciones oPublicacion)
        {
            var cn = new Conexion();
            using (var conexion = new SqlConnection(cn.GetCadenaSQL()))
            {
                conexion.Open();
                SqlCommand cmd = new SqlCommand("sp_EliminarPost", conexion);
                cmd.Parameters.AddWithValue("IdPost", oPublicacion.IdPost);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                conexion.Close();
            }


            return RedirectToAction("Index", "Home");
        }
    }
}
