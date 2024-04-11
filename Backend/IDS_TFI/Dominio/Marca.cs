using System.ComponentModel.DataAnnotations.Schema;

namespace IDS_TFI.Dominio
{
	[Table("Marcas")]
	public class Marca
	{
		public int Id { get; set; }
		public string? Descripcion { get; set; }
	}
}
