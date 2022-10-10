// ------------------------------------------------------------------------------
// Clase para crear trozos de código en VB y C#                      (07/Jul/04)
// los métodos también generan el código de VB
// Inicialmente se usará código de VB para crear el de C#
// al menos en lo que a los tipos de datos, tipos de métodos, etc.
// 
// Nota: Ver las revisiones en Revisiones.md
//
// Convertido a C# con:                                         (06/oct/22 20.16)
// https://converter.telerik.com/
// Con algo de ayuda
// 
// ©Guillermo 'guille' Som, 2004, 2007, 2018-2022
// ------------------------------------------------------------------------------
using System;

using elGuille.Util.Developer.Data;

//using Microsoft.VisualBasic;

namespace elGuille.Util.Developer
{
    // 
    public enum eLenguaje
    {
        eVBNET,
        eCS,
        VisualBasic = eVBNET,
        CSharp = eCS
    }

    public class ConvLang
    {
        public static eLenguaje Lang;
        // las comprobaciones se harán sin tener en cuenta mayúsculas/minúsculas
        private static string[] tiposVB = new[] { "", "short", "integer", "long", "string", "object", "double", "single", "date", "decimal", "boolean", "byte", "char" };
        private static string[] tiposCS = new[] { "var", "short", "int", "long", "string", "object", "double", "float", "DateTime", "decimal", "bool", "byte", "char" };
        private static string[] modificadoresVB = new[] { "public", "private", "friend", "protected", "shared", "overrides", "overridable", "shadows" };
        private static string[] modificadoresCS = new[] { "public", "private", "internal", "protected", "static", "override", "virtual", "new" };
        private static string[] compVB = new[] { "=", "<>" };
        private static string[] compCS = new[] { "==", "!=" };
        // Private Shared instrCS() As String = {"is !", " is", " null", "!", ".Rows[0]", "new ", ", this", " this.", " this(", " this,", "(this,", "true", "false", " + ", "' '"}
        // Mejorar las comprobaciones de IsNot e Is y añado And, AndAlso, Or y OrElse. (01/oct/22 09.10)
        private static string[] instrVB = new[] { "CBool", "CByte", "CChar", "CDate", "CDbl", "CDec", "CInt", "CLng", "CObj", "CSByte", "CShort", "CSng", "CStr", "CUInt", "CULng", "CUShort", "OrElse", "AndAlso", "Or", "And", "IsNot", "Is", "Nothing", "Not", ".Rows(0)", "New ", ", Me", " Me.", " Me(", " Me,", "(Me,", "True", "False", " & ", "\" \""};
        private static string[] instrCS = new[] { "Convert.ToBoolean", "Convert.ToByte", "Convert.ToChar", "Convert.ToDateTime", "Convert.ToDouble", "Convert.ToDecimal", "Convert.ToInt32", "Convert.ToInt64", "", "Convert.ToSByte", "Convert.ToInt16", "Convert.ToSingle", "Convert.ToString", "Convert.ToUInt32", "Convert.ToUint64", "Convert.ToUint16", " || ", " && ", " | ", " & ", " != ", " == ", " null ", " ! ", ".Rows[0]", "new ", ", this", " this.", " this(", " this,", "(this,", "true", "false", " + ", "' '" };

        /// <summary>
        ///         ''' El parámetro puede incluir más de uno (separado por espacio)
        ///         ''' por ejemplo: Public Shared
        ///         ''' </summary>
        private static string modificador(string modif)
        {
            // el parámetro puede incluir más de uno (separado por espacio)
            // por ejemplo: Public Shared
            if (Lang == eLenguaje.eVBNET)
                return modif;
            // 
            string s1 = "";
            int i;
            string[] a = modif.Split(' ');
            // 
            foreach (string s in a)
            {
                if (s != "")
                {
                    i = Array.IndexOf(modificadoresVB, s.ToLower());
                    if (i > -1)
                        s1 += " " + modificadoresCS[i];
                    else
                        s1 += " " + s;
                }
            }
            return s1.TrimStart();
        }

        // ----------------------------------------------------------------------
        // Nuevos métodos agregados a la versión 2.0                 (30/Nov/18)
        // ----------------------------------------------------------------------

