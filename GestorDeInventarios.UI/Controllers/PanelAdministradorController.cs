using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace GestorDeInventarios.UI.Controllers
{
    public class PanelAdministradorController : Controller
    {
        // GET: PanelAdministradorController
        public async Task<ActionResult> Index()
        {
            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync("https://apigestiondeinventario.azurewebsites.net/api/Usuarios/ObtenerUsuariosSinSuscripcion");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            List<Usuario> laListaDeSuscripciones = JsonConvert.DeserializeObject<List<Usuario>>(apiResponse);

            return View(laListaDeSuscripciones);
        }

        // GET: PanelAdministradorController/Details/5
        public async Task<ActionResult> RealizarSuscripcion(int id)
        {
            var httpClient = new HttpClient();

            var respuesta = await httpClient.GetAsync($"https://apigestiondeinventario.azurewebsites.net/api/Usuarios/SuscribirUsuarioPorId/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();


            return RedirectToAction("Index");
        }
        
    }
}
