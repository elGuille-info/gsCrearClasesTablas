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
using System.Data.SqlClient;

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
        // 
        public static string[] NombresTablas()
        {
            string[] nomTablas = null;
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
                nomTablas = new string[1];
                nomTablas[0] = "ERROR: " + ex.Message;
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
                nomTablas = new string[i + 1];
                for (i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    // si el valor de TABLE_SCHEMA no es dbo, es que es una tabla de un usuario particular
                    if (dt.Rows[i]["TABLE_SCHEMA"].ToString().ToLower() != "dbo")
                        nomTablas[i] = dt.Rows[i]["TABLE_SCHEMA"].ToString() + "." + dt.Rows[i]["TABLE_NAME"].ToString();
                    else
                        nomTablas[i] = dt.Rows[i]["TABLE_NAME"].ToString();
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