Feature: Procesamiento de pagos

Scenario: Procesamiento exitoso
	Given que se ha autenticado con el las credenciales "vendedor", "vendedor"
	And que existe una venta iniciada
	And la venta contiene los siguientes productos:
	| Articulo | Cantidad |
	| 1        | 1        |
	| 2        | 1        |
	When envío una solicitud de pago en efectivo
	Then debería recibir el estado en aprobado
