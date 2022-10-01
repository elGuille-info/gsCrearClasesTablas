# gsCrearClasesTablas
Generar clases a partir de una tabla de una base de datos de SQL Server o de Access. Usando .NET 6.0 para Windows

Versión para .NET 6.0 para Windows basado en el código publicado en: [CrearClaseTabla](https://github.com/elGuille-info/CrearClaseTabla) para .NET Framework 4.8.1

_**Nota:**_

>Este proyecto utiliza net6.0-windows en el proyecto principal (el de la interfaz del usuario porque utiliza WindowsForms.<br>
>La librería (o biblioteca) de clases utiliza también net6.0-windows porque tiene referenciado System.Data.OleDb Versión 6.0.0 para poder acceder a las clases para crar el código de las bases de datos *.mdb (de tipo Access) y ese paquete es solo compatible con Windows.<br>
>También utiliza System.Data.SqlClient Versión 4.8.3 pero ese es compatible con otras plataformas.

>Si alguna vez me da por crear este proyecto para "mobile" (ya sea Windows, Android o iOS) tendría que prescindir de la conversión de bases de tipo Access (.mdb), pero eso aún no me lo he planteado, aunque nunca se sabe si craré el proyecto para .NET MAUI 🤔

