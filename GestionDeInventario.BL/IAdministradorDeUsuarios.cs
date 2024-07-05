using GestionDeInventarios.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventario.BL
{
    public interface IAdministradorDeUsuarios
    {

        public String ObtengaElNombreDelUsuarioPorID(int id);
        public Usuario? CreeUnNuevoUsuario(UsuarioRegistro usuarioRegistro);
        public void RegistreUnNuevoUsuarioConOAuth(UsuarioRegistroOAuth _usuarioOAuth);
        public Usuario? ObtengaElInicioDeSesionDelUsuario(string email, string password);
        public Boolean EstaRegistradoElCorreo(String email);
        public void InicieLaSesionConOAuth(UsuarioRegistroOAuth _usuarioOAuth);
        public Boolean EstaRegistradoElUsuarioConOAuth(String idOauth);
        public void EnvieUnCorreoInformativoAlUsuario(String _emailDeDestino, TipoDeCorreo _tipoDeCorreo, Usuario _user);
        public void ActualiceLosDatosDelUsuario(UsuarioActualizado _usuario, String _email, String username);
    }
}
