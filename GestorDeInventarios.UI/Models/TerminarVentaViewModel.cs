using GestorDeInventarios.UI.Models;

namespace GestorDeInventario.UI.Models
{
    public class TerminarVentaViewModel
    {
        public int VentaId { get; set; }
        public List<InventarioViewModel> CarritoCompras { get; set; }
    }
}
