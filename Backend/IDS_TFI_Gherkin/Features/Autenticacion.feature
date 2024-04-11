Feature: Autenticacion

Scenario: Inicio de sesión como vendedor
	When se inicia sesión con las credenciales "vendedor", "vendedor"
	Then debería obtener un token de autorización

Scenario: Inicio de venta sin autorizacion
	When se inicia una venta
	Then se debería obtener un codigo de estado 400
	
Scenario: Inicio de venta con autorizacion
	Given que se inicia sesión con las credenciales "vendedor", "vendedor"
	When se inicia una venta
	Then se debería obtener un codigo de estado 200
