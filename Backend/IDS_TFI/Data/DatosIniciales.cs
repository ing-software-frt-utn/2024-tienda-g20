using IDS_TFI.Dominio;
using System.Collections.Generic;

namespace IDS_TFI.Data
{
	public static class DatosIniciales
	{
		private static Empleado[] Empleados =>
		[
			new Empleado()
			{
				Nombre = "A",
				Apellido = "Administrativo",
				FechaNacimiento = DateTime.Now,
				Legajo = 1,
				Usuario = "admin",
				Contrasena = "admin",
				Rol = Rol.Administrativo
			},
			new Empleado()
			{
				Nombre = "V",
				Apellido = "Vendedor",
				FechaNacimiento = DateTime.Now,
				Legajo = 2,
				Usuario = "vendedor",
				Contrasena = "vendedor",
				Rol = Rol.Vendedor
			}
		];

		private static TipoDeTalle[] TiposDeTalle => [
			new TipoDeTalle(){
				Descripcion = "Americano"
			},
			new TipoDeTalle(){
				Descripcion = "Europeo / Latinoamericano"
			}
		];

		private static TipoDeComprobante[] TiposDeComprobantes => [
			new TipoDeComprobante(){
				Descripcion = "Factura A"
			},
			new TipoDeComprobante(){
				Descripcion = "Factura B"
			}
		];

		private static Categoria[] Categorias => [
			new Categoria(){
				Descripcion = "Calzado"
			},
			new Categoria(){
				Descripcion = "Jeans"
			}
		];

		private static Marca[] Marcas => [
			new Marca(){
				Descripcion = "Adidas"
			},
			new Marca(){
				Descripcion = "Nike"
			}
		];

		private static Color[] Colores => [
			new Color(){
				Descripcion = "Azul"
			},
			new Color(){
				Descripcion = "Negro"
			},
			new Color(){
				Descripcion= "Blanco"
			}
		];

		private static Talle[] Talles(TipoDeTalle[] tiposDeTalle) => [
			new Talle(){
				Descripcion = "4",
				Tipo = tiposDeTalle[0]
			},
			new Talle(){
				Descripcion = "4.5",
				Tipo = tiposDeTalle[0]
			},
			new Talle(){
				Descripcion = "5",
				Tipo = tiposDeTalle[0]
			},
			new Talle(){
				Descripcion = "35",
				Tipo = tiposDeTalle[1]
			},
			new Talle(){
				Descripcion = "36",
				Tipo = tiposDeTalle[1]
			},
			new Talle(){
				Descripcion = "37",
				Tipo = tiposDeTalle[1]
			},
		];
		private static Articulo[] Articulos(Marca[] marcas, Categoria[] categorias, Color[] colores, Talle[] talles) => [
			new Articulo(){
				Descripcion="Zapatillas",
				Marca = marcas[0],
				Categoria = categorias[0],
				Codigo = 1234,
				Costo = 10000,
				MargenDeGanancia = 0.2,
				Colores = [colores[0], colores[1], colores[2]],
				Talles = [talles[0], talles[1], talles[2] ]
			}
		];

		private static Pais[] Paises => [
			new Pais(){
				Nombre= "Argentina"
			}
		];

		private static Provincia[] Provincias(Pais[] paises) => [
			new Provincia(){
				Nombre = "Tucuman",
				Pais = paises[0]
			}
		];

		private static Direccion[] Direcciones(Provincia[] provincias) => [
			new Direccion(){
				Provincia = provincias[0],
				Calle = "Virgen de la Merced",
				Numero = 1050,
				CodigoPostal = 4000
			}
		];

		private static Cliente[] Clientes(Direccion[] direcciones) => [
			new Cliente(){
				DNI=1,
				CUIT=1,
				CondicionTributaria = CondicionTributaria.ConsumidorFinal,
				RazonSocial = "Consumidor",
				Direccion = direcciones[0]
			},
			new Cliente(){
				DNI=2,
				CUIT=2,
				CondicionTributaria = CondicionTributaria.ResponsableInscripto,
				RazonSocial = "Responsable",
				Direccion = direcciones[0]
			},
		];

		private static Stock[] Productos(Articulo[] articulos)
		{
			var prods = new List<Stock>();
			Color[] cols;
			Talle[] talles;
			foreach (var art in articulos)
			{
				cols = art.Colores!.ToArray();
				talles = art.Talles!.ToArray();
				for (int c = 0; c < cols.Length; c++)
				{
					for (int t = 0; t < talles.Length; t++)
					{
						prods.Add(new Stock()
						{
							Cantidad = 100,
							Articulo = art,
							Color = cols[c],
							Talle = talles[t]
						});
					}
				}
			}

			return [.. prods];
		}

		public const int COMPROBANTE_FACTURA_A = 1;
		public const int COMPROBANTE_FACTURA_B = 2;

		internal static void Cargar(DataContext ctx)
		{
			ctx.Empleados.AddRange(Empleados);

			var categorias = Categorias;
			ctx.Categorias.AddRange(categorias);

			var marcas = Marcas;
			ctx.Marcas.AddRange(marcas);

			var tiposDeTalle = TiposDeTalle;
			ctx.TiposDeTalle.AddRange(tiposDeTalle);

			var talles = Talles(tiposDeTalle);
			ctx.Talles.AddRange(talles);

			var colores = Colores;
			ctx.Colores.AddRange(colores);

			var articulos = Articulos(marcas, categorias, colores, talles);
			ctx.Articulos.AddRange(articulos);

			ctx.TiposDeComprobantes.AddRange(TiposDeComprobantes);

			var paises = Paises;
			ctx.Paises.AddRange(paises);

			var provincias = Provincias(paises);
			ctx.Provincias.AddRange(provincias);

			var direcciones = Direcciones(provincias);
			ctx.Direcciones.AddRange(direcciones);

			ctx.Clientes.AddRange(Clientes(direcciones));

			ctx.Stock.AddRange(Productos(articulos));
		}
	}
}
