using IDS_TFI.Dominio;
using Moq;

namespace Dominio
{
	public class VentaTests
	{
		[Test]
		public void CalcularTotal()
		{
			const int cantidad = 5;
			const double precio = 10, iva = 0.2, ganancia = 0.2;
			const double totalEsperado = precio * (1 + ganancia) * (1 + iva) * cantidad;

			var articulo = new Mock<Articulo>();
			articulo.Object.Costo = precio;
			articulo.Object.IVA = iva;
			articulo.Object.MargenDeGanancia = ganancia;

			var producto = new Mock<Stock>();
			producto.Setup((p) => p.Articulo).Returns(articulo.Object);

			var venta = new Venta();
			venta.AgregarLinea(producto.Object, cantidad);

			var total = venta.Total();

			Assert.That(total, Is.EqualTo(totalEsperado));
		}

		[Test]
		public void AgregarProducto()
		{
			const int cantidad = 5;
			var venta = new Venta();

			var producto = new Mock<Stock>();

			venta.AgregarLinea(producto.Object, cantidad);

			Assert.That(venta.LineasDeVentas!, Has.Count.EqualTo(1));
		}

		[Test]
		public void QuitarProducto()
		{
			const int cantidad1 = 2, cantidad2 = 4, cantidadAQuitar = 6;

			var producto1 = new Mock<Stock>();
			var producto2 = new Mock<Stock>();

			var venta = new Venta();
			venta.AgregarLinea(producto1.Object, cantidad1);
			venta.AgregarLinea(producto2.Object, cantidad2);

			venta.QuitarProducto(producto2.Object, cantidadAQuitar);

			Assert.That(venta.LineasDeVentas!, Has.Count.EqualTo(1));
		}
	}
}
