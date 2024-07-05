using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionDeInventarios.Model
{
    public class InventariosEditar
    {
        [HiddenInput]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo Categoria es requerido")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "El campo Precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El campo Precio debe ser un número positivo")]
        public decimal Precio { get; set; }
    }
}
