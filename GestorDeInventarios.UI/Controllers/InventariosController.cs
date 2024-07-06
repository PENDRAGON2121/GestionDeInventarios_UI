using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GestorDeInventario.UI.Controllers
{

    public class InventariosController : Controller
    {

        public async Task<ActionResult> Index(string nombre)
        {

            if (nombre is null)
            {
                var httpClient = new HttpClient();
                List<GestionDeInventarios.Model.Inventario> laListaDelInventario;

                var respuesta = await httpClient.GetAsync("https://localhost:7218/api/Inventario");
                string apiResponse = await respuesta.Content.ReadAsStringAsync();
                laListaDelInventario = JsonConvert.DeserializeObject<List<Inventario>>(apiResponse);
                return View(laListaDelInventario);
            }
            else
            {
                var httpClient = new HttpClient();
                List<GestionDeInventarios.Model.Inventario> laListaDelInventarioPorNombre;

                var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/PorNombre/{nombre}");
                    string apiResponse = await respuesta.Content.ReadAsStringAsync();
                    laListaDelInventarioPorNombre = JsonConvert.DeserializeObject<List<GestionDeInventarios.Model.Inventario>>(apiResponse);
                    return View(laListaDelInventarioPorNombre);

            }
        }

        public async Task<ActionResult> Details(int id)
        {
            GestionDeInventarios.Model.Inventario inventarios;
            List<GestionDeInventarios.Model.HistorialDeInventario> historialDeCambios;

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            inventarios = JsonConvert.DeserializeObject<GestionDeInventarios.Model.Inventario>(apiResponse);


            var respuestaHistorial = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/Historial/{id}");
            string apiResponseHistorial = await respuestaHistorial.Content.ReadAsStringAsync();
            historialDeCambios = JsonConvert.DeserializeObject<List<GestionDeInventarios.Model.HistorialDeInventario>>(apiResponseHistorial);
            
            ViewBag.Historial = historialDeCambios;

            return View(inventarios);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(GestionDeInventarios.Model.Inventario inventarios)
        {
            try
            {
                inventarios.UsuarioCreador = User.Identity.Name;
                var httpClient = new HttpClient();

                string json = JsonConvert.SerializeObject(inventarios);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Inventario", byteContent);


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Edit(int id)
        {

            GestionDeInventarios.Model.Inventario inventarios;
            GestionDeInventarios.Model.InventariosEditar inventariosEditar = new GestionDeInventarios.Model.InventariosEditar();

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            inventarios = JsonConvert.DeserializeObject<GestionDeInventarios.Model.Inventario>(apiResponse);
            

            inventariosEditar.Id = inventarios.Id;
            inventariosEditar.Nombre = inventarios.Nombre;
            inventariosEditar.Categoria = inventarios.Categoria;
            inventariosEditar.Precio = inventarios.Precio;

            return View(inventariosEditar);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(GestionDeInventarios.Model.Inventario inventarios)
        {
            try
            {

                inventarios.UsuarioCreador = User.Identity.Name;
                var httpClient = new HttpClient();

                string json = JsonConvert.SerializeObject(inventarios);
                var buffer = System.Text.Encoding.UTF8.GetBytes(json);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var respuesta = await httpClient.PutAsync("https://localhost:7218/api/Inventario", byteContent);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
