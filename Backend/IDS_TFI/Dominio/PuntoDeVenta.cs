namespace IDS_TFI.Dominio
{
	public class PuntoDeVenta
	{
		public int Id { get; set; }
		public int Numero { get; set; }

		public Venta? VentaActiva { get; private set; }

		public virtual ICollection<Venta>? Ventas { get; }

		public void IniciarVenta()
		{
			if (VentaActiva != null)
				throw new InvalidOperationException("Ya existe una venta iniciada");

			VentaActiva = new Venta()
			{
				Fecha = DateTime.Now
			};
		}

		public void AutorizarPago() => throw new NotImplementedException();

		public void EmitirComprobante(long numero, TipoDeComprobante tipoDeComprobante)
		{
			if (VentaActiva is null)
				throw new InvalidOperationException("No existe una venta iniciada");

			if (Ventas==null)
				throw new NullReferenceException("Lista de ventas nula");

			VentaActiva.Comprobante = new Comprobante()
			{
				NroComprobante = numero,
				Tipo = tipoDeComprobante
			};

			Ventas.Add(VentaActiva);
			VentaActiva = null;
		}

		public void CancelarVenta()
		{
			VentaActiva = null;
		}
	}
}
