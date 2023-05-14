// ----------------------------------------------------------------------------
// Funciones de conversión a varios tipos                           (23/Jun/15)
// Principalmente pensados en los obtenidos de una base de datos
// Definida como clase con los miembros compartidos (es como un módulo)
// Primeras funciones creadas en 2011 para Solicitudes empleo
//
// Añado la propiedad CultureES que la usaba desde Extensiones.vb   (14/may/23)
// 
// Convertida a C# a partir de ConversorTipos.vb
// con ayuda de: https://converter.telerik.com/
//
// Y algunos ajustes en los parámetros ByRef que deben ser out en C#
//
// (c)Guillermo 'guille' Som, 2011, 2015, 2018, 2019, 2021, 2023
// ----------------------------------------------------------------------------

using System;

/// <summary>
/// Conversor de tipo usados para cuando se leen o guardan en una base de datos
/// Hay funciones para convertir los siguientes tipos:
/// Boolean, DateTime, Integer, Long
/// 
/// Yo suelo usarlos de esta forma:
/// TIPOData (antes ValorTIPO) para asignar de un valor leído de la base de datos al tipo
/// DataTIPO para asignar lo leído de la base de datos a una cadena
/// </summary>
public class ConversorTipos
{
    // La versión de C# usada es la 10.0
    //#error version

    // Esta propiedad la usaba desde Extensiones.vb          (14/may/23 10.07)
    private static System.Globalization.CultureInfo _CultureES;

    /// <summary>
    ///     ''' Devuelve un objeto con el valor de la cultura en español-España (es-ES).
    ///     ''' </summary>
    ///     ''' <returns>Un objeto del tipo <see cref="CultureInfo"/> con la cultura para es-ES.</returns>
    private static System.Globalization.CultureInfo CultureES
    {
        get
        {
            if (_CultureES == null)
            {
                _CultureES = System.Globalization.CultureInfo.CreateSpecificCulture("es-ES");
            }
            return _CultureES;
        }
    }
    // --------------------------------------------------------------------------
    // Nuevas definiciones más específicas                           (24/May/19)
    // Empiezan con el tipo usado y seguido del tipo de argumento
    // --------------------------------------------------------------------------

    // 
    // Boolean
    // 

    public static string BoolObj2Str(object obj)
    {
        return ConversorTipos.DataBoolean(obj, false);
    }
    public static bool BoolObj2Bool(object obj)
    {
        return ConversorTipos.BooleanData(obj);
    }
    public static bool BoolStr2Bool(string str)
    {
        if (str == "" || str == "0")
            return false;
        return System.Convert.ToBoolean(str);
    }
    /// <summary>
    /// Devuelve 1 o 0 según el valor del argumento
    /// </summary>
    public static string Bool2Str(bool b)
    {
        return b ? "1" : "0";
    }

    // 
    // Fechas
    // 

    public static string DateObj2Str(object obj)
    {
        return ConversorTipos.DataDateTime(obj);
    }
    public static DateTime DateObj2Date(object obj)
    {
        return ConversorTipos.DateTimeData(obj);
    }
    public static DateTime DateStr2Date(string str)
    {
        return ConversorTipos.DataDateTimePicker(str);
    }
    public static string Date2Str(DateTime d)
    {
        return d.ToString("dd/MM/yyyy");
    }
    public static string Date2Str(DateTime d, bool usarMinValue)
    {
        return ConversorTipos.DataDateTime(d, usarMinValue);
    }

    // 
    // TimeSpan
    // 

    public static string TimeSpanObj2Str(object obj)
    {
        return ConversorTipos.DataTimeSpan(obj);
    }
    public static TimeSpan TimeSpanObj2TimeSpan(object obj)
    {
        return ConversorTipos.TimeSpanData(obj);
    }
    public static TimeSpan TimeSpanStr2TimeSpan(string str)
    {
        return ConversorTipos.TimeSpanData(str);
    }
    public static string TimeSpan2Str(TimeSpan ts)
    {
        return ts.ToString(@"hh\:mm");
    }

    // 
    // Decimal
    // 

    public static string DecimalObj2Str(object obj)
    {
        return ConversorTipos.DataDecimal(obj);
    }
    public static decimal DecimalObj2Decimal(object obj)
    {
        return ConversorTipos.DecimalData(obj);
    }
    public static decimal DecimalStr2Decimal(string str)
    {
        return ConversorTipos.DecimalData(str);
    }
    public static string Decimal2Str(decimal d)
    {
        return d.ToString("0.##");
    }

    // 
    // Integer
    // 

