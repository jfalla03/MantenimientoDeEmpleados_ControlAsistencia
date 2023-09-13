using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public partial class SueldoEmpleado
    {
        public string Id { get; set; }
        public string CodEmpleado { get; set; }
        public decimal Monto { get; set; }
        public string Periodo { get; set; }
        public string FechaPago { get; set; }
        public decimal Impuestos { get; set; }
        public string Comentarios { get; set; }

        public virtual Empleado CodEmpleadoNavigation { get; set; }
    }
}
