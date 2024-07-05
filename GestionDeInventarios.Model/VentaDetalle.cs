using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventarios.Model
{
    public class VentaDetalle
    {
        public int Id { get; set; }
        public int Id_Venta { get; set; }
        public int Id_Inventario { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoDescuento { get; set; }
    }
}
