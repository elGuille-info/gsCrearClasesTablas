# gsCrearClasesTablas
Generar clases a partir de una tabla de una base de datos de SQL Server o de Access. Usando .NET 6.0 para Windows

Versión para .NET 6.0 para Windows basado en el código publicado en: [CrearClaseTabla](https://github.com/elGuille-info/CrearClaseTabla) para .NET Framework 4.8.1

Mira en el blog para saber más y los enlaces a las versiones anteriores: [Generar las clases (de VB o C#) de una tabla de SQL Server o Access (mdb)](https://www.elguillemola.com/generar-las-clases-de-una-tabla-de-sql-server-o-access-mdb/)

<br>
<br>

_**Nota:**_

>Este proyecto utiliza net6.0-windows en el proyecto principal (el de la interfaz del usuario porque utiliza WindowsForms.<br>
>En la última versión el proyecto con la interfaz del usuario utiliza los paquetes NuGet para acceso a SQL Server y Access (OleDb) de esta forma la DLL ya no es exclusiva para Windows.<br>
>Los paquetes de Nuget son: System.Data.OleDb Versión 6.0.0 y System.Data.SqlClient Versión 4.8.3.<br>
>La librería (o biblioteca) de clases utiliza net6.0 porque ya no tiene referenciada System.Data.OleDb Versión 6.0.0.<br>

>Si alguna vez me da por crear este proyecto para "mobile" (ya sea Windows, Android o iOS) seguramente tendría que prescindir de la conversión de bases de tipo Access (.mdb), al menos en las versiones para Androi e iOS, pero eso aún no me lo he planteado, aunque nunca se sabe si crearé el proyecto para .NET MAUI 🤔<br>
>Pues... ya me he decidido... al menos el proyecto está creado 😉

>Según parece la versión de Android no soporta el SqlConnection/SqlDataAdapter, cosa extraña porque en otro proyecto de Xamarin los uso con el mismo paquete de NuGet: System.Data.SqlClient Versión 4.8.3. (incluso es la versión 4.8.2)

>Aunque el error lo da para todos los proyectos, en el de Windows e iOS dice que está disponible.
>A ver si te puedo poner la captura.

