
namespace IDS_TFI.Dominio
{
	public class Comprobante
	{
		public int Id { get; set; }
		public long NroComprobante { get; set; }
		public virtual TipoDeComprobante? Tipo { get; set; }
	}

	public class TipoDeComprobante
	{
		public int Id { get; set; }
		public string? Descripcion { get; set; }
	}
}
