using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace IDS_TFI.Data
{
	public enum ResultadoAutorizacion
	{
		Error,
		Exitoso,
		TokenDenegado,
		PagoDenegado,
		TransaccionRepetida
	}

	public static class AutorizacionDePagos
	{
		private const string URL_TOKENS = "https://developers.decidir.com/api/v2/tokens";
		private const string URL_PAYMENTS = "https://developers.decidir.com/api/v2/payments";

		private const string API_KEY_TOKENS = "b192e4cb99564b84bf5db5550112adea";
		private const string API_KEY_PAYMENTS = "566f2c897b5e4bfaa0ec2452f5d67f13";

#if DEBUG
		private static readonly RequestInfo DebugRequest = new()
		{
			NumeroTarjeta = "4507990000004905",
			MesVencimiento = "08",
			AñoVencimiento = "24",
			CodigoSeguridad = "123",
			NombrePropietario = "John Doe",
			Identificacion = new Identification()
			{
				Tipo = "dni",
				Numero = "25123456"
			}
		};
#endif

		public async static Task<ResultadoAutorizacion> Autorizar(RequestInfo request, string idTransaccion, double monto)
		{
			var _handler = new HttpClientHandler
			{
				CookieContainer = new CookieContainer(),
				UseCookies = true,
				AllowAutoRedirect = false
			};

			var _client = new HttpClient(_handler);

#if DEBUG
			request = DebugRequest;
#endif
			var token = await SolicitarToken(request, _client);
			if (token == null)
				return ResultadoAutorizacion.TokenDenegado;

			var confirmacion = await ConfirmarPago(_client, token, request.NumeroTarjeta!, idTransaccion, monto);
			return confirmacion;
		}

		private async static Task<string?> SolicitarToken(RequestInfo info, HttpClient _client)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, URL_TOKENS);
			request.Headers.Add("apikey", API_KEY_TOKENS);
			request.Headers.Add("Cache-Control", "no-cache");

			request.Content = ToJson(info);

			var solicitarToken = await _client.SendAsync(request);
			if (!solicitarToken.IsSuccessStatusCode)
				return null;

			var json = await solicitarToken.Content.ReadAsStringAsync();
			var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

			var status = response?["status"] ?? null;
			if (status?.ToString() == "active")
			{
				return response!["id"].ToString();
			}
			else
			{
				return null;
			}
		}

		private async static Task<ResultadoAutorizacion> ConfirmarPago(HttpClient _client, string token, string nroTarjeta, string idTransaccion, double monto)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, URL_PAYMENTS);
			request.Headers.Add("apikey", API_KEY_PAYMENTS);
			request.Headers.Add("Cache-Control", "no-cache");
			var text = BuildPaymentInfo(token, nroTarjeta, idTransaccion, monto);
			var content = new StringContent(text, null, "application/json");
			request.Content = content;
			var confirmarPago = await _client.SendAsync(request);

			if (!confirmarPago.IsSuccessStatusCode)
			{
				var error = await confirmarPago.Content.ReadAsStringAsync();
				var args = JsonConvert.DeserializeObject<AuthError>(error);
				if (args != null)
				{
					switch (args.Tipo)
					{
						case "invalid_request_error":
							if (args.Errores == null) break;
							foreach (var er in args.Errores)
							{
								if (er.Codigo == "repeated")
								{
									switch (er.Parametro)
									{
										case "site_transaction_id":
											return ResultadoAutorizacion.TransaccionRepetida;
									}
								}
							}
							break;
					}
				}
				return ResultadoAutorizacion.Error;
			}

			var json = await confirmarPago.Content.ReadAsStringAsync();
			var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

			var status = response?["status"] ?? null;
			if (status?.ToString() == "approved")
			{
				return ResultadoAutorizacion.Exitoso;
			}
			else
			{
				return ResultadoAutorizacion.PagoDenegado;
			}
		}

		private static StringContent ToJson(object obj)
		{
			string jsonContent = JsonConvert.SerializeObject(obj);
			return new StringContent(jsonContent, Encoding.UTF8, "application/json");
		}

		private static string BuildPaymentInfo(string token, string nroTarjeta, string idTransaccion, double monto)
		{
			var bin = nroTarjeta[..6];
			var builder = new StringBuilder();
			builder.Append('{');
			builder.Append($"\"site_transaction_id\":\"{idTransaccion}\",");
			builder.Append("\"payment_method_id\" : 1,");
			builder.Append($"\"token\" : \"{token}\",");
			builder.Append($"\"bin\" : \"{bin}\",");
			builder.Append($"\"amount\" : {(int)monto},");
			builder.Append("\"currency\" : \"ARS\",");
			builder.Append("\"installments\" : 1,");
			builder.Append("\"description\" : \"\",");
			builder.Append("\"payment_type\" : \"single\",");
			builder.Append("\"establishment_name\" : \"single\",");
			builder.Append("\"sub_payments\": [{");
			builder.Append("\"site_id\": \"\",");
			builder.Append($"\"amount\": {(int)monto},");
			builder.Append("\"installments\": null");
			builder.Append("}]");
			builder.Append('}');

			return builder.ToString();
		}

		private class AuthError
		{
			[JsonProperty(PropertyName = "error_type")] public string? Tipo { get; set; }
			[JsonProperty(PropertyName = "validation_errors")] public ICollection<ValidationError>? Errores { get; set; }
		}

		private class ValidationError
		{
			[JsonProperty(PropertyName = "code")] public string? Codigo { get; set; }
			[JsonProperty(PropertyName = "param")] public string? Parametro { get; set; }
		}
	}

	[Serializable]
	public class RequestInfo
	{
		[JsonProperty(PropertyName = "card_number")]
		public string? NumeroTarjeta { get; init; }

		[JsonProperty(PropertyName = "card_expiration_month")]
		public string? MesVencimiento { get; init; }

		[JsonProperty(PropertyName = "card_expiration_year")]
		public string? AñoVencimiento { get; init; }

		[JsonProperty(PropertyName = "security_code")]
		public string? CodigoSeguridad { get; init; }

		[JsonProperty(PropertyName = "card_holder_name")]
		public string? NombrePropietario { get; init; }

		[JsonProperty(PropertyName = "card_holder_identification")]
		public Identification? Identificacion { get; set; }
	}

	[Serializable]
	public class Identification
	{
		[JsonProperty(PropertyName = "type")]
		public string Tipo { get; init; } = "dni";

		[JsonProperty(PropertyName = "number")]
		public string? Numero { get; init; }
	}
	
}
