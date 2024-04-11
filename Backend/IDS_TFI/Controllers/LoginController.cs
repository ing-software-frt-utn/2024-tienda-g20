using IDS_TFI.Data;
using IDS_TFI.Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDS_TFI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		// POST: api/Login
		/// <summary>
		/// Recibe credenciales de inicio de sesión y retorna el objeto empleado correspondiente
		/// </summary>
		/// <returns>
		/// - 200 + el empleado al tener datos correctos<br/>
		/// - 401 para contraseña incorrecta
		/// - 404 si no se encuentra el usuario
		/// - 400 si el usuario o la contraseña ingresada estan vacios
		/// </returns>
		[HttpPost]
		public async Task<ActionResult<LoginOutput>> Login(LoginData credenciales)
		{
			if (string.IsNullOrWhiteSpace(credenciales.Usuario) || string.IsNullOrWhiteSpace(credenciales.Contra))
				BadRequest();

			var empleados = await context.Empleados.ToListAsync();

			foreach (var empleado in empleados)
			{
				if (empleado.Usuario == credenciales.Usuario)
				{
					if (empleado.Contrasena == credenciales.Contra)
					{
						return Ok(new LoginOutput(
							empleado,
							Autenticacion.CrearToken(empleado)
						));
					}
					else
						return Unauthorized("Contraseña incorrecta");
				}
			}
			return NotFound();
		}

		public record LoginData(string Usuario, string Contra);

		public record LoginOutput(Empleado Empleado, LoginToken Token);
	}
}
