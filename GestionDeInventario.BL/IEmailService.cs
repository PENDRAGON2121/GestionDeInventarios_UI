using GestionDeInventarios.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeInventario.BL
{
    public interface IEmailService
    {
        void SendEmail(EmailDTO _request);
    }
}
