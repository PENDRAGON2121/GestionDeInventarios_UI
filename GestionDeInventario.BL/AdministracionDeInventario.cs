using GestionDeInventario.DA;
using GestionDeInventarios.Model;

namespace GestionDeInventario.BL
{
    public class AdministracionDeInventario
    {
        public readonly DbGestionDeInventario Conexion;
        public List<Inventario> listaDeInventario = new List<Inventario>();

        public AdministracionDeInventario(DbGestionDeInventario contexto)
        {
            Conexion = contexto;
        }

        public void RegistreUnInventario(Inventario inventarios, String usuario)
        {
            inventarios.Cantidad = 0;
            Conexion.Inventarios.Add(inventarios);
            Conexion.SaveChanges();

            RegistreHistorialDeCambios(inventarios.Id, usuario, TipoModificacion.Creacion);


        }
        public List<Inventario> ObtengaLaLista()
        {


            foreach (var item in Conexion.Inventarios)
            {

                listaDeInventario.Add(new Inventario { Id = item.Id, Nombre = item.Nombre, Categoria = item.Categoria, Cantidad = item.Cantidad, Precio = item.Precio });

            }

            return listaDeInventario;

        }
        public List<Inventario> ObtengaLaListaDeAjustes()
        {
            foreach (var item in Conexion.Inventarios)
            {
                listaDeInventario.Add(new Inventario { Id = item.Id, Nombre = item.Nombre, Categoria = item.Categoria, Cantidad = item.Cantidad, Precio = item.Precio });
            }
            return listaDeInventario;

        }
        public List<Inventario> ObtengaLaListaPorNombre(string nombre)
        {



            List<Inventario> laListaDeInventarioFiltradaPorNombre;

            laListaDeInventarioFiltradaPorNombre = Conexion.Inventarios.Where(x => x.Nombre.Contains(nombre)).ToList();

            return laListaDeInventarioFiltradaPorNombre;
        }
        public Inventario ObtengaElInventarioPorIdentificacion(int Id)
        {
            foreach (var inventario in Conexion.Inventarios)
            {
                if (inventario.Id == Id)
                { return inventario; }
            }
            return null;
        }
        public void EditeElInventario(int Id, string Nombre, Categoria Categoria, Decimal Precio, String usuario)
        {
            Inventario inventario;

            inventario = ObtengaElInventarioPorIdentificacion(Id);

            inventario.Id = Id;
            inventario.Nombre = Nombre;
            inventario.Categoria = Categoria;
            inventario.Precio = Precio;

            Conexion.Inventarios.Update(inventario);
            Conexion.SaveChanges();

            RegistreHistorialDeCambios(Id, usuario, TipoModificacion.Modificacion);

        }
        public List<HistorialDeInventario> ObtengaElHistorialDeCambios(int id)
        {
            List<HistorialDeInventario> historialDeCambios = new List<HistorialDeInventario>();
            foreach (var item in Conexion.Historicos)
            {
                if (item.InventarioId == id)
                {
                    HistorialDeInventario _historial = new HistorialDeInventario();
                    _historial.Id = item.Id;
                    _historial.TipoModificacion = item.TipoModificacion;
                    _historial.Fecha = item.Fecha;
                    _historial.Usuario = item.Usuario;
                    historialDeCambios.Add(_historial);
                }
            }

            return historialDeCambios;
        }
        public void RegistreHistorialDeCambios(int idInventario, String Usuario, TipoModificacion tipoModificacion)
        {
            HistorialDeInventario historialDeInventario = new HistorialDeInventario();
            historialDeInventario.InventarioId = idInventario;
            historialDeInventario.Usuario = Usuario;
            historialDeInventario.Fecha = System.DateTime.Now;

            if (TipoModificacion.Creacion == tipoModificacion)
            {
                historialDeInventario.TipoModificacion = TipoModificacion.Creacion;
            }
            else
            {
                historialDeInventario.TipoModificacion = TipoModificacion.Modificacion;
            }

            Conexion.Historicos.Add(historialDeInventario);
            Conexion.SaveChanges();

        }

