using Azure.Core;
using IDS_TFI.Controllers;
using IDS_TFI.Data;
using IDS_TFI.Dominio;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static IDS_TFI.Controllers.LoginController;

namespace IDS_TFI_Gherkin
{
	internal class ApiPago
	{
		private readonly HttpClient _client;
		private LoginToken? token;
		private HttpResponseMessage? ultimaRespuesta;

		public ApiPago()
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

		internal async Task EnviarSolicitudEfectivo(Carrito venta)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, "api/Venta/pagar");
			request.Headers.Add(Autenticacion.ID_HEADER, token?.Id.ToString());
			request.Headers.Add(Autenticacion.HASH_HEADER, token?.Hash.ToString());
			request.Content = ToJson(new PagoRequest(venta, null, TipoDePago.Efectivo, null));

			ultimaRespuesta = await _client.SendAsync(request);
		}

		internal async Task IniciarSesion(string usuario, string password)
		{
			var loginResponse = await _client.PostAsJsonAsync("api/Login/", new LoginData(usuario, password));
			var loginContent = await loginResponse.Content.ReadFromJsonAsync<LoginOutput>();
			token = loginContent?.Token;
		}

		internal HttpStatusCode ObtenerEstadoPago()
		{
			return ultimaRespuesta?.StatusCode ?? HttpStatusCode.ServiceUnavailable;
		}

		private static StringContent ToJson(object obj)
		{
			string jsonContent = JsonConvert.SerializeObject(obj);
			return new StringContent(jsonContent, Encoding.UTF8, "application/json");
		}
	}
}
