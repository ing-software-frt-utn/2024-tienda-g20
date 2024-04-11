namespace IDS_TFI.Dominio
{
	public class Pago
	{
		public int Id { get; set; }
		public DateTime Fecha { get; set; }
		public double Monto { get; set; }
		public TipoDePago Tipo { get; set; }
	}

	public enum TipoDePago
	{
		Efectivo,
		TarjetaDebito,
		TarjetaCredito
	}
}
