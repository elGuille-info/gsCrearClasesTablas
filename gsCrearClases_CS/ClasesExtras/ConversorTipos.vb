'------------------------------------------------------------------------------
' Funciones de conversión a varios tipos                            (23/Jun/15)
' Principalmente pensados en los obtenidos de una base de datos
'
' Definida como clase con los miembros compartidos (es como un módulo)
' Primeras funciones creadas en 2011 para Solicitudes empleo
'
' Añado la propiedad CultureES que la usaba desde Extensiones.vb (14/may/23)
' 
' (c)Guillermo 'guille' Som, 2011, 2015, 2018, 2019, 2021, 2023
'------------------------------------------------------------------------------
Option Strict On
Option Infer On

Imports System
'Imports System.Globalization

''' <summary>
''' Conversor de tipo usados para cuando se leen o guardan en una base de datos
''' Hay funciones para convertir los siguientes tipos:
''' Boolean, DateTime, Integer, Long
''' 
''' Yo suelo usarlos de esta forma:
''' TIPOData (antes ValorTIPO) para asignar de un valor leído de la base de datos al tipo
''' DataTIPO para asignar lo leído de la base de datos a una cadena
''' </summary>
Public Class ConversorTipos

    ' Esta propiedad la usaba desde Extensiones.vb          (14/may/23 10.07)
    Private Shared _CultureES As System.Globalization.CultureInfo

    ''' <summary>
    ''' Devuelve un objeto con el valor de la cultura en español-España (es-ES).
    ''' </summary>
    ''' <returns>Un objeto del tipo <see cref="CultureInfo"/> con la cultura para es-ES.</returns>
    Private Shared ReadOnly Property CultureES As System.Globalization.CultureInfo
        Get
            If _CultureES Is Nothing Then
                _CultureES = System.Globalization.CultureInfo.CreateSpecificCulture("es-ES")
            End If
            Return _CultureES
        End Get
    End Property

    '--------------------------------------------------------------------------
    ' Nuevas definiciones más específicas                           (24/May/19)
    ' Empiezan con el tipo usado y seguido del tipo de argumento
    '--------------------------------------------------------------------------

    '
    ' Boolean
    '

    Public Shared Function BoolObj2Str(obj As Object) As String
        Return ConversorTipos.DataBoolean(obj, False)
    End Function
    Public Shared Function BoolObj2Bool(obj As Object) As Boolean
        Return ConversorTipos.BooleanData(obj)
    End Function
    Public Shared Function BoolStr2Bool(str As String) As Boolean
        If str = "" OrElse str = "0" Then
            Return False
        End If
        Return CBool(str)
    End Function
    ''' <summary>
    ''' Devuelve 1 o 0 según el valor del argumento
    ''' </summary>
    Public Shared Function Bool2Str(b As Boolean) As String
        If b Then
            Return "1"
        Else
            Return "0"
        End If
    End Function

    '
    ' Fechas
    '

    Public Shared Function DateObj2Str(obj As Object) As String
        Return ConversorTipos.DataDateTime(obj)
    End Function
    Public Shared Function DateObj2Date(obj As Object) As Date
        Return ConversorTipos.DateTimeData(obj)
    End Function
    Public Shared Function DateStr2Date(str As String) As Date
        Return ConversorTipos.DataDateTimePicker(str)
    End Function
    Public Shared Function Date2Str(d As Date) As String
        Return d.ToString("dd/MM/yyyy")
    End Function
    Public Shared Function Date2Str(d As Date, usarMinValue As Boolean) As String
        Return ConversorTipos.DataDateTime(d, usarMinValue)
    End Function

    '
    ' TimeSpan
    '

    Public Shared Function TimeSpanObj2Str(obj As Object) As String
        Return ConversorTipos.DataTimeSpan(obj)
    End Function
    Public Shared Function TimeSpanObj2TimeSpan(obj As Object) As TimeSpan
        Return ConversorTipos.TimeSpanData(obj)
    End Function
    Public Shared Function TimeSpanStr2TimeSpan(str As String) As TimeSpan
        Return ConversorTipos.TimeSpanData(str)
    End Function
    Public Shared Function TimeSpan2Str(ts As TimeSpan) As String
        Return ts.ToString("hh\:mm")
    End Function

    '
    ' Decimal
    '

    Public Shared Function DecimalObj2Str(obj As Object) As String
        Return ConversorTipos.DataDecimal(obj)
    End Function
    Public Shared Function DecimalObj2Decimal(obj As Object) As Decimal
        Return ConversorTipos.DecimalData(obj)
    End Function
    Public Shared Function DecimalStr2Decimal(str As String) As Decimal
        Return ConversorTipos.DecimalData(str)
    End Function
    Public Shared Function Decimal2Str(d As Decimal) As String
        Return d.ToString("0.##")
    End Function

    '
    ' Integer
    '

    Public Shared Function IntegerObj2Str(obj As Object) As String
        Return ConversorTipos.DataInteger(obj)
    End Function
    Public Shared Function IntegerObj2Integer(obj As Object) As Integer
        Return ConversorTipos.IntegerData(obj)
    End Function
    Public Shared Function IntegerStr2Integer(str As String) As Integer
        Return ConversorTipos.IntegerData(str)
    End Function
    Public Shared Function Integer2Str(d As Decimal) As String
        Return d.ToString
    End Function




    '--------------------------------------------------------------------------
    ' Las funciones DataTIPO
    '--------------------------------------------------------------------------

    ''' <summary>
    ''' Convierte un tipo Object en un valor Boolean (como cadena)
    ''' "0" para False, "1" para True.
    ''' Si el argumento es nulo, se devuelve una cadena vacía
    ''' si se indica el argumento DevolverNULL.
    ''' Ese objeto es el valor leído de la base de datos.
    ''' </summary>
    Public Shared Function DataBoolean(obj As Object,
                                       Optional devolverNULL As Boolean = False) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            If devolverNULL Then
                Return ""
            Else
                Return "0"
            End If
        Else
            Return If(BooleanData(obj), "1", "0")
        End If
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor Decimal,
    ''' pero se devuelve como cadena.
    ''' Ese objeto es el valor leído de la base de datos
    ''' Si el contenido es válido se devuelve el valor
    ''' si no, se devuelve una cadena vacía.
    ''' Se quitan los ceros que haya después del signo decimal,
    ''' si no tiene decimales, no se muestran los ceros.
    ''' </summary>
    Public Shared Function DataDecimal(obj As Object) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return ""
        Else
            ' Conversión extra para evitar "sustos"                 (06/Oct/11)
            Dim d As Decimal = 0
            Decimal.TryParse(obj.ToString, d)

            Return d.ToString.TrimEnd({"."c, ","c}) '.TrimEnd("0"c).TrimEnd({"."c, ","c})
        End If
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor Integer,
    ''' pero se devuelve como cadena.
    ''' Si el parámetro es nulo se devuelve una cadena vacía.
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function DataInt32(obj As Object) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return ""
        Else
            Dim d As Integer = 0
            Integer.TryParse(obj.ToString, d)

            Return d.ToString
        End If
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor Integer,
    ''' pero se devuelve como cadena.
    ''' Si el parámetro es nulo se devuelve una cadena vacía.
    ''' </summary>
    ''' <remarks>16/May/19</remarks>
    Public Shared Function DataInteger(obj As Object) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return ""
        Else
            Dim d As Integer = 0
            Integer.TryParse(obj.ToString, d)

            Return d.ToString
        End If
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor Long,
    ''' pero se devuelve como cadena.
    ''' Si el parámetro es nulo se devuelve una cadena vacía.
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function DataInt64(obj As Object) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return ""
        Else
            Dim d As Long = 0
            Long.TryParse(obj.ToString, d)

            Return d.ToString
        End If
    End Function

    ''' <summary>
    ''' Convierte una fecha en cadena.<br/>
    ''' Si la fecha es 01/01/1900 se devuelve una cadena vacía
    ''' </summary>
    ''' <remarks>24/Jun/15</remarks>
    Public Shared Function DataDateTime(dt As Date) As String
        Return DataDateTime(dt, False)
    End Function

    ''' <summary>
    ''' Convierte una fecha en cadena.<br/>
    ''' Si usarMinValue es False, se devuelve una cadena vacía si la fecha es 1/1/1900
    ''' </summary>
    ''' <remarks>24/Jun/15</remarks>
    Public Shared Function DataDateTime(dt As Date, usarMinValue As Boolean) As String
        Dim s = dt.ToString("dd/MM/yyyy")
        If dt.Year < 1900 Then s = "01/01/1900"
        If s = "01/01/1900" AndAlso usarMinValue = False Then s = ""

        Return s
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en una cadena.<br/>
    ''' Ese objeto es el valor leído de la base de datos.<br/>
    ''' Si la fecha es válida se devuelve en formato dd/MM/yyyy,
    ''' si no, se devuelve una cadena vacía.<br/>
    ''' Si la fecha es 01/01/1900 devuelve una cadena vacía.
    ''' </summary>
    Public Shared Function DataDateTime(obj As Object) As String
        Return DataDateTime(obj, False)
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en una cadena.<br/>
    ''' Ese objeto es el valor leído de la base de datos.<br/>
    ''' Si la fecha es válida se devuelve en formato dd/MM/yyyy,
    ''' si no, se devuelve una cadena vacía.<br/>
    ''' Si la fecha es 01/01/1900 devuelve una cadena vacía,
    ''' salvo si usarMinValue = True, en cuyo caso no se devuelve una cadena vacía
    ''' si no el valor 01/01/1900.
    ''' </summary>
    ''' <returns>Una cadena con solo la fecha.</returns>
    Public Shared Function DataDateTime(obj As Object, usarMinValue As Boolean) As String
        Dim sMin As String = ""
        If usarMinValue Then
            sMin = "01/01/1900"
        End If
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return sMin
        Else
            'Return CDate(obj).ToString("dd/MM/yyyy")
            ' por si es una cadena vacía                            (12/Oct/11)
            If TypeOf obj Is String AndAlso String.IsNullOrWhiteSpace(obj.ToString) Then
                Return sMin
            End If

            ' hacer las cosas bien                                  (04/Dic/18)
            Dim d = New DateTime(1900, 1, 1, 0, 0, 0)

            'Dim culture = Globalization.CultureInfo.CreateSpecificCulture("es-ES")
            Dim styles = Globalization.DateTimeStyles.None

            'DateTime.TryParse(txt, d)
            ' Usar siempre la conversión al estilo de España        (29/Mar/21)
            Date.TryParse(obj.ToString, CultureES, styles, d)
            'Date.TryParse(obj.ToString, d)
            If d.Year < 1900 Then
                d = New DateTime(1900, 1, 1, 0, 0, 0)
            End If
            Dim s = d.ToString("dd/MM/yyyy")
            If s = "01/01/1900" AndAlso usarMinValue = False Then s = ""
            Return s
        End If

    End Function

    ''' <summary>
    ''' Convierte una cadena en un valor DateTime
    ''' para asignar los DateTimePicker a partir del valor del TextBox asociado.<br/>
    ''' Si la fecha no es válida se devuelve la fecha actual.<br/>
    ''' También se usa para asignar el campo en los parámetros del SqlCommand.
    ''' </summary>
    ''' <returns>Solo la parte de la fecha (no la hora).</returns>
    Public Shared Function DataDateTimePicker(str As String) As Date
        Dim d As New DateTime(1900, 1, 1, 0, 0, 0)

        If Not (String.IsNullOrWhiteSpace(str) OrElse str.Equals(DBNull.Value)) Then
            'Dim culture = Globalization.CultureInfo.CreateSpecificCulture("es-ES")
            Dim styles = Globalization.DateTimeStyles.None

            ' Usar siempre la conversión al estilo de España        (29/Mar/21)
            Date.TryParse(str, CultureES, styles, d)

            'DateTime.TryParse(str, d)
            If d.Year < 1900 Then
                d = New DateTime(1900, 1, 1, 0, 0, 0)
            End If
        Else
            ' asignar el 01/01/1900 si es un valor en blanco        (07/Jul/15)
            d = New DateTime(1900, 1, 1, 0, 0, 0)
        End If

        Return d.Date
    End Function

    ''' <summary>
    ''' Convierte una cadena en una fecha (DateTime.Date)
    ''' devuelve DBNull si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>Solo devuelve la parte de la fecha.</remarks>
    Public Shared Function DataDateTimeDate(str As String) As Object
        Dim d As Date

        If Date.TryParse(str, d) Then
            Return d.Date
        Else
            Return DBNull.Value
        End If
    End Function

    ''' <summary>
    ''' Convierte un valor TimeSpan en una cadena con formato HH:mm
    ''' </summary>
    ''' <remarks>24/Mar/2019</remarks>
    Public Shared Function DataTimeSpan(val As TimeSpan) As String
        Return val.ToString("hh\:mm")
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor TimeSpan,
    ''' pero se devuelve como cadena.
    ''' </summary>
    ''' <remarks>24/Mar/2019</remarks>
    Public Shared Function DataTimeSpan(obj As Object) As String
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return "00:00"
        Else
            Dim d As TimeSpan = New TimeSpan(0, 0, 0)
            TimeSpan.TryParse(obj.ToString, d)

            Return d.ToString("hh\:mm")
        End If
    End Function


    '--------------------------------------------------------------------------
    ' Las funciones TIPOData. Devuelve el TIPO y recibe Object o String.
    '--------------------------------------------------------------------------

    ''' <summary>
    ''' Convierte un tipo Object en un valor Boolean
    ''' Ese objeto es el valor leído de la base de datos
    ''' Si el contenido es válido se devuelve el valor
    ''' si no, se devuelve un valor falso.
    ''' </summary>
    Public Shared Function BooleanData(obj As Object) As Boolean
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return False
        Else
            Dim o = False
            Boolean.TryParse(obj.ToString, o)

            Return o
        End If
    End Function

    ''' <summary>
    ''' convierte una cadena en un valor Decimal
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    Public Shared Function DecimalData(str As String) As Decimal
        Dim d As Decimal = 0

        Decimal.TryParse(str, d)

        Return d
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un valor Decimal,
    ''' </summary>
    ''' <remarks>04/Dic/18</remarks>
    Public Shared Function DecimalData(obj As Object) As Decimal
        Return DecimalData(DataDecimal(obj))
    End Function

    ''' <summary>
    ''' convierte una cadena en un Double
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function DoubleData(str As String) As Double
        Dim i As Double = 0

        Double.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un Single
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function SingleData(str As String) As Single
        Dim i As Single = 0

        Single.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un Byte
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function ByteData(str As String) As Byte
        Dim i As Byte = 0

        Byte.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un SByte
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function SByteData(str As String) As SByte
        Dim i As SByte = 0

        SByte.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un Int16
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function Int16Data(str As String) As Short
        Dim i As Short = 0

        Short.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un Integer
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>26/Mar/2019</remarks>
    Public Shared Function IntegerData(str As String) As Integer
        Dim i As Integer = 0

        Integer.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un Integer
    ''' </summary>
    ''' <remarks>26/Mar/2019</remarks>
    Public Shared Function IntegerData(obj As Object) As Integer
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return Int32Data("")
        End If
        Return Int32Data(obj.ToString)
    End Function

    ''' <summary>
    ''' convierte una cadena en un Integer
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    Public Shared Function Int32Data(str As String) As Integer
        Dim i As Integer = 0

        Integer.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un Integer
    ''' </summary>
    ''' <remarks>04/Dic/18</remarks>
    Public Shared Function Int32Data(obj As Object) As Integer
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return 0 'Int32Data("")
        End If
        Return Int32Data(obj.ToString)
    End Function

    ''' <summary>
    ''' convierte una cadena en un Long (Int64)
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function Int64Data(str As String) As Long
        Dim i As Long = 0

        Long.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' Convierte un tipo Object en un Long
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function Int64Data(obj As Object) As Long
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return Int64Data("")
        End If
        Return Int64Data(obj.ToString)
    End Function

    ''' <summary>
    ''' convierte una cadena en un UInt16
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function UInt16Data(str As String) As UShort
        Dim i As UShort = 0

        UShort.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un UInt32 (UInteger)
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    Public Shared Function UInt32Data(str As String) As UInteger
        Dim i As UInteger = 0

        UInteger.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un UInt64 (Ulong)
    ''' devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function UInt64Data(str As String) As ULong
        Dim i As ULong = 0

        ULong.TryParse(str, i)

        Return i
    End Function

    ''' <summary>
    ''' convierte una cadena en un Char
    ''' devuelve un valor Nothing (nulo) si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function CharData(str As String) As Char
        Dim c As Char = Nothing

        Char.TryParse(str, c)

        Return c
    End Function

    ''' <summary>
    ''' Convierte una cadena en una fecha (Date)
    ''' Si la fecha es incorrecta devuelve el 01/01/1900
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function DateTimeData(str As String) As Date
        Dim f = New Date(1900, 1, 1, 0, 0, 0)

        Date.TryParse(str, f)

        Return f
    End Function

    ''' <summary>
    ''' Convierte una cadena en una fecha (Date)
    ''' Si la fecha es incorrecta devuelve el 01/01/1900
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function DateTimeData(obj As Object) As Date
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return DateTimeData("")
        End If
        Return DateTimeData(obj.ToString)
    End Function

    ''' <summary>
    ''' convierte una cadena en un TimeSpan
    ''' devuelve {00:00:00} (nulo) si no es un valor adecuado (cadena vacía o nulo)
    ''' </summary>
    ''' <remarks>15/Dic/18</remarks>
    Public Shared Function TimeSpanData(str As String) As TimeSpan
        Dim c As New TimeSpan(0, 0, 0)

        TimeSpan.TryParse(str, c)

        Return c
    End Function

    ''' <summary>
    ''' Convierte un Object en un TimeSpan
    ''' </summary>
    ''' <remarks>24/Mar/2018</remarks>
    Public Shared Function TimeSpanData(obj As Object) As TimeSpan
        If obj Is Nothing OrElse obj.Equals(DBNull.Value) Then
            Return New TimeSpan(0, 0, 0)
        Else
            Dim d As New TimeSpan(0, 0, 0)
            TimeSpan.TryParse(obj.ToString, d)

            Return d
        End If
    End Function

End Class
