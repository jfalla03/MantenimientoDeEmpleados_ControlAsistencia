using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Controllers
{
    //[Authorize]
    public class HomeController : Controller
    {

        private readonly EFFICALContext _dbContext;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public HomeController(EFFICALContext dbContext, ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;

        }


        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult Index()
        {
            return View();
        }


        //Vistas Empleado//

        //Perfil//
        [Authorize(Roles = "Empleado")]
        public IActionResult Perfil()
        {
            var userEmail = User.FindFirst("Correo")?.Value;
            var empleado = _dbContext.Empleados.FirstOrDefault(e => e.EmailEmpleado == userEmail);

            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }


        //Actualizar Contraseña//
        [HttpPost]
        public IActionResult ActualizarContraseña(string contraseñaActual, string nuevaContraseña, string confirmarContraseña)
        {
            // Obtener el nombre de usuario del usuario autenticado
            string nombreUsuario = User.Identity.Name;

            // Buscar el usuario en la base de datos utilizando el nombre de usuario
            var usuario = _dbContext.Usuarios.FirstOrDefault(u => u.Nombre == nombreUsuario);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            // Verificar que la contraseña actual sea correcta
            if (usuario.Clave != contraseñaActual)
            {
                ModelState.AddModelError("contraseñaActual", "La contraseña actual no es correcta.");
                return View();
            }

            // Verificar que la nueva contraseña y la confirmación coincidan
            if (nuevaContraseña != confirmarContraseña)
            {
                ModelState.AddModelError("confirmarContraseña", "La confirmación de contraseña no coincide.");
                return View();
            }

            // Actualizar la contraseña con la nueva contraseña
            usuario.Clave = nuevaContraseña;

            _dbContext.SaveChanges();

            TempData["Mensaje"] = "Contraseña actualizada exitosamente.";

            return RedirectToAction("Index");
        }


        //Solicitud Inasistencia//
        [Authorize(Roles = "Empleado")]
        [HttpGet]
        public IActionResult Solicitar()
        {
            return View("Solicitar"); // El nombre de tu vista de marcación
        }

        [Authorize(Roles = "Empleado")]
        [HttpGet]
        public IActionResult MarcarAsistenciaView()
        {
            return View("MarcarAsistencia"); // El nombre de tu vista de marcación
        }



        //Marcar Asistencia//
        [Authorize(Roles = "Empleado")]
        [HttpPost]
        public IActionResult MarcarAsistencia(string tipoMarcacion)
        {
            // Obtener el nombre de usuario del usuario autenticado
            string nombreUsuario = User.Identity.Name;

            // Buscar el empleado en la base de datos utilizando el nombre de usuario
            var empleado = _dbContext.Empleados.FirstOrDefault(e => e.NombreUsuario == nombreUsuario);

            if (empleado == null)
            {
                return NotFound("Empleado no encontrado.");
            }

            // Obtener el registro de asistencia del empleado para la fecha actual
            var asistencia = _dbContext.ControlDeAsistencia.FirstOrDefault(a => a.CodEmpleado == empleado.CodEmpleado && a.FechAsistencia.Date == DateTime.Today);
            if (asistencia == null)
            {
                // Si no existe un registro de asistencia para la fecha actual, crear uno nuevo
                asistencia = new ControlDeAsistencium
                {
                    CodAsistencia = "AS" + (_dbContext.ControlDeAsistencia.Count() + 1).ToString("D3"),
                    FechAsistencia = DateTime.Today,
                    CodEmpleado = empleado.CodEmpleado
                };
                _dbContext.ControlDeAsistencia.Add(asistencia);
            }

            // Obtener la hora actual del sistema
            DateTime horaActual = DateTime.Now;

            // Dependiendo del tipo de marcación, actualizar la hora correspondiente
            string mensajeExito = string.Empty;
            string mensajeError = string.Empty;
            switch (tipoMarcacion)
            {
                case "Entrada":
                    asistencia.HoraEntrada = horaActual.TimeOfDay;
                    mensajeExito = $"Hora de entrada marcada exitosamente para {empleado.NomEmpleado} el {horaActual.ToString("dd/MM/yyyy")} a las {horaActual.ToString("HH:mm:ss")}.";
                    asistencia.EstadoAsistencia = "LABORANDO";
                    break;
                case "InicioDescanso":
                    asistencia.HoraDescansoInicio = horaActual.TimeOfDay;
                    mensajeExito = $"Inicio de descanso marcado exitosamente para {empleado.NomEmpleado} el {horaActual.ToString("dd/MM/yyyy")} a las {horaActual.ToString("HH:mm:ss")}.";
                    asistencia.EstadoAsistencia = "DESCANSO";
                    break;
                case "FinDescanso":
                    asistencia.HoraDescansoFin = horaActual.TimeOfDay;
                    mensajeExito = $"Fin de descanso marcado exitosamente para {empleado.NomEmpleado} el {horaActual.ToString("dd/MM/yyyy")} a las {horaActual.ToString("HH:mm:ss")}.";
                    asistencia.EstadoAsistencia = "LABORANDO";
                    break;
                case "Salida":
                    asistencia.HoraSalida = horaActual.TimeOfDay;
                    mensajeExito = $"Hora de salida marcada exitosamente para {empleado.NomEmpleado} el {horaActual.ToString("dd/MM/yyyy")} a las {horaActual.ToString("HH:mm:ss")}.";
                    asistencia.EstadoAsistencia = "FIN TURNO";
                    break;
                default:
                    return BadRequest("Tipo de marcación inválido.");

            }

            // Establecer el mensaje en TempData
            TempData["Mensaje"] = "Se ha marcado la asistencia exitosamente.";

            // Guardar los cambios en la base de datos
            _dbContext.SaveChanges();

            // Mostrar el mensaje de éxito para el tipo de marcación correspondiente
            ViewBag.Mensaje = mensajeExito;
            ViewBag.MensajeError = mensajeError;

            return View();
        }




        //ver mi sueldo//
        [Authorize(Roles = "Empleado")]
        public IActionResult VerSueldo()
        {
            // Obtener el código del empleado actual basado en el nombre de usuario
            string codigoEmpleado = ObtenerCodigoEmpleado();

            // Buscar el sueldo del empleado en la base de datos utilizando el código del empleado
            var sueldo = _dbContext.SueldoEmpleados.FirstOrDefault(s => s.CodEmpleado == codigoEmpleado);

            if (sueldo == null)
            {
                // Manejo si no se encuentra el sueldo para el usuario actual.
                return RedirectToAction("Error");
            }

            return View(sueldo);
        }

        private string ObtenerCodigoEmpleado()
        {
            string nombreUsuario = User.Identity.Name;

            // Buscar el empleado en la base de datos utilizando el nombre de usuario
            var empleado = _dbContext.Empleados.FirstOrDefault(e => e.NombreUsuario == nombreUsuario);

            if (empleado != null)
            {
                return empleado.CodEmpleado; // Devolver el código del empleado
            }

            // Si no se encuentra el empleado, devuelve un valor predeterminado o lanza una excepción
            // Aquí puedes decidir cómo manejar esta situación según tu lógica
            throw new Exception("No se pudo obtener el código del empleado actual.");
        }



        // Solicitar Justificacion//
        [Authorize(Roles = "Empleado")]
        [HttpPost]
        public IActionResult Solicitar(SolicitudesJustificacion solicitud, IFormFile RutaImagen, [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (ModelState.IsValid)
            {
                solicitud.CodEmpleado = ObtenerCodigoEmpleadoActual();
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.Estado = "Pendiente";

                _dbContext.SolicitudesJustificacions.Add(solicitud);
                _dbContext.SaveChanges();

                if (RutaImagen != null && RutaImagen.Length > 0)
                {
                    string nombreArchivo = Path.GetFileName(RutaImagen.FileName);
                    string rutaArchivoRelativa = Path.Combine("imagenes", nombreArchivo);

                    string rutaArchivoAbsoluta = Path.Combine(webHostEnvironment.WebRootPath, rutaArchivoRelativa);

                    using (var stream = new FileStream(rutaArchivoAbsoluta, FileMode.Create))
                    {
                        RutaImagen.CopyTo(stream);
                    }

                    solicitud.RutaImagen = rutaArchivoRelativa;
                    _dbContext.SaveChanges();
                }

                TempData["Mensaje"] = "Solicitud enviada correctamente.";
                return RedirectToAction("Index", "Home");
            }

            return View(solicitud);
        }
        private string ObtenerCodigoEmpleadoActual()
        {
            // Obtener el nombre de usuario del usuario autenticado
            string nombreUsuario = User.Identity.Name;

            // Buscar el empleado en la base de datos utilizando el nombre de usuario
            var empleado = _dbContext.Empleados.FirstOrDefault(e => e.NombreUsuario == nombreUsuario);

            if (empleado != null)
            {
                return empleado.CodEmpleado; // Devolver el código del empleado
            }

            // Si no se encuentra el empleado, devuelve un valor predeterminado o lanza una excepción
            // Aquí puedes decidir cómo manejar esta situación según tu lógica
            throw new Exception("No se pudo obtener el código del empleado actual.");
        }


        [Authorize(Roles = "Empleado")]
        public IActionResult ListarMisSolicitudes()
        {
            var empleadoId = ObtenerCodigoEmpleadoActual();
            var solicitudes = _dbContext.SolicitudesJustificacions
                .Where(s => s.CodEmpleado == empleadoId)
                .ToList();

            return View(solicitudes);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------//

        //Vistas Admin//

        //Lista de empleados//
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListaEmpleados()
        {
            var empleados = await _dbContext.Empleados.ToListAsync();
            return View(empleados);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = _dbContext.Empleados.Find(id);
            if (empleado == null)
            {
                return NotFound();
            }

            return View(empleado);
        }


        //Editar Empleados//
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Empleado empleado)
        {
            if (id != empleado.CodEmpleado)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(empleado);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.CodEmpleado))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListaEmpleados));
            }
            return View(empleado);
        }

        private bool EmpleadoExists(string id)
        {
            return _dbContext.Empleados.Any(e => e.CodEmpleado == id);
        }


        //[Authorize(Roles = "Administrador")]
        //public IActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var empleado = _dbContext.Empleados.Find(id);
        //    if (empleado == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(empleado);
        //}

        //[Authorize(Roles = "Administrador")]
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteConfirmed(string id)
        //{
        //    var empleado = _dbContext.Empleados.Find(id);
        //    _dbContext.Empleados.Remove(empleado);
        //    _dbContext.SaveChanges();
        //    return RedirectToAction(nameof(Index));
        //}


        //Crear usuario y empleado//
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }



        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Usuarios.Add(usuario);
                _dbContext.SaveChanges();

                TempData["Mensaje"] = "Usuario creado exitosamente.";

                return RedirectToAction("Index", "Home");
            }
            return View(usuario);
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult CreateEmpleado()
        {
            string mensaje = TempData["Mensaje"] as string;
            ViewBag.Mensaje = mensaje;

            return View();
        }



        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Empleados.Add(empleado);
                _dbContext.SaveChanges();
                return RedirectToAction("Index", "Home"); // Redirigir a la página principal o donde desees
            }
            return View(empleado);
        }




        //Listar sueldo de empleados//
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> listaSueldo()
        {
            var sueldo = await _dbContext.SueldoEmpleados.ToListAsync();
            return View(sueldo);
        }




        //Aprobar o rechazar solicitudes//
        [Authorize(Roles = "Administrador")]
        public IActionResult VerSolicitudes()
        {
            var solicitudes = _dbContext.SolicitudesJustificacions.ToList();
            return View(solicitudes);
        }

        [Authorize(Roles = "Administrador")]
       public IActionResult Aprobar(int cod)
        {
            var solicitud = _dbContext.SolicitudesJustificacions.FirstOrDefault(s => s.CodSolicitud == cod);

            if (solicitud != null)
            {
                solicitud.Estado = "Aprobada";
                _dbContext.SaveChanges();
            }

            return RedirectToAction("VerSolicitudes");
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Rechazar(int cod)
        {
            var solicitud = _dbContext.SolicitudesJustificacions.FirstOrDefault(s => s.CodSolicitud == cod);

            if (solicitud != null)
            {
                solicitud.Estado = "Rechazada";
                _dbContext.SaveChanges();
            }

            return RedirectToAction("VerSolicitudes");
        }


        [Authorize(Roles = "Administrador")]
        public IActionResult DescargarImagen(string filePath)
        {
            // Verifica si el archivo existe
            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                var fileName = Path.GetFileName(filePath);

                // Determina el tipo de contenido adecuado según la extensión del archivo
                var contentType = "application/octet-stream"; // Tipo de contenido genérico por defecto

                var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();

                if (fileExtension == ".jpg" || fileExtension == ".jpeg")
                {
                    contentType = "image/jpeg";
                }
                else if (fileExtension == ".png")
                {
                    contentType = "image/png";
                }
                else if (fileExtension == ".pdf")
                {
                    contentType = "application/pdf";
                }
                // Agrega más tipos de contenido según las extensiones de archivo que utilices

                return File(fileBytes, contentType, fileName);
            }
            else
            {
                return NotFound();
            }
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}