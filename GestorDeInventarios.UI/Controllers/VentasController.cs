using GestionDeInventario.BL;
using GestionDeInventario.DA;
using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;

namespace GestorDeInventario.UI.Controllers
{
    public class VentasController : Controller
    {
 

        public async Task<ActionResult> Index()
        {
            HttpClient httpClient = new HttpClient();
            
            
            List<GestionDeInventarios.Model.Ventas> laListaDeVentas;

            var respuesta = await httpClient.GetAsync("https://localhost:7218/api/Ventas");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            laListaDeVentas = JsonConvert.DeserializeObject<List<Ventas>>(apiResponse);

            var resp = await httpClient.GetAsync($"https://localhost:7218/api/AperturaDeCaja/TieneApertura/{User.Identity.Name}");
            string apertura = await resp.Content.ReadAsStringAsync();

            Boolean tieneApertura = bool.Parse(apertura);

            if (!tieneApertura)
            {
                TempData["Mensaje"] = "No hay una caja abierta, por favor abra una caja para poder realizar ventas";
                return RedirectToAction("index", "AperturaDeCaja");
            }

            return View(laListaDeVentas);
        }

        public async Task<ActionResult> Create()
        {

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/AperturaDeCaja/UltimaApertura");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            AperturaDeLaCaja aperturaCaja = JsonConvert.DeserializeObject<AperturaDeLaCaja>(apiResponse);

            var nuevaVenta = new Ventas
            {
                NombreCliente = "",
                Fecha = DateTime.Now,
                UserId = User.Identity.Name,
                IdAperturaDeCaja = aperturaCaja.Id,
                Estado = EstadoDeVenta.Pendiente
            };

            List<GestionDeInventarios.Model.Inventario> laListaDelInventario;

            var resp = await httpClient.GetAsync("https://localhost:7218/api/Inventario");
            string apiResp = await resp.Content.ReadAsStringAsync();
            List<Inventario> laListaDeInventario = JsonConvert.DeserializeObject<List<Inventario>>(apiResp);

            ViewData["Inventario"] = laListaDeInventario;

            return View(nuevaVenta);
        }

