namespace IDS_TFI.Dominio
{
	public class LineaDeVenta
	{
		public int Id { get; set; }
		public int Cantidad { get; set; }
		public virtual Stock? Producto { get; set; }

		public double Subtotal => (Producto?.Articulo?.PrecioVenta ?? 0) * Cantidad;
	}
}
