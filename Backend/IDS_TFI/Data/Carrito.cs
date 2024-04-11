namespace IDS_TFI.Data
{
	[Serializable]
	public class Carrito
	{
		public List<InfoVenta> Productos { get; set; }

		public Carrito()
		{
			Productos = [];
		}

		public void Agregar(InfoVenta info)
		{
			for (int i=0; i<Productos.Count; i++)
			{
				var p = Productos[i];
				if (p.Producto == info.Producto)
				{
					Productos.RemoveAt(i);
					var cant = p.Cantidad + info.Cantidad;
					if (cant < 0) cant = 0;
					Productos.Add(new InfoVenta(p.Producto, cant));
					return;
				}
			}
			Productos.Add(info);
		}

		public void Agregar(int producto, int cantidad)
		{
			for (int i = 0; i < Productos.Count; i++)
			{
				var p = Productos[i];
				if (p.Producto == producto)
				{
					Productos.RemoveAt(i);
					var cant = p.Cantidad + cantidad;
					if (cant < 0) cant = 0;
					Productos.Add(new InfoVenta(p.Producto, cant));
					return;
				}
			}
			Productos.Add(new InfoVenta(producto, cantidad));
		}
	}

	public record InfoVenta(int Producto, int Cantidad);
}
