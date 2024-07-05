using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionDeInventarios.Model
{
    public class AperturaDeCajaNueva
    {
        [HiddenInput]
        public int Id { get; set; }
        public string UserId { get; set; }

        [Required(ErrorMessage = "El campo fecha de inicio es requerido")]
        public DateTime FechaDeInicio { get; set; }

        public EstadoDeCaja Estado { get; set; }
    }
}
