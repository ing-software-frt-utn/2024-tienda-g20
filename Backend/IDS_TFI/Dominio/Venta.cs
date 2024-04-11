namespace IDS_TFI.Dominio
{
	public class Venta
	{
		public int Id { get; set; }
		public int NroVenta { get; set; }
		public DateTime Fecha { get; set; }
		public virtual Cliente? Cliente { get; set; }
		public virtual Pago? Pago { get; set; }
		public virtual Comprobante? Comprobante { get; set; }
		public virtual ICollection<LineaDeVenta>? LineasDeVentas { get; set; }

		public Venta()
		{
			LineasDeVentas = [];
			Fecha = DateTime.Now;
		}

		public double Total()
		{
			double total = 0;
			if (LineasDeVentas == null)
				throw new NullReferenceException("Lista de lineas de ventas nula");
			foreach (var linea in LineasDeVentas)
			{
				total += linea.Subtotal;
			}
			return total;
		}

		public void AgregarLinea(Stock producto, int cantidad)
		{
			if (LineasDeVentas == null)
				throw new NullReferenceException("Lista de lineas de ventas nula");
			
			LineasDeVentas.Add(new LineaDeVenta() { Producto = producto, Cantidad = cantidad });
		}

		public void AgregarProducto(Stock producto)
		{
			if (LineasDeVentas == null)
				throw new NullReferenceException("Lista de lineas de ventas nula");

			var lineaExistente = LineasDeVentas.First((l) => l.Producto?.Id == producto.Id);

			if (lineaExistente != null)
				lineaExistente.Cantidad += 1;
			else
				LineasDeVentas.Add(new LineaDeVenta() { Producto = producto, Cantidad = 1 });
		}

		public void QuitarProducto(Stock producto, int cantidad = 1)
		{
			if (LineasDeVentas == null)
				throw new NullReferenceException("Lista de lineas de ventas nula");

			var lineaExistente = LineasDeVentas.First((l) => l.Producto?.Id == producto.Id);
			if (lineaExistente == null)
				return;

			if (lineaExistente.Cantidad <= cantidad)
				LineasDeVentas.Remove(lineaExistente);
			else
				lineaExistente.Cantidad -= cantidad;
		}

		public void Pagar(Cliente? cliente, TipoDePago tipoPago, Comprobante comprobante)
		{
			Cliente = cliente;
			Pago = new Pago()
			{
				Fecha = DateTime.Now,
				Monto = Total(),
				Tipo = tipoPago
			};

			Comprobante = comprobante;
		}
	}
}
