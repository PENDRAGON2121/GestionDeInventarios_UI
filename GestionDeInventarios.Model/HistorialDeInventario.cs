using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GestionDeInventarios.Model
{
    [Table("HistorialDeInventario")]
    public class HistorialDeInventario
    {
        [HiddenInput]
        public int Id { get; set; }

        public int InventarioId { get; set; }

        public string Usuario { get; set; } = null!;

        public DateTime Fecha { get; set; }

        public TipoModificacion TipoModificacion { get; set; }

    }
}
