using System.Net;
using static IDS_TFI.Controllers.LoginController;
using System.Net.Http.Json;
using IDS_TFI.Data;

namespace IDS_TFI_Gherkin
{
	public class ApiLogin
	{
		private readonly HttpClient _client;
		private LoginToken? token;
		private HttpResponseMessage? ultimaRespuesta;

		public LoginToken? Token => token;

		public HttpStatusCode StatusCode => ultimaRespuesta?.StatusCode ?? HttpStatusCode.ServiceUnavailable;

		public ApiLogin()
		{
			var _handler = new HttpClientHandler
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

		public void Dispose()
		{
			_client.Dispose();
		}

		internal async Task IniciarSesion(string usuario, string password)
		{
			var loginResponse = await _client.PostAsJsonAsync("api/Login/", new LoginData(usuario, password));
			var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginOutput>();
			token = loginContent?.Token;
		}

		internal async Task IniciarVenta()
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/iniciar");
			if (token != null)
			{
				request.Headers.Add(Autenticacion.ID_HEADER, token.Id.ToString());
				request.Headers.Add(Autenticacion.HASH_HEADER, token.Hash.ToString());
			}

			ultimaRespuesta = await _client.SendAsync(request);
		}
	}
}
