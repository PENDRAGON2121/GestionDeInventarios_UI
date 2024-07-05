using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventarios.Model
{
    public class UsuarioActualizado
    {
        [Required]
        public required string Nombre { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Clave { get; set; }

        [DataType(DataType.Password)]
        [Compare("Clave", ErrorMessage = "Las Claves No Coinciden.")]
        public required string ConfirmarClave { get; set; }

        public String? Correo { get; set; }
        public String? NombreDelUsuario { get; set; }


    }
}
