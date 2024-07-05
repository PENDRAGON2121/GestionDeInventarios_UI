using GestionDeInventario.BL;
using GestionDeInventario.DA;
using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GestorDeInventario.UI.Controllers
{
    public class VentasController : Controller
    {
        private readonly GestionDeLasVentas _GestionDeLasVentas;
        private readonly AdministracionDeInventario _GestionDeInventario;

        public VentasController(DbGestionDeInventario conexion)
        {
            _GestionDeLasVentas = new GestionDeLasVentas(conexion);
            _GestionDeInventario = new AdministracionDeInventario(conexion);
        }

        public ActionResult Index()
        {
            List<GestionDeInventarios.Model.Ventas> laListaDeVentas;
            laListaDeVentas = _GestionDeLasVentas.ObtengaLaListaDeVentas();

            Boolean cajaAbierta = _GestionDeInventario.TieneAperturaDeCaja(User.Identity.Name);

            if (!cajaAbierta)
            {
                TempData["Mensaje"] = "No hay una caja abierta, por favor abra una caja para poder realizar ventas";
                return RedirectToAction("index", "AperturaDeCaja");
            }

            return View(laListaDeVentas);
        }

        public ActionResult Create()
        {
            AperturaDeLaCaja aperturaCaja = _GestionDeInventario.ObtengaLaUltimaAperturaDeCaja();

            var nuevaVenta = new Ventas
            {
                NombreCliente = "",
                Fecha = DateTime.Now,
                UserId = User.Identity.Name,
                IdAperturaDeCaja = aperturaCaja.Id,
                Estado = EstadoDeVenta.Pendiente
            };

            List<Inventario> laListaDeInventario = _GestionDeInventario.ObtengaLaLista();

            ViewData["Inventario"] = laListaDeInventario;

            return View(nuevaVenta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GestionDeInventarios.Model.Ventas venta)
        {
            try
            {
                AperturaDeLaCaja aperturaCaja = _GestionDeInventario.ObtengaLaUltimaAperturaDeCaja();

                venta.IdAperturaDeCaja = aperturaCaja.Id;
                venta.UserId = aperturaCaja.UserId;

                _GestionDeLasVentas.RegistrarVenta(venta);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Terminar(Ventas ventaTemporal, List<GestionDeInventarios.Model.Inventario> carritoCompra, int ventaId)
        {
            _GestionDeLasVentas.TerminarLaVenta(ventaTemporal.Id, carritoCompra);
            TempData["IdVenta"] = ventaId;

            return RedirectToAction(nameof(Index));
        }

        public ActionResult MostrarInventario(int ventaId)
        {

            List<GestionDeInventarios.Model.Inventario> laListaDelInventario;
            laListaDelInventario = _GestionDeLasVentas.ObtengaLaListaDeInventarios();

            return View(laListaDelInventario);
        }

        public ActionResult MostrarCarrito(int ventaId)
        {
            if (ventaId != 0)
            {
                Response.Cookies.Append("ventaId", ventaId.ToString());
            }
            List<GestionDeInventarios.Model.Inventario> carritoCompras;
            carritoCompras = _GestionDeLasVentas.ObtengaElCarrito();

            return View(carritoCompras);
        }

        public ActionResult AgregaAlCarrito(Inventario inventarios)
        {
            List<Inventario> carritoComprasTemp = _GestionDeLasVentas.AgregarItemAlCarrito(inventarios);
            return RedirectToAction(nameof(MostrarCarrito), new { ventaId = TempData["IdVenta"] });
        }

        public ActionResult BorrarDelCarrito(int id)
        {
            List<Inventario> carritoCompras = _GestionDeLasVentas.ObtengaElCarrito();
            if (carritoCompras != null)
            {
                var item = carritoCompras.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    carritoCompras.Remove(item);
                    _GestionDeLasVentas.ActualiceElCarrito(carritoCompras);
                }
            }
            return RedirectToAction(nameof(MostrarCarrito), new { ventaId = TempData["IdVenta"] });
        }

        [HttpPost]
        public IActionResult TerminarVenta(int _ventaId, List<Inventario> carritoCompras, List<int> listaCantidades)
        {
            int indice = 0;

            if (Request.Cookies.TryGetValue("ventaId", out string ventaIdCookie))
            {
                int ventaId = int.Parse(ventaIdCookie);

                try
                {
                    var venta = _GestionDeLasVentas.ObtenerVentaPorId(ventaId);
                    
                    indice = 0;
                    if (venta != null)
                    {
                        decimal total = 0;
                        foreach (var item in carritoCompras)
                        {
                            total += item.Precio * listaCantidades[indice];
                            indice++;
                        }
                        venta.Total = total;
                        _GestionDeLasVentas.ActualizarVenta(venta);
                    }
                    indice = 0;
                    if (venta != null)
                    {
                        decimal subtotal = 0;
                        foreach (var item in carritoCompras)
                        {
                            subtotal += item.Precio * listaCantidades[indice];
                            indice++;
                        }
                        venta.SubTotal = subtotal;
                        _GestionDeLasVentas.ActualizarVenta(venta);
                    }

                    indice = 0;
                    foreach (var item in carritoCompras)
                    {
                        var inventario = _GestionDeInventario.ObtengaElInventarioPorIdentificacion(item.Id);
                        if (inventario != null)
                        {
                            inventario.Cantidad -= listaCantidades[indice];
                            _GestionDeInventario.ActualiceElInventario(inventario);
                        }
                        indice++;
                    }
                    return RedirectToAction("DescuentoYPago");
 

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        public ActionResult DescuentoYPago()
        {

            return View("DescuentoYPago");
        }

        [HttpPost]
        public IActionResult DescuentoYPago(Ventas ventaTemp)
        {
            if (Request.Cookies.TryGetValue("ventaId", out string ventaIdCookie))
            {
                int ventaId = int.Parse(ventaIdCookie);

                try
                {
                    var venta = _GestionDeLasVentas.ObtenerVentaPorId(ventaId);
                    if (venta != null) {
                        venta.PorcentajeDescuento = ventaTemp.PorcentajeDescuento;
                        decimal porcentaje = ventaTemp.PorcentajeDescuento / 100m;
                        decimal montoDescuentoTemp = venta.SubTotal * porcentaje;
                        decimal montoDescuento = Math.Round(montoDescuentoTemp, 2);
                        venta.MontoDescuento = montoDescuento;
                        venta.Total = venta.SubTotal - montoDescuento;
                        venta.Estado = EstadoDeVenta.Terminada;
                        venta.TipoDePago = ventaTemp.TipoDePago;
                        _GestionDeLasVentas.ActualizarVenta(venta);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error");
                }
            }
            return RedirectToAction("Error");
        }
    }
}
