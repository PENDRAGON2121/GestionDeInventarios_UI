using GestionDeInventarios.Model;
using GestorDeInventarios.UI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace GestorDeInventario.UI.Controllers
{
    public class LoginController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registro()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(UsuarioRegistro _usuario)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Verifique que los datos estén correctos.");
                return View();
            }
            
            var httpClient = new HttpClient();

            string json = JsonConvert.SerializeObject(_usuario);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Usuarios/Registre", byteContent);

            return RedirectToAction("Index", "login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UsuarioSesion usuario)
        {

            //Conexion con api
            var httpClient = new HttpClient();
            string json = JsonConvert.SerializeObject(usuario);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Usuarios/ObtengaElInicioDeSesion", byteContent);

            //Deserializacion de la respuesta
            string apiResponse = await respuesta.Content.ReadAsStringAsync(); 
            Usuario? _usuario= JsonConvert.DeserializeObject<Usuario>(apiResponse);

            

            if (_usuario.Email is null)
            {

                ModelState.AddModelError(string.Empty, "ERROR EN EL INGRESO");
                ModelState.AddModelError(string.Empty, "INTENTOS FALLIDOS:" + _usuario.LoginAttempts);
                if (_usuario.IsBlocked is true)
                {
                    ModelState.AddModelError(string.Empty, "BLOQUEADO HASTA: " + _usuario.BlockedUntil);
                }

                return View();
            }
            String role = _usuario.Role.ToString();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _usuario.Name),
                new Claim(ClaimTypes.Email, _usuario.Email),
                new Claim(ClaimTypes.Role, role),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult ActualizarDatosDelUsuario()
        {
            var username = User.Identity.Name;

            ViewBag.Username = username;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarDatosDelUsuario(UsuarioActualizado _usuarioActualizado)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Verifique que los datos estén correctos.");
                return View();
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var username = User.Identity.Name;

            _usuarioActualizado.Correo = email;
            _usuarioActualizado.NombreDelUsuario = username;


            //Conexion con api
            var httpClient = new HttpClient();
            string json = JsonConvert.SerializeObject(_usuarioActualizado);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Usuarios/Actualice", byteContent);

            //respuesta es ok
            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToAction("Logout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar los datos.");
                return View();
            }
        }

        //Controlador de sesion con Google
        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                }
            );
        }

        //Respuesta de Google
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            UsuarioRegistroOAuth _usuario = new UsuarioRegistroOAuth
            {
                IdOauth = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = result.Principal.FindFirstValue(ClaimTypes.Name),
                Email = result.Principal.FindFirstValue(ClaimTypes.Email)
            };

            //Conexion con api
            var httpClient = new HttpClient();
            string json = JsonConvert.SerializeObject(_usuario);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Usuarios/InicioDeSesionConOAuth", byteContent);

            //respuesta es ok
            if (respuesta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "login");

        }

        // Controlador de sesion con Facebook
        public async Task LoginFacebook()
        {
            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("FacebookResponse")
                }
            );
        }

        //Respuesta de facebook
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            UsuarioRegistroOAuth _usuario = new UsuarioRegistroOAuth
            {
                IdOauth = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier),
                Name = result.Principal.FindFirstValue(ClaimTypes.Name),
                Email = result.Principal.FindFirstValue(ClaimTypes.Email)
            };

            //Conexion con api
            var httpClient = new HttpClient();
            string json = JsonConvert.SerializeObject(_usuario);
            var buffer = System.Text.Encoding.UTF8.GetBytes(json);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var respuesta = await httpClient.PostAsync("https://localhost:7218/api/Usuarios/InicioDeSesionConOAuth", byteContent);

            //respuesta es ok
            if (respuesta.IsSuccessStatusCode)
            {
            return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "login");

        }

        //Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "login");
        }
    }
}
