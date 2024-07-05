using Azure.Identity;
using GestionDeInventario.BL;
using GestionDeInventario.DA;
using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Bcpg;
using System.Net.Http;

namespace GestorDeInventario.UI.Controllers
{
    public class AjusteDeInventarioController : Controller
    {
        private readonly AdministracionDeInventario _GestionDeInventarios;


        public AjusteDeInventarioController(DbGestionDeInventario conexion)
        {
            _GestionDeInventarios = new AdministracionDeInventario(conexion);
        }

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

            var ajuste = new GestionDeInventarios.Model.AjusteDeInventario
            {

                UserId = User.Identity.Name
            };

            GestionDeInventarios.Model.AjusteDeInventario ajusteDeInventarios;


            var httpClient = new HttpClient();

            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/AjusteDelInventario/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            ajusteDeInventarios = JsonConvert.DeserializeObject<AjusteDeInventario>(apiResponse);


            int idUsuario = int.Parse(ajusteDeInventarios.UserId);
            var resp = await httpClient.GetAsync($"https://localhost:7218/api/Usuarios/ObtenerNombreDelUsuarioPorId/{idUsuario}");
            string nombre = await resp.Content.ReadAsStringAsync();
            ajusteDeInventarios.UserId = nombre;

            return View(ajusteDeInventarios);
        }

        public async Task<ActionResult> DetalleDeInventario(int id)
        {


            GestionDeInventarios.Model.Inventario inventarios;

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            inventarios = JsonConvert.DeserializeObject<GestionDeInventarios.Model.Inventario>(apiResponse);

            return View(inventarios);
        }
        public async Task<ActionResult> ListadoDeAjustes(int id)
        {

            List<GestionDeInventarios.Model.AjusteDeInventario> laListaDelInventario;
            HttpClient httpClient = new HttpClient();

            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/AjusteDelInventario/ListaPorId/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();
            laListaDelInventario = JsonConvert.DeserializeObject<List<AjusteDeInventario>>(apiResponse);


            int _idDelUsuario = int.Parse(laListaDelInventario[0].UserId);

            var resp = await httpClient.GetAsync($"https://localhost:7218/api/Usuarios/ObtenerNombreDelUsuarioPorId/{_idDelUsuario}");
            string nombre = await resp.Content.ReadAsStringAsync();
            
            ViewBag.Nombre = nombre;

            return View(laListaDelInventario);
        }

        public async Task<ActionResult> CrearNuevoAjuste(int id)
        {
            var ajuste = new GestionDeInventarios.Model.AjusteDeInventario
            {

                UserId = User.Identity.Name
            };

            GestionDeInventarios.Model.Inventario inventarios;
            GestionDeInventarios.Model.AjusteDeInventario ajusteDeInventarios = new GestionDeInventarios.Model.AjusteDeInventario();

            var httpClient = new HttpClient();
            var respuesta = await httpClient.GetAsync($"https://localhost:7218/api/Inventario/{id}");
            string apiResponse = await respuesta.Content.ReadAsStringAsync();

            inventarios = JsonConvert.DeserializeObject<GestionDeInventarios.Model.Inventario>(apiResponse);

            ajusteDeInventarios.Id_Inventario = id;
            ajusteDeInventarios.CantidadActual = inventarios.Cantidad;
            ajusteDeInventarios.Fecha = DateTime.Now;
            ajusteDeInventarios.UserId = ajuste.UserId;


            return View(ajusteDeInventarios);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearNuevoAjuste(AjusteDeInventario ajusteDeInventarios)
        {
            GestionDeInventarios.Model.Inventario inventarios;
            GestionDeInventarios.Model.AjusteDeInventario ajusteDeInventariosAgregar = new GestionDeInventarios.Model.AjusteDeInventario();


            try
            {
                var httpClient = new HttpClient();
                String usuario = User.Identity.Name;
                var resp = await httpClient.GetAsync($"https://localhost:7218/api/AjusteDelInventario/ObtenerIdPorNombre/{usuario}");
                var nombre = await resp.Content.ReadAsStringAsync();


                //int UserId = _GestionDeInventarios.BusqueElIdDelUsuarioPorNombre(User.Identity.Name);

                var ajuste = new GestionDeInventarios.Model.AjusteDeInventario
                {

                    UserId = nombre
                }; 


                inventarios = _GestionDeInventarios.ObtengaElInventarioPorIdentificacion(ajusteDeInventarios.Id);


                ajusteDeInventariosAgregar.Id_Inventario = inventarios.Id;
                ajusteDeInventariosAgregar.CantidadActual = inventarios.Cantidad;
                ajusteDeInventariosAgregar.Ajuste = ajusteDeInventarios.Ajuste;
                ajusteDeInventariosAgregar.Tipo = ajusteDeInventarios.Tipo;
                ajusteDeInventariosAgregar.Observaciones = ajusteDeInventarios.Observaciones;
                ajusteDeInventariosAgregar.Fecha = DateTime.Now;
                ajusteDeInventariosAgregar.UserId = ajuste.UserId;

                _GestionDeInventarios.RegistreUnAjusteDeInventario(ajusteDeInventariosAgregar);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
