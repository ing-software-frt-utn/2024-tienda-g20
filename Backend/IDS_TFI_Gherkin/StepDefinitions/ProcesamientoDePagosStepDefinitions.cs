using IDS_TFI.Data;
using NUnit.Framework;
using System.Net;
using TechTalk.SpecFlow.Assist;

namespace IDS_TFI_Gherkin.StepDefinitions
{
	[Binding]
	public class ProcesamientoDePagosStepDefinitions
	{
		private Carrito? venta;
		private ApiPago? api;

		[BeforeScenario]
		public void Setup()
		{
			api = new ApiPago();
		}

		[AfterScenario]
		public void TearDown()
		{
			api?.Dispose();
		}

		[Given(@"que se ha autenticado con el las credenciales ""([^""]*)"", ""([^""]*)""")]
		public async Task GivenQueSeHaAutenticadoConElLasCredenciales(string usuario, string password)
		{
			await api!.IniciarSesion(usuario, password);
		}


		[Given(@"que existe una venta iniciada")]
		public void GivenQueExisteUnaVentaIniciada()
		{
			venta = new Carrito();
		}

		[Given(@"la venta contiene los siguientes productos:")]
		public void GivenLaVentaContieneLosSiguientesProductos(Table table)
		{
			if (venta == null)
				throw new InvalidOperationException("Venta no iniciada");

			var productos = table.CreateSet<LineaDeCompra>();
			foreach (var p in productos)
			{
				venta.Agregar(p.Articulo, p.Cantidad);
			}
		}

		[When(@"envío una solicitud de pago en efectivo")]
		public async Task WhenEnvioUnaSolicitudDePagoEfectivo()
		{
			if (venta == null)
				throw new InvalidOperationException("Venta no iniciada");

			await api!.EnviarSolicitudEfectivo(venta);
		}

		[Then(@"debería recibir el estado en aprobado")]
		public void ThenDeberiaRecibirElEstadoEnAprobado()
		{
			var estado = api!.ObtenerEstadoPago();
			Assert.That(estado, Is.EqualTo(HttpStatusCode.OK));
		}
	}

	public class LineaDeCompra
	{
		public int Articulo { get; set; }
		public int Cantidad { get; set; }
	}
}
