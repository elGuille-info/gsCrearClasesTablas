'------------------------------------------------------------------------------
' Clase para crear una clase a partir de una tabla de Access        (12/Jul/04)
'
' Basado en el código de    crearClaseVB                            (21/Mar/04)
'                           CrearClaseSQL                           (07/Jul/04)
'
' Todos los métodos son estáticos (compartidos) para usarlos sin crear una instancia
'
' Muevo esta clase al formulario ya que en la DLL no es necesaria   (05/oct/22)
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
Imports System.Data.OleDb

Imports elGuille.Util.Developer
Imports elGuille.Util.Developer.Data

' Le quito el Namespace, para que esté en el de este proyecto. (05/oct/22 11.29)
'Namespace elGuille.Util.Developer.Data
Public Class CrearClaseOleDb
    Inherits CrearClase

    Public Shared Function Conectar(baseDeDatos As String) As String
        Return Conectar(baseDeDatos, "", "", "")
    End Function
    Public Shared Function Conectar(baseDeDatos As String,
                                    cadenaSelect As String) As String
        Return Conectar(baseDeDatos, cadenaSelect, "", "")
    End Function
    Public Shared Function Conectar(baseDeDatos As String,
                                    cadenaSelect As String,
                                    provider As String) As String
        Return Conectar(baseDeDatos, cadenaSelect, provider, "")
    End Function
    Public Shared Function Conectar(baseDeDatos As String,
                                    cadenaSelect As String,
                                    provider As String,
                                    password As String) As String
        ' si se produce algún error, se devuelve una cadena empezando por ERROR
        Conectado = False
        If provider = "" Then
            provider = "Microsoft.Jet.OLEDB.4.0"
        ElseIf provider.IndexOf("Provider=") > -1 Then
            Dim i As Integer = provider.IndexOf("=")
            provider = provider.Substring(i)
        End If

        If password <> "" Then
            CadenaConexion = "Provider=" & provider & "; Data Source=" & baseDeDatos & "; Jet OLEDB:Database Password=" & password
        Else
            CadenaConexion = "Provider=" & provider & "; Data Source=" & baseDeDatos
        End If

        If cadenaSelect = "" Then
            Conectado = True
            Return ""
        End If

        Dim dbDataAdapter As New OleDbDataAdapter(cadenaSelect, CadenaConexion) With {
            .MissingSchemaAction = MissingSchemaAction.AddWithKey
        }

        ' Limpiar el contenido de ElDataTable                    (08/Jun/05)
        'ElDataTable = New DataTable
        ' Usar Clear en vez de crear un neuvo objeto.  (14/may/23 11.24)
        ElDataTable.Clear()

        Try
            dbDataAdapter.Fill(ElDataTable)
            System.Threading.Thread.Sleep(100)
            Conectado = True
        Catch ex As OleDbException
            If ex.Message.ToLower().IndexOf("not a valid password") > -1 Then
                Return "ERROR en fill, el password indicado no es correcto (o no se ha indicado): " & ex.Message & " - " & ex.GetType().Name
            Else
                Return "ERROR en fill:" & vbCrLf & ex.Message & " - " & ex.GetType().Name
            End If
        Catch ex As Exception
            Return "ERROR en fill:" & vbCrLf & ex.Message & " - " & ex.GetType().Name
        End Try
        '
        Return ""
    End Function
    '
    'Public Shared Function NombresTablas() As String()
    ''' <summary>
    ''' Devuelve una colección de tipo string con las tablas.
    ''' </summary>
    ''' <remarks>Antes usaba un array de tipo string.</remarks>
    Public Shared Function NombresTablas() As List(Of String)
        Dim nomTablas As List(Of String) = Nothing
        Dim dt As DataTable
        Dim restrictions() As Object = {Nothing, Nothing, Nothing, "TABLE"}
        Dim dbConnection As New OleDbConnection(CadenaConexion)

        Try
            dbConnection.Open()
        Catch ex As Exception
            nomTablas = New List(Of String) From {
                "ERROR: " & ex.Message
            }
            Conectado = False
            Return nomTablas
        End Try

        dt = dbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, restrictions)
        Dim i As Integer = dt.Rows.Count - 1
        If i > -1 Then
            nomTablas = New List(Of String)
            For i = 0 To dt.Rows.Count - 1
                nomTablas.Add(dt.Rows(i).Item("TABLE_NAME").ToString())
            Next
        End If

        Return nomTablas
    End Function
    '
    Public Shared Function GenerarClase(lang As eLenguaje,
                                            usarCommandBuilder As Boolean,
                                            nombreClase As String,
                                            nomTabla As String,
                                            baseDeDatos As String,
                                            cadenaSelect As String,
                                            password As String,
                                            provider As String) As String
        Dim s As String
        '
        NombreTabla = nomTabla
        If NombreTabla = "" OrElse nombreClase = "" Then
            Return "ERROR, no se ha indicado el nombre de la tabla o de la clase."
        End If
        s = Conectar(baseDeDatos, cadenaSelect, provider, password)
        If Conectado = False OrElse s <> "" Then
            Return s
        End If
        '
        ' Comprobar si el nombre de la clase tiene espacios     (02/Nov/04)
        ' de ser así, cambiarlo por un guión bajo.
        ' Bug reportado por David Sans
        nombreClase = nombreClase.Replace(" ", "_")
        '
        Return CrearClase.GenerarClaseOleDb(lang, usarCommandBuilder, nombreClase, baseDeDatos, cadenaSelect, password, provider)
    End Function
    Public Shared Function GenerarClase(lang As eLenguaje,
                                            usarCommandBuilder As Boolean,
                                            nombreClase As String,
                                            nomTabla As String,
                                            baseDeDatos As String,
                                            cadenaSelect As String) As String
        '
        Return GenerarClase(lang, usarCommandBuilder, nombreClase, nomTabla, baseDeDatos, cadenaSelect, "", "")
    End Function
    Public Shared Function GenerarClase(lang As eLenguaje,
                                            usarCommandBuilder As Boolean,
                                            nombreClase As String,
                                            nomTabla As String,
                                            baseDeDatos As String,
                                            cadenaSelect As String,
                                            provider As String) As String
        '
        Return GenerarClase(lang, usarCommandBuilder, nombreClase, nomTabla, baseDeDatos, cadenaSelect, "", provider)
    End Function
End Class

'End Namespace
