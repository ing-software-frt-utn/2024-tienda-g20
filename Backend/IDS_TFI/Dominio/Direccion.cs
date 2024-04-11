using System.ComponentModel.DataAnnotations;

namespace IDS_TFI.Dominio
{
	public class Pais
	{
		[Key]
		public int Codigo { get; set; }
		public string? Nombre { get; set; }
	}

	public class Provincia
	{
		[Key]
		public int Codigo { get; set; }
		public string? Nombre { get; set; }
		public virtual Pais? Pais { get; set; }
	}

	public class Direccion
	{
		[Key]
		public int Codigo { get; set; }
		public int CodigoPostal { get; set; }
		public string? Calle { get; set; }
		public int Numero { get; set; }
		public virtual Provincia? Provincia { get; set; }
}
}
