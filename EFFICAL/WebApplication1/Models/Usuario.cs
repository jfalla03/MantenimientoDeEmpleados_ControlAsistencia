using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Empleados = new HashSet<Empleado>();
        }

        [Required(ErrorMessage = "El campo Nombre de Usuario es requerido.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El campo Correo de Usuario es requerido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El campo Clave de Usuario es requerido.")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El campo Roles de Usuario es requerido.")]
        public string Roles { get; set; }

        public virtual RecuperarContraseña RecuperarContraseña { get; set; }
        public virtual ICollection<Empleado> Empleados { get; set; }
    }
}
