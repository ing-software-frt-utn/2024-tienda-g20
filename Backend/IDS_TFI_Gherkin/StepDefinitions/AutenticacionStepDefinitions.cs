using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IDS_TFI_Gherkin.StepDefinitions
{
	[Binding]
	public class AutenticacionStepDefinitions
	{
		private ApiLogin? api;

		[BeforeScenario]
		public void Setup()
		{
			api = new ApiLogin();
		}

		[AfterScenario]
		public void TearDown()
		{
			api!.Dispose();
		}

		[When(@"se inicia sesión con las credenciales ""([^""]*)"", ""([^""]*)""")]
		public async Task WhenSeIniciaSesionConLasCredenciales(string usuario, string password)
		{
			await api!.IniciarSesion(usuario, password);
		}

		[Given(@"que se inicia sesión con las credenciales ""([^""]*)"", ""([^""]*)""")]
		public async Task GivenSeHaIniciadoSesionConLasCredencialesVendedor(string usuario, string password)
		{
			await api!.IniciarSesion(usuario, password);
		}


		[Then(@"debería obtener un token de autorización")]
		public void ThenDeberiaObtenerUnTokenDeAutorizacion()
		{
			var token = api!.Token;
			Assert.That(token, Is.Not.Null);
		}

		[When(@"se inicia una venta")]
		public async Task WhenSeIntentaIniciarUnaVenta()
		{
			await api!.IniciarVenta();
		}

		[Then(@"se debería obtener un codigo de estado (.*)")]
		public void ThenSeDeberiaObtenerUnErrorDeEstado(int status)
		{
			Assert.That((int)(api!.StatusCode), Is.EqualTo(status));
		}

	}
}
