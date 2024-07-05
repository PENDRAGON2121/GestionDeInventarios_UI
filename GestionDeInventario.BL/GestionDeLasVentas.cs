using GestionDeInventario.DA;
using GestionDeInventarios.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventario.BL
{
    public class GestionDeLasVentas
    {
        private readonly DbGestionDeInventario _contexto;
        private static List<Inventario> CarritoCompras = new List<Inventario>();

        public GestionDeLasVentas(DbGestionDeInventario contexto)
        {
            _contexto = contexto;
        }

        // Métodos sobre la venta
        public void RegistrarVenta(Ventas venta)
        {
            DateTime fechaActual = DateTime.Now;
            Ventas nuevaVenta = new Ventas
            {
                NombreCliente = venta.NombreCliente,
                Fecha = fechaActual,
                TipoDePago = TipoDePago.Efectivo,
                Total = 0,
                SubTotal = 0,
                PorcentajeDescuento = 0,
                MontoDescuento = 0,
                UserId = venta.UserId,
                Estado = EstadoDeVenta.Pendiente,
                IdAperturaDeCaja = venta.IdAperturaDeCaja
            };

            _contexto.Ventas.Add(nuevaVenta);
            _contexto.SaveChanges();
        }

        public void AplicarDescuento(int idVenta, decimal porcentajeDescuento)
        {
            Ventas venta = _contexto.Ventas.Find(idVenta);

            if (venta != null)
            {
                venta.PorcentajeDescuento = (int)porcentajeDescuento;
                venta.MontoDescuento = venta.SubTotal * (porcentajeDescuento / 100m);
                venta.Total = venta.SubTotal - venta.MontoDescuento;
                _contexto.SaveChanges();
            }
            else
            {
                Console.WriteLine("La venta no existe.");
            }
        }

        public void TerminarLaVenta(int idVenta, List<Inventario> carritoCompra)
        {
            Ventas venta = _contexto.Ventas.Find(idVenta);

            foreach (var carritoItem in carritoCompra)
            {
                var inventarioItem = _contexto.Inventarios.Find(carritoItem.Id);

                var detalleVenta = new VentaDetalle
                {
                    Id_Venta = venta.Id,
                    Id_Inventario = carritoItem.Id,
                    Cantidad = carritoItem.Cantidad,
                    Precio = inventarioItem.Precio,
                    Monto = inventarioItem.Precio * carritoItem.Cantidad,
                    MontoDescuento = inventarioItem.Precio * carritoItem.Cantidad * (venta.PorcentajeDescuento / 100m)
                };

                _contexto.DetallesDeVenta.Add(detalleVenta);
                inventarioItem.Cantidad -= carritoItem.Cantidad;
            }

            carritoCompra.Clear();
            _contexto.SaveChanges();
        }

        public List<Ventas> ObtengaLaListaDeVentas()
        {
            List<Ventas> ListaDeVentas = new List<Ventas>();

            foreach (var item in _contexto.Ventas)
            {
                ListaDeVentas.Add(new Ventas
                {
                    Id = item.Id,
                    NombreCliente = item.NombreCliente,
                    Fecha = item.Fecha,
                    TipoDePago = item.TipoDePago,
                    Total = item.Total,
                    SubTotal = item.SubTotal,
                    PorcentajeDescuento = item.PorcentajeDescuento,
                    MontoDescuento = item.MontoDescuento,
                    UserId = item.UserId,
                    Estado = item.Estado,
                    IdAperturaDeCaja = item.IdAperturaDeCaja
                });
            }

            return ListaDeVentas;
        }

        public Ventas ObtengaItemPorId(int Id)
        {
            Ventas ventaTemp = new Ventas();

            foreach (var item in _contexto.Ventas)
            {
                if (item.Id == Id)
                {
                    ventaTemp.Id = item.Id;
                    ventaTemp.NombreCliente = item.NombreCliente;
                    ventaTemp.Fecha = item.Fecha;
                    ventaTemp.TipoDePago = item.TipoDePago;
                    ventaTemp.Total = item.Total;
                    ventaTemp.SubTotal = item.SubTotal;
                    ventaTemp.PorcentajeDescuento = item.PorcentajeDescuento;
                    ventaTemp.MontoDescuento = item.MontoDescuento;
                    ventaTemp.UserId = item.UserId;
                    ventaTemp.Estado = item.Estado;
                    ventaTemp.IdAperturaDeCaja = item.IdAperturaDeCaja;

                    return ventaTemp;
                }
            }

            return null;
        }
        public List<Inventario> ObtengaLaListaDeInventarios()
        {
            List<Inventario> listaDeInventario = new List<Inventario>();

            foreach (var item in _contexto.Inventarios)
            {

                listaDeInventario.Add(new Inventario { Id = item.Id, Nombre = item.Nombre, Categoria = item.Categoria, Cantidad = item.Cantidad, Precio = item.Precio });

            }
            return listaDeInventario;
        }
        public List<Inventario> ObtengaElCarrito()
        {
            return CarritoCompras.ToList();
        }

        public List<Inventario> AgregarItemAlCarrito(Inventario nuevoItem)
        {
            CarritoCompras.Add(nuevoItem);

            return ObtengaElCarrito();
        }

        public Ventas ObtenerVentaPorId(int ventaId)
        {
            return _contexto.Ventas.FirstOrDefault(v => v.Id == ventaId);
        }

        public void ActualizarVenta(Ventas venta)
        {
            _contexto.Ventas.Update(venta);
            _contexto.SaveChanges();
        }

        public void RegistrarVentaDetalle(VentaDetalle ventaDetalle)
        {
            _contexto.DetallesDeVenta.Add(ventaDetalle);
            _contexto.SaveChanges();
        }

        public void ActualiceElCarrito(List<Inventario> carritoCompras)
        {
            CarritoCompras = carritoCompras;
        }

    }
}
