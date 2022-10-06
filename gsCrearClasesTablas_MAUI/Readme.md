# gsCrearClasesTablas_MAUI


### Nota sobre worloads de ios
Al crear el proyecto me daba error de que no estaba instalado el workload para ios (o algo  así)

El proyecto está creado con .NET 6.0 (usando el .NET 6.0 SDK v6.0.401), pero tengo instalado el .NET 7 (release candidate) y al ejecutar el comando:
```
dotnet workload install ios
```
Según parece se instalaba el del SDK de la versión 7.

La forma de arreglarlo ha sido:
1- Te pones en el directorio del proyecto.
2- dotnet workload restore gsCrearClasesTablas_MAUI.csproj
 y con esto funciona todo bien.

 Ahora al ejecutar dotnet --version me muestra la 6.0.401 en vez de la pre-release de .NET 7: (7.0.100-rc.1.22431.12)
 >Lo que no sé es lo que hice para que se cambiara al 6.0.401

 Probaré esto mismo con otro proyecto que tampoco me permitía usar el proyecto para ios.
 >Al abrir ese otro proyecto ya no me da los errores, se ve que al instalarlo para este proyecto "se ha reparado" en el resto.


