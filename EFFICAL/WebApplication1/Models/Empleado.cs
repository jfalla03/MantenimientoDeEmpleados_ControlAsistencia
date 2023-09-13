using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public partial class Empleado
    {
        public Empleado()
        {
            ControlDeAsistencia = new HashSet<ControlDeAsistencium>();
            SolicitudesJustificacions = new HashSet<SolicitudesJustificacion>();
            SueldoEmpleados = new HashSet<SueldoEmpleado>();
        }


        [Required(ErrorMessage = "Campo requerido.")]
        public string CodEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string NomEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string ApeEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string DniEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string DirecMpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string TeleEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string EmailEmpleado { get; set; }


        [Required(ErrorMessage = "Campo requerido.")]
        public string NombreUsuario { get; set; }


        public virtual Usuario NombreUsuarioNavigation { get; set; }
        public virtual ICollection<ControlDeAsistencium> ControlDeAsistencia { get; set; }
        public virtual ICollection<SolicitudesJustificacion> SolicitudesJustificacions { get; set; }
        public virtual ICollection<SueldoEmpleado> SueldoEmpleados { get; set; }
    }
}