    public static string IntegerObj2Str(object obj)
    {
        return ConversorTipos.DataInteger(obj);
    }
    public static int IntegerObj2Integer(object obj)
    {
        return ConversorTipos.IntegerData(obj);
    }
    public static int IntegerStr2Integer(string str)
    {
        return ConversorTipos.IntegerData(str);
    }
    public static string Integer2Str(decimal d)
    {
        return d.ToString();
    }


    // --------------------------------------------------------------------------
    // Las funciones DataTIPO
    // --------------------------------------------------------------------------

    /// <summary>
    /// Convierte un tipo Object en un valor Boolean (como cadena)
    /// "0" para False, "1" para True.
    /// Si el argumento es nulo, se devuelve una cadena vacía
    /// si se indica el argumento DevolverNULL.
    /// Ese objeto es el valor leído de la base de datos.
    /// </summary>
    public static string DataBoolean(object obj, bool devolverNULL = false)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        {
            return devolverNULL ? "" : "0";
        }
        else
            return BooleanData(obj) ? "1" : "0";
    }

    /// <summary>
    /// Convierte un tipo Object en un valor Decimal,
    /// pero se devuelve como cadena.
    /// Ese objeto es el valor leído de la base de datos
    /// Si el contenido es válido se devuelve el valor
    /// si no, se devuelve una cadena vacía.
    /// Se quitan los ceros que haya después del signo decimal,
    /// si no tiene decimales, no se muestran los ceros.
    /// </summary>
    public static string DataDecimal(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return "";
        else
        {
            // Conversión extra para evitar "sustos"                 (06/Oct/11)
            decimal d;
            decimal.TryParse(obj.ToString(), out d);

            return d.ToString().TrimEnd(new[] { '.', ',' });
        }
    }

    /// <summary>
    /// Convierte un tipo Object en un valor Integer,
    /// pero se devuelve como cadena.
    /// Si el parámetro es nulo se devuelve una cadena vacía.
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static string DataInt32(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return "";
        else
        {
            int d;
            int.TryParse(obj.ToString(), out d);

            return d.ToString();
        }
    }

    /// <summary>
    /// Convierte un tipo Object en un valor Integer,
    /// pero se devuelve como cadena.
    /// Si el parámetro es nulo se devuelve una cadena vacía.
    /// </summary>
    /// <remarks>16/May/19</remarks>
    public static string DataInteger(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return "";
        else
        {
            int d;
            int.TryParse(obj.ToString(), out d);

            return d.ToString();
        }
    }

    /// <summary>
    /// Convierte un tipo Object en un valor Long,
    /// pero se devuelve como cadena.
    /// Si el parámetro es nulo se devuelve una cadena vacía.
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static string DataInt64(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return "";
        else
        {
            long d;
            long.TryParse(obj.ToString(), out d);

            return d.ToString();
        }
    }

    /// <summary>
    /// Convierte una fecha en cadena.<br/>
    /// Si la fecha es 01/01/1900 se devuelve una cadena vacía
    /// </summary>
    /// <remarks>24/Jun/15</remarks>
    public static string DataDateTime(DateTime dt)
    {
        return DataDateTime(dt, false);
    }

    /// <summary>
    /// Convierte una fecha en cadena.<br/>
    /// Si usarMinValue es False, se devuelve una cadena vacía si la fecha es 1/1/1900
    /// </summary>
    /// <remarks>24/Jun/15</remarks>
    public static string DataDateTime(DateTime dt, bool usarMinValue)
    {
        var s = dt.ToString("dd/MM/yyyy");
        if (dt.Year < 1900)
            s = "01/01/1900";
        if (s == "01/01/1900" && usarMinValue == false)
            s = "";

        return s;
    }

    /// <summary>
    /// Convierte un tipo Object en una cadena.<br/>
    /// Ese objeto es el valor leído de la base de datos.<br/>
    /// Si la fecha es válida se devuelve en formato dd/MM/yyyy,
    /// si no, se devuelve una cadena vacía.<br/>
    /// Si la fecha es 01/01/1900 devuelve una cadena vacía.
    /// </summary>
    public static string DataDateTime(object obj)
    {
        return DataDateTime(obj, false);
    }

    /// <summary>
    /// Convierte un tipo Object en una cadena.<br/>
    /// Ese objeto es el valor leído de la base de datos.<br/>
    /// Si la fecha es válida se devuelve en formato dd/MM/yyyy,
    /// si no, se devuelve una cadena vacía.<br/>
    /// Si la fecha es 01/01/1900 devuelve una cadena vacía,
    /// salvo si usarMinValue = True, en cuyo caso no se devuelve una cadena vacía
    /// si no el valor 01/01/1900.
    /// </summary>
    /// <returns>Una cadena con solo la fecha.</returns>
    public static string DataDateTime(object obj, bool usarMinValue)
    {
        string sMin = "";
        if (usarMinValue)
            sMin = "01/01/1900";
        if (obj == null || obj.Equals(DBNull.Value))
            return sMin;
        else
        {
            // Return CDate(obj).ToString("dd/MM/yyyy")
            // por si es una cadena vacía                            (12/Oct/11)
            if (obj is string && string.IsNullOrWhiteSpace(obj.ToString()))
                return sMin;

            // hacer las cosas bien                                  (04/Dic/18)
            var d = new DateTime(1900, 1, 1, 0, 0, 0);

            var styles = System.Globalization.DateTimeStyles.None;

            // DateTime.TryParse(txt, d)
            // Usar siempre la conversión al estilo de España        (29/Mar/21)
            DateTime.TryParse(obj.ToString(), CultureES, styles, out d);
            if (d.Year < 1900)
                d = new DateTime(1900, 1, 1, 0, 0, 0);
            var s = d.ToString("dd/MM/yyyy");
            if (s == "01/01/1900" && usarMinValue == false)
                s = "";
            return s;
        }
    }

    /// <summary>
    /// Convierte una cadena en un valor DateTime
    /// para asignar los DateTimePicker a partir del valor del TextBox asociado.<br/>
    /// Si la fecha no es válida se devuelve la fecha actual.<br/>
    /// También se usa para asignar el campo en los parámetros del SqlCommand.
    /// </summary>
    /// <returns>Solo la parte de la fecha (no la hora).</returns>
    public static DateTime DataDateTimePicker(string str)
    {
        if (string.IsNullOrWhiteSpace(str) || str.Equals(DBNull.Value))
        {
            // asignar el 01/01/1900 si es un valor en blanco        (07/Jul/15)
            return new DateTime(1900, 1, 1, 0, 0, 0);
        }
        var styles = System.Globalization.DateTimeStyles.None;
        // Usar siempre la conversión al estilo de España        (29/Mar/21)
        if (DateTime.TryParse(str, CultureES, styles, out DateTime d) == false)
        { return new DateTime(1900, 1, 1, 0, 0, 0); }
        return d.Date;
    }

    /// <summary>
    /// Convierte una cadena en una fecha (DateTime.Date)
    /// devuelve DBNull si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>Solo devuelve la parte de la fecha.</remarks>
    public static object DataDateTimeDate(string str)
    {
        if (DateTime.TryParse(str, out DateTime d))
        { return d.Date; }
        else
        { return DBNull.Value; }
    }

    /// <summary>
    /// Convierte un valor TimeSpan en una cadena con formato HH:mm
    /// </summary>
    /// <remarks>24/Mar/2019</remarks>
    public static string DataTimeSpan(TimeSpan val)
    {
        return val.ToString(@"hh\:mm");
    }

    /// <summary>
    /// Convierte un tipo Object en un valor TimeSpan,
    /// pero se devuelve como cadena.
    /// </summary>
    /// <remarks>24/Mar/2019</remarks>
    public static string DataTimeSpan(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        { return "00:00"; }
        else
        {
            //TimeSpan d;
            if (TimeSpan.TryParse(obj.ToString(), out TimeSpan d) == false)
            { return "00:00"; }
            //{ d = new TimeSpan(0, 0, 0); }
            return d.ToString(@"hh\:mm");
        }
    }


    // --------------------------------------------------------------------------
    // Las funciones TIPOData. Devuelve el TIPO y recibe Object o String.
    // --------------------------------------------------------------------------

    /// <summary>
    /// Convierte un tipo Object en un valor Boolean
    /// Ese objeto es el valor leído de la base de datos
    /// Si el contenido es válido se devuelve el valor
    /// si no, se devuelve un valor falso.
    /// </summary>
    public static bool BooleanData(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return false;
        else
        {
            //bool o;
            if (bool.TryParse(obj.ToString(), out bool o) == false)
            { return false; }
            return o;
        }
    }

    /// <summary>
    /// convierte una cadena en un valor Decimal
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    public static decimal DecimalData(string str)
    {
        // Usando inlined declaration
        //decimal d;

        if (decimal.TryParse(str, out decimal d) == false)
        { return 0; }
        return d;
    }

    /// <summary>
    /// Convierte un tipo Object en un valor Decimal,
    /// </summary>
    /// <remarks>04/Dic/18</remarks>
    public static decimal DecimalData(object obj)
    {
        return DecimalData(DataDecimal(obj));
    }

    /// <summary>
    /// convierte una cadena en un Double
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static double DoubleData(string str)
    {
        //double i;
        if (double.TryParse(str, out double i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// convierte una cadena en un Single
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static float SingleData(string str)
    {
        //float i;
        if (float.TryParse(str, out float i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// convierte una cadena en un Byte
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static byte ByteData(string str)
    {
        //byte i;
        if (byte.TryParse(str, out byte i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// convierte una cadena en un SByte
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static sbyte SByteData(string str)
    {
        //sbyte i;
        if (sbyte.TryParse(str, out sbyte i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// convierte una cadena en un Int16
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static short Int16Data(string str)
    {
        //short i;
        if (short.TryParse(str, out short i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// convierte una cadena en un Integer
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>26/Mar/2019</remarks>
    public static int IntegerData(string str)
    {
        //int i;
        if (int.TryParse(str, out int i) == false)
        { return 0; }
        return i;
    }

    /// <summary>
    /// Convierte un tipo Object en un Integer
    /// </summary>
    /// <remarks>26/Mar/2019</remarks>
    public static int IntegerData(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        { return Int32Data(""); }
        return Int32Data(obj.ToString());
    }

    /// <summary>
    /// convierte una cadena en un Integer
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    public static int Int32Data(string str)
    {
        //int i;
        if (int.TryParse(str, out int i) == false)
        { return 0; }

        return i;
    }

    /// <summary>
    /// Convierte un tipo Object en un Integer
    /// </summary>
    /// <remarks>04/Dic/18</remarks>
    public static int Int32Data(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        { return 0; }
        return Int32Data(obj.ToString());
    }

    /// <summary>
    /// convierte una cadena en un Long (Int64)
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static long Int64Data(string str)
    {
        //long i;
        if (long.TryParse(str, out long i) == false)
        { return 0; }

        return i;
    }

    /// <summary>
    /// Convierte un tipo Object en un Long
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static long Int64Data(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        { return Int64Data(""); }
        return Int64Data(obj.ToString());
    }

    /// <summary>
    /// convierte una cadena en un UInt16
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static ushort UInt16Data(string str)
    {
        //ushort i;
        if (ushort.TryParse(str, out ushort i) == false)
        { return 0; }

        return i;
    }

    /// <summary>
    /// convierte una cadena en un UInt32 (UInteger)
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    public static uint UInt32Data(string str)
    {
        //uint i;
        if (uint.TryParse(str, out uint i) == false)
        { return 0; }

        return i;
    }

    /// <summary>
    /// convierte una cadena en un UInt64 (Ulong)
    /// devuelve cero si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static ulong UInt64Data(string str)
    {
        //ulong i;
        if (ulong.TryParse(str, out ulong i) == false)
        { return 0; }

        return i;
    }

    /// <summary>
    /// convierte una cadena en un Char
    /// devuelve un valor Nothing (nulo) si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static char CharData(string str)
    {
        //char c;
        if (char.TryParse(str, out char c) == false)
        { return default; }
        //{ return '\0'; }

        return c;
    }

    /// <summary>
    /// Convierte una cadena en una fecha (Date)
    /// Si la fecha es incorrecta devuelve el 01/01/1900
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static DateTime DateTimeData(string str)
    {
        //var f = new DateTime(1900, 1, 1, 0, 0, 0);

        if (DateTime.TryParse(str, out DateTime f) == false)
        { return new DateTime(1900, 1, 1, 0, 0, 0); }
        //{ return default; }

        return f;
    }

    /// <summary>
    /// Convierte una cadena en una fecha (Date)
    /// Si la fecha es incorrecta devuelve el 01/01/1900
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static DateTime DateTimeData(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
        { return DateTimeData(""); }
        return DateTimeData(obj.ToString());
    }

    /// <summary>
    /// convierte una cadena en un TimeSpan
    /// devuelve {00:00:00} (nulo) si no es un valor adecuado (cadena vacía o nulo)
    /// </summary>
    /// <remarks>15/Dic/18</remarks>
    public static TimeSpan TimeSpanData(string str)
    {
        //TimeSpan c = new TimeSpan(0, 0, 0);

        if (TimeSpan.TryParse(str, out TimeSpan c) == false)
        { return default; }

        return c;
    }

    /// <summary>
    /// Convierte un Object en un TimeSpan
    /// </summary>
    /// <remarks>24/Mar/2018</remarks>
    public static TimeSpan TimeSpanData(object obj)
    {
        if (obj == null || obj.Equals(DBNull.Value))
            return new TimeSpan(0, 0, 0);
        else
        {
            //TimeSpan d = new TimeSpan(0, 0, 0);
            if (TimeSpan.TryParse(obj.ToString(), out TimeSpan d) == false)
            { return default; }

            return d;
        }
    }
}
