using GestionDeInventario.BL;
using GestionDeInventario.DA;
using GestionDeInventarios.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeInventario.UI.Controllers
{
    public class AperturaDeCajaController : Controller
    {
        private readonly AdministracionDeInventario _GestionDeInventarios;
        private readonly IAdministradorDeUsuarios _administradorDeUsuarios;

        public AperturaDeCajaController(DbGestionDeInventario conexion)
        {
            _GestionDeInventarios = new AdministracionDeInventario(conexion);
            _administradorDeUsuarios = new AdministradorDeUsuarios(conexion);
        }

        public ActionResult Index()
        {
            var ajuste = new GestionDeInventarios.Model.AperturaDeCajaNueva
            {

                UserId = User.Identity.Name
            };

            if (TempData.ContainsKey("Mensaje"))
            {
                ViewBag.Mensaje = TempData["Mensaje"].ToString();
            }

            ViewBag.EstaLaCajaAbierta = _GestionDeInventarios.TieneAperturaDeCaja(ajuste.UserId);
            var laListaDelInventario = _GestionDeInventarios.ObtengaLaListaDeCajas();
            if (laListaDelInventario.Count == 0)
            {
                return RedirectToAction("CrearAperturaDeCaja");
            }
            else
            {
                return View(laListaDelInventario);
            }

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
        public ActionResult CrearAperturaDeCaja(AperturaDeCajaNueva apertura)
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

                _GestionDeInventarios.RegistreUnaAperturaDeCaja(aperturasDeCaja);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult CerrarLaCaja(int Id)
        {
            var caja = _GestionDeInventarios.ObtengaLaAperturaDeCajasPorIdentificacion(Id);

            return View("CerrarCaja", caja);

        }
        [HttpPost]
        public ActionResult CerrarLaCaja()
        {
            var ultimoRegistro = _GestionDeInventarios.ObtengaLaUltimaAperturaDeCaja();
            return View("CerrarLaCaja", ultimoRegistro);

        }

        public ActionResult AcumuladoDeVentas(int id)
        {
            AcumuladoDeVentas acumulado;

            acumulado = _GestionDeInventarios.ObtengaElAcumuladoDeCaja(id);


            return View("AcumuladoDeVentas", acumulado);
        }
        public ActionResult validarCierre(int id)
        {
            _GestionDeInventarios.CerrarUnaCaja();
            return RedirectToAction("Index");


        }
    }
}
