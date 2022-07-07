

using System.ComponentModel.DataAnnotations;

namespace InstaMazz.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }


        public string Nombre { get; set; }


        public string UserName { get; set; }


        public string Contraseña { get; set; }


        public string email { get; set; }


        public string ConfirmarClave { get; set; }
    }
}
