namespace IDS_TFI.Dominio
{
	public class Cliente
	{
		public int Id { get; set; }
		public int DNI { get; set; }
		public int CUIT { get; set; }
		public string? RazonSocial { get; set; }
		public CondicionTributaria CondicionTributaria { get; set; }
		public virtual Direccion? Direccion { get; set; }
	}
}
