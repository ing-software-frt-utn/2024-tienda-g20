namespace IDS_TFI.Dominio
{
	public class Talle
	{
		public int Id { get; set; }
		public string? Descripcion { get; set; }
		public virtual TipoDeTalle? Tipo { get; set; }
	}

	public class TipoDeTalle
	{
		public int Id { get; set; }
		public string? Descripcion { get; set; }
	}
}
