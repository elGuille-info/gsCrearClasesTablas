'------------------------------------------------------------------------------
' Clase para crear una clase a partir de una tabla de SQL Server    (08/Jul/04)
' Basado en el código anteriormente incluido en crearClasesSQLVB    (07/Jul/04)
'
' Todos los métodos son estáticos (compartidos) para usarlos sin crear una instancia
'
' Muevo esta clase al formulario ya que en la DLL no es necesaria   (05/oct/22)
'
' Quito esta clase del proyecto gsCrearClasesTablas y uso la de la DLL de C#. (08/oct/22 14.05)
'
' ©Guillermo 'guille' Som, 2004, 2005, 2007, 2022
'------------------------------------------------------------------------------
Option Strict On
Option Explicit On
'
Imports System
Imports Microsoft.VisualBasic
'
Imports System.Data
'Imports System.Data.SqlClient
' Usando <PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.1" />
' Ahora da el erro: ERROR: A connection was successfully established with the server, but then an error occurred during the login process. (provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)
Imports Microsoft.Data.SqlClient

Imports elGuille.Util.Developer
Imports elGuille.Util.Developer.Data

' Le quito el Namespace, para que esté en el de este proyecto. (05/oct/22 11.29)
'Namespace elGuille.Util.Developer.Data
Public Class CrearClaseSQL
    Inherits CrearClase
    '
    '
    Public Shared Function Conectar(dataSource As String,
                                        initialCatalog As String,
                                        cadenaSelect As String) As String
        Return Conectar(dataSource, initialCatalog, cadenaSelect, "", "", False)
    End Function

    Public Shared Function Conectar(dataSource As String,
                                        initialCatalog As String,
                                        cadenaSelect As String,
                                        userId As String,
                                        password As String,
                                        seguridadSQL As Boolean) As String
        ' si se produce algún error, se devuelve una cadena empezando por ERROR
        Conectado = False
        '
        cadenaConexion = "data source=" & dataSource & "; initial catalog=" & initialCatalog & ";"
        '
        If seguridadSQL Then
            If userId <> "" Then
                cadenaConexion &= "user id=" & userId & ";"
            End If
            If password <> "" Then
                cadenaConexion &= "password=" & password & ";"
            End If
        Else
            cadenaConexion &= "Integrated Security=yes;"
        End If
        ' A ver si así no da error en la app de escritorio.
        cadenaConexion &= "TrustServerCertificate=True;"
        '
        If cadenaSelect = "" Then
            ' si no se indica la cadena Select también se conecta
            ' esto es útil para averiguar las tablas de la base
            Conectado = True
            Return ""
        End If
        '
        Dim dbDataAdapter As New SqlDataAdapter(cadenaSelect, cadenaConexion)
        '
        dbDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
        '
        ' Limpiar el contenido de mDataTable                    (08/Jun/05)
        mDataTable = New DataTable
        '
        Try
            dbDataAdapter.Fill(mDataTable)
            System.Threading.Thread.Sleep(100)
            Conectado = True
        Catch ex As Exception
            Return "ERROR: en Fill: " & ex.Message '& " - " & ex.GetType().Name
        End Try
        '
        Return ""
    End Function
    '
    Public Shared Function NombresTablas() As String()
        Dim nomTablas() As String = Nothing
        Dim dt As New DataTable
        Dim i As Integer
        Dim dbConnection As New SqlConnection(cadenaConexion)
        '
        Try
            dbConnection.Open()
        Catch ex As Exception
            ReDim nomTablas(0)
            nomTablas(0) = "ERROR: " & ex.Message
            Conectado = False
            Return nomTablas
        End Try
        '
        Dim schemaDA As New SqlDataAdapter("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_TYPE", dbConnection)
        '
        schemaDA.Fill(dt)
        i = dt.Rows.Count - 1
        If i > -1 Then
            ReDim nomTablas(i)
            For i = 0 To dt.Rows.Count - 1
                ' si el valor de TABLE_SCHEMA no es dbo, es que es una tabla de un usuario particular
                If dt.Rows(i).Item("TABLE_SCHEMA").ToString().ToLower() <> "dbo" Then
                    nomTablas(i) = dt.Rows(i).Item("TABLE_SCHEMA").ToString() & "." & dt.Rows(i).Item("TABLE_NAME").ToString()
                Else
                    nomTablas(i) = dt.Rows(i).Item("TABLE_NAME").ToString()
                End If
            Next
        End If
        ' 
        Return nomTablas
    End Function
    '
    Public Shared Function GenerarClase(lang As eLenguaje,
                                            usarCommandBuilder As Boolean,
                                            nombreClase As String,
                                            nomTabla As String,
                                            dataSource As String,
                                            initialCatalog As String,
                                            cadenaSelect As String,
                                            userId As String,
                                            password As String,
                                            usarSeguridadSQL As Boolean) As String
        Dim s As String
        '
        nombreTabla = nomTabla
        If nombreTabla = "" OrElse nombreClase = "" Then
            Return "ERROR, no se ha indicado el nombre de la tabla o de la clase."
        End If
        s = Conectar(dataSource, initialCatalog, cadenaSelect, userId, password, usarSeguridadSQL)
        If Conectado = False OrElse s <> "" Then
            Return s
        End If
        '
        ' Comprobar si el nombre de la clase tiene espacios     (02/Nov/04)
        ' de ser así, cambiarlo por un guión bajo.
        ' Bug reportado por David Sans
        nombreClase = nombreClase.Replace(" ", "_")
        '
        Return CrearClase.GenerarClaseSQL(lang, usarCommandBuilder, nombreClase, dataSource, initialCatalog, cadenaSelect, userId, password, usarSeguridadSQL)
    End Function
    Public Shared Function GenerarClase(lang As eLenguaje,
                                            usarCommandBuilder As Boolean,
                                            nombreClase As String,
                                            nomTabla As String,
                                            dataSource As String,
                                            initialCatalog As String,
                                            cadenaSelect As String) As String
        '
        Return GenerarClase(lang, usarCommandBuilder, nombreClase, nomTabla, dataSource, initialCatalog, cadenaSelect, "", "", False)
    End Function
End Class

'End Namespace
