using GestionDeInventarios.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventario.DA
{
    public class DbGestionDeInventario : DbContext
    {

        public DbGestionDeInventario(DbContextOptions<DbGestionDeInventario> opciones) : base(opciones)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<HistorialDeInventario> Historicos { get; set; }
        public DbSet<AjusteDeInventario> AjustesDelInventario { get; set; }
        public DbSet<AperturaDeLaCaja> AperturasDeCaja { get; set; }
        public DbSet<Ventas> Ventas { get; set; }

        public DbSet<VentaDetalle> DetallesDeVenta { get; set; }
    }
}
