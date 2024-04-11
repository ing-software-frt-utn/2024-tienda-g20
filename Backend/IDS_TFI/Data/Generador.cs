namespace IDS_TFI.Data
{
	public static class Generador
	{
		public static string GenerarID(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
		}

		public static int GenerarNumero(int length)
		{
			return Random.Shared.Next((int)Math.Pow(10, length));
		}

		public static long GenerarGranNumero(int length)
		{
			return Random.Shared.NextInt64((long)Math.Pow(10, length));
		}
	}
}
