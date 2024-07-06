using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventarios.Model
{

    [Table("Users")]//tabla en la base de datos
    public class Usuario
    {
        [Key]
        public int ID { get; set; }
        public string? OauthID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Rol? Role { get; set; }
        public int? LoginAttempts { get; set; } = 0;
        public Boolean? IsBlocked { get; set; } = false;
        public DateTime? BlockedUntil { get; set; }
        public Boolean? Suscrito { get; set; } = false;
    }
}
