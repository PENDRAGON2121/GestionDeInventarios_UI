using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionDeInventarios.Model
{
    [Table("AjusteDeInventario")]
    public class AjusteDeInventario
    {
        [HiddenInput]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo ID Inventario es requerido")]
        [Display(Name = "ID del Inventario")]
        public int Id_Inventario { get; set; }
        [Required(ErrorMessage = "El campo Cantidad Actual es requerido")]
        [Display(Name = "Cantidad Actual")]
        public int CantidadActual { get; set; }
        [Required(ErrorMessage = "El campo Ajuste es requerido")]
        public int Ajuste { get; set; }
        [Required(ErrorMessage = "El campo Tipo  es requerido")]
        public TipoDeAjuste Tipo { get; set; }
        [Required(ErrorMessage = "El campo Observaciones es requerido")]
        public string Observaciones { get; set; }
        [Required(ErrorMessage = "El campo ID de Usuario es requerido")]
        [Display(Name = "ID de Usuario")]
        public String UserId { get; set; }
        [Required(ErrorMessage = "El campo Fecha es requerido")]
        public DateTime Fecha { get; set; }

    }
}
