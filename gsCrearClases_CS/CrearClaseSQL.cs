// ------------------------------------------------------------------------------
// Clase para crear una clase a partir de una tabla de SQL Server    (08/Jul/04)
// Basado en el código anteriormente incluido en crearClasesSQLVB    (07/Jul/04)
// 
// Todos los métodos son estáticos (compartidos) para usarlos sin crear una instancia
// 
// Muevo esta clase al formulario ya que en la DLL no es necesaria   (05/oct/22)
//
// Convertida a C#.                                             (06/oct/22 17.55)
// https://converter.telerik.com/
// Con algo de ayuda
// 
// ©Guillermo 'guille' Som, 2004, 2005, 2007, 2022
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

using System.Data;

// Con esta importación debería acceder a SqlDataAdapter y SqlConnection.
// con el paquete System.Data.SqlClient Versión 4.8.3
//using System.Data.SqlClient;
// A ver si con esto... usando: <PackageReference Include="Microsoft.Data.SqlClient" Version="5.0.1" />
// En Android sigue dando el mismo error:
//      ERROR: A connection was successfully established with the server, but then an error occurred during the pre-login handshake. (provider: TCP Provider, error: 35 - An internal exception was caught)
// pero en Windows e iOS funciona y
// dicen que es mejor usar Microsoft.Data.SqlClient que System.Data.SqlClient porque el primero está más actualizado.
using Microsoft.Data.SqlClient;

//using elGuille.Util.Developer;
//using elGuille.Util.Developer.Data;

namespace elGuille.Util.Developer.Data
{
    public class CrearClaseSQL : CrearClase
    {
        // 
        // 
        public static string Conectar(string dataSource, string initialCatalog, string cadenaSelect)
        {
            return Conectar(dataSource, initialCatalog, cadenaSelect, "", "", false);
        }

        /// <summary>
        /// Conectar con el servidor de SQL Server.
        /// </summary>
        /// <param name="dataSource">El servidor de SQL Server.</param>
        /// <param name="initialCatalog">La base de datos.</param>
        /// <param name="cadenaSelect">Opcional, la cadena de selección.</param>
        /// <param name="userId">El usuario con acceso a la base de datos.</param>
        /// <param name="password">El password del usaurio.</param>
        /// <param name="seguridadSQL">True si se usa la seguridad integrada de Windows.</param>
        /// <remarks>añado TrustServerCertificate=True; a la cadena de conexión.</remarks>
        public static string Conectar(string dataSource, string initialCatalog, string cadenaSelect, string userId, string password, bool seguridadSQL)
        {
            // si se produce algún error, se devuelve una cadena empezando por ERROR
            Conectado = false;
            // 
            cadenaConexion = "data source=" + dataSource + "; initial catalog=" + initialCatalog + ";";
            // 
            if (seguridadSQL)
            {
                if (userId != "")
                    cadenaConexion += "user id=" + userId + ";";
                if (password != "")
                    cadenaConexion += "password=" + password + ";";
            }
            else
                cadenaConexion += "Integrated Security=yes;";

            // Añadir TrustServerCertificate=True y Encrypt=false a la cadena de conexión para que funcione en Android.
            //cadenaConexion += "TrustServerCertificate=True;Encrypt=false;";
            //cadenaConexion += "TrustServerCertificate=True;MultiSubnetFailover=True;";
            //cadenaConexion += "MultiSubnetFailover=True;";
            cadenaConexion += "TrustServerCertificate=True;";
            // 
            if (cadenaSelect == "")
            {
                // si no se indica la cadena Select también se conecta
                // esto es útil para averiguar las tablas de la base
                Conectado = true;
                return "";
            }
            // 
            SqlDataAdapter dbDataAdapter = new SqlDataAdapter(cadenaSelect, cadenaConexion);
            // 
            dbDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            // 
            // Limpiar el contenido de mDataTable                    (08/Jun/05)
            mDataTable = new DataTable();
            // 
            try
            {
                dbDataAdapter.Fill(mDataTable);
                System.Threading.Thread.Sleep(100);
                Conectado = true;
            }
            catch (Exception ex)
            {
                return "ERROR: en Fill: " + ex.Message;
            }// & " - " & ex.GetType().Name
             // 
            return "";
        }

        //public static string[] NombresTablas()

        /// <summary>
        /// Devuelve una colección de tipo string con los nombres de las tablas.
        /// </summary>
        /// <remarks>Antes usaba un array de tipo string.</remarks>
        public static List<string> NombresTablas()
        {
            //string[] nomTablas = null;
            List<string> nomTablas = new();

            DataTable dt = new DataTable();
            int i;
            SqlConnection dbConnection = new SqlConnection(cadenaConexion);
            // 
            try
            {
                dbConnection.Open();
            }
            catch (Exception ex)
            {
                //nomTablas = new string[1];
                //nomTablas[0] = "ERROR: " + ex.Message;
                nomTablas.Add("ERROR: " + ex.Message);
                Conectado = false;
                return nomTablas;
            }
            // 
            SqlDataAdapter schemaDA = new SqlDataAdapter("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_TYPE", dbConnection);
            // 
            schemaDA.Fill(dt);
            i = dt.Rows.Count - 1;
            if (i > -1)
            {
                //nomTablas = new string[i + 1];
                for (i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    var tableName = dt.Rows[i]["TABLE_NAME"].ToString();
                    // Comprobar que la tabla tenga texto.
                    //  Por si es por esto por lo que no se muestra en el picker de Maui.
                    // Pero se ve que no es esto... :-/
                    if (string.IsNullOrWhiteSpace(tableName))
                    {
                        continue;
                    }
                    // si el valor de TABLE_SCHEMA no es dbo, es que es una tabla de un usuario particular
                    if (dt.Rows[i]["TABLE_SCHEMA"].ToString().ToLower() != "dbo")
                        nomTablas.Add(dt.Rows[i]["TABLE_SCHEMA"].ToString() + "." + tableName);
                    else
                        nomTablas.Add(tableName);
                }
            }
            // 
            return nomTablas;
        }
        // 
        public static string GenerarClase(eLenguaje lang, bool usarCommandBuilder, string nombreClase, string nomTabla, string dataSource, string initialCatalog, string cadenaSelect, string userId, string password, bool usarSeguridadSQL)
        {
            string s;
            // 
            nombreTabla = nomTabla;
            if (nombreTabla == "" || nombreClase == "")
                return "ERROR, no se ha indicado el nombre de la tabla o de la clase.";
            s = Conectar(dataSource, initialCatalog, cadenaSelect, userId, password, usarSeguridadSQL);
            if (Conectado == false || s != "")
                return s;
            // 
            // Comprobar si el nombre de la clase tiene espacios     (02/Nov/04)
            // de ser así, cambiarlo por un guión bajo.
            // Bug reportado por David Sans
            nombreClase = nombreClase.Replace(" ", "_");
            // 
            return CrearClase.GenerarClaseSQL(lang, usarCommandBuilder, nombreClase, dataSource, initialCatalog, cadenaSelect, userId, password, usarSeguridadSQL);
        }
        public static string GenerarClase(eLenguaje lang, bool usarCommandBuilder, string nombreClase, string nomTabla, string dataSource, string initialCatalog, string cadenaSelect)
        {
            // 
            return GenerarClase(lang, usarCommandBuilder, nombreClase, nomTabla, dataSource, initialCatalog, cadenaSelect, "", "", false);
        }
    }
}