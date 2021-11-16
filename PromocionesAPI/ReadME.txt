El sistema corre local sobre: https://localhost:5001/swagger/index.html
Se conecta a una base publica mongo creada por mi que se especifica 
en el appsettings.json.


Un ejemplo de json de creación sería el siguiente:

{
  "mediosDePago": ["Debito","Efectivo"],
  "bancos": ["BNA","BBVA"],
  "categoriasProductos":["Celulares", "Audio"], 
  "maximaCantidadDeCuotas": 0,
  "valorInteresCuotas": 0,
  "porcentajeDeDescuento": 10,
  "fechaInicio": "2021-11-11T20:29:36.039Z",
  "fechaFin": "2021-11-11T20:29:36.039Z"
}

Los valores de medios de pago, bancos y categorias se agregaron como constantes 
en el código dado que es un challenge, esto no sería asi en un sistema funcional.