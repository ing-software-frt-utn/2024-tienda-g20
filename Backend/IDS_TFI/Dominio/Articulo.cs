namespace IDS_TFI.Dominio
{
	public class Articulo
	{
		public int Id { get; set; }
		public long Codigo { get; set; }
		public string? Descripcion { get; set; }
		public double Costo { get; set; }
		public double MargenDeGanancia { get; set; }
		public double IVA { get; set; } = 0.21;
		public virtual Marca? Marca { get; set; }
		public virtual Categoria? Categoria { get; set; }

		public virtual ICollection<Talle>? Talles { get; set; }
		public virtual ICollection<Color>? Colores { get; set; }

		public double NetoGravado => Costo + Costo * MargenDeGanancia;
		public double CostoIVA => NetoGravado * IVA;
		public double PrecioVenta => NetoGravado + CostoIVA;
	}
}
