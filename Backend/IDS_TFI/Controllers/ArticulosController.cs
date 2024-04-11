using IDS_TFI.Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDS_TFI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ArticulosController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpGet("{id}")]
		public ActionResult<Articulo> GetArticulo(int id)
		{
			var art = context.Articulos
				.Where((x) => x.Id == id)
				.Include((x) => x.Marca)
				.Include((x) => x.Categoria)
				.Include((x) => x.Colores)
				.Include((x) => x.Talles)!.ThenInclude((t) => t.Tipo)
				.FirstOrDefault();
			if (art == null)
				return NotFound();
			else
				return Ok(art);
		}

		[HttpGet("stock/{id}/{talle}/{color}")]
		public ActionResult<Stock> GetStock(int id, int talle, int color)
		{
			var prod = GetStock(context, id, talle, color);

			if (prod == null)
				return NotFound();
			else
				return Ok(prod);
		}

		[HttpGet]
		public ActionResult<IEnumerable<Articulo>> GetArticulos()
		{
			return context.Articulos;
		}

		public static Stock? GetStock(DataContext context, int id, int talle, int color)
		{
			var art = context.Articulos.Find(id);
			if (art == null)
				return null;

			var prod = context.Stock
				.Where((p) => p.Articulo!.Id == id && p.Talle!.Id == talle && p.Color!.Id == color)
				.Include((p) => p.Articulo)
				.Include((p) => p.Talle)
				.Include((p) => p.Color)
				.FirstOrDefault();

			if (prod == null)
				return null;
			else
				return prod;
		}
	}
}