        /// <summary>
        ///         ''' Poner comentarios XML
        ///         ''' Se pondrán en una sola línea.
        ///         ''' </summary>
        public static string DocumentacionXML(string coment)
        {
            if (Lang == eLenguaje.eCS)
                return "///<summary>" + coment + "</summary>";
            else
                return "'''<summary>" + coment + "</summary>";
        }

        /// <summary>
        /// Crea documentación XML con varias líneas
        /// </summary>
        /// <remarks>Cambio el segundo parámetro por params (antes solo string[] coments)</remarks>
        public static string DocumentacionXML(string indentacion, params string[] coments)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string iniDocXML;

            if (Lang == eLenguaje.eCS)
                iniDocXML = "/// ";
            else
                iniDocXML = "''' ";

            sb.AppendFormat("{0}{1}<summary>{2}", indentacion, iniDocXML, CrearClase.CrLf);
            for (var i = 0; i <= coments.Length - 1; i++)
                sb.AppendFormat("{0}{1}{2}{3}", indentacion, iniDocXML, coments[i], CrearClase.CrLf);
            sb.AppendFormat("{0}{1}</summary>{2}", indentacion, iniDocXML, CrearClase.CrLf);

            return sb.ToString();
        }

        /// <summary>
        ///         ''' Poner un comentario
        ///         ''' </summary>
        public static string Comentario()
        {
            if (Lang == eLenguaje.eCS)
                return "//";
            else
                return "'";
        }
        // esta sobrecarga la uso para indicar si se pone o no el comentario
        // (para cuando las opciones son opcionales)
        public static string Comentario(bool poner)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (poner)
                    return "//";
                else
                    return "";
            }
            else if (poner)
                return "'";
            else
                return "";
        }
        public static string Comentario(string coment)
        {
            if (Lang == eLenguaje.eCS)
                return "//" + coment;
            else
                return "'" + coment;
        }
        public static string Imports(string espacio)
        {
            if (Lang == eLenguaje.eCS)
                return "using " + espacio + ";";
            else
                return "Imports " + espacio;
        }
        public static string Class(string modif, string nombre)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} class {1}{{", modificador(modif), nombre);
            else
                return string.Format("{0} Class {1}", modif, nombre);
        }
        public static string EndClass()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Class";
        }
        // 
        public static string Constructor(string nombreClase)
        {
            return Constructor("", nombreClase);
        }
        public static string Constructor(string modif, string nombreClase)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "")
                    modif = "Friend";
                return string.Format("{0} {1}(){{", modificador(modif), nombreClase);
            }
            else if (modif == "")
                return "Friend Sub New()";
            else
                return string.Format("{0} Sub New()", modif);
        }
        public static string Constructor(string modif, string nombreClase, string var, string elTipo)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "")
                    modif = "Friend";
                return string.Format("{0} {1}({2}){{", modificador(modif), nombreClase, Variable(var, elTipo));
            }
            else if (modif == "")
                return "Friend Sub New()";
            else
                return string.Format("{0} Sub New({1})", modif, Variable(var, elTipo));
        }
        // 
        /// <summary>
        ///         ''' While con argumento
        ///         ''' </summary>
        ///         ''' <remarks>24/May/2019</remarks>
        public static string While(string var)
        {
            if (Lang == eLenguaje.eCS)
                return "while (" + var + ")";
            else
                return "While " + var;
        }

        /// <summary>
        ///         ''' While sin argumentos
        ///         ''' </summary>
        ///         ''' <remarks>24/May/2019</remarks>
        public static string While()
        {
            if (Lang == eLenguaje.eCS)
                return "while{";
            else
                return "While";
        }

        /// <summary>
        ///         ''' End While
        ///         ''' </summary>
        ///         ''' <remarks>24/May/2019</remarks>
        public static string EndWhile()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End While";
        }
        // 
        public static string Try()
        {
            if (Lang == eLenguaje.eCS)
                return "try{";
            else
                return "Try";
        }
        public static string EndTry()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Try";
        }
        public static string Catch()
        {
            if (Lang == eLenguaje.eCS)
                return "}catch{";
            else
                return "Catch";
        }
        public static string Catch(string var, string elTipo)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("}}catch({0}){{", Variable(var, elTipo));
            else
                return "Catch " + Variable(var, elTipo);
        }
        public static string Finally()
        {
            if (Lang == eLenguaje.eCS)
                return "finally{";
            else
                return "Finally";
        }
        // 
        public static string Get()
        {
            if (Lang == eLenguaje.eCS)
                return "get{";
            else
                return "Get";
        }
        public static string EndGet()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Get";
        }
        public static string Set(string elTipo)
        {
            if (Lang == eLenguaje.eCS)
                return "set{";
            else
                return "Set(value As " + elTipo + ")";
        }
        public static string EndSet()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Set";
        }
        // 
        public static string Property(string modif, string elTipo, string nombre)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} {2}{{", modificador(modif), Tipo(elTipo), nombre);
            else
                // Le quito los paréntesis, ya que en las propiedades de VB no son necesarios. v3.0.0.1 (05/oct/22 10.32)
                return string.Format("{0} Property {1} As {2}", modif, nombre, elTipo);
        }
        public static string Property(string modif, string elTipo, string index, string tipoIndex)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} this[{2} {3}]{{", modificador(modif), Tipo(elTipo), Tipo(tipoIndex), index);
            else
                return string.Format("{0} Default Property Item({1} As {2}) As {3}", modif, index, tipoIndex, elTipo);
        }
        public static string PropertyRead(string modif, string elTipo, string nombre)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} {2}{{", modificador(modif), Tipo(elTipo), nombre);
            else
                // Return String.Format("{0} ReadOnly Property {1}() As {2}", modif, nombre, elTipo)
                // Le quito los paréntesis, ya que en las propiedades de VB no son necesarios. v3.0.0.1 (05/oct/22 10.35)
                return string.Format("{0} ReadOnly Property {1} As {2}", modif, nombre, elTipo);
        }
        public static string PropertyRead(string modif, string elTipo, string index, string tipoIndex)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} this[{2} {3}]{{", modificador(modif), Tipo(elTipo), Tipo(tipoIndex), index);
            else
                return string.Format("{0} Default ReadOnly Property Item({1} As {2}) As {3}", modif, index, tipoIndex, elTipo);
        }
        public static string PropertyWrite(string modif, string elTipo, string nombre)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} {2}{{", modificador(modif), Tipo(elTipo), nombre);
            else
                // Return String.Format("{0} WriteOnly Property {1}() As {2}", modif, nombre, elTipo)
                // Le quito los paréntesis, ya que en las propiedades de VB no son necesarios. v3.0.0.1 (05/oct/22 10.35)
                return string.Format("{0} WriteOnly Property {1} As {2}", modif, nombre, elTipo);
        }
        public static string PropertyWrite(string modif, string elTipo, string index, string tipoIndex)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} this[{2} {3}]{{", modificador(modif), Tipo(elTipo), Tipo(tipoIndex), index);
            else
                return string.Format("{0} Default WriteOnly Property Item({1} As {2}) As {3}", modif, index, tipoIndex, elTipo);
        }
        public static string EndProperty()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Property";
        }
        public static string Sub(string modif, string nombre)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} void {1}(){{", modificador(modif), nombre);
            else
                return string.Format("{0} Sub {1}()", modif, nombre);
        }
        public static string Sub(string modif, string nombre, string vNombre, string vTipo, params string[] vars)
        {
            // los parámetros opcionales (paramarray) se usará para indicar el nombre de la variable y el tipo separados por coma
            // en caso de usar ByRef (ref en C#) indicarlo en el nombre de la variable: ... uno, String, ByRef dos, Integer
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // 
            sb.Append(Variable(vNombre, vTipo));
            for (int i = 0; i <= vars.Length - 2; i += 2)
                sb.AppendFormat(", {0}", Variable(vars[i], vars[i + 1]));
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} void {1}({2}){{", modificador(modif), nombre, sb.ToString());
            else
                return string.Format("{0} Sub {1}({2})", modif, nombre, sb.ToString());
        }
        public static string EndSub()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Sub";
        }
        public static string Function(string modif, string nombre, string elTipo)
        {
            // 
            if (elTipo == "" || elTipo.ToLower() == "void" || elTipo.ToLower() == "sub")
                return Sub(modif, nombre);
            // 
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} {2}(){{", modificador(modif), Tipo(elTipo), nombre);
            else
                return string.Format("{0} Function {1}() As {2}", modif, nombre, elTipo);
        }
        public static string Function(string modif, string nombre, string elTipo, string vNombre, string vTipo, params string[] vars)
        {
            // 
            if (elTipo == "" || elTipo.ToLower() == "void" || elTipo.ToLower() == "sub")
                return Sub(modif, nombre, vNombre, vTipo, vars);
            // 
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            // 
            sb.Append(Variable(vNombre, vTipo));
            for (int i = 0; i <= vars.Length - 2; i += 2)
                sb.AppendFormat(", {0}", Variable(vars[i], vars[i + 1]));
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1} {2}({3}){{", modificador(modif), Tipo(elTipo), nombre, sb.ToString());
            else
                return string.Format("{0} Function {1}({2}) As {3}", modif, nombre, sb.ToString(), elTipo);
        }
        public static string EndFunction()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End Function";
        }
        // 
        public static string If(string var, string comp, string valor)
        {
            if (Lang == eLenguaje.eCS)
            {
                int i = Array.IndexOf(compVB, comp);
                if (i > -1)
                    comp = compCS[i];
                return string.Format("if({0} {1} {2}){{", comprobarParam(var), comprobarParam(comp), comprobarParam(valor));
            }
            else
                return string.Format("If {0} {1} {2} Then", var, comp, valor);
        }
        public static string ElseIf(string var, string comp, string valor)
        {
            if (Lang == eLenguaje.eCS)
            {
                int i = Array.IndexOf(compVB, comp);
                if (i > -1)
                    comp = compCS[i];
                return string.Format("}}else if({0} {1} {2}){{", comprobarParam(var), comprobarParam(comp), comprobarParam(valor));
            }
            else
                return string.Format("ElseIf {0} {1} {2} Then", var, comp, valor);
        }
        public static string Else()
        {
            if (Lang == eLenguaje.eCS)
                return "}else{";
            else
                return "Else";
        }
        public static string EndIf()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End If";
        }
        // 
        public static string End(string param)
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "End " + param;
        }
        // 
        public static string ForEach(string var, string elTipo, string donde)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("foreach({0} {1} in {2}){{", Tipo(elTipo), var, donde);
            else
                return string.Format("For Each {0} As {1} In {2}", var, elTipo, donde);
        }
        public static string For(string var, string ini, string fin)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("for({0} = {1}; {0} <= {2}; {0}++){{", var, ini, fin);
            else
                return string.Format("For {0} = {1} To {2}", var, ini, fin);
        }
        public static string For(string var, string ini, string fin, string incr)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("for({0} = {1}; {0} <= {2}; {0} + {3}){{", var, ini, fin, incr);
            else
                return string.Format("For {0} = {1} To {2} Step {3}", var, ini, fin, incr);
        }
        public static string Next()
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "Next";
        }
        public static string Next(string param)
        {
            if (Lang == eLenguaje.eCS)
                return "}";
            else
                return "Next " + param;
        }
        // 
        public static string Return(string valor)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("return {0};", comprobarParam(valor));
            else
                return "Return " + valor;
        }
        public static string Exit(string salirDe)
        {
            if (Lang == eLenguaje.eCS)
            {
                if ("function sub property operator".IndexOf(salirDe.ToLower()) > -1)
                    return "return;";
                else
                    return "break;";
            }
            else
                return "Exit " + salirDe;
        }

        /// <summary>
        ///         ''' La declaración de la instrucción Using usando una declaración New.
        ///         ''' En VB: Using con As New SqlConnection(sCon)
        ///         ''' En C#: using (SqlConnection con = new SqlConnection(sCon))
        ///         ''' VB: Dim con As New SqlConnection(sCon)
        ///         '''     Using con
        ///         ''' C#: SqlConnection con = new SqlConnection(sCon)
        ///         '''     using (con)
        ///         ''' </summary>
        ///         ''' <remarks>20/Mar/2019</remarks>
        public static string Using(string nombre, string elTipo, string param)
        {
            if (Lang == eLenguaje.CSharp)
                return string.Format("using ({0} {1} = new {0}({2})){{", Tipo(elTipo), nombre, comprobarParam(param));
            else
                return string.Format("Using {0} As New {1}({2})", nombre, elTipo, param);
        }

        /// <summary>
        ///         ''' La declaración de la instrucción Using usando una variable.
        ///         ''' VB: Using con
        ///         ''' C#: using (con)
        ///         ''' </summary>
        ///         ''' <remarks>20/Mar/2019</remarks>
        public static string Using(string nombre)
        {
            if (Lang == eLenguaje.CSharp)
                return string.Format("using ({0})", nombre);
            else
                return string.Format("Using {0}", nombre);
        }

        /// <summary>
        ///         ''' El cierre de la instrucción End Using.
        ///         ''' </summary>
        ///         ''' <remarks>20/Mar/2019</remarks>
        public static string EndUsing()
        {
            if (Lang == eLenguaje.CSharp)
                return "}";
            else
                return "End Using";
        }

        public static string Asigna(string var, string valor)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} = {1};", comprobarParam(var), comprobarParam(valor));
            else if (valor.StartsWith("\"") == false)
                return string.Format("{0} = {1}", var, valor).Replace("[", "(").Replace("]", ")");
            else
                return string.Format("{0} = {1}", var, valor);
        }
        public static string AsignaNew(string var, string valor)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} = new {1};", comprobarParam(var), comprobarParam(valor));
            else
                return string.Format("{0} = New {1}", var, valor);
        }
        public static string AsignaNew(string var, string valor, string param)
        {
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} = new {1}({2});", comprobarParam(var), comprobarParam(valor), comprobarParam(param));
            else
                return string.Format("{0} = New {1}({2})", var, valor, param);
        }
        // 
        public static string Instruccion(string cod)
        {
            if (Lang == eLenguaje.eCS)
                return comprobarParam(cod) + ";";
            else
                return cod;
        }
        public static string FinInstruccion()
        {
            if (Lang == eLenguaje.eCS)
                return ";";
            else
                return "";
        }
        // 
        public static string Tipo(string elTipo)
        {
            // Si no se indica el tipo, devolver una cadena vacía. (01/oct/22 08.38)
            // If String.IsNullOrWhiteSpace(elTipo) Then Return ""

            if (Lang == eLenguaje.eCS)
            {
                // Mejor comprobar que es una cadena vaía, por si IndexOf no comprueba el valor "". (01/oct/22 09.41)
                // If String.IsNullOrWhiteSpace(elTipo) Then
                // Return "var "
                // End If
                // Probar si funciona con "". Sí, funciona.
                int i = Array.IndexOf(tiposVB, elTipo.ToLower());
                if (i > -1)
                    elTipo = tiposCS[i];
            }
            return elTipo;
        }
        // 
        public static string Variable(string nombre, string elTipo)
        {
            // El tipo de datos lo daremos en formato VB
            if (Lang == eLenguaje.eCS)
                return string.Format("{0} {1}", Tipo(elTipo), nombre);
            else
                return string.Format("{0} As {1}", nombre, elTipo);
        }
        public static string DeclaraVariable(string modif, string nombre, string elTipo)
        {
            return Variable(modif, nombre, elTipo);
        }
        public static string Variable(string modif, string nombre, string elTipo)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "" || modif.ToLower() == "dim")
                    return string.Format("{0} {1};", Tipo(elTipo), nombre);
                else
                    return string.Format("{0} {1} {2};", modificador(modif), Tipo(elTipo), nombre);
            }
            else if (modif == "")
                return string.Format("{0} As {1}", nombre, elTipo);
            else
                return string.Format("{0} {1} As {2}", modif, nombre, elTipo);
        }
        public static string DeclaraVariable(string modif, string nombre, string elTipo, string valor)
        {
            return Variable(modif, nombre, elTipo, valor);
        }
        public static string Variable(string modif, string nombre, string elTipo, string valor)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "" || modif.ToLower() == "dim")
                    return string.Format("{0} {1} = {2};", Tipo(elTipo), nombre, comprobarParam(valor));
                else
                    return string.Format("{0} {1} {2} = {3};", modificador(modif), Tipo(elTipo), nombre, comprobarParam(valor));
            }
            else
               // Si no se indica el tipo, no usar As. (01/oct/22 09.50)
               if (elTipo == "")
            {
                if (modif == "")
                    return string.Format("{0} {1} = {2}", nombre, elTipo, valor);
                else
                    return string.Format("{0} {1} {2} = {3}", modif, nombre, elTipo, valor);
            }
            else if (modif == "")
                return string.Format("{0} As {1} = {2}", nombre, elTipo, valor);
            else
                return string.Format("{0} {1} As {2} = {3}", modif, nombre, elTipo, valor);
        }
        public static string DeclaraVariableNew(string modif, string nombre, string elTipo)
        {
            return VariableNew(modif, nombre, elTipo);
        }
        public static string VariableNew(string modif, string nombre, string elTipo)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "" || modif.ToLower() == "dim")
                {
                    if (elTipo.IndexOf("(") > -1)
                        return string.Format("{0} {1} = new {0};", Tipo(elTipo), nombre);
                    else
                        return string.Format("{0} {1} = new {0}();", Tipo(elTipo), nombre);
                }
                else if (elTipo.IndexOf("(") > -1)
                    return string.Format("{0} {1} {2} = new {1};", modificador(modif), Tipo(elTipo), nombre);
                else
                    return string.Format("{0} {1} {2} = new {1}();", modificador(modif), Tipo(elTipo), nombre);
            }
            else if (modif == "")
                return string.Format("{0} As New {1}", nombre, elTipo);
            else
                return string.Format("{0} {1} As New {2}", modif, nombre, elTipo);
        }
        public static string DeclaraVariableNewParam(string modif, string nombre, string elTipo)
        {
            return VariableNewParam(modif, nombre, elTipo);
        }
        public static string VariableNewParam(string modif, string nombre, string elTipo)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "" || modif.ToLower() == "dim")
                    return string.Format("{0} {1} = new {0};", Tipo(elTipo), nombre);
                else
                    return string.Format("{0} {1} {2} = new {1};", modificador(modif), Tipo(elTipo), nombre);
            }
            else if (modif == "")
                return string.Format("{0} As New {1}", nombre, elTipo);
            else
                return string.Format("{0} {1} As New {2}", modif, nombre, elTipo);
        }
        public static string DeclaraVariableNewParam(string modif, string nombre, string elTipo, string param)
        {
            return VariableNewParam(modif, nombre, elTipo, param);
        }
        public static string VariableNewParam(string modif, string nombre, string elTipo, string param)
        {
            if (Lang == eLenguaje.eCS)
            {
                if (modif == "" || modif.ToLower() == "dim")
                    return string.Format("{0} {1} = new {0}({2});", Tipo(elTipo), nombre, comprobarParam(param));
                else
                    return string.Format("{0} {1} {2} = new {1}({3});", modificador(modif), Tipo(elTipo), nombre, comprobarParam(param));
            }
            else if (modif == "")
                return string.Format("{0} As New {1}({2})", nombre, elTipo, param);
            else
                return string.Format("{0} {1} As New {2}({3})", modif, nombre, elTipo, param);
        }
        // 
        // comprobar si tiene New...
        private static string comprobarParam(string var)
        {
            // por ejemplo, en un parámetro se puede indicar "New LosQueSea"
            if (Lang == eLenguaje.eCS)
            {
                // Habría que comprobar si hay más de una instrucción en la cadena a evaluar. (01/oct/22 10.24)
                for (int i = 0; i <= instrVB.Length - 1; i++)
                    var = (" " + var).Replace(instrVB[i], instrCS[i]).Substring(1);
                // 
                return var; // (" " & var).Replace("New ", "new ").Replace(" Me.", " this.").Replace(" Me(", " this(").Replace(" Me,", " this,").Replace("(Me,", "(this,").Replace(" True", " true").Replace(" False", " false")
            }
            else
                return var;
        }
    }
}
