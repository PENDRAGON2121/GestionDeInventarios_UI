using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventarios.Model
{
    public class Ventas
    {
        public int Id { get; set; }
        public string NombreCliente { get; set; }
        public DateTime Fecha { get; set; }
        public TipoDePago TipoDePago { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public int PorcentajeDescuento { get; set; }
        public decimal MontoDescuento { get; set; }
        public string UserId { get; set; }
        public EstadoDeVenta Estado { get; set; }
        public int IdAperturaDeCaja { get; set; }
    }
}
