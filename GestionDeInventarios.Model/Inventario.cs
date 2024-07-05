using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace GestionDeInventarios.Model
{
    public class Inventario
    {
        [HiddenInput]
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        public string Nombre { get; set; } = null!;
        [Required(ErrorMessage = "El campo Categoria es requerido")]
        public Categoria Categoria { get; set; }
        [Required(ErrorMessage = "El campo Cantidad es requerido")]
        public int Cantidad { get; set; } = 0;
        [Required(ErrorMessage = "El campo Precio es requerido")]
        public Decimal Precio { get; set; } = 0;


        [NotMapped]
        public List<HistorialDeInventario> HistorialDeCambios { get; set; } = new List<HistorialDeInventario>();
        [NotMapped]
        public string? UsuarioCreador { get; set; }
    }
}