        //TODO: PARA MI-> REALIZAR EL MODULO DE ACEPTACION DEL ADMIN AL SUSCRIP TAMBIEN SU VIEW Y AÑADIR UN CAMPO EN LA TABLA DE USUARIOS

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GestionDeInventarios.Model.Ventas venta)
        {
            try
            {

                var httpClient = new HttpClient();
                var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/AperturaDeCaja/UltimaApertura");
                string apiResponse = await respuesta.Content.ReadAsStringAsync();
                AperturaDeLaCaja aperturaCaja = JsonConvert.DeserializeObject<AperturaDeLaCaja>(apiResponse);

                

                venta.IdAperturaDeCaja = aperturaCaja.Id;
                venta.UserId = aperturaCaja.UserId;

                string json = JsonConvert.SerializeObject(venta);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                await httpClient.PostAsync("https://localhost:7218/api/Ventas", byteContent);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult>Terminar(Ventas ventaTemporal, List<GestionDeInventarios.Model.Inventario> carritoCompra, int ventaId)
        {

            var httpClient = new HttpClient();

            string json = JsonConvert.SerializeObject(carritoCompra);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            await httpClient.PostAsync($"https://localhost:7218/api/Ventas/{ventaTemporal.Id}", byteContent);


            TempData["IdVenta"] = ventaId;

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult>MostrarInventario(int ventaId)
        {


            var httpClient = new HttpClient();
            List<GestionDeInventarios.Model.Inventario> laListaDelInventario;

            var respuesta = await httpClient.GetAsync("https://localhost:7218/api/Inventario");
            
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            laListaDelInventario = JsonConvert.DeserializeObject<List<Inventario>>(apiResponse);

            return View(laListaDelInventario);
        }

        public async Task<ActionResult> MostrarCarrito(int ventaId)
        {
            if (ventaId != 0)
            {
                Response.Cookies.Append("ventaId", ventaId.ToString());
            }
            List<GestionDeInventarios.Model.Inventario> carritoCompras;

            var httpClient = new HttpClient();

            var respuesta = await httpClient.GetAsync("https://localhost:7218/api/Ventas/ObtenerElCarrito");

            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            carritoCompras = JsonConvert.DeserializeObject<List<Inventario>>(apiResponse);



            return View(carritoCompras);
        }

        public async Task<ActionResult> AgregaAlCarrito(Inventario inventarios)
        {

            var httpClient = new HttpClient();
            string json = JsonConvert.SerializeObject(inventarios);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Ventas/AgregarAlCarrito", byteContent);
            string apiResponse = await respuesta.Content.ReadAsStringAsync();

            List<Inventario> carritoComprasTemp;
            carritoComprasTemp = JsonConvert.DeserializeObject<List<Inventario>>(apiResponse);

            return RedirectToAction(nameof(MostrarCarrito), new { ventaId = TempData["IdVenta"] });
        }

        public async Task<ActionResult>BorrarDelCarrito(int id)
        {

            List<Inventario> carritoCompras;
            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync("https://localhost:7218/api/Ventas/ObtenerElCarrito");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            carritoCompras = JsonConvert.DeserializeObject<List<Inventario>>(apiResponse);

            if (carritoCompras != null)
            {
                var item = carritoCompras.FirstOrDefault(x => x.Id == id);
                if (item != null)
                {
                    carritoCompras.Remove(item);


                    string json = JsonConvert.SerializeObject(carritoCompras);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    await httpClient.PostAsync("https://localhost:7218/api/Ventas/ActualiceElCarrito", byteContent);

                }
            }
            return RedirectToAction(nameof(MostrarCarrito), new { ventaId = TempData["IdVenta"] });
        }

        [HttpPost]
        public async Task<IActionResult> TerminarVenta(int _ventaId, List<Inventario> carritoCompras, List<int> listaCantidades)
        {
            int indice = 0;

            if (Request.Cookies.TryGetValue("ventaId", out string ventaIdCookie))
            {
                int ventaId = int.Parse(ventaIdCookie);

                try
                {
                    var httpClient = new HttpClient();
                    var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Ventas/VentaPorId/{ventaId}");
                    string apiResponse = await respuesta.Content.ReadAsStringAsync();
                    var venta = JsonConvert.DeserializeObject<Ventas>(apiResponse);


                    
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
                    }

                    string json = JsonConvert.SerializeObject(venta);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    var ponse = await httpClient.PutAsync("https://localhost:7218/api/Ventas", byteContent);


                    indice = 0;
                    foreach (var item in carritoCompras)
                    {

                        var resp= await httpClient.GetAsync($"https://localhost:7218/api/Inventario/{item.Id}");
                        string apiResp= await resp.Content.ReadAsStringAsync();
                        Inventario inventario = JsonConvert.DeserializeObject<GestionDeInventarios.Model.Inventario>(apiResp);


                        



                        if (inventario != null)
                        {
                            inventario.Cantidad -= listaCantidades[indice];


                            string jsons = JsonConvert.SerializeObject(inventario);
                            var bufferr = System.Text.Encoding.UTF8.GetBytes(jsons);
                            var byteContentt = new ByteArrayContent(bufferr);
                            byteContentt.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                            var respuestas = await httpClient.PutAsync("https://localhost:7218/api/Inventario/Actualice", byteContentt);

                            //_GestionDeInventario.ActualiceElInventario(inventario);
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
        public async Task<IActionResult>DescuentoYPago(Ventas ventaTemp)
        {
            if (Request.Cookies.TryGetValue("ventaId", out string ventaIdCookie))
            {
                int ventaId = int.Parse(ventaIdCookie);

                try
                {

                    var httpClient = new HttpClient();
                    var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Ventas/ObtenerVentaPorId/{ventaId}");
                    string apiResponse = await respuesta.Content.ReadAsStringAsync();
                    var venta = JsonConvert.DeserializeObject<Ventas>(apiResponse);

                    //var venta = _GestionDeLasVentas.ObtenerVentaPorId(ventaId);
                    
                    if (venta != null) {
                        venta.PorcentajeDescuento = ventaTemp.PorcentajeDescuento;
                        decimal porcentaje = ventaTemp.PorcentajeDescuento / 100m;
                        decimal montoDescuentoTemp = venta.SubTotal * porcentaje;
                        decimal montoDescuento = Math.Round(montoDescuentoTemp, 2);
                        venta.MontoDescuento = montoDescuento;
                        venta.Total = venta.SubTotal - montoDescuento;
                        venta.Estado = EstadoDeVenta.Terminada;
                        venta.TipoDePago = ventaTemp.TipoDePago;


                        string json = JsonConvert.SerializeObject(venta);
                        var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                        var byteContent = new ByteArrayContent(buffer);
                        byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        var r = await httpClient.PutAsync("https://localhost:7218/api/Ventas", byteContent);

                        //_GestionDeLasVentas.ActualizarVenta(venta);
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
