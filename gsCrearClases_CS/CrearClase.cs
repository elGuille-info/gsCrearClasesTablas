// ------------------------------------------------------------------------------
// Clase genérica para crear una clase                               (13/Jul/04)
// Esta clase se usará como base de las de SQL y OleDb
// 
// Nota: Ver las revisiones en Revisiones.txt
// Usar el fichero Revisiones.md en vez del .txt                 (01/oct/22 12.49)
// Quitar de gitHub los ficheros elguille.snk                    (01/oct/22 12.52)
// Creo el fichero elGuille_compartido.snk para firmar los ensamblados.    (13.13)
//
// Convertido a C# con:                                         (06/oct/22 20.18)
// https://converter.telerik.com/
// Con bastante ayuda
// 
// ©Guillermo 'guille' Som, 2004, 2005, 2007, 2018-2022
// ------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
//using System.Data.SqlClient;
//using System.Data.OleDb;
using elGuille.Util.Developer;

namespace elGuille.Util.Developer.Data
{
    public class CrearClase
    {
        /// <summary>
        /// El retorno de carro y cambio de línea. (vbCrLf)
        /// </summary>
        public static string CrLf => "\r\n";
        // 
        protected static DataTable mDataTable = new DataTable();
        protected static string cadenaConexion;
        protected static string nombreTabla { get; set; } = "Tabla1";
        // 
        public static bool Conectado;
        // 
        // Campos para usar desde las clases derivadas
        // para usar con SQL
        private static string dataSource;
        private static string initialCatalog;
        private static string userId;
        private static bool usarSeguridadSQL;
        // 
        // para usar con OleDb (Access)
        private static string baseDeDatos;
        private static string provider;
        // 
        // para ambos tipos de bases de datos
        private static bool usarCommandBuilder = true;
        private static string dbPrefix = "Sql";
        private static string password;
        private static bool esSQL;
        private static eLenguaje lang;
        private static string nombreClase;
        private static string cadenaSelect;

        // Si se usa ExecuteScalar / NonQuery en vez de DataAdapter

        /// <summary>
        /// Si se usa DataAdapter o ExecuteScalar ExecuteNonQuery
        /// </summary>
        /// <remarks>07/Abr/19 13.17
        /// Ya lo tenía con UsarExecuteScalar del 23/Mar/19
        /// </remarks>
        public static bool UsarDataAdapter { get; set; } = true;

        // Si se utiliza .Parameters.Add o .Parameters.AddWithValue  (19/Mar/19)

        /// <summary>
        /// Si se utiliza .Parameters.Add o .Parameters.AddWithValue.
        /// True  usará .Parameters.AddWithValue
        /// False usará .Parameters.Add
        /// Valor predeterminado = True
        /// </summary>
        /// <remarks>19/Mar/2019</remarks>
        public static bool UsarAddWithValue { get; set; } = true;

        /// <summary>
        /// Si se usa Overrides (override en C#) en los métodos
        /// Actualizar, Crear y Borrar.
        /// </summary>
        /// <remarks>25/Mar/2019</remarks>
        public static bool UsarOverrides { get; set; } = true;

        /// <summary>
        /// Para definir las propiedades autoimplementadas (salvo cuando es string).
        /// </summary>
        public static bool PropiedadAuto { get; set; } = true;

