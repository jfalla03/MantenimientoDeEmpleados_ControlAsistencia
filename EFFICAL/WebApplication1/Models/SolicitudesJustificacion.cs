using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SolicitudesJustificacion
    {
        public int CodSolicitud { get; set; }
        public string CodEmpleado { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
        public string RutaImagen { get; set; }

        public virtual Empleado CodEmpleadoNavigation { get; set; }
    }
}