        public AjusteDeInventario ObtengaElAjustesPorIdentificacion(int Id)
        {
            AjusteDeInventario ajusteDeInventarios = new AjusteDeInventario();


            foreach (var item in Conexion.AjustesDelInventario)
            {

                if (item.Id == Id)
                {
                    ajusteDeInventarios.Id = item.Id;
                    ajusteDeInventarios.CantidadActual = item.CantidadActual;
                    ajusteDeInventarios.Ajuste = item.Ajuste;
                    ajusteDeInventarios.Tipo = item.Tipo;
                    ajusteDeInventarios.Observaciones = item.Observaciones;
                    ajusteDeInventarios.Fecha = item.Fecha;
                    ajusteDeInventarios.UserId = item.UserId;


                    return ajusteDeInventarios;

                }

            }

            return null;

        }
        public List<AjusteDeInventario> ObtengaLaListaDeAjustesPorIdentificacion(int Id)
        {
            List<AjusteDeInventario> laListaDeAjustesFiltradaPorIdentificacion;
            laListaDeAjustesFiltradaPorIdentificacion = Conexion.AjustesDelInventario.Where(x => x.Id_Inventario == Id).ToList();
            return laListaDeAjustesFiltradaPorIdentificacion;
        }
        public void RegistreUnAjusteDeInventario(GestionDeInventarios.Model.AjusteDeInventario ajusteDeInventarios)
        {
            Inventario inventarios = ObtengaElInventarioPorIdentificacion(ajusteDeInventarios.Id_Inventario);

            if (inventarios != null)
            {
                ajusteDeInventarios.CantidadActual = inventarios.Cantidad;

                if (ajusteDeInventarios.Tipo == TipoDeAjuste.Aumento)
                {
                    inventarios.Cantidad = inventarios.Cantidad + ajusteDeInventarios.Ajuste;
                }
                else if (ajusteDeInventarios.Tipo == TipoDeAjuste.Disminucion)
                {
                    if (inventarios.Cantidad - ajusteDeInventarios.Ajuste >= 0)
                    {
                        inventarios.Cantidad = inventarios.Cantidad - ajusteDeInventarios.Ajuste;
                    }
                    else
                    {
                        inventarios.Cantidad = 0;
                    }
                }
                Conexion.AjustesDelInventario.Add(ajusteDeInventarios);
                ActualiceElInventario(inventarios);
            }

        }
        public void ActualiceElInventario(GestionDeInventarios.Model.Inventario inventario)
        {
            Conexion.Inventarios.Update(inventario);
            Conexion.SaveChanges();
        }
        public int BusqueElIdDelUsuarioPorNombre(string nombre)
        {
            foreach (var item in Conexion.Usuarios)
            {
                if (item.Name == nombre)
                {
                    return item.ID;
                }
            }
            return 0;
        }
        public Boolean TieneAperturaDeCaja(String USERID)
        {
            var lista = Conexion.AperturasDeCaja.ToList();

            if (lista.Count != 0)
            {
                var item = lista.Last();
                if (item.UserId == USERID && item.Estado == EstadoDeCaja.Cerrada)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }


        }
        public List<GestionDeInventarios.Model.AperturaDeLaCaja> ObtengaLaListaDeCajas()
        {
            var lista = Conexion.AperturasDeCaja.ToList();
            return lista;

        }
        public void RegistreUnaAperturaDeCaja(AperturaDeCajaNueva apertura)
        {
            AperturaDeLaCaja nuevaApertura = new AperturaDeLaCaja();
            nuevaApertura.UserId = apertura.UserId;
            nuevaApertura.FechaDeInicio = apertura.FechaDeInicio;
            nuevaApertura.Estado = apertura.Estado;
            Conexion.AperturasDeCaja.Add(nuevaApertura);
            Conexion.SaveChanges();
        }
        public AperturaDeLaCaja ObtengaLaAperturaDeCajasPorIdentificacion(int Id)
        {

            AperturaDeLaCaja aperturasDeCaja = new AperturaDeLaCaja();

            foreach (var item in Conexion.AperturasDeCaja)
            {

                if (item.Id == Id)
                {
                    aperturasDeCaja.Id = item.Id;
                    aperturasDeCaja.UserId = item.UserId;
                    aperturasDeCaja.FechaDeInicio = item.FechaDeInicio;
                    aperturasDeCaja.FechaDeCierre = item.FechaDeCierre;
                    aperturasDeCaja.Estado = item.Estado;


                    return aperturasDeCaja;

                }

            }

            return null;

        }
        public void CerrarUnaCaja()
        {
            AperturaDeLaCaja aperturasDeCaja = ObtengaLaUltimaAperturaDeCaja();

            aperturasDeCaja.FechaDeCierre = DateTime.Now;
            aperturasDeCaja.Estado = EstadoDeCaja.Cerrada;
            Conexion.AperturasDeCaja.Update(aperturasDeCaja);
            Conexion.SaveChanges();
        }
        public AperturaDeLaCaja ObtengaLaUltimaAperturaDeCaja()
        {
            var lista = Conexion.AperturasDeCaja.ToList();
            var item = lista.Last();
            return item;
        }
        public AcumuladoDeVentas ObtengaElAcumuladoDeCaja(int idCaja)
        {
            AcumuladoDeVentas acumulado = new AcumuladoDeVentas();
            decimal sinpe = 0;
            decimal efectivo = 0;
            decimal tarjeta = 0;

            foreach (var item in Conexion.Ventas)
            {

                if (item.TipoDePago == TipoDePago.Tarjeta && item.IdAperturaDeCaja == idCaja)
                {
                    tarjeta = tarjeta + item.Total;
                }
                else if (item.TipoDePago == TipoDePago.Efectivo && item.IdAperturaDeCaja == idCaja)
                {
                    efectivo = efectivo + item.Total;
                }
                else if (item.TipoDePago == TipoDePago.SinpeMovil && item.IdAperturaDeCaja == idCaja)
                {
                    sinpe = sinpe + item.Total;
                }

            }
            acumulado.AcomuladoEfectivo = efectivo;
            acumulado.AcumuladoTarjeta = tarjeta;
            acumulado.AcumuladoSinpeMovil = sinpe;


            return acumulado;
        }

    }
}
