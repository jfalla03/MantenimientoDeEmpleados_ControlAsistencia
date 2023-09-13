using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks; // Asegúrate de reemplazar esto con tu namespace correcto
using WebApplication1.Models;
using System.Net.Mail;
using System.Net;

namespace WebApplication1.Controllers
{
	public class AccesoController : Controller
	{
		private readonly EFFICALContext db;

		public AccesoController(EFFICALContext contexto)
		{
			db = contexto;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> Index(Usuario _usuario)
        {
            var usuario = db.Usuarios.FirstOrDefault(u => u.Correo == _usuario.Correo && u.Clave == _usuario.Clave);

            if (usuario == null)
            {
                ViewBag.MensajeError = "Correo o contraseña incorrectos. Verifica tus datos e intenta nuevamente.";
                return View();
            }
            else
            {
                // Identificar el rol del usuario antes de crear las claims
                var roles = usuario.Roles.Split(',').Select(r => r.Trim()).ToList();

                var claims = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.Name, usuario.Nombre),
					new Claim("Correo", usuario.Correo),
				}, CookieAuthenticationDefaults.AuthenticationScheme);

                foreach (string rol in roles)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, rol));
                }

                // Asegúrate de que "Empleado" esté en la lista de roles antes de agregarlo a las claims
                if (roles.Contains("Empleado"))
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, "Empleado"));
                }

                var claimsPrincipal = new ClaimsPrincipal(claims);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                if (roles.Contains("Administrador") || roles.Contains("Empleado"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }


        }


        public async Task<IActionResult> Salir()
		{

			#region AUTENTICACTION
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			#endregion

			return RedirectToAction("Index");

		}

		public IActionResult RecuperarContraseña()
		{
			return View();
		}

		[HttpPost]
		public IActionResult RecuperarContraseña(string correo)
		{
			if (ModelState.IsValid)
			{
				// Verificar si el correo pertenece a un usuario registrado
				var usuario = db.Usuarios.FirstOrDefault(u => u.Correo == correo);
				if (usuario == null)
				{
					ModelState.AddModelError("", "El correo ingresado no es válido");
					return View();
				}

				// Eliminar cualquier registro existente para el usuario
				var registrosExistentes = db.RecuperarContraseñas.Where(r => r.Nombreusuario == usuario.Nombre);
				db.RecuperarContraseñas.RemoveRange(registrosExistentes);

				// Generar un token único (puede ser un GUID)
				string token = Guid.NewGuid().ToString();

				// Guardar el registro de recuperación en la base de datos
				var registroRecuperacion = new RecuperarContraseña
				{
					Nombreusuario = usuario.Nombre,
					Token = token,
					Expiracion = DateTime.Now.AddMinutes(30) // 15 minutos de expiración
				};
				db.RecuperarContraseñas.Add(registroRecuperacion);
				db.SaveChanges();

				// Enviar el correo electrónico con el enlace al usuario
				EnviarCorreo(usuario.Correo, token);

				ViewBag.CorreoEnviado = true;
			}

			return View();
		}

		private void EnviarCorreo(string correo, string token)
		{
			var link = Url.Action("CambiarContraseña", "Acceso", new { token }, protocol: Request.Scheme);

			var smtpClient = new SmtpClient("smtp.gmail.com", 587)
			{
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential("yoselinadiavr@gmail.com", "gbbvjyyerugttvsn"),
				EnableSsl = true
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("yoselinadiavr@gmail.com"),
				To = { correo },
				Subject = "Recuperación de contraseña",
				Body = $"Hola, para cambiar tu contraseña, haz clic en el siguiente enlace: {link}"
			};

			smtpClient.Send(mailMessage);
		}

		public IActionResult CambiarContraseña(string token)
		{
			// Aquí puedes verificar el token y la expiración
			var registroRecuperacion = db.RecuperarContraseñas.FirstOrDefault(r => r.Token == token && r.Expiracion >= DateTime.Now);
			if (registroRecuperacion == null)
			{
				// Token inválido o expirado
				return RedirectToAction("RecuperarContraseña");
			}

			var model = new CambiarContraseñaViewModel
			{
				Token = token
			};

			return View(model);
		}
		[HttpPost]
		public IActionResult CambiarContraseña(CambiarContraseñaViewModel model)
		{
			if (ModelState.IsValid)
			{
				var registroRecuperacion = db.RecuperarContraseñas.FirstOrDefault(r => r.Token == model.Token && r.Expiracion >= DateTime.Now);
				if (registroRecuperacion == null)
				{
					// Token inválido o expirado
					return RedirectToAction("RecuperarContraseña");
				}

				// Buscar al usuario por el nombre en el registro de recuperación
				var usuario = db.Usuarios.FirstOrDefault(u => u.Nombre == registroRecuperacion.Nombreusuario);

				if (usuario != null)
				{
					// Actualizar la contraseña del usuario con la nueva
					usuario.Clave = model.NuevaContraseña;

					// Eliminar el registro de recuperación
					db.RecuperarContraseñas.Remove(registroRecuperacion);

					// Guardar los cambios en la base de datos
					db.SaveChanges();

					ViewBag.ContraseñaCambiada = true;

					// Redirigir al usuario a la página principal de inicio de sesión
					return RedirectToAction("Index", "Acceso");
				}
				else
				{
					ModelState.AddModelError("", "No se encontró un usuario válido.");
				}
			}

			return View(model);
		}

	}
}

