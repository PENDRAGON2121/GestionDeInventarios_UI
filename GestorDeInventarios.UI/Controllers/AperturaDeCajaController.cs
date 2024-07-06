using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GestorDeInventario.UI.Controllers
{
    public class AperturaDeCajaController : Controller
    {

        public async Task<ActionResult> Index()
        {
            var ajuste = new GestionDeInventarios.Model.AperturaDeCajaNueva
            {

                UserId = User.Identity.Name
            };

            if (TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
            }

            HttpClient httpClient = new HttpClient();
            var resp = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja/TieneApertura/{ajuste.UserId}");
            string apertura = await resp.Content.ReadAsStringAsync();

            Boolean tieneApertura = bool.Parse(apertura);
            
            ViewBag.EstaLaCajaAbierta = tieneApertura;


            
            List<AperturaDeLaCaja> laListaDelInventario;

            var respuesta = await httpClient.GetAsync("https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            laListaDelInventario = JsonConvert.DeserializeObject<List<AperturaDeLaCaja>>(apiResponse);

            
            
                return View(laListaDelInventario);
            

        }

        public ActionResult CrearAperturaDeCaja()
        {

            var ajuste = new GestionDeInventarios.Model.AperturaDeCajaNueva
            {

                UserId = User.Identity.Name
            };

            GestionDeInventarios.Model.AperturaDeCajaNueva aperturasDeCaja = new GestionDeInventarios.Model.AperturaDeCajaNueva();


            aperturasDeCaja.UserId = ajuste.UserId;
            aperturasDeCaja.FechaDeInicio = DateTime.Now;
            aperturasDeCaja.Estado = EstadoDeCaja.Abierta;


            return View(aperturasDeCaja);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearAperturaDeCaja(AperturaDeCajaNueva apertura)
        {
            try
            {
                var ajuste = new AjusteDeInventario
                {

                    UserId = User.Identity.Name
                };

                AperturaDeCajaNueva aperturasDeCaja = new GestionDeInventarios.Model.AperturaDeCajaNueva();
                aperturasDeCaja.UserId = ajuste.UserId;
                aperturasDeCaja.FechaDeInicio = DateTime.Now;
                aperturasDeCaja.Estado = EstadoDeCaja.Abierta;


                var httpClient = new HttpClient();

                string json = JsonConvert.SerializeObject(aperturasDeCaja);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var respuesta = await httpClient.PostAsync("https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja", byteContent);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public async Task<ActionResult> CerrarLaCaja(int Id)
        {

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja/{Id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            AperturaDeLaCaja apertura = JsonConvert.DeserializeObject<AperturaDeLaCaja>(apiResponse);

            return View("CerrarCaja", apertura);

        }
        [HttpPost]
        public async Task<ActionResult> CerrarLaCaja()
        {
            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja/UltimaApertura");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            AperturaDeLaCaja apertura = JsonConvert.DeserializeObject<AperturaDeLaCaja>(apiResponse);

            var ultimoRegistro = apertura;
            return View("CerrarLaCaja", ultimoRegistro);

        }

        public async Task<ActionResult> AcumuladoDeVentas(int id)
        {
            AcumuladoDeVentas acumulado;

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja/AcumuladoDeLaCaja/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            acumulado = JsonConvert.DeserializeObject<AcumuladoDeVentas>(apiResponse);

            return View("AcumuladoDeVentas", acumulado);
        }
        public async Task<ActionResult> validarCierre(int id)
        {
            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/AperturaDeCaja/CierreDeCaja");
            return RedirectToAction("Index");


        }
    }
}
