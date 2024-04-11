namespace IDS_TFI.Data
{
	public class Mensaje(string v)
	{
		public string Texto { get; } = v;
		public object? Objeto { get; init; }
	}
}
