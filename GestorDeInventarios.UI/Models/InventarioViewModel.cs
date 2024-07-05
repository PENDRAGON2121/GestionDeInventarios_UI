namespace GestorDeInventarios.UI.Models
{
    public class InventarioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
