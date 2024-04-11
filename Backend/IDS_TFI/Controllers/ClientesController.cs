using IDS_TFI.Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDS_TFI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ClientesController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpGet("{DNI}")]
		public ActionResult<Cliente> BuscarCliente(int DNI)
		{
			var cliente = BuscarCliente(context, DNI);
			if (cliente == null)
				return NotFound();
			else
				return cliente;
		}

		public static Cliente? BuscarCliente(DataContext context, int DNI)
		{
			var cliente = context.Clientes
				.Where((c) => c.DNI == DNI)
				.Include((c) => c.Direccion)
				.Include((c) => c.Direccion!.Provincia)
				.Include((c) => c.Direccion!.Provincia!.Pais)
				.FirstOrDefault();
			return cliente;
		}
	}
}
