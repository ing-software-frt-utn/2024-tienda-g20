namespace IDS_TFI.Dominio
{
	public class Stock
	{
		public int Id { get; set; }
		public int Cantidad { get; set; }
		public virtual Talle? Talle { get; set; }
		public virtual Color? Color { get; set; }
		public virtual Articulo? Articulo { get; set; }
	}
}
