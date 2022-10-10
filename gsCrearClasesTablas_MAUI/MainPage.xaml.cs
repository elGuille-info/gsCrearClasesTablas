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

                expOpcionesSQL.IsExpanded = true;
                expOpcionesComandos.IsExpanded = true;
                expOpcionesTablas.IsExpanded = true;
                expOpcionesTablas.Refrescar(true, true);
                expOpcionesCodigo.IsExpanded = true;
                expOpcionesCodigo.Refrescar(true, true);

                grbOpciones.IsEnabled = false;
                btnGenerarClase.IsEnabled = false;
                Panel1.IsEnabled = false;
                btnLimpiar.IsEnabled = false;

                LeerConfig();

                // Si el servidor de SQL, la base de datos y
                // o la seguridad integrada o el usuario/password están puestos ocultar el panel de SQL.
                if (txtDataSource.Text.Any() && txtInitialCatalog.Text.Any() && 
                    (chkSeguridadSQL.IsChecked == false || (chkSeguridadSQL.IsChecked && txtUserId.Text.Any() && txtPassword.Text.Any())))
                {
                    expOpcionesSQL.IsExpanded = false;
                    expOpcionesComandos.IsExpanded = false;
                }
                expOpcionesSQL.Refrescar(true, true);
                expOpcionesComandos.Refrescar(true, true);

                LaPrimeraVez = false;
            }
        }

        private void btnMostrarTablas_Clicked(object sender, EventArgs e)
        {
            // Para ver el tamaño de la ventana:
            // el ancho más adecuado es 1170, con un alto de 1150
            // Lo pongo en 1700x1800 en Windows (1118,66 x 1114,66).
            txtCodigo.Text = $"Tamaño de la ventana: Width: {Width}, Height: {Height}";

            btnMostrarTablas.IsEnabled = false;

            GuardarConfig();

            btnLimpiar.IsEnabled = false;

            // No tener en cuenta la cadena select para mostrar las tablas
            txtSelect.Text = "";
            if (optSQL.IsChecked)
                CrearClaseSQL.Conectar(txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
            //else
            //    CrearClaseOleDb.Conectar(this.txtNombreBase.Text, txtSelect.Text, txtProvider.Text, txtAccessPassword.Text);
            // 
            
            grbOpciones.IsEnabled = CrearClase.Conectado;
            btnGenerarClase.IsEnabled = CrearClase.Conectado;
            Panel1.IsEnabled = CrearClase.Conectado;
            btnCopiarClipBoard.IsEnabled = CrearClase.Conectado;
            // 
            //  No se puede usar Clear si está con data source.
            //cboTablas.Items.Clear();
            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                listViewTablas.ItemsSource = null;
                listViewTablas.IsEnabled = false;
            }
            else
            {
                cboTablas.ItemsSource = null;
                cboTablas.IsEnabled = false;
            }

            if (CrearClase.Conectado == false)
            {
                btnMostrarTablas.IsEnabled = true;
                return;
            }
            
            List<TablaItem> nomTablas = null;
            if (optSQL.IsChecked)
                nomTablas = CrearClaseSQL.NombresTablasItem();
            //else
            //    nomTablas = CrearClaseOleDb.NombresTablas();
            // 

            if ((nomTablas == null) || nomTablas[0].Nombre.StartsWith("ERROR"))
            {
                grbOpciones.IsEnabled = false;
                btnGenerarClase.IsEnabled = false;
                Panel1.IsEnabled = false;
                btnCopiarClipBoard.IsEnabled = false;

                // Mostrar el error.
                if (nomTablas != null)
                {
                    LabelInfoTablas.Text = nomTablas[0].Nombre;
                    // Mostrarlo también en el texto del código para poder copiarlo.
                    txtCodigo.Text = nomTablas[0].Nombre;
                }
                else
                    LabelInfoTablas.Text = "No se ha podido mostrar las tablas.";

                btnMostrarTablas.IsEnabled = true;
                return;
            }
            LabelInfoTablas.Text = $"Hay {nomTablas.Count} tablas en la base de datos {txtInitialCatalog.Text}.";

            btnLimpiar.IsEnabled = true;

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                listViewTablas.ItemsSource = nomTablas;
                listViewTablas.IsEnabled = true;
                listViewTablas.SelectedItem = nomTablas[0];
                listViewTablas.Focus();
            }
            else
            {
                cboTablas.ItemsSource = nomTablas;
                cboTablas.IsEnabled = true;
                // Seleccionar el primer elemento.
                cboTablas.SelectedItem = nomTablas[0];

                cboTablas.Focus();
            }

            btnMostrarTablas.IsEnabled = true;
        }

        private void listViewTablas_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (listViewTablas.SelectedItem == null)
                return;

            string laTabla = (e.SelectedItem as TablaItem).Nombre; // e.SelectedItem.ToString();
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

        private void cboTablas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboTablas.SelectedItem == null)
                return;

            string laTabla = (cboTablas.SelectedItem as TablaItem).Nombre;
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

            string laTabla;

            if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                laTabla = listViewTablas.SelectedItem.ToString();
            }
            else
            {
                laTabla = cboTablas.SelectedItem.ToString();
            }

            if (optVB.IsToggled)
            {
                if (optSQL.IsChecked)
                    txtCodigo.Text = CrearClaseSQL.GenerarClase(eLenguaje.eVBNET, usarCB, txtClase.Text, laTabla, txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
                //else
                //    txtCodigo.Text = CrearClaseOleDb.GenerarClase(eLenguaje.eVBNET, usarCB, txtClase.Text, cboTablas.Text, txtNombreBase.Text, txtSelect.Text, txtAccessPassword.Text, txtProvider.Text);
            }
            else if (optSQL.IsChecked)
                txtCodigo.Text = CrearClaseSQL.GenerarClase(eLenguaje.eCS, usarCB, txtClase.Text, laTabla, txtDataSource.Text, txtInitialCatalog.Text, txtSelect.Text, txtUserId.Text, txtPassword.Text, chkSeguridadSQL.IsChecked);
            //else
            //    txtCodigo.Text = CrearClaseOleDb.GenerarClase(eLenguaje.eCS, usarCB, txtClase.Text, cboTablas.Text, txtNombreBase.Text, txtSelect.Text, txtAccessPassword.Text, txtProvider.Text);

            btnCopiarClipBoard.IsEnabled = true;
        }

        /// <summary>
        /// Leer los valores de la configuración (local).
        /// </summary>
        private void LeerConfig()
        {
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
            chkUsarCommandBuilder.IsEnabled = chkUsarDataAdapter.IsChecked;
        }
        /// <summary>
        /// Guardar los valores en la configuración (local).
        /// </summary>
        private void GuardarConfig()
        {
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

        private void grbOpciones_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                //App.HabilitarContenedor(grbOpciones, grbOpciones.IsEnabled, limpiar: false);
                // ¡En los checkBox Funciona asignado el valor al revés! ???
                // Los colores... ¡OJÚ!

                chkUsarDataAdapter.IsEnabled = grbOpciones.IsEnabled;
                //chkUsarCommandBuilder.IsEnabled = grbOpciones.IsEnabled;
                chkUsarAddWithValue.IsEnabled = grbOpciones.IsEnabled;
                chkUsarOverrides.IsEnabled = grbOpciones.IsEnabled;

                Panel1.IsEnabled = grbOpciones.IsEnabled;
                
                btnGenerarClase.IsEnabled = grbOpciones.IsEnabled;
                //btnCopiarClipBoard.IsEnabled = grbOpciones.IsEnabled;
                btnCopiarClipBoard.IsEnabled = string.IsNullOrWhiteSpace(txtCodigo.Text) == false;
            }
        }

        private void Panel1_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                optCS.IsEnabled = Panel1.IsEnabled;
                optVB.IsEnabled = Panel1.IsEnabled;
            }
        }

        private void StackLayout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                if (txtUserId == null) return;

                txtUserId.IsEnabled = (sender as StackLayout).IsEnabled;
                txtPassword.IsEnabled = txtUserId.IsEnabled;
            }
        }

        //private void UsarDataAdapterTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    chkUsarDataAdapter.IsChecked = !chkUsarDataAdapter.IsChecked;
        //}

        //private void UsarCommandBuilderTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    chkUsarCommandBuilder.IsChecked = !chkUsarCommandBuilder.IsChecked;
        //}

        //private void UsarAddWithValueTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    chkUsarAddWithValue.IsChecked = !chkUsarAddWithValue.IsChecked;
        //}

        //private void UsarOverridesTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    chkUsarOverrides.IsChecked = !chkUsarOverrides.IsChecked;
        //}

        //private void SQLTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    optSQL.IsChecked = !optSQL.IsChecked;
        //}

        //private void SeguridadSQLTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    chkSeguridadSQL.IsChecked = !chkSeguridadSQL.IsChecked;
        //}

        //private void VBTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    optVB.IsToggled = !optVB.IsToggled;
        //}

        //private void CSTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    optCS.IsToggled = !optCS.IsToggled;
        //}

        private void btnLimpiar_Clicked(object sender, EventArgs e)
        {
            cboTablas.ItemsSource = null;
            listViewTablas.ItemsSource = null;
            grbOpciones.IsEnabled = false;
            txtSelect.Text = "";
            txtClase.Text = "";
            btnLimpiar.IsEnabled = false;
            LabelInfoTablas.Text = $"Pulsa en 'Mostrar Tablas' para ver las tablas de {txtInitialCatalog.Text}.";
        }

        private async void btnCopiarClipBoard_Clicked(object sender, EventArgs e)
        {
            //Device.BeginInvokeOnMainThread(() =>{});
            if (MainThread.IsMainThread)
            {
                try
                {
                    // Code to run if this is the main thread
                    await Clipboard.SetTextAsync(txtCodigo.Text);
                }
                catch { }
            }
        }

        //private void OpcionesSQLTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    grbSQL.IsVisible = !grbSQL.IsVisible;
        //}

        //private void OpcionesTablasTapGestureRecognizer_Tapped(object sender, EventArgs e)
        //{
        //    grbTablas.IsVisible = !grbTablas.IsVisible;
        //}

        private void expOpcionesSQL_Expanded(object sender, bool isExpanded)
        {
            grbSQL.IsVisible = isExpanded;
        }

        private void expOpcionesTablas_Expanded(object sender, bool isExpanded)
        {
            grbTablas.IsVisible = isExpanded;
        }

        private void expOpcionesComandos_Expanded(object sender, bool isExpanded)
        {
            grbOpcionesComandos.IsVisible = isExpanded;
        }

        private void expOpcionesCodigo_Expanded(object sender, bool isExpanded)
        {
            grbOpcionesCodigo.IsVisible = isExpanded;
        }
    }
}