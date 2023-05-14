# gsCrearClases_CS

Proyecto para generar las clases convertido a C# a partir del código de Visual Basic.


### Actualización del 14-may-23

Añado las clases ConversorTipos para Visual Basic y C#, estas clases no se usan en esta DLL, pero las utilizan los programas que usen el código generado por esta biblioteca de clases.

He actualizado el paquete de NuGet para Microsoft.Data.SqlClient (v5.1.1)


### Actualización del 09-oct-22

Añado la clase TablaItem para los nombres de las tablas de SQL Server para añadir a las listas que muestran las tablas.

### Actualización del 08-oct-22

Ahora utiliza Microsoft.Data.SqlClient 5.0.1 en lugar de System.Data.SqlClient Versión 4.8.3.
La razón es (según dicen):
>System.Data.SqlClient is in servicing mode and is not updating on regular basis, but for addressing security issues and important updates. We suggest using Microsoft.Data.SqlClient as active ADO.NET library which gets updated and implements new features.<br>
>[JRahnama commented on Jan 19](https://github.com/dotnet/SqlClient/issues/1479#issuecomment-1016700827)


_**Nota:**_

>Lo he convertido a C# porque eso de la compatibilidad binaria se lo pasa un poco por el forro... Y con la versión de Visual Basic, tener una referencia al proyecto de VB y compilar para iOS (no probé con Android ni con Windows) daba error indicando que no encontraba:<br>
>C:\Program Files\dotnet\packs\Microsoft.iOS.Sdk\15.4.447\tools\msbuild\iOS\Xamarin.iOS.VB.targets.
