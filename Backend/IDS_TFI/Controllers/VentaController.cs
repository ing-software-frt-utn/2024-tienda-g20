using IDS_TFI.Data;
using IDS_TFI.Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IDS_TFI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VentaController(DataContext context) : ControllerBase
	{
		private const double MAXIMO_VENTAS_ANONIMAS = 90000;

		private readonly DataContext context = context;

		[HttpGet("pagos")]
		public ActionResult<IEnumerable<TipoDePago>> TiposDePago()
		{
			var tipos = from t in Enum.GetValues<TipoDePago>() select t.ToString();
			return Ok(tipos);
		}

		[HttpPost("pagar")]
		public async Task<ActionResult<Mensaje>> Pagar(PagoRequest info,
			[FromHeader(Name = Autenticacion.ID_HEADER)] string userID,
			[FromHeader(Name = Autenticacion.HASH_HEADER)] string userHash)
		{
			if (!Autenticacion.Verificar(userID, userHash, context, Rol.Vendedor))
				return Unauthorized(new Mensaje("No se encuentra autorizado"));

			if (info.Carrito.Productos.Count==0)
				return BadRequest(new Mensaje("Lista de productos vacía"));

			int nroVenta = Generador.GenerarNumero(9);

			var venta = new Venta();
			venta.NroVenta = nroVenta;
			Cliente? cliente = null;
			if (info.Dni == null)
				venta.Cliente = null;
			else
			{
				cliente = ClientesController.BuscarCliente(context, info.Dni.Value);
				venta.Cliente = cliente;
			}

			foreach (var (producto, cantidad) in ObtenerLineas(info.Carrito))
				venta.AgregarLinea(producto, cantidad);

			var condicion = cliente?.CondicionTributaria ?? CondicionTributaria.ConsumidorFinal;
			TipoDeComprobante? tipoDeComprobante;

			if (condicion == CondicionTributaria.ResponsableInscripto || condicion == CondicionTributaria.Monotributo)
			{
				tipoDeComprobante = context.TiposDeComprobantes.Find(DatosIniciales.COMPROBANTE_FACTURA_A);
			}
			else
			{
				tipoDeComprobante = context.TiposDeComprobantes.Find(DatosIniciales.COMPROBANTE_FACTURA_B);
			}

			var comprobante = new Comprobante()
			{
				NroComprobante = Generador.GenerarGranNumero(18), //Normalmente sería por AFIP
				Tipo = tipoDeComprobante
			};

			venta.Pagar(cliente, info.TipoDePago, comprobante);

			double monto = venta.Total();

			if (info.TipoDePago == TipoDePago.Efectivo)
			{
				if (monto > MAXIMO_VENTAS_ANONIMAS && info.Dni == null)
				{
					return BadRequest(new Mensaje("No se permite venta anónima para el monto actual"));
				}

				var _msj = ConfirmarVenta(venta);
				return Ok(_msj);
			}

			if (cliente == null)
				return BadRequest(new Mensaje("No se permite venta anónima con tarjeta"));

			if (info.Tarjeta == null)
				return BadRequest(new Mensaje("Datos de tarjeta faltantes"));

			var datosTarjeta = info.Tarjeta;

			var request = new RequestInfo()
			{
				AñoVencimiento = datosTarjeta.Vencimiento.Year.ToString()[^2..],
				MesVencimiento = datosTarjeta.Vencimiento.Month.ToString(),
				CodigoSeguridad = datosTarjeta.CodigoSeguridad.ToString(),
				NumeroTarjeta = datosTarjeta.NumeroTarjeta.ToString(),
				NombrePropietario = cliente.RazonSocial,
				Identificacion = new Identification()
				{
					Tipo = "dni",
					Numero = cliente.DNI.ToString()
				}
			};

			var autorizacion = await AutorizacionDePagos.Autorizar(request, nroVenta.ToString(), monto);

			if (autorizacion != ResultadoAutorizacion.Exitoso)
				return Unauthorized(new Mensaje("Pago rechazado") { Objeto = autorizacion.ToString() });

			var msj = ConfirmarVenta(venta);
			return Ok(msj);
		}

		private Mensaje ConfirmarVenta(Venta venta)
		{
			context.Ventas.Add(venta);
			context.SaveChanges();
			return new Mensaje("Pago exitoso")
			{
				Objeto = venta
			};
		}

		[HttpPost("iniciar")]
		public ActionResult<Carrito> IniciarVenta(
			[FromHeader(Name = Autenticacion.ID_HEADER)] string userID,
			[FromHeader(Name = Autenticacion.HASH_HEADER)] string userHash)
		{
			if (!Autenticacion.Verificar(userID, userHash, context, Rol.Vendedor))
				return Unauthorized();

			return Ok(new Carrito());
		}

		[HttpPost("agregar")]
		public ActionResult<Carrito> AgregarProducto(VentaRequest info,
			[FromHeader(Name = Autenticacion.ID_HEADER)] string userID,
			[FromHeader(Name = Autenticacion.HASH_HEADER)] string userHash)
		{
			if (!Autenticacion.Verificar(userID, userHash, context, Rol.Vendedor))
				return Unauthorized();

			if (info.Carrito == null)
				return BadRequest();

			var prod = ArticulosController.GetStock(context, info.Articulo, info.Talle, info.Color);
			if (prod == null)
				return NotFound();

			info.Carrito.Agregar(new InfoVenta(prod.Id, info.Cantidad));
			return Ok(info.Carrito);
		}

		[HttpPost("quitar")]
		public ActionResult<Carrito> QuitarProducto(VentaRequest info,
			[FromHeader(Name = Autenticacion.ID_HEADER)] string userID,
			[FromHeader(Name = Autenticacion.HASH_HEADER)] string userHash)
		{
			if (!Autenticacion.Verificar(userID, userHash, context, Rol.Vendedor))
				return Unauthorized();

			if (info.Carrito == null)
				return BadRequest();

			var prod = ArticulosController.GetStock(context, info.Articulo, info.Talle, info.Color);
			if (prod == null)
				return NotFound();

			info.Carrito.Agregar(new InfoVenta(prod.Id, -info.Cantidad));
			return Ok(info.Carrito);
		}

		[HttpPost("carrito")]
		public ActionResult<IEnumerable<LineaDeVenta>> ListarProductos(Carrito carrito,
			[FromHeader(Name = Autenticacion.ID_HEADER)] string userID,
			[FromHeader(Name = Autenticacion.HASH_HEADER)] string userHash)
		{
			if (!Autenticacion.Verificar(userID, userHash, context, Rol.Vendedor))
				return Unauthorized();

			if (carrito == null)
				return BadRequest();

			var lineas = ObtenerLineas(carrito);

			var lineasDeVenta = from l in lineas
								select new LineaDeVenta()
								{
									Cantidad = l.cantidad,
									Producto = l.producto
								};

			return Ok(lineasDeVenta);
		}

		private List<(Stock producto, int cantidad)> ObtenerLineas(Carrito carrito)
		{
			//Estoy seguro que hay una mejor forma de hacer esto
			var lista = new Dictionary<int, int>();

			foreach (var i in carrito.Productos)
			{
				if (i.Cantidad <= 0) continue;
				lista.Add(i.Producto, i.Cantidad);
			}

			var ids = lista.Keys.ToArray();

			var productos = context.Stock
				.Where((p) => ids.Contains(p.Id))
				.Include((p) => p.Color)
				.Include((p) => p.Talle)
				.Include((p) => p.Talle!.Tipo)
				.Include((p) => p.Articulo)
				.Include((p) => p.Articulo!.Categoria)
				.Include((p) => p.Articulo!.Marca);

			var lineasDeVenta = new List<(Stock s, int cantidad)>();
			foreach (var p in productos)
				lineasDeVenta.Add(new(p, lista[p.Id]));

			return lineasDeVenta;
		}
	}

	public record VentaRequest(Carrito Carrito, int Articulo, int Talle, int Color, int Cantidad);
	public record PagoRequest(Carrito Carrito, int? Dni, TipoDePago TipoDePago, InfoTarjeta? Tarjeta);
	public record InfoTarjeta(long NumeroTarjeta, DateTime Vencimiento, int CodigoSeguridad);
}
