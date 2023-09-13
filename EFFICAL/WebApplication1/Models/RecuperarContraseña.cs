using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class RecuperarContraseña
    {
        public string Nombreusuario { get; set; }
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }

        public virtual Usuario NombreusuarioNavigation { get; set; }
    }
}
