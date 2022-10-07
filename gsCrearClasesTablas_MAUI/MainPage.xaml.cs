using System.Diagnostics;

using elGuille.Util.Developer;
using elGuille.Util.Developer.Data;

namespace gsCrearClasesTablas_MAUI
{
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// El path local de la aplicación.
        /// </summary>
        private string FolderPath { get; set; }
        /// <summary>
        /// El fichero de configuración.
        /// </summary>
        private string FicConfig { get; set; }

        private bool LaPrimeraVez { get; set; } = true;

        private FileVersionInfo AppInfo { get; set; }

        public MainPage()
        {
            InitializeComponent();

            var ensamblado = typeof(MainPage).Assembly;
            AppInfo = FileVersionInfo.GetVersionInfo(ensamblado.Location);

            FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //FicConfig = Path.Combine(FolderPath, $"{App.AppName}.txt");
            FicConfig = Path.Combine(FolderPath, "gsCrearClasesTablas.txt");
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (LaPrimeraVez)
            {
                var sCopyR = "©Guillermo Som (elGuille), 2004-2007, 2018-";
                var elAño = 2022;
                if (DateTime.Today.Year > 2022)
                    elAño = DateTime.Today.Year;
                LabelInfo.Text = $"  {sCopyR}{elAño}  ";

                //LabelVersion.Text = $"  {AppInfo.ProductName} - v{AppInfo.ProductVersion} ({AppInfo.FileVersion})  ";
                LabelVersion.Text = $"  {AppInfo.ProductName} - v{AppInfo.ProductMajorPart}.{AppInfo.ProductMinorPart}.{AppInfo.ProductBuildPart} ({AppInfo.FileVersion})  ";

                LeerConfig();
                LaPrimeraVez = false;
            }
        }

        private void btnMostrarTablas_Clicked(object sender, EventArgs e)
        {
            GuardarConfig();

            // No tener en cuenta la cadena select para mostrar las tablas
            txtSelect.Text = "";
            if (optSQL.IsChecked)
                CrearClaseSQL.Conectar(txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
            //else
            //    CrearClaseOleDb.Conectar(this.txtNombreBase.Text, txtSelect.Text, txtProvider.Text, txtAccessPassword.Text);
            // 
            btnGenerarClase.IsEnabled = CrearClase.Conectado;
            Panel1.IsEnabled = CrearClase.Conectado;
            //btnGuardar.IsEnabled = CrearClase.Conectado;
            // 
            //  No se puede usar Clear si está con data source.
            //cboTablas.Items.Clear();
            cboTablas.ItemsSource = null;
            cboTablas.IsEnabled = false;

            if (CrearClase.Conectado == false)
                return;
            // 
            //string[] nomTablas = null;
            List<string> nomTablas = null;
            if (optSQL.IsChecked)
                nomTablas = CrearClaseSQL.NombresTablas();
            //else
            //    nomTablas = CrearClaseOleDb.NombresTablas();
            // 
            if ((nomTablas == null) || nomTablas[0].StartsWith("ERROR"))
            {
                btnGenerarClase.IsEnabled = false;
                Panel1.IsEnabled = false;
                //btnGuardar.Enabled = false;
                return;
            }

            //foreach (string s in nomTablas)
            //{
            //    cboTablas.Items.Add(s);
            //}
            cboTablas.ItemsSource = nomTablas;

            cboTablas.IsEnabled = true;
            if (cboTablas.Items.Count > 0)
                cboTablas.SelectedIndex = 0;

            cboTablas.Focus();
        }

        private void cboTablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cboTablas.ItemsSource == null || cboTablas.SelectedIndex < 0)
            if (cboTablas.SelectedIndex < 0)
            {
                return;
            }

