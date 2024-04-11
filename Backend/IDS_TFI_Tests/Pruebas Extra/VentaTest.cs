using IDS_TFI.Controllers;
using IDS_TFI.Data;
using IDS_TFI.Dominio;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using static IDS_TFI.Controllers.LoginController;

namespace APITests
{
	public class VentaTests
	{
		private HttpClient _client;
		private HttpClientHandler _handler;

		[SetUp]
		public void Setup()
		{
			_handler = new HttpClientHandler
			{
				CookieContainer = new CookieContainer(),
				UseCookies = true,
				AllowAutoRedirect = false
			};

			_client = new HttpClient(_handler)
			{
				BaseAddress = new Uri("http://localhost:5118/")
			};
		}

		[Test]
		public async Task AutorizacionDeVenta()
		{
			var iniciarVenta = await _client.PostAsync("api/Venta/iniciar", null);
			Assert.That(iniciarVenta.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized).Or.EqualTo(HttpStatusCode.BadRequest));

			var loginResponse = await _client.PostAsJsonAsync("api/Login/", new LoginData("vendedor", "vendedor"));
			var respLogin = await loginResponse.Content.ReadFromJsonAsync<LoginOutput>();
			var token = respLogin?.Token;
			Assert.Multiple(() =>
			{
				Assert.That(loginResponse.IsSuccessStatusCode, Is.True);
				Assert.That(token, Is.Not.Null);
			});

			var request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/iniciar");
			request.Headers.Add(Autenticacion.ID_HEADER, token.Id.ToString());
			request.Headers.Add(Autenticacion.HASH_HEADER, token.Hash.ToString());

			iniciarVenta = await _client.SendAsync(request);
			Assert.That(iniciarVenta.StatusCode, Is.EqualTo(HttpStatusCode.OK));
		}


		[Test]
		public async Task Carro()
		{
			var loginResponse = await _client.PostAsJsonAsync("api/Login/", new LoginData("vendedor", "vendedor"));
			var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginOutput>();
			var token = loginContent?.Token;
			Assert.Multiple(() =>
			{
				Assert.That(loginResponse.IsSuccessStatusCode, Is.True);
				Assert.That(token, Is.Not.Null);
			});

			var request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/iniciar");
			request.Headers.Add(Autenticacion.ID_HEADER, token.Id.ToString());
			request.Headers.Add(Autenticacion.HASH_HEADER, token.Hash.ToString());

			var iniciarVenta = await _client.SendAsync(request);
			var carrito = await iniciarVenta.Content.ReadFromJsonAsync<Carrito>();
			Assert.Multiple(() =>
			{
				Assert.That(iniciarVenta.StatusCode, Is.EqualTo(HttpStatusCode.OK));
				Assert.That(carrito, Is.Not.Null);
			});

			var busqueda = await _client.GetAsync("api/Articulos/1");
			var articulo = await busqueda.Content.ReadFromJsonAsync<Articulo>();
			var colores = articulo!.Colores!.ToArray();
			var talles = articulo!.Talles!.ToArray();

			var productosAComprar = new List<(int articulo, int talle, int color, int cantidad)>();
			for (int c = 0; c < colores.Length; c++)
			{
				for (int t = 0; t < talles.Length; t++)
				{
					productosAComprar.Add((articulo.Id, talles[t].Id, colores[c].Id, 1));
				}
			}

			foreach (var prod in productosAComprar)
			{
				request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/agregar");
				request.Headers.Add(Autenticacion.ID_HEADER, token.Id.ToString());
				request.Headers.Add(Autenticacion.HASH_HEADER, token.Hash.ToString());
				request.Content = ToJson(new VentaRequest(carrito, prod.articulo, prod.talle, prod.color, prod.cantidad));

				var compra = await _client.SendAsync(request);
				carrito = await compra.Content.ReadFromJsonAsync<Carrito>();
				Assert.Multiple(() =>
				{
					Assert.That(compra.StatusCode, Is.EqualTo(HttpStatusCode.OK));
					Assert.That(carrito, Is.Not.Null);
				});
			}

			request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/carrito");
			request.Headers.Add(Autenticacion.ID_HEADER, token.Id.ToString());
			request.Headers.Add(Autenticacion.HASH_HEADER, token.Hash.ToString());
			request.Content = ToJson(carrito);

			var listado = await _client.SendAsync(request);
			Assert.That(listado.StatusCode, Is.EqualTo(HttpStatusCode.OK));

			var lineas = await listado.Content.ReadFromJsonAsync<IEnumerable<LineaDeVenta>>();
			Assert.That(lineas, Is.Not.Null);

			Assert.That(lineas.Count(), Is.EqualTo(productosAComprar.Count));
		}

		private static StringContent ToJson(object obj)
		{
			string jsonContent = JsonConvert.SerializeObject(obj);
			return new StringContent(jsonContent, Encoding.UTF8, "application/json");
		}

		[TearDown]
		public void TearDown()
		{
			_client?.Dispose();
			_handler?.Dispose();
		}
	}
}