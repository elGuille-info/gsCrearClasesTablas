# gsCrearClasesTablas
Generar clases a partir de una tabla de una base de datos de SQL Server o de Access. Usando .NET 6.0 para Windows

Versión para .NET 6.0 para Windows basado en el código publicado en: [CrearClaseTabla](https://github.com/elGuille-info/CrearClaseTabla) para .NET Framework 4.8.1

Mira en el blog para saber más y los enlaces a las versiones anteriores: [Generar las clases (de VB o C#) de una tabla de SQL Server o Access (mdb)](https://www.elguillemola.com/generar-las-clases-de-una-tabla-de-sql-server-o-access-mdb/)

<br>
<br>

### Ya está disponible en Microsoft Store la versión de Windows

Para descargar desde la app Microsoft Store: ms-windows-store://pdp/?productid=9MVGBRWG1W2V <br>
Para descargar desde el navegador (te manda a la MS Store): [gsCrearClasesTablas_MAUI](https://www.microsoft.com/store/apps/9MVGBRWG1W2V)<br>

<br>
<br>

### El _nightmare_ de los assets para Windows

Pues eso... que otras veces me ha funcionado bien elegir una imagen para crear las imágenes usadas por Windows (para poder publicarla en la Microsoft Store), pero en esta aplicación me hada dado un poquillo _polc*lo_.

Esta es una captura de otra aplicación (en la que sí funcionó como debía)

<picture>
<img src="https://github.com/elGuille-info/gsCrearClasesTablas/blob/master/Screenshot%202022-10-10%20231513.png" width="650">
</picture>

<br>
<br>

El tema es que te vas al manifiesto de Windows (Package.appxmanifest), te vas a la pestaña **Visual Assets**, en **Asset Generator** le indicas la imagen base que quieres utilizar, eliges los formatos a usar (Scales) y le das al botón **Generate** y te genera las imágenes.
O eso debería hacer.
Pero en esta aplicación en vez de los nombres predeterminados para cada tipo (Small Tile, Medium Tile, etc.) pone $placeholder$.png y... aunque lo he cambiado, sigue usando ese nombre (que no es válido para una imagen).

Al final he tenido que generar cada tipo de imagen por separado y cambiando los nombres de los ficheros para que en vez de usar $placeholder$ tenga el prefijo del nombre del tipo de imagen, y después ir eligiendo las imágenes correspondientes (teniendo cuidado de poner el nombre correcto en vez del _placeholder_ ese de las narices.


<br>
<br>


_**Nota 10-oct-22 20.36:**_ <br>
>Quito el proyecto **gsCrearClases** de Visual Basic para .NET 6.0 porque ya no lo utilizo.<br>
>
>Creo la compilación para gsCrearClasesTablas para WindowsForms con .NET 6. (Release v3.0.5)<br>
>[gsCrearClasesTablas WindowsForms](https://github.com/elGuille-info/gsCrearClasesTablas/releases/tag/v3.0.5)

<br>
<br>

_**Nota 08-oct-22 14.28:**_ <br>
>El proyecto **gsCrearClases** de Visual Basic ya no lo utilizo en ninguno de los proyectos.<br>
>El proyecto **gsCrearClases_CS** (de C#) utiliza Microsoft.Data.SqlClient 5.0.1 en lugar de System.Data.SqlClient Versión 4.8.3.<br>
>El proyecto **gsCrearClasesTablas** (como app de escritorio para Windows) utiliza la DLL de gsCrearClases_CS y para el acceso a las bases de datos de Access (.mdb) sigue usando la clases CrearClasesOleDb con el paquete de NuGet System.Data.OleDb 6.0.0.<br>
>El proyecto **gsCrearClasesTablas_MAUI** utiliza la DLL de gsCrearClases_CS y no permite crear clases a partir de bases de datos de Access (solo de SQL Server).<br>
>Está operativo (y funcional) tanto en Windows como en iOS (iPhone).<br>
>En Android falla al acceder a las tablas, dando el error:<br>
>	_ERROR: A connection was successfully established with the server, but then an error occurred during the pre-login handshake. (provider: TCP Provider, error: 35 - An internal exception was caught)._<br>
>Ese mismo error daba en la app de escritorio si no tenía _TrustServerCertificate=True;_ en la cadena de conexión.<br>

<br>
<br>

_**Nota 06-oct-22 21.16:**_
>He creado el proyecto de gsCrearClases en C# para poder usarlo sin problemas con la app móvil (he probado con iOS).<br>
>Resulta que al compilar la pp para iOS (iPhone) daba error indicando que no encontraba esto:<br>
C:\Program Files\dotnet\packs\Microsoft.iOS.Sdk\15.4.447\tools\msbuild\iOS\Xamarin.iOS.VB.targets.<br>
>Al estar generado el proyecto en C#, ya no da ese error.<br>
>En ambos casos, no utiliza esa DLL (aún).
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

>Aunque el error lo da para todos los proyectos, en el de Windows e iOS dice que está disponible.<br>
>A ver si te puedo poner la captura.<br>

![El error en Visual Studio 2022](https://github.com/elGuille-info/gsCrearClasesTablas/blob/master/Screenshot%202022-10-06%20182150.png)
<br>