            string laTabla = cboTablas.SelectedItem.ToString();
            txtSelect.Text = "SELECT * FROM " + laTabla;
            int i = laTabla.IndexOf(".");
            // Si la tabla contiene espacios,                            (02/Nov/04)
            // sustituirlos por guiones bajos.
            // Bug reportado por David Sans
            if (i > -1)
                txtClase.Text = laTabla.Substring(i + 1).Replace(" ", "_");
            else
                txtClase.Text = laTabla.Replace(" ", "_");
        }

        private void btnGenerarClase_Clicked(object sender, EventArgs e)
        {
            txtCodigo.Text = "";
            GuardarConfig();
            // 
            // Si la tabla contiene espacios,                            (02/Nov/04)
            // sustituirlos por guiones bajos.
            // Bug reportado por David Sans
            txtClase.Text = txtClase.Text.Replace(" ", "_");

            // Lo asigno en la clase base ya que son métodos compartidos (22/Mar/19)
            CrearClase.UsarDataAdapter = chkUsarDataAdapter.IsChecked;
            if (chkUsarDataAdapter.IsChecked == false)
                chkUsarCommandBuilder.IsEnabled = false;
            CrearClase.UsarAddWithValue = chkUsarAddWithValue.IsChecked;
            CrearClase.UsarOverrides = chkUsarOverrides.IsChecked;

            // Si no está habilitado es que no se utiliza                (07/Abr/19)
            // ya que solo se usa con DataAdapter
            bool usarCB = chkUsarCommandBuilder.IsChecked;
            if (chkUsarCommandBuilder.IsEnabled == false)
                usarCB = false;

            if (optVB.IsToggled)
            {
                if (optSQL.IsChecked)
                    txtCodigo.Text = CrearClaseSQL.GenerarClase(eLenguaje.eVBNET, usarCB, txtClase.Text, cboTablas.SelectedItem.ToString(), txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
                //else
                //    txtCodigo.Text = CrearClaseOleDb.GenerarClase(eLenguaje.eVBNET, usarCB, txtClase.Text, cboTablas.Text, txtNombreBase.Text, txtSelect.Text, txtAccessPassword.Text, txtProvider.Text);
            }
            else if (optSQL.IsChecked)
                txtCodigo.Text = CrearClaseSQL.GenerarClase(eLenguaje.eCS, usarCB, txtClase.Text, cboTablas.SelectedItem.ToString(), txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
            //else
            //    txtCodigo.Text = CrearClaseOleDb.GenerarClase(eLenguaje.eCS, usarCB, txtClase.Text, cboTablas.Text, txtNombreBase.Text, txtSelect.Text, txtAccessPassword.Text, txtProvider.Text);
        }

        /// <summary>
        /// Leer los valores de la configuración (local).
        /// </summary>
        private void LeerConfig()
        {
            //if (string.IsNullOrWhiteSpace(FicConfig))
            //{
            //    var FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //    FicConfig = Path.Combine(FolderPath, $"{App.AppName}.txt");
            //}
            if (!File.Exists(FicConfig))
            {
                return;
            }
            string sTmp;
            using (var sr = new StreamReader(FicConfig, System.Text.Encoding.Default, true))
            {
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                optSQL.IsChecked = sTmp == "1";
                sTmp = "";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                txtDataSource.Text = sTmp;
                sTmp = "";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                txtInitialCatalog.Text = sTmp;
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                chkSeguridadSQL.IsChecked = sTmp == "1";
                sTmp = "";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                txtUserId.Text = sTmp;
                sTmp = "";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                txtPassword.Text = sTmp;
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                chkUsarDataAdapter.IsChecked = sTmp == "1";
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                chkUsarCommandBuilder.IsChecked = sTmp == "1";
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                chkUsarAddWithValue.IsChecked = sTmp == "1";
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                chkUsarOverrides.IsChecked = sTmp == "1";
                sTmp = "1";
                if (!sr.EndOfStream)
                    sTmp = sr.ReadLine();
                optVB.IsToggled = sTmp == "1";
                //optCS.IsToggled = ! optVB.IsToggled;
            }
        }
        /// <summary>
        /// Guardar los valores en la configuración (local).
        /// </summary>
        private void GuardarConfig()
        {
            //if (string.IsNullOrWhiteSpace(FicConfig))
            //{
            //    var FolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            //    FicConfig = Path.Combine(FolderPath, $"{App.AppName}.txt");
            //}

            using (var sw = new StreamWriter(FicConfig, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(optSQL.IsChecked ? "1" : "0");
                sw.WriteLine(txtDataSource.Text);
                sw.WriteLine(txtInitialCatalog.Text);
                sw.WriteLine(chkSeguridadSQL.IsChecked ? "1" : "0");
                sw.WriteLine(txtUserId.Text);
                sw.WriteLine(txtPassword.Text);
                sw.WriteLine(chkUsarDataAdapter.IsChecked ? "1" : "0");
                sw.WriteLine(chkUsarCommandBuilder.IsChecked ? "1" : "0");
                sw.WriteLine(chkUsarAddWithValue.IsChecked ? "1" : "0");
                sw.WriteLine(chkUsarOverrides.IsChecked ? "1" : "0");
                sw.WriteLine(optVB.IsToggled ? "1" : "0");
            }
        }
    }
}