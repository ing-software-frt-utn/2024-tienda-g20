using IDS_TFI.Data;

namespace SeviciosExternos
{
	public class AutorizadorDePagos
	{
		[Test]
		public async Task AutorizarPago()
		{
			const string tarjeta = "1234567812345678";
			const double monto = 12512;
			const string dni = "2410245";
			const string añoVencimiento = "24", mesVencimiento = "10";
			const string codigo = "123";
			const string propietario = "Test";

			string transaccion = Generador.GenerarID(10);

			var info = new RequestInfo()
			{
				AñoVencimiento = añoVencimiento,
				MesVencimiento = mesVencimiento,
				CodigoSeguridad = codigo,
				Identificacion = new Identification()
				{
					Tipo = "dni",
					Numero = dni
				},
				NombrePropietario = propietario,
				NumeroTarjeta = tarjeta
			};

			var autorizacion = await AutorizacionDePagos.Autorizar(info, transaccion, monto);
			Assert.That(autorizacion, Is.EqualTo(ResultadoAutorizacion.Exitoso));
		}
	}
}
