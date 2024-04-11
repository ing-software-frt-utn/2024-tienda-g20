using IDS_TFI.Dominio;

namespace IDS_TFI.Data
{
	public static class Autenticacion
	{
		public const string ID_HEADER = "Auth-ID", HASH_HEADER = "Auth-Token";

		public static LoginToken CrearToken(Empleado empleado)
		{
			return new LoginToken(empleado.Id, empleado.GetHashCode());
		}

		public static bool Verificar(string? idStr, string? hashStr, DataContext context, Rol rol)
		{
			if (idStr == null || hashStr == null)
				return false;

			if (!int.TryParse(idStr, out var id) || !int.TryParse(hashStr, out var hash))
				return false;

			var empleado = context.Empleados.Find(id);
			if (empleado == null || empleado.Rol != rol || empleado.GetHashCode() != hash)
				return false;

			return true;
		}
	}

	public record LoginToken(int Id, int Hash);
}