        // estos métodos sólo se usarán desde las clases derivadas
        protected static string GenerarClaseOleDb(eLenguaje lang, bool usarCommandBuilder, string nombreClase, string baseDeDatos, string cadenaSelect, string password, string provider)
        {
            esSQL = false;
            if (provider == "")
                provider = "Microsoft.Jet.OLEDB.4.0";
            CrearClase.lang = lang;
            CrearClase.usarCommandBuilder = usarCommandBuilder;
            CrearClase.baseDeDatos = baseDeDatos;
            CrearClase.nombreClase = nombreClase;
            CrearClase.cadenaSelect = cadenaSelect;
            CrearClase.password = password;
            CrearClase.provider = provider;
            dbPrefix = "OleDb";
            // ------------------------------------------------------------------
            // Esto es lo que hace que no funcione                   (08/Jun/05)
            // ------------------------------------------------------------------
            // crear una nueva instancia del dataTable               (07/Feb/05)
            // mDataTable = New DataTable
            // ------------------------------------------------------------------
            // 
            return generarClase();
        }
        // 
        protected static string GenerarClaseSQL(eLenguaje lang, bool usarCommandBuilder, string nombreClase, string dataSource, string initialCatalog, string cadenaSelect, string userId, string password, bool usarSeguridadSQL)
        {
            esSQL = true;
            CrearClase.lang = lang;
            CrearClase.usarCommandBuilder = usarCommandBuilder;
            CrearClase.nombreClase = nombreClase;
            CrearClase.dataSource = dataSource;
            CrearClase.initialCatalog = initialCatalog;
            CrearClase.cadenaSelect = cadenaSelect;
            CrearClase.userId = userId;
            CrearClase.password = password;
            CrearClase.usarSeguridadSQL = usarSeguridadSQL;
            dbPrefix = "Sql";
            // ------------------------------------------------------------------
            // Esto es lo que hace que no funcione                   (08/Jun/05)
            // ------------------------------------------------------------------
            // crear una nueva instancia del dataTable               (07/Feb/05)
            // mDataTable = New DataTable
            // ------------------------------------------------------------------
            // 
            return generarClase();
        }
        // 
        private static string generarClase()
        {
            // generar la clase a partir de la tabla seleccionada,
            // las columnas son los campos a usar
            // 
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string s;
            System.Text.StringBuilder sb1;
            System.Text.StringBuilder sb2;
            System.Text.StringBuilder sb3;
            string campoIDnombre = "ID";
            string campoIDtipo = "System.Int32";
            Hashtable campos = new Hashtable();
            string novalidos = @" -ºª!|@#$%&/()=?¿*+^'¡-<>,.;:{}[]Çç€\" + "\"" + "\t";
            // 
            // buscar el campo autoincremental de la tabla           (12/Jul/04)
            // también se buscará si es Unique
            foreach (DataColumn col in mDataTable.Columns)
            {
                // comprobar si tiene caracteres no válidos          (14/Jul/04)
                // en caso de que sea así, sustituirlos por un guión bajo
                int i;
                s = col.ColumnName;
                do
                {
                    i = s.IndexOfAny(novalidos.ToCharArray());
                    if (i > -1)
                    {
                        if (i == s.Length - 1)
                            s = s.Substring(0, i) + "_";
                        else if (i > 0)
                            s = s.Substring(0, i) + "_" + s.Substring(i + 1);
                        else
                            s = "_" + s.Substring(i + 1);
                    }
                }
                while (i > -1);
                campos.Add(col.ColumnName, s);
                // 
                // No siempre el predeterminado es AutoIncrement
                if (col.AutoIncrement || col.Unique)
                {
                    campoIDnombre = s; // col.ColumnName
                    campoIDtipo = col.DataType.ToString();
                }
            }
            // 
            // 
            ConvLang.Lang = lang;
            sb.AppendFormat("{0}{1}", ConvLang.Comentario("------------------------------------------------------------------------------"), CrLf);
            if (esSQL)
            {
                sb.AppendFormat("{0} Clase {1} generada automáticamente con CrearClaseSQL{2}", ConvLang.Comentario(), nombreClase, CrLf);
                sb.AppendFormat("{0} de la tabla '{1}' de la base '{2}'{3}", ConvLang.Comentario(), nombreTabla, initialCatalog, CrLf);
            }
            else
            {
                sb.AppendFormat("{0} Clase {1} generada automáticamente con CrearClaseOleDb{2}", ConvLang.Comentario(), nombreClase, CrLf);
                sb.AppendFormat("{0} de la tabla '{1}' de la base '{2}'{3}", ConvLang.Comentario(), nombreTabla, baseDeDatos, CrLf);
            }
            // Al mostrar MMM/ se muestra mar./ para el mes de marzo (22/Mar/19)
            sb.AppendFormat("{0} Fecha: {1}{2}", ConvLang.Comentario(), DateTime.Now.ToString("dd/MMM/yyyy HH:mm:ss").Replace("./", "/"), CrLf);
            sb.AppendFormat("{0}{1}", ConvLang.Comentario(), CrLf);
            // Cambio 'guille' por (elGuille)                        (22/Mar/19)
            if (DateTime.Now.Year > 2022)
                sb.AppendFormat("{0}{1}", ConvLang.Comentario(string.Format(" ©Guillermo Som (elGuille), 2004-{0}", DateTime.Now.Year)), CrLf);
            else
                sb.AppendFormat("{0}{1}", ConvLang.Comentario(" ©Guillermo Som (elGuille), 2004-2022"), CrLf);
            sb.AppendFormat("{0}{1}", ConvLang.Comentario("------------------------------------------------------------------------------"), CrLf);
            // 
            if (lang == eLenguaje.eVBNET)
            {
                sb.AppendFormat("Option Strict On{0}", CrLf);
                // Añado Option Infer On                             (17/Nov/18)
                sb.AppendFormat("Option Infer On{0}", CrLf);
                sb.AppendFormat("{0}{1}", ConvLang.Comentario(), CrLf);
                // Añado Imports Microsoft.VisualBasic               (17/Nov/18)
                // No hay que indicar Imports en la cadena           (20/Nov/18)
                sb.AppendFormat("{0}{1}", ConvLang.Imports("Microsoft.VisualBasic"), CrLf);
            }
            // las importaciones de espacios de nombres
            sb.AppendFormat("{0}{1}", ConvLang.Imports("System"), CrLf);
            sb.AppendFormat("{0}{1}", ConvLang.Imports("System.Data"), CrLf);
            if (esSQL)
            {
                sb.AppendFormat("{0}{1}", ConvLang.Imports("System.Data.SqlClient"), CrLf);
                sb.AppendFormat("{0}{1}", ConvLang.Comentario(" Por si se utiliza Microsoft.Data en lugar de System.Data."), CrLf);
                sb.AppendFormat("{0}{1}", ConvLang.Imports("Microsoft.Data.SqlClient"), CrLf);
            }
                
            else
                sb.AppendFormat("{0}{1}", ConvLang.Imports("System.Data.OleDb"), CrLf);
            sb.AppendFormat("{0}{1}", ConvLang.Comentario(), CrLf);
            // 
            // declaración clase
            sb.AppendFormat("{0}{1}", ConvLang.Class("Public", nombreClase), CrLf);
            // 
            // ------------------------------------------------------------------
            // los campos privados para usar con las propiedades
            // ------------------------------------------------------------------
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Las variables privadas"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" TODO: Revisar los tipos de los campos"), CrLf);
            // Si se usan las propiedades autoimplementadas solo crear las variables privadas para el tipo string.
            if (PropiedadAuto)
            {
                // Los nombres de los campos privados empiezan con m_ (30/Nov/18)
                foreach (DataColumn col in mDataTable.Columns)
                {
                    if (col.DataType.ToString() == "System.String")
                    {
                        sb.AppendFormat("    {0}{1}", ConvLang.Variable("Private", "m_" + campos[col.ColumnName].ToString(), col.DataType.ToString()), CrLf);
                    }
                }
            }
            else
            {
                foreach (DataColumn col in mDataTable.Columns)
                    // Los nombres de los campos privados empiezan con m_ (30/Nov/18)
                    sb.AppendFormat("    {0}{1}", ConvLang.Variable("Private", "m_" + campos[col.ColumnName].ToString(), col.DataType.ToString()), CrLf);
            }
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(""), CrLf);
            // 
            // ajustarAncho: método privado para ajustar los caracteres de los campos de tipo String
            // sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Este método se usará para ajustar los anchos de las propiedades"), vbCrLf)
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    ", " Este método se usará para ajustar los anchos de las propiedades"));
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Private", "ajustarAncho", "String", "cadena", "String", "ancho", "Integer"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.VariableNewParam("Dim", "sb", "System.Text.StringBuilder", "New String(\" \"c, ancho)"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" devolver la cadena quitando los espacios en blanco"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" esto asegura que no se devolverá un tamaño mayor ni espacios \"extras\""), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Return("(cadena & sb.ToString()).Substring(0, ancho).Trim()"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // Propiedades públicas de instancia
            // 
            // ------------------------------------------------------------------
            // propiedades públicas mapeadas a cada columna de la tabla
            // ------------------------------------------------------------------
            // Añadir líneas de separación para facilitar la lectura (30/Nov/18)
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("------------------------------------------------------------------"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Las propiedades públicas"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" TODO: Revisar los tipos de las propiedades"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("------------------------------------------------------------------"), CrLf);
            foreach (DataColumn col in mDataTable.Columns)
            {
                if (col.DataType.ToString() == "System.Byte[]")
                {
                    sb.AppendFormat("    {0}{1}", ConvLang.Property("Public", "System.Byte()", campos[col.ColumnName].ToString()), CrLf);
                }
                else
                {
                    // Si se usa Overrides, y es autoincrement,      (13/Abr/19)
                    // añadirle el Overrides
                    if (UsarOverrides && col.AutoIncrement == true)
                        sb.AppendFormat("    {0}{1}", ConvLang.Property("Public Overrides", col.DataType.ToString(), campos[col.ColumnName].ToString()), CrLf);
                    else
                        sb.AppendFormat("    {0}{1}", ConvLang.Property("Public", col.DataType.ToString(), campos[col.ColumnName].ToString()), CrLf);
                }
                if (PropiedadAuto == false || col.DataType.ToString() == "System.String")
                {
                    sb.AppendFormat("        {0}{1}", ConvLang.Get(), CrLf);

                    if (col.DataType.ToString() != "System.String")
                    {
                        sb.AppendFormat("            {0}{1}", ConvLang.Return(" m_" + campos[col.ColumnName].ToString()), CrLf);
                    }
                    else if (col.MaxLength > 255)
                    {
                        sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Seguramente sería mejor sin ajustar el ancho..."), CrLf);
                        sb.AppendFormat("            {0}{1}", ConvLang.Comentario(ConvLang.Return(string.Format("ajustarAncho(m_{0},{1})", campos[col.ColumnName].ToString(), col.MaxLength))), CrLf);
                        sb.AppendFormat("            {0}{1}", ConvLang.Return(" m_" + campos[col.ColumnName].ToString()), CrLf);
                    }
                    else
                    {
                        sb.AppendFormat("            {0}{1}", ConvLang.Return(string.Format("ajustarAncho(m_{0},{1})", campos[col.ColumnName].ToString(), col.MaxLength)), CrLf);
                    }
                    sb.AppendFormat("        {0}{1}", ConvLang.EndGet(), CrLf);

                    if (col.DataType.ToString() == "System.Byte[]")
                    {
                        sb.AppendFormat("        {0}{1}", ConvLang.Set("System.Byte()"), CrLf);
                    }
                    else
                    {
                        sb.AppendFormat("        {0}{1}", ConvLang.Set(col.DataType.ToString()), CrLf);
                    }
                    sb.AppendFormat("            {0}{1}", ConvLang.Asigna("m_" + campos[col.ColumnName].ToString(), "value"), CrLf);
                    sb.AppendFormat("        {0}{1}", ConvLang.EndSet(), CrLf);
                    sb.AppendFormat("    {0}{1}", ConvLang.EndProperty(), CrLf);
                }
                else
                {
                    if (lang == eLenguaje.eCS)
                    {
                        sb.AppendFormat("        get; set; }}{0}", CrLf);
                    }
                }
            }
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // Item: propiedad predeterminada (indizador)
            // permite acceder a los campos mediante un índice numérico
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    "," Propiedad predeterminada (indizador) Permite acceder mediante un índice numérico"));
            sb.AppendFormat("    {0}{1}", ConvLang.Property("Public", "String", "index", "Integer"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Devuelve el contenido del campo indicado en index"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" (el índice corresponde con la columna de la tabla)"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Get(), CrLf);
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                if (i == 0)
                    sb.AppendFormat("            {0}{1}", ConvLang.If("index", "=", "0"), CrLf);
                else
                    sb.AppendFormat("            {0}{1}", ConvLang.ElseIf("index", "=", i.ToString()), CrLf);
                if (col.DataType.ToString() == "System.Byte[]")
                    // TODO: convertir el array de bytes en una cadena...
                    sb.AppendFormat("                {0}{1}", ConvLang.Return("<Binario largo>"), CrLf);
                else
                    sb.AppendFormat("                {0}{1}", ConvLang.Return("Me." + campos[col.ColumnName].ToString() + ".ToString()"), CrLf);
            }
            sb.AppendFormat("            {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Para que no de error el compilador de C# y"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" se devuelva el valor <NULO> en caso de que no exista ese campo."), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Return("\"<NULO>\""), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndGet(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Set("String"), CrLf);
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                if (i == 0)
                    sb.AppendFormat("            {0}{1}", ConvLang.If("index", "=", "0"), CrLf);
                else
                    sb.AppendFormat("            {0}{1}", ConvLang.ElseIf("index", "=", i.ToString()), CrLf);
                // 
                switch (col.DataType.ToString())
                {
                    case "System.String":
                        {
                            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), "value"), CrLf);
                            break;
                        }

                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Char":
                    case "System.TimeSpan":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("Me.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("ConversorTipos.{1}Data(value)", col.ColumnName, col.DataType.ToString().Replace("System.", ""))), CrLf);
                            break;
                        }

                    case "System.Byte[]":
                        {
                            sb.AppendFormat("                {0} Es un Binario largo (array de Byte){1}", ConvLang.Comentario(), CrLf);
                            sb.AppendFormat("                {0} y por tanto no se le puede asignar el contenido de una cadena...{1}", ConvLang.Comentario(), CrLf);
                            break;
                        }

                    default:
                        {
                            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(" TODO: Comprobar la conversión a realizar"), CrLf);
                            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(string.Format("       con el tipo {0}", col.DataType.ToString())), CrLf);
                            sb.AppendFormat("                {2}{0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), "value"), CrLf, ConvLang.Comentario());
                            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), col.DataType.ToString() + ".Parse(value)"), CrLf);
                            break;
                        }
                }
            }
            sb.AppendFormat("            {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndSet(), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndProperty(), CrLf);
            // 
            // ------------------------------------------------------------------
            // La propiedad Item usando el nombre de la columna      (11/Jul/04)
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    "," Propiedad predeterminada (indizador) Permite acceder mediante el nombre de una columna"));
            sb.AppendFormat("    {0}{1}", ConvLang.Property("Public", "String", "index", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Devuelve el contenido del campo indicado en index"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" (el índice corresponde al nombre de la columna)"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Get(), CrLf);
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                if (i == 0)
                    sb.AppendFormat("            {0}{1}", ConvLang.If("index", "=", "\"" + campos[col.ColumnName].ToString() + "\""), CrLf);
                else
                    sb.AppendFormat("            {0}{1}", ConvLang.ElseIf("index", "=", "\"" + campos[col.ColumnName].ToString() + "\""), CrLf);
                if (col.DataType.ToString() == "System.Byte[]")
                    // TODO: convertir el array de bytes en una cadena...
                    sb.AppendFormat("                {0}{1}", ConvLang.Return("<Binario largo>"), CrLf);
                else
                    sb.AppendFormat("                {0}{1}", ConvLang.Return("Me." + campos[col.ColumnName].ToString() + ".ToString()"), CrLf);
            }
            sb.AppendFormat("            {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Para que no de error el compilador de C# y"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" se devuelva el valor <NULO> en caso de que no exista ese campo."), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Return("\"" + "<NULO>" + "\""), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndGet(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Set("String"), CrLf);
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                if (i == 0)
                    sb.AppendFormat("            {0}{1}", ConvLang.If("index", "=", "\"" + campos[col.ColumnName].ToString() + "\""), CrLf);
                else
                    sb.AppendFormat("            {0}{1}", ConvLang.ElseIf("index", "=", "\"" + campos[col.ColumnName].ToString() + "\""), CrLf);
                // 
                switch (col.DataType.ToString())
                {
                    case "System.String":
                        {
                            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), "value"), CrLf);
                            break;
                        }

                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Char":
                    case "System.TimeSpan":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("Me.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("ConversorTipos.{1}Data(value)", col.ColumnName, col.DataType.ToString().Replace("System.", ""))), CrLf);
                            break;
                        }

                    case "System.Byte[]":
                        {
                            sb.AppendFormat("                {0} Es un Binario largo (array de Byte){1}", ConvLang.Comentario(), CrLf);
                            sb.AppendFormat("                {0} y por tanto no se le puede asignar el contenido de una cadena...{1}", ConvLang.Comentario(), CrLf);
                            break;
                        }

                    default:
                        {
                            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(" TODO: Comprobar la conversión a realizar"), CrLf);
                            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(string.Format("       con el tipo {0}", col.DataType.ToString())), CrLf);
                            sb.AppendFormat("                {2}{0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), "value"), CrLf, ConvLang.Comentario());
                            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("Me." + campos[col.ColumnName].ToString(), col.DataType.ToString() + ".Parse(value)"), CrLf);
                            break;
                        }
                }
            }
            sb.AppendFormat("            {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndSet(), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndProperty(), CrLf);

            sb.AppendLine();
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("-------------------------------------------------------------------------"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Campos y métodos compartidos (estáticos) para gestionar la base de datos"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("-------------------------------------------------------------------------"), CrLf);
            sb.AppendLine();

            // ------------------------------------------------------------------
            // la cadena de conexión
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}{1}", ConvLang.DocumentacionXML("    "," La cadena de conexión a la base de datos.",
                " Definida Public para poder asignar otro valor",
                " por si se usan diferentes bases de datos."
            ), CrLf);
            sb1 = new System.Text.StringBuilder();
            if (lang == eLenguaje.eCS)
                sb1.Append("@");
            if (esSQL)
            {
                sb1.AppendFormat("\"Data Source={0};", dataSource);
                sb1.AppendFormat(" Initial Catalog={0};", initialCatalog);
                if (usarSeguridadSQL)
                    sb1.AppendFormat(" user id={0}; password={1}", userId, password);
                else
                    sb1.Append(" Integrated Security=yes;");
            }
            else if (password != "")
                sb1.AppendFormat("{0}Provider={1}; Data Source={2}; Jet OLEDB:Database Password={3}", "\"", provider, baseDeDatos, password);
            else
                sb1.AppendFormat("{0}Provider={1}; Data Source={2}", "\"", provider, baseDeDatos);
            sb1.Append("\"");
            // Añado Property a CadenaConexion y CadenaSelect        (13/Abr/19)
            // para que sea más fácil saber las referencias que tienen.
            sb.AppendFormat("    {0}{1}", ConvLang.DeclaraVariable("Public Shared Property", "CadenaConexion", "String", sb1.ToString()), CrLf);
            // 
            // ------------------------------------------------------------------
            // la cadena de selección (campo público)
            // ------------------------------------------------------------------
            sb.AppendFormat("    {0}{1}", ConvLang.DocumentacionXML(" La cadena de selección"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.DeclaraVariable("Public Shared Property", "CadenaSelect", "String", "\"" + cadenaSelect + "\""), CrLf);
            sb.AppendLine();

            // ------------------------------------------------------------------
            // constructores
            // Uno sin parámetros y otro que recibe la cadena de conexión
            // ------------------------------------------------------------------
            sb.AppendFormat("    {0}{1}", ConvLang.Constructor("Public", nombreClase), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndSub(), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Constructor("Public", nombreClase, "conex", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Asigna("CadenaConexion", "conex"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndSub(), CrLf);

            sb.AppendLine();
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("-----------------------------------------"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Métodos compartidos (estáticos) privados"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("-----------------------------------------"), CrLf);
            sb.AppendLine();

            // ------------------------------------------------------------------
            // row2<nombreClase>: asigna una fila de la tabla a un objeto del tipo de la clase
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    "," Asigna una fila de la tabla a un objeto " + nombreClase));
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "Row2Tipo", nombreClase, "r", "DataRow"), CrLf);
            sb.AppendFormat("        {2} asigna a un objeto {0} los datos del dataRow indicado{1}", nombreClase, CrLf, ConvLang.Comentario());
            sb.AppendFormat("        {0}{1}", ConvLang.VariableNew("Dim", "o_" + nombreClase, nombreClase), CrLf);
            sb.AppendLine();
            foreach (DataColumn col in mDataTable.Columns)
            {
                switch (col.DataType.ToString())
                {
                    case "System.String":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"].ToString()", col.ColumnName)), CrLf);
                            break;
                        }

                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Char":
                    case "System.TimeSpan":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("ConversorTipos.{1}Data(r[\"{0}\"].ToString())", col.ColumnName, col.DataType.ToString().Replace("System.", ""))), CrLf);
                            break;
                        }

                    case "System.Byte[]":
                        {
                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"]", col.ColumnName)), CrLf, ConvLang.Comentario());
                            break;
                        }

                    default:
                        {
                            // El resto de tipos
                            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" TODO: Comprobar la conversión a realizar"), CrLf);
                            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(string.Format("       con el tipo {0}", col.DataType.ToString())), CrLf);
                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"]", col.ColumnName)), CrLf, ConvLang.Comentario());
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("{1}.Parse(r[\"{0}\"].ToString())", col.ColumnName, col.DataType.ToString())), CrLf);
                            break;
                        }
                }
            }
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("o_" + nombreClase), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // <nombreClase>2Row: asigna un objeto de la clase a la fila indicada
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    ",string.Format(" Asigna un objeto {0} a la fila indicada", nombreClase)));
            sb.AppendFormat("    {0}{1}", ConvLang.Sub("Private Shared", string.Format("{0}2Row", nombreClase), "o_" + nombreClase, nombreClase, "r", "DataRow"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(string.Format(" asigna un objeto {0} al dataRow indicado", nombreClase)), CrLf);
            foreach (DataColumn col in mDataTable.Columns)
            {
                // Si es AutoIncrement no asignarle un valor         (10/Jul/04)
                // si es Unique y no AutoIncrement se debe asignar   (13/Jul/04)
                if (col.AutoIncrement)
                {
                    sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" TODO: Comprueba si esta asignación debe hacerse"), CrLf);
                    sb.AppendFormat("        {0}{1}", ConvLang.Comentario("       pero mejor lo dejas comentado ya que es un campo autoincremental o único"), CrLf);
                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna(string.Format("r[\"{0}\"]", col.ColumnName), string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString())), CrLf, ConvLang.Comentario());
                }
                else
                    sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("r[\"{0}\"]", col.ColumnName), string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString())), CrLf);
            }
            sb.AppendFormat("    {0}{1}", ConvLang.EndSub(), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // nuevo<nombreClase>: crea una nueva fila y la asigna a un objeto de la clase
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    ",string.Format(" Crea una nueva fila y la asigna a un objeto {0}", nombreClase)
            ));
            sb.AppendFormat("    {0}{1}", ConvLang.Sub("Private Shared", "nuevo" + nombreClase, "dt", "DataTable", "o_" + nombreClase, nombreClase), CrLf);
            sb.AppendFormat("        {2} Crear un nuevo {0}{1}", nombreClase, CrLf, ConvLang.Comentario());
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "dr", "DataRow", "dt.NewRow()"), CrLf);
            // ------------------------------------------------------------------
            // En lugar de "o" & nombreClase.Substring(0, 1),        (02/Nov/04)
            // usar "o_"  & nombreClase.Substring(0, 1)
            // ya que si la clase empieza por R,
            // se creará una variable llamada oR, que no es válida.
            // Gracias a David Sans por la indicación.
            // ------------------------------------------------------------------
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "o_" + nombreClase.Substring(0, 1), nombreClase, "Row2Tipo" + "(dr)"), CrLf);
            sb.AppendLine();
            foreach (DataColumn col in mDataTable.Columns)
                sb.AppendFormat("        o_{0}.{1} = o_{2}.{1}{3}{4}", nombreClase.Substring(0, 1), campos[col.ColumnName].ToString(), nombreClase, ConvLang.FinInstruccion(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}2Row(o_{1}, dr){2}{3}", nombreClase, nombreClase.Substring(0, 1), ConvLang.FinInstruccion(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Instruccion("dt.Rows.Add(dr)"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndSub(), CrLf);
            // 
            // Métodos públicos compartidos (estáticos)
            // 
            sb.AppendLine();
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("------------------"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario(" Métodos públicos"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Comentario("------------------"), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // Tabla: devuelve una tabla con los datos indicados en la cadena de selección
            // hay dos sobrecargas: una sin parámetros y
            // otra en la que se indica la cadena de selección a usar
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}{1}", ConvLang.DocumentacionXML("    "," Devuelve una tabla con los datos indicados en la cadena de selección"
            ), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "Tabla", "DataTable"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Return("Tabla(CadenaSelect)"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "Tabla", "DataTable", "sel", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(string.Format(" devuelve una tabla con los datos de la tabla {0}", nombreTabla)), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Variable("Dim", "da", dbPrefix + "DataAdapter"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.VariableNewParam("Dim", "dt", "DataTable", string.Format("{0}{1}{0}", "\"", nombreClase)), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Try(), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.AsignaNew("da", dbPrefix + "DataAdapter(sel, CadenaConexion)"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("da.Fill(dt)"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Catch(), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Return("Nothing"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndTry(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("dt"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // Buscar                                                (10/Jul/04)
            // ------------------------------------------------------------------
            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    "," Busca en la tabla los datos indicados en el parámetro.",
                " El parámetro contendrá lo que se usará después del WHERE.",
                " Si no se encuentra lo buscado, se devuelve un valor nulo."
            ));
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "Buscar", nombreClase, "sWhere", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "o_" + nombreClase, nombreClase, "Nothing"), CrLf);
            // Dim sel As String = "SELECT * FROM Clientes WHERE " & sWhere
            sb.AppendFormat("        {0}{1}", ConvLang.Variable("Dim", "sel", "String", string.Format("{0}SELECT * FROM {1} WHERE {0} & sWhere", "\"", nombreTabla)), CrLf);
            // Using con As New SqlConnection(CadenaConexion)
            sb.AppendFormat("        {0}{1}", ConvLang.Using("con", "SqlConnection", "CadenaConexion"), CrLf);
            // Dim cmd As New SqlCommand()
            sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariableNew("Dim", "cmd", "SqlCommand"), CrLf);
            // Dim reader As SqlDataReader
            sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "reader", "SqlDataReader"), CrLf);
            // cmd.CommandType = CommandType.Text
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandType", "CommandType.Text"), CrLf);
            // cmd.Connection = con
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.Connection", "con"), CrLf);
            // cmd.CommandText = sel
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandText", "sel"), CrLf);
            // Try
            sb.AppendFormat("            {0}{1}", ConvLang.Try(), CrLf);
            // con.Open()
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Open()"), CrLf);
            // reader = cmd.ExecuteReader()
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("reader", "cmd.ExecuteReader()"), CrLf);
            // While reader.Read
            sb.AppendFormat("                {0}{1}", ConvLang.While("reader.Read()"), CrLf);
            sb.AppendFormat("                    {0}{1}", ConvLang.Asigna("o_" + nombreClase, "Reader2Tipo(reader)"), CrLf);
            // If o_Clientes.ID > 0 Then
            sb.AppendFormat("                    {0}{1}", ConvLang.If($"o_{nombreClase}.ID", ">", "0"), CrLf);
            // Exit While
            sb.AppendFormat("                        {0}{1}", ConvLang.Exit("While"), CrLf);
            sb.AppendFormat("                    {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.EndWhile(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("reader.Close()"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Close()"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Return("Nothing"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.EndTry(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndUsing(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("o_" + nombreClase), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // Métodos públicos de instancia
            // 
            // ------------------------------------------------------------------
            // Actualizar: Actualiza los datos en la tabla usando la instancia actual
            // ------------------------------------------------------------------

            // 
            // Cambio la actualización e inserción por comandos,     (22/Mar/19)
            // sin usar el dataAdapter
            // 

            sb.AppendFormat("{0}", ConvLang.DocumentacionXML("    "," Actualizar: Actualiza los datos en la tabla usando la instancia actual",
                " Si la instancia no hace referencia a un registro existente, se creará uno nuevo",
                " Para comprobar si el objeto en memoria coincide con uno existente,",
                " se comprueba si el " + campoIDnombre + " existe en la tabla.",
                " TODO: Si en lugar de " + campoIDnombre + " usas otro campo, indicalo en la cadena SELECT",
                " También puedes usar la sobrecarga en la que se indica la cadena SELECT a usar"
            ));
            if (UsarOverrides)
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Overrides", "Actualizar", "String"), CrLf);
            else
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public", "Actualizar", "String"), CrLf);

            sb.AppendFormat("        {0} TODO: Poner aquí la selección a realizar para acceder a este registro{1}", ConvLang.Comentario(), CrLf);
            sb.AppendFormat("        {0}       yo uso el {2} que es el identificador único de cada registro{1}", ConvLang.Comentario(), CrLf, campoIDnombre);
            if (campoIDtipo.IndexOf("String") > -1)
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "sel", "String", string.Format("{0}SELECT * FROM {1} WHERE {2} = '{0} & Me.{2} & {0}'{0}", "\"", nombreTabla, campoIDnombre)), CrLf);
            else
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "sel", "String", string.Format("{0}SELECT * FROM {1} WHERE {2} = {0} & Me.{2}.ToString()", "\"", nombreTabla, campoIDnombre)), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.Return("Actualizar(sel)"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            sb.AppendFormat(ConvLang.DocumentacionXML("    "," Actualiza los datos de la instancia actual.",
                " En caso de error, devolverá la cadena de error empezando por ERROR:."
            ));
            if (UsarDataAdapter)
                sb.AppendFormat(ConvLang.DocumentacionXML("    "," Si la instancia no hace referencia a un registro existente, se creará uno nuevo."
                ));
            else
                sb.AppendFormat(ConvLang.DocumentacionXML("    "," Usando ExecuteNonQuery si la instancia no hace referencia a un registro existente, NO se creará uno nuevo."
                ));
            if (UsarOverrides)
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Overrides", "Actualizar", "String", "sel", "String"), CrLf);
            else
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public", "Actualizar", "String", "sel", "String"), CrLf);
            sb.AppendFormat("        {1} Actualiza los datos indicados{0}", CrLf, ConvLang.Comentario());
            sb.AppendFormat("        {1} El parámetro, que es una cadena de selección, indicará el criterio de actualización{0}", CrLf, ConvLang.Comentario());
            sb.AppendLine();
            // El código para usar Command o DataAdapter             (07/Abr/19)
            if (UsarDataAdapter)
            {
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "cnn", dbPrefix + "Connection"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "da", dbPrefix + "DataAdapter"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariableNewParam("Dim", "dt", "DataTable", string.Format("{0}{1}{0}", "\"", nombreClase)), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.AsignaNew("cnn", dbPrefix + "Connection", "CadenaConexion"), CrLf);
                sb.AppendFormat("        {0}{1}{2}", ConvLang.Comentario(), ConvLang.AsignaNew("da", dbPrefix + "DataAdapter", "CadenaSelect" + ", cnn"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.AsignaNew("da", dbPrefix + "DataAdapter", "sel, cnn"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Asigna("da.MissingSchemaAction", "MissingSchemaAction.AddWithKey"), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("-------------------------------------------"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Esta no es la más óptima, pero funcionará"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("-------------------------------------------"), CrLf);
                sb.AppendFormat("        {2}{0}{1}", ConvLang.DeclaraVariableNewParam("Dim", "cb", dbPrefix + "CommandBuilder", "da"), CrLf, ConvLang.Comentario(!usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna("da.UpdateCommand", "cb.GetUpdateCommand()"), CrLf, ConvLang.Comentario(!usarCommandBuilder));
                sb.AppendLine();
                // 
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("--------------------------------------------------------------------"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Esta está más optimizada pero debes comprobar que funciona bien..."), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("--------------------------------------------------------------------"), CrLf);
                sb.AppendFormat("        {2}{0}{1}", ConvLang.DeclaraVariable("Dim", "sCommand", "String"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Comentario(), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{1} El comando UPDATE{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{1} TODO: Comprobar cual es el campo de índice principal (sin duplicados){0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {3}{1}       Yo compruebo que sea un campo llamado {2}, pero en tu caso puede ser otro{0}", CrLf, ConvLang.Comentario(), campoIDnombre, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {3}{1}       Ese campo, (en mi caso {2}) será el que hay que poner al final junto al WHERE.{0}", CrLf, ConvLang.Comentario(), campoIDnombre, ConvLang.Comentario(usarCommandBuilder));
            }
            else
            {
                // Dim msg = ""
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "msg", "String"), CrLf);
                sb.AppendLine();
                // Using con As New SqlConnection(CadenaConexion)
                sb.AppendFormat("        {0}{1}", ConvLang.Using("con", "SqlConnection", "CadenaConexion"), CrLf);
                // Dim tran As SqlTransaction = Nothing
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "tran", "SqlTransaction", "Nothing"), CrLf);
                // Try
                sb.AppendFormat("            {0}{1}", ConvLang.Try(), CrLf);
                // con.Open()
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("con.Open()"), CrLf);
                // tran = con.BeginTransaction()
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("tran", "con.BeginTransaction()"), CrLf);
                // Dim cmd As New SqlCommand()
                sb.AppendLine();
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariableNew("Dim", "cmd", "SqlCommand"), CrLf);
                // cmd.CommandType = CommandType.StoredProcedure
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandType", "CommandType.StoredProcedure")), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandType", "CommandType.Text"), CrLf);
                // cmd.Connection = con
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.Connection", "con"), CrLf);
                // cmd.CommandText = "ActualizarCliente"
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandText", $"\"Actualizar{nombreTabla}\"")), CrLf, "\"");
                sb.AppendLine();
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "sCommand", "String"), CrLf);
            }
            // 
            sb1 = new System.Text.StringBuilder();
            sb2 = new System.Text.StringBuilder();
            // 
            sb1.AppendFormat("{0}UPDATE {1} SET ", "\"", nombreTabla);
            // 
            s = "";
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                // si el campo tiene caracteres no válidos           (14/Jul/04)
                // ponerlo entre corchetes
                s = col.ColumnName;
                if (campos[col.ColumnName].ToString() != s)
                    s = "[" + col.ColumnName + "]";
                if (campos[col.ColumnName].ToString() != campoIDnombre)
                {
                    if (esSQL)
                        sb2.AppendFormat("{0} = @{0}, ", s);
                    else
                        sb2.AppendFormat("{0} = ?, ", s);
                }
            }
            s = sb2.ToString().TrimEnd();
            if (s.EndsWith(","))
                sb1.AppendFormat("{0} ", s.Substring(0, s.Length - 1));
            else
                sb1.AppendFormat("{0} ", s);
            if (esSQL)
                sb1.AppendFormat(" WHERE ({1} = @{1}){0}", "\"", campoIDnombre);
            else
                sb1.AppendFormat(" WHERE ({1} = ?){0}", "\"", campoIDnombre);
            // 
            if (UsarDataAdapter)
            {
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna("sCommand", sb1.ToString()), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.AsignaNew("da.UpdateCommand", dbPrefix + "Command", "sCommand, cnn"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                if (esSQL)
                {
                    foreach (DataColumn col in mDataTable.Columns)
                    {
                        switch (col.DataType.ToString())
                        {
                            case "System.String":
                                {
                                    if (col.MaxLength > 255)
                                    {
                                        sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", col.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                        if (UsarAddWithValue)
                                        {
                                            s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, col.MaxLength, "\"");
                                            sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, 0, "\"");
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            s = string.Format("{2}@{0}{2}, SqlDbType.NText, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"");
                                            sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{2}@{0}{2}, SqlDbType.NText, {1}, {2}{0}{2}", col.ColumnName, 0, "\"");
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                    }
                                    else if (UsarAddWithValue)
                                    {
                                        s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, col.MaxLength, "\"");
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        s = string.Format("{2}@{0}{2}, SqlDbType.NVarChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"");
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }

                                    break;
                                }

                            default:
                                {
                                    if (UsarAddWithValue)
                                    {
                                        s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"", tipoSQL(col.DataType.ToString()));
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{1}@{0}{1}, {2}, 0, {1}{0}{1}", col.ColumnName, "\"", tipoSQL(col.DataType.ToString()));
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }

                                    break;
                                }
                        }
                    }
                }
                else
                {
                    int j = mDataTable.Columns.Count;
                    int k;
                    string[] sp = new string[j + 1];
                    for (k = 0; k <= j; k++)
                        sp[k] = "p" + (k + 1).ToString();
                    k = 0;
                    string sp1;
                    DataColumn colID = null;
                    foreach (DataColumn col in mDataTable.Columns)
                    {
                        if (campos[col.ColumnName].ToString() == campoIDnombre)
                            colID = col;
                        else
                        {
                            sp1 = sp[k];
                            k += 1;
                            switch (col.DataType.ToString())
                            {
                                case "System.String":
                                    {
                                        if (col.MaxLength > 255)
                                        {
                                            sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", col.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                            if (UsarAddWithValue)
                                            {
                                                s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, col.MaxLength, "\"", sp1);
                                                sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                                s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, 0, "\"", sp1);
                                                sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            }
                                            else
                                            {
                                                s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"", sp1);
                                                sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                                s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, 0, "\"", sp1);
                                                sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            }
                                        }
                                        else if (UsarAddWithValue)
                                        {
                                            s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, col.MaxLength, "\"", sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"", sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        if (UsarAddWithValue)
                                        {
                                            s = string.Format("{1}@{3}{1}, {0}", col.ColumnName, "\"", tipoOleDb(col.DataType.ToString()), sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{1}@{3}{1}, {2}, 0, {1}{0}{1}", col.ColumnName, "\"", tipoOleDb(col.DataType.ToString()), sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                    sp1 = sp[j - 1];
                    switch (colID.DataType.ToString())
                    {
                        case "System.String":
                            {
                                if (colID.MaxLength > 255)
                                {
                                    sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", colID.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                    if (UsarAddWithValue)
                                    {
                                        s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                        sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, 0, "\"", sp1);
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                        sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, 0, "\"", sp1);
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                }
                                else if (UsarAddWithValue)
                                {
                                    s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }
                                else
                                {
                                    s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }

                                break;
                            }

                        default:
                            {
                                if (UsarAddWithValue)
                                {
                                    s = string.Format("{1}@{3}{1}, {0}", colID.ColumnName, "\"", tipoOleDb(colID.DataType.ToString()), sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }
                                else
                                {
                                    sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                    s = string.Format("{1}@{3}{1}, {2}, 0, {1}{0}{1}", colID.ColumnName, "\"", tipoOleDb(colID.DataType.ToString()), sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }

                                break;
                            }
                    }
                    s = string.Format("{0}@{2}{0}, {1}, 0, {0}{0}", "\"", "OleDbType.Integer", sp[j]);
                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                }
                // 
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Try(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("da.Fill(dt)"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"ERROR: \" & ex.Message"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                // 
                sb.AppendFormat("        {0}{1}", ConvLang.If("dt.Rows.Count", "=", "0"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" crear uno nuevo"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("Crear()"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Else(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion(nombreClase + "2Row(Me, dt.Rows(0))"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.EndIf(), CrLf);
                // 
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Try(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("da.Update(dt)"), CrLf);
                sb.AppendFormat("            dt.AcceptChanges(){0}{1}", ConvLang.FinInstruccion(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"Actualizado correctamente\""), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"ERROR: \" & ex.Message"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.EndTry(), CrLf);
            }
            else
            {
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("sCommand", sb1.ToString()), CrLf);
                // cmd.CommandText = sCommand                        (26/Abr/19)
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandText", "sCommand"), CrLf);
                sb.AppendLine();
                // No usar With ya que C# no lo soporta... (ni lo tengo definido :-P )
                // Nota: solo para SQL Server y AddWithValue
                // cmd.Parameters.AddWithValue("@ID", ID)
                foreach (DataColumn col in mDataTable.Columns)
                {
                    switch (col.DataType.ToString())
                    {
                        case "System.String":
                            {
                                s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"");
                                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("cmd.Parameters.AddWithValue(" + s + ")"), CrLf);
                                break;
                            }

                        default:
                            {
                                s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"");
                                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("cmd.Parameters.AddWithValue(" + s + ")"), CrLf);
                                break;
                            }
                    }
                }
                sb.AppendLine();
                // cmd.Transaction = tran
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.Transaction", "tran"), CrLf);
                // cmd.ExecuteNonQuery()
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("cmd.ExecuteNonQuery()"), CrLf);
                sb.AppendLine();
                // ' Si llega aquí es que todo fue bien,
                // ' por tanto, llamamos al método Commit
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Si llega aquí es que todo fue bien,"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" por tanto, llamamos al método Commit."), CrLf);
                // tran.Commit()
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("tran.Commit()"), CrLf);
                sb.AppendLine();
                // msg = "Se ha actualizado el Cliente correctamente."
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("msg", "\"Se ha actualizado un " + nombreClase + " correctamente.\""), CrLf);
                sb.AppendLine();
                // Catch ex As Exception
                sb.AppendFormat("            {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                // msg = $"ERROR: {ex.Message}"
                sb.AppendFormat("              {0}{1}", ConvLang.Asigna("msg", "$\"ERROR: {ex.Message}\""), CrLf);
                // ' Si hay error, deshacemos lo que se haya hecho
                sb.AppendFormat("              {0}{1}", ConvLang.Comentario(" Si hay error, deshacemos lo que se haya hecho."), CrLf);
                // Try
                sb.AppendFormat("              {0}{1}", ConvLang.Try(), CrLf);
                // Añadir comprobación de nulo en el objeto tran     (17-abr-21)
                // If tran IsNot Nothing Then
                sb.AppendFormat("                  {0}{1}", ConvLang.If("tran", "IsNot", "Nothing"), CrLf);
                // tran.Rollback()
                sb.AppendFormat("                        {0}{1}", ConvLang.Instruccion("tran.Rollback()"), CrLf);
                // End If
                sb.AppendFormat("                  {0}{1}", ConvLang.EndIf(), CrLf);
                // Catch ex2 As Exception
                sb.AppendFormat("              {0}{1}", ConvLang.Catch("ex2", "Exception"), CrLf);
                // msg &= $" (ERROR RollBack: {ex.Message})"
                sb.AppendFormat("               {0}{1}", ConvLang.Asigna("msg", "$\"ERROR RollBack: {ex2.Message}\""), CrLf);
                // End Try
                sb.AppendFormat("              {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                sb.AppendFormat("            {0}{1}", ConvLang.Finally(), CrLf);
                // If Not (con is nothing) then
                sb.AppendFormat("              {0}{1}", ConvLang.If("", "Not", "(con Is Nothing)"), CrLf);
                // con.Close()
                sb.AppendFormat("                  {0}{1}", ConvLang.Instruccion("con.Close()"), CrLf);
                // End If
                sb.AppendFormat("              {0}{1}", ConvLang.EndIf(), CrLf);
                // End Try
                sb.AppendFormat("            {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                // End Using
                sb.AppendFormat("            {0}{1}", ConvLang.EndUsing(), CrLf);
                sb.AppendLine();
                // Return msg
                sb.AppendFormat("            {0}{1}", ConvLang.Return("msg"), CrLf);
            }
            // 
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // Crear: Crea un nuevo registro usando el contenido de la instancia
            // ------------------------------------------------------------------
            sb.AppendFormat(ConvLang.DocumentacionXML("    "," Crear un nuevo registro",
                " En caso de error, devolverá la cadena de error empezando por ERROR:."
            ));
            if (UsarOverrides)
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Overrides", "Crear", "String"), CrLf);
            else
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public", "Crear", "String"), CrLf);
            if (UsarDataAdapter)
            {
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "cnn", dbPrefix + "Connection"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "da", dbPrefix + "DataAdapter"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariableNewParam("Dim", "dt", "DataTable", string.Format("{0}{1}{0}", "\"", nombreClase)), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.AsignaNew("cnn", dbPrefix + "Connection", "CadenaConexion"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.AsignaNew("da", dbPrefix + "DataAdapter", "CadenaSelect" + ", cnn"), CrLf);
                sb.AppendFormat("        {0}{1}{2}", ConvLang.Comentario(), ConvLang.AsignaNew("da", dbPrefix + "DataAdapter", "CadenaSelect" + ", CadenaConexion"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Asigna("da.MissingSchemaAction", "MissingSchemaAction.AddWithKey"), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("-------------------------------------------"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Esta no es la más óptima, pero funcionará"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("-------------------------------------------"), CrLf);
                sb.AppendFormat("        {2}{0}{1}", ConvLang.DeclaraVariableNewParam("Dim", "cb", dbPrefix + "CommandBuilder", "da"), CrLf, ConvLang.Comentario(!usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna("da.InsertCommand", "cb.GetInsertCommand()"), CrLf, ConvLang.Comentario(!usarCommandBuilder));
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("--------------------------------------------------------------------"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" Esta está más optimizada pero debes comprobar que funciona bien..."), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Comentario("--------------------------------------------------------------------"), CrLf);
                sb.AppendFormat("        {2}{0}{1}", ConvLang.DeclaraVariable("Dim", "sCommand", "String"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Comentario(), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0} El comando INSERT{1}", ConvLang.Comentario(), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0} TODO: No incluir el campo de clave primaria incremental{1}", ConvLang.Comentario(), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {3}{0}       Yo compruebo que sea un campo llamado {2}, pero en tu caso puede ser otro{1}", ConvLang.Comentario(), CrLf, campoIDnombre, ConvLang.Comentario(usarCommandBuilder));
            }
            else
            {
                // Dim msg = ""
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "msg", "String"), CrLf);
                sb.AppendLine();
                // Using con As New SqlConnection(CadenaConexion)
                sb.AppendFormat("        {0}{1}", ConvLang.Using("con", "SqlConnection", "CadenaConexion"), CrLf);
                // Dim tran As SqlTransaction = Nothing
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "tran", "SqlTransaction", "Nothing"), CrLf);
                sb.AppendLine();
                // Try
                sb.AppendFormat("            {0}{1}", ConvLang.Try(), CrLf);
                // con.Open()
                sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Open()"), CrLf);
                // tran = con.BeginTransaction()
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("tran", "con.BeginTransaction()"), CrLf);
                sb.AppendLine();
                // Dim cmd As New SqlCommand()
                sb.AppendFormat("                {0}{1}", ConvLang.DeclaraVariableNew("Dim", "cmd", "SqlCommand"), CrLf);
                // cmd.CommandType = CommandType.StoredProcedure
                sb.AppendFormat("                {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandType", "CommandType.StoredProcedure")), CrLf);
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.CommandType", "CommandType.Text"), CrLf);
                // cmd.Connection = con
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.Connection", "con"), CrLf);
                // cmd.CommandText = "CrearCliente"
                sb.AppendFormat("                {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandText", $"\"Crear{nombreTabla}\"")), CrLf, "\"");
                sb.AppendLine();
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "sCommand", "String"), CrLf);
            }
            // 
            sb1 = new System.Text.StringBuilder();
            sb2 = new System.Text.StringBuilder();
            sb3 = new System.Text.StringBuilder();
            sb1.AppendFormat("{0}INSERT INTO {1} (", "\"", nombreTabla);
            // 
            for (int i = 0; i <= mDataTable.Columns.Count - 1; i++)
            {
                DataColumn col = mDataTable.Columns[i];
                s = col.ColumnName;
                if (campos[col.ColumnName].ToString() != s)
                    s = "[" + col.ColumnName + "]";
                // si no es AutoIncrement debe estar en los parámetros
                if (col.AutoIncrement == false)
                {
                    sb2.AppendFormat("{0}, ", s);
                    if (esSQL)
                        sb3.AppendFormat("@{0}, ", s);
                    else
                        sb3.Append("?, ");
                }
            }
            s = sb2.ToString().TrimEnd();
            if (s.EndsWith(","))
                sb1.AppendFormat("{0}", s.Substring(0, s.Length - 1));
            else
                sb1.AppendFormat("{0}", s);
            sb1.Append(") ");
            // 
            s = sb3.ToString().TrimEnd();
            if (s.EndsWith(","))
                sb1.AppendFormat(" VALUES({0}) ", s.Substring(0, s.Length - 1));
            else
                sb1.AppendFormat(" VALUES({0}) ", s);
            // 
            if (UsarDataAdapter == false)
                // Añadirle SELECT @@Identity al comando INSERT
                sb1.Append("SELECT @@Identity");
            sb1.AppendFormat("{0}", "\"");
            // 
            if (UsarDataAdapter)
            {
                sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna("sCommand", sb1.ToString()), CrLf, ConvLang.Comentario(usarCommandBuilder));
                sb.AppendFormat("        {2}{0}{1}", ConvLang.AsignaNew("da.InsertCommand", dbPrefix + "Command", "sCommand, cnn"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                // 
                if (esSQL)
                {
                    foreach (DataColumn col in mDataTable.Columns)
                    {
                        switch (col.DataType.ToString())
                        {
                            case "System.String":
                                {
                                    if (col.MaxLength > 255)
                                    {
                                        sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", col.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                        if (UsarAddWithValue)
                                        {
                                            s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, col.MaxLength, "\"");
                                            sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, 0, "\"");
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            s = string.Format("{2}@{0}{2}, SqlDbType.NText, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"");
                                            sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{2}@{0}{2}, SqlDbType.NText, {1}, {2}{0}{2}", col.ColumnName, 0, "\"");
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                    }
                                    else if (UsarAddWithValue)
                                    {
                                        s = string.Format("{2}@{0}{2}, {0}", col.ColumnName, col.MaxLength, "\"");
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        s = string.Format("{2}@{0}{2}, SqlDbType.NVarChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"");
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }

                                    break;
                                }

                            default:
                                {
                                    if (UsarAddWithValue)
                                    {
                                        s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"", tipoSQL(col.DataType.ToString()));
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{1}@{0}{1}, {2}, 0, {1}{0}{1}", col.ColumnName, "\"", tipoSQL(col.DataType.ToString()));
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }

                                    break;
                                }
                        }
                    }
                }
                else
                {
                    int j = mDataTable.Columns.Count;
                    int k;
                    string[] sp = new string[j + 1];
                    for (k = 0; k <= j; k++)
                        sp[k] = "p" + (k + 1).ToString();
                    k = 0;
                    string sp1;
                    DataColumn colID = null;
                    foreach (DataColumn col in mDataTable.Columns)
                    {
                        if (campos[col.ColumnName].ToString() == campoIDnombre)
                            colID = col;
                        else
                        {
                            sp1 = sp[k];
                            k += 1;
                            switch (col.DataType.ToString())
                            {
                                case "System.String":
                                    {
                                        if (col.MaxLength > 255)
                                        {
                                            sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", col.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                            if (UsarAddWithValue)
                                            {
                                                s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, col.MaxLength, "\"", sp1);
                                                sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                                s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, 0, "\"", sp1);
                                                sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            }
                                            else
                                            {
                                                s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"", sp1);
                                                sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.UpdateCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                                s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, 0, "\"", sp1);
                                                sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                            }
                                        }
                                        else if (UsarAddWithValue)
                                        {
                                            s = string.Format("{2}@{3}{2}, {0}", col.ColumnName, col.MaxLength, "\"", sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", col.ColumnName, col.MaxLength, "\"", sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        if (UsarAddWithValue)
                                        {
                                            s = string.Format("{1}@{3}{1}, {0}", col.ColumnName, "\"", tipoOleDb(col.DataType.ToString()), sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }
                                        else
                                        {
                                            sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                            s = string.Format("{1}@{3}{1}, {2}, 0, {1}{0}{1}", col.ColumnName, "\"", tipoOleDb(col.DataType.ToString()), sp1);
                                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        }

                                        break;
                                    }
                            }
                        }
                    }
                    sp1 = sp[j - 1];
                    switch (colID.DataType.ToString())
                    {
                        case "System.String":
                            {
                                if (colID.MaxLength > 255)
                                {
                                    sb.AppendFormat("        {3}{2} TODO: Este campo seguramente es MEMO y el valor debería ser cero en lugar de {0}{1}", colID.MaxLength, CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                    if (UsarAddWithValue)
                                    {
                                        s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                        sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, 0, "\"", sp1);
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                    else
                                    {
                                        s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                        sb.AppendFormat("        {3}{0}{1}{2}", ConvLang.Comentario(), ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                        s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, 0, "\"", sp1);
                                        sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                    }
                                }
                                else if (UsarAddWithValue)
                                {
                                    s = string.Format("{2}@{3}{2}, {0}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }
                                else
                                {
                                    s = string.Format("{2}@{3}{2}, OleDbType.VarWChar, {1}, {2}{0}{2}", colID.ColumnName, colID.MaxLength, "\"", sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }

                                break;
                            }

                        default:
                            {
                                if (UsarAddWithValue)
                                {
                                    s = string.Format("{1}@{3}{1}, {0}", colID.ColumnName, "\"", tipoOleDb(colID.DataType.ToString()), sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.AddWithValue(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }
                                else
                                {
                                    sb.AppendFormat("        {2}{1} TODO: Comprobar el tipo de datos a usar...{0}", CrLf, ConvLang.Comentario(), ConvLang.Comentario(usarCommandBuilder));
                                    s = string.Format("{1}@{3}{1}, {2}, 0, {1}{0}{1}", colID.ColumnName, "\"", tipoOleDb(colID.DataType.ToString()), sp1);
                                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                                }

                                break;
                            }
                    }
                    s = string.Format("{0}@{2}{0}, {1}, 0, {0}{0}", "\"", "OleDbType.Integer", sp[j]);
                    sb.AppendFormat("        {2}{0}{1}", ConvLang.Instruccion("da.InsertCommand.Parameters.Add(" + s + ")"), CrLf, ConvLang.Comentario(usarCommandBuilder));
                }
                // 
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Try(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("da.Fill(dt)"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"ERROR: \" & ex.Message"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Instruccion("nuevo" + nombreClase + "(dt, Me)"), CrLf);
                sb.AppendLine();
                sb.AppendFormat("        {0}{1}", ConvLang.Try(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("da.Update(dt)"), CrLf);
                sb.AppendFormat("            dt.AcceptChanges(){0}{1}", ConvLang.FinInstruccion(), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"Se ha creado un nuevo " + nombreClase + "\""), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Return("\"ERROR: \" & ex.Message"), CrLf);
                sb.AppendFormat("        {0}{1}", ConvLang.EndTry(), CrLf);
            }
            else
            {
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("sCommand", sb1.ToString()), CrLf);
                // cmd.CommandText = sCommand                        (26/Abr/19)
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandText", "sCommand"), CrLf);
                sb.AppendLine();
                // No usar With ya que C# no lo soporta... (ni lo tengo definido :-P )
                // Nota: solo para SQL Server y AddWithValue
                // cmd.Parameters.AddWithValue("@ID", ID)
                foreach (DataColumn col in mDataTable.Columns)
                {
                    if (col.AutoIncrement == false)
                    {
                        switch (col.DataType.ToString())
                        {
                            case "System.String":
                                {
                                    s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"");
                                    sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("cmd.Parameters.AddWithValue(" + s + ")"), CrLf);
                                    break;
                                }

                            default:
                                {
                                    s = string.Format("{1}@{0}{1}, {0}", col.ColumnName, "\"");
                                    sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("cmd.Parameters.AddWithValue(" + s + ")"), CrLf);
                                    break;
                                }
                        }
                    }
                }
                sb.AppendLine();
                // cmd.Transaction = tran
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.Transaction", "tran"), CrLf);
                sb.AppendLine();
                // Nuevo código comprobando que ExecuteScalar no devuelva nulo. 
                // código anterior
                // Dim id As Integer = CInt(cmd.ExecuteScalar())
                // sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "id", "Integer", "CInt(cmd.ExecuteScalar())"), vbCrLf)
                // Nuevo código:
                // Dim id As Integer
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "id", "Integer"), CrLf);
                // ' Comprobación extra al usar ExecuteScalar. (01/oct/22 08.14)
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Comprobación extra al usar ExecuteScalar. (01/oct/22 08.14)"), CrLf);
                // Dim obj = cmd.ExecuteScalar()
                sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "obj", "", "cmd.ExecuteScalar()"), CrLf);
                // If DBNull.Value.Equals(obj) OrElse obj Is Nothing Then
                sb.AppendFormat("            {0}{1}", ConvLang.If("DBNull.Value.Equals(obj) OrElse obj", "Is", "Nothing"), CrLf);
                // id = -1
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("id", "-1"), CrLf);
                // Return "ERROR al crear el Rango."
                sb.AppendFormat("                {0}{1}", ConvLang.Return($"\"ERROR al crear {nombreClase}.\""), CrLf);
                // Else
                sb.AppendFormat("            {0}{1}", ConvLang.Else(), CrLf);
                // id = CInt(obj)
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("id", "CInt(obj)"), CrLf);
                object obj;
                obj = "1";
                int iObj;
                iObj = System.Convert.ToInt32(obj);
                // End If
                sb.AppendFormat("            {0}{1}", ConvLang.EndIf(), CrLf);
                // 
                sb.AppendLine();
                // Me.ID = id
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("Me.ID", "id"), CrLf);
                sb.AppendLine();
                // ' Si llega aquí es que todo fue bien,
                // ' por tanto, llamamos al método Commit
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" Si llega aquí es que todo fue bien,"), CrLf);
                sb.AppendFormat("            {0}{1}", ConvLang.Comentario(" por tanto, llamamos al método Commit."), CrLf);
                // tran.Commit()
                sb.AppendFormat("            {0}{1}", ConvLang.Instruccion("tran.Commit()"), CrLf);
                sb.AppendLine();
                // msg = "Se ha actualizado el Cliente correctamente."
                sb.AppendFormat("            {0}{1}", ConvLang.Asigna("msg", "\"Se ha creado un " + nombreClase + " correctamente.\""), CrLf);
                sb.AppendLine();
                // Catch ex As Exception
                sb.AppendFormat("            {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
                // msg = $"ERROR: {ex.Message}"
                sb.AppendFormat("                {0}{1}", ConvLang.Asigna("msg", "$\"ERROR: {ex.Message}\""), CrLf);
                // Try
                sb.AppendFormat("                {0}{1}", ConvLang.Try(), CrLf);
                // ' Si hay error, deshacemos lo que se haya hecho
                sb.AppendFormat("                    {0}{1}", ConvLang.Comentario(" Si hay error, deshacemos lo que se haya hecho."), CrLf);
                // Añadir comprobación de nulo en el objeto tran     (17-abr-21)
                // If tran IsNot Nothing Then
                sb.AppendFormat("                    {0}{1}", ConvLang.If("tran", "IsNot", "Nothing"), CrLf);
                // tran.Rollback()
                sb.AppendFormat("                        {0}{1}", ConvLang.Instruccion("tran.Rollback()"), CrLf);
                // End If
                sb.AppendFormat("                    {0}{1}", ConvLang.EndIf(), CrLf);
                // Catch ex2 As Exception
                sb.AppendFormat("              {0}{1}", ConvLang.Catch("ex2", "Exception"), CrLf);
                // msg &= $" (ERROR RollBack: {ex.Message})"
                sb.AppendFormat("                  {0}{1}", ConvLang.Asigna("msg", "$\"ERROR RollBack: {ex2.Message})\""), CrLf);
                // End Try
                sb.AppendFormat("              {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                sb.AppendFormat("            {0}{1}", ConvLang.Finally(), CrLf);
                // If Not (con is nothing) then
                sb.AppendFormat("                {0}{1}", ConvLang.If("", "Not", "(con Is Nothing)"), CrLf);
                // con.Close()
                sb.AppendFormat("                    {0}{1}", ConvLang.Instruccion("con.Close()"), CrLf);
                // End If
                sb.AppendFormat("                {0}{1}", ConvLang.EndIf(), CrLf);
                // End Try
                sb.AppendFormat("            {0}{1}", ConvLang.EndTry(), CrLf);
                sb.AppendLine();
                // End Using
                sb.AppendFormat("        {0}{1}", ConvLang.EndUsing(), CrLf);
                sb.AppendLine();
                // Return msg
                sb.AppendFormat("        {0}{1}", ConvLang.Return("msg"), CrLf);
            }
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // ------------------------------------------------------------------
            // Borrar: Borra el registro con el mismo ID que tenga la clase
            // En caso de que quieras usar otro criterio para comprobar cual es el registro actual
            // cambia la comparación
            // ------------------------------------------------------------------
            sb.AppendFormat(ConvLang.DocumentacionXML("    "," Borrar el registro con el mismo ID que tenga la clase.",
                " NOTA: En caso de que quieras usar otro criterio",
                " para comprobar cuál es el registro actual, cambia la comparación."
            ));
            if (UsarOverrides)
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Overrides", "Borrar", "String"), CrLf);
            else
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public", "Borrar", "String"), CrLf);
            sb.AppendFormat("        {0} TODO: Poner aquí la selección a realizar para acceder a este registro{1}", ConvLang.Comentario(), CrLf);
            sb.AppendFormat("        {0}       yo uso el {2} que es el identificador único de cada registro{1}", ConvLang.Comentario(), CrLf, campoIDnombre);
            if (campoIDtipo.IndexOf("String") > -1)
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "where", "String", string.Format("{0}{2} = '{0} & Me.{2} & {0}'{0}", "\"", nombreTabla, campoIDnombre)), CrLf);
            else
                sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "where", "String", string.Format("{0}{2} = {0} & Me.{2}.ToString()", "\"", nombreTabla, campoIDnombre)), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("Borrar(where)"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            sb.AppendFormat(ConvLang.DocumentacionXML("    "," Borrar el registro o los registros indicados en la cadena WHERE.",
                " La cadena indicada se usará después de la cláusula WHERE de TSQL.",
                " Ejemplo, si la cadena es: Nombre = 'Pepe' AND Telefono = '666777999'",
                " Ejecutará: WHERE Nombre = 'Pepe' AND Telefono = '666777999'",
                " Y borrará todos los registros de esta tabla que coincidan."
            ));
            if (UsarOverrides)
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Overrides", "Borrar", "String", "where", "String"), CrLf);
            else
                sb.AppendFormat("    {0}{1}", ConvLang.Function("Public", "Borrar", "String", "where", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "msg", "String", "\"" + "\""), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "sCon", "String", "CadenaConexion"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariable("Dim", "sel", "String", string.Format("{1}DELETE FROM {0} WHERE {1} & where", nombreClase, "\"")), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Using("con", "SqlConnection", "sCon"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "tran", "SqlTransaction", "Nothing"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("            {0}{1}", ConvLang.Try(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.DeclaraVariableNew("Dim", "cmd", "SqlCommand"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandType", "CommandType.StoredProcedure")), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.CommandType", "CommandType.Text"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.Connection", "con"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(ConvLang.Asigna("cmd.CommandText", $"\"Borrar{nombreTabla}\"")), CrLf, "\"");
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.CommandText", "sel"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Open()"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("tran", "con.BeginTransaction()"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("cmd.Transaction", "tran"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("cmd.ExecuteNonQuery()"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(" Si llega aquí es que todo fue bien,"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Comentario(" por tanto, llamamos al método Commit."), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("tran.Commit()"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("msg", "\"Eliminado correctamente los registros con : \" & where & \".\""), CrLf);
            sb.AppendLine();
            sb.AppendFormat("            {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("msg", "\"ERROR al eliminar los registros con : \" & where & \".\" & ex.Message"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("              {0}{1}", ConvLang.Try(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("tran.Rollback()"), CrLf);
            sb.AppendFormat("              {0}{1}", ConvLang.Catch("ex2", "Exception"), CrLf);
            sb.AppendFormat("               {0}{1}", ConvLang.Asigna("msg", "$\"ERROR RollBack: {ex2.Message})\""), CrLf);
            sb.AppendFormat("              {0}{1}", ConvLang.EndTry(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("            {0}{1}", ConvLang.Finally(), CrLf);
            // If Not (con is nothing) then
            sb.AppendFormat("              {0}{1}", ConvLang.If("", "Not", "(con Is Nothing)"), CrLf);
            // con.Close()
            sb.AppendFormat("                  {0}{1}", ConvLang.Instruccion("con.Close()"), CrLf);
            // End If
            sb.AppendFormat("              {0}{1}", ConvLang.EndIf(), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.EndTry(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.EndUsing(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("msg"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            // TablaCol y Reader2Tipo                                (24/May/19)
            // 
            sb.AppendFormat(ConvLang.DocumentacionXML("    ",$" Devuelve una colección de {nombreClase} según la cadena select."
            ));
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "TablaCol", $"List(Of {nombreClase})", "sel", "String"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.DeclaraVariableNew("Dim", "col", $"List(Of {nombreClase})"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Using("con", "SqlConnection", "CadenaConexion"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariableNew("Dim", "cmd", "SqlCommand"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.DeclaraVariable("Dim", "reader", "SqlDataReader"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandType", "CommandType.Text"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.Connection", "con"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Asigna("cmd.CommandText", "sel"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Try(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Open()"), CrLf);
            sb.AppendLine();
            sb.AppendFormat("                {0}{1}", ConvLang.Asigna("reader", "cmd.ExecuteReader()"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.While("reader.Read()"), CrLf);
            sb.AppendFormat("                    {0}{1}", ConvLang.DeclaraVariable("Dim", "r", nombreClase, "Reader2Tipo(reader)"), CrLf);
            sb.AppendFormat("                    {0}{1}", ConvLang.Instruccion("col.Add(r)"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.EndWhile(), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("reader.Close()"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Instruccion("con.Close()"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.Catch("ex", "Exception"), CrLf);
            sb.AppendFormat("                {0}{1}", ConvLang.Return("col"), CrLf);
            sb.AppendFormat("            {0}{1}", ConvLang.EndTry(), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.EndUsing(), CrLf);
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("col"), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            sb.AppendFormat(ConvLang.DocumentacionXML("    "," Asigna los datos desde un SqlDataReader."
            ));
            sb.AppendFormat("    {0}{1}", ConvLang.Function("Public Shared", "Reader2Tipo", nombreClase, "r", "SqlDataReader"), CrLf);
            sb.AppendFormat("        {0}{1}", ConvLang.VariableNew("Dim", "o_" + nombreClase, nombreClase), CrLf);
            sb.AppendLine();
            foreach (DataColumn col in mDataTable.Columns)
            {
                switch (col.DataType.ToString())
                {
                    case "System.String":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"].ToString()", col.ColumnName)), CrLf);
                            break;
                        }

                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Single":
                    case "System.Decimal":
                    case "System.Double":
                    case "System.Byte":
                    case "System.SByte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.Boolean":
                    case "System.DateTime":
                    case "System.Char":
                    case "System.TimeSpan":
                        {
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("ConversorTipos.{1}Data(r[\"{0}\"].ToString())", col.ColumnName, col.DataType.ToString().Replace("System.", ""))), CrLf);
                            break;
                        }

                    case "System.Byte[]":
                        {
                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"]", col.ColumnName)), CrLf, ConvLang.Comentario());
                            break;
                        }

                    default:
                        {
                            // El resto de tipos
                            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(" TODO: Comprobar la conversión a realizar"), CrLf);
                            sb.AppendFormat("        {0}{1}", ConvLang.Comentario(string.Format("       con el tipo {0}", col.DataType.ToString())), CrLf);
                            sb.AppendFormat("        {2}{0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("r[\"{0}\"]", col.ColumnName)), CrLf, ConvLang.Comentario());
                            sb.AppendFormat("        {0}{1}", ConvLang.Asigna(string.Format("o_{0}.{1}", nombreClase, campos[col.ColumnName].ToString()), string.Format("{1}.Parse(r[\"{0}\"].ToString())", col.ColumnName, col.DataType.ToString())), CrLf);
                            break;
                        }
                }
            }
            sb.AppendLine();
            sb.AppendFormat("        {0}{1}", ConvLang.Return("o_" + nombreClase), CrLf);
            sb.AppendFormat("    {0}{1}", ConvLang.EndFunction(), CrLf);
            sb.AppendLine();
            // 
            sb.AppendFormat("{0}{1}", ConvLang.EndClass(), CrLf);
            // 
            return sb.ToString();
        }
        // 
        private static string tipoSQL(string elTipo)
        {
            string[] aCTS = new[] { "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.Decimal", "System.Single", "System.Double", "System.Byte", "System.DateTime", "System.Guid", "System.Object" };
            string[] aSQL = new[] { "Bit", "SmallInt", "Int", "BigInt", "Decimal", "Real", "Float", "TinyInt", "DateTime", "UniqueIdentifier", "Variant" };
            int i = Array.IndexOf(aCTS, elTipo);
            if (i > -1)
                return "SqlDbType." + aSQL[i];
            return "SqlDbType.Int";
        }
        // 
        private static string tipoOleDb(string elTipo)
        {
            string[] aCTS = new[] { "System.Byte[]", "System.Boolean", "System.Int16", "System.Int32", "System.Int64", "System.Decimal", "System.Single", "System.Double", "System.Byte", "System.DateTime", "System.Guid", "System.Object", "System.String" };
            string[] aOle = new[] { "LongVarBinary", "Boolean", "SmallInt", "Integer", "BigInt", "Decimal", "Single", "Double", "UnsignedTinyInt", "Date", "Guid", "Variant", "VarWChar" };
            int i = Array.IndexOf(aCTS, elTipo);
            if (i > -1)
                return "OleDbType." + aOle[i];
            return "OleDbType.Integer";
        }
    }
}
