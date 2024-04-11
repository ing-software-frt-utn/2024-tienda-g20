namespace IDS_TFI.Dominio
{
	public class Empleado
	{
		public int Id { get; set; }
		public int Legajo { get; set; }
		public string? Nombre { get; set; }
		public string? Apellido { get; set; }
		public string? Usuario { get; set; }
		public string? Contrasena { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public Rol Rol { get; set; }

		public int Edad
		{
			get
			{
				var fechaActual = DateTime.Now;
				int años = fechaActual.Year - FechaNacimiento.Year;
				if (fechaActual.DayOfYear < FechaNacimiento.DayOfYear)
					años--;
				return años;
			}
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Id, Legajo, Nombre, Apellido, Usuario, Contrasena, Rol);
		}
	}
	public enum Rol
	{
		Ninguno,
		Administrativo,
		Vendedor
	}
}
