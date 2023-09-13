using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class ControlDeAsistencium
    {
        public string CodAsistencia { get; set; }
        public DateTime FechAsistencia { get; set; }
        public TimeSpan? HoraEntrada { get; set; }
        public TimeSpan? HoraDescansoInicio { get; set; }
        public TimeSpan? HoraDescansoFin { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public string EstadoAsistencia { get; set; }
        public string CodEmpleado { get; set; }

        public virtual Empleado CodEmpleadoNavigation { get; set; }
    }
}
