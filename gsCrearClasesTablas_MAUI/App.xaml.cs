namespace gsCrearClasesTablas_MAUI
{
    public partial class App : Application
    {
        /// <summary>
        /// El nombre de la aplicación.
        /// </summary>
        public static string AppName { get; set; } = "gsCrearClasesTablas_MAUI";

        /// <summary>
        /// La versión de la aplicación.
        /// </summary>
        public static string AppVersion { get; } = "3.0.3";

        /// <summary>
        /// La versión del fichero (la revisión)
        /// </summary>
        public static string AppFileVersion { get; } = "3.0.3.2";

        /// <summary>
        /// La fecha de última actualización
        /// </summary>
        public static string AppFechaVersion { get; } = "08-oct-2022";

        // Intentar no pasar de estas marcas: 60 caracteres. 2         3         4         5         6
        //                                ---------|---------|---------|---------|---------|---------|
        //[COPIAR]AppDescripcionCopia = " preparando el interfaz"


        public App()
        {
            InitializeComponent();

            // Indicar el tamaño para la app de Windows.
            Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
            {
#if WINDOWS
            // Asignar manualmente el tamaño. 
            int winWidth = 1700;
            int winHeight = 1800;

                        var mauiWindow = handler.VirtualView;
                        var nativeWindow = handler.PlatformView;
                        nativeWindow.Activate();
                        IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                        appWindow.Resize(new Windows.Graphics.SizeInt32(winWidth, winHeight));
#endif
            });


            MainPage = new AppShell();
        }

        /// <summary>
        /// Deshabilitar los controles del grid indicado.
        /// </summary>
        /// <param name="elContenedor">El control con los controles a deshabilitar.</param>
        /// <param name="habilitar">Si se habilita o no. En Entry y Editor se asigna IsReadOnly en vez de IsEnabled.</param>
        /// <param name="limpiar">Si es Entry o Editor limpiar el texto, en otros casos los pone como no activados.</param>
        /// <remarks>Si es InputView se asigna readonly en vez de enabled.</remarks>
        public static async void HabilitarContenedor(Layout elContenedor, bool habilitar, bool limpiar)
        {
            foreach (View v in elContenedor.Children)
            {
                if (v is Layout)
                {
                    HabilitarContenedor(v as Layout, habilitar, limpiar);
                }
                else
                {
                    PonerHabilitado(v, habilitar, limpiar);
                }
            }
            await App.Refrescar(5);
        }

        /// <summary>
        /// Poner habilitado o no el control indicado.
        /// </summary>
        /// <param name="v">El control. Puede ser Editor, Entry, CheckBox, SwitchLabel, CheckLabelTri./param>
        /// <param name="habilitar">Si se habilita o no. En Entry y Editor se asigna IsReadOnly en vez de IsEnabled.</param>
        /// <param name="limpiar">Si es Entry o Editor limpiar el texto, en otros casos los pone como no activados.</param>
        /// <remarks>Si es InputView se asigna readonly en vez de enabled.</remarks>
        public static void PonerHabilitado(View v, bool habilitar, bool limpiar)
        {
            if (v is Entry || v is Editor)
            {
                var ve = (v as InputView);
                // Utilizar los colores definidos en los recursos v1.5.5.163

                PonerReadOnlyInputView(ve, habilitar == false);

                if (limpiar)
                {
                    ve.Text = "";
                }
            }
            else if (v is Label)
            {
                v.IsEnabled = habilitar;
            }
            else if (v is CheckBox)
            {
                v.IsEnabled = habilitar;
                if (limpiar)
                {
                    (v as CheckBox).IsChecked = false;
                }
            }
            else if (v is Switch)
            {
                v.IsEnabled = habilitar;
                if (limpiar)
                {
                    (v as Switch).IsToggled = false;
                }
            }
        }

        /// <summary>
        /// Poner IsReadOnly en el Editor o Entry y asignar el color según el tema.
        /// </summary>
        /// <param name="ve">El control (Entry o Editor).</param>
        /// <param name="esReadOnly">Si es ReadOnly.</param>
        public static void PonerReadOnlyInputView(InputView ve, bool esReadOnly)
        {
            ve.IsReadOnly = esReadOnly;

            Color textC; // = Color.Navy;
            Color backC; // = Color.Transparent;

            if (esReadOnly)
            {
                if (Application.Current.RequestedTheme == AppTheme.Dark)
                {
                    textC = (Color)Application.Current.Resources["IWTextReadOnly1Dark"];
                    backC = (Color)Application.Current.Resources["IWBackReadOnly1Dark"];
                }
                else
                {
                    textC = (Color)Application.Current.Resources["IWTextReadOnly1Light"];
                    backC = (Color)Application.Current.Resources["IWBackReadOnly1Light"];
                }
            }
            else
            {
                if (Application.Current.RequestedTheme == AppTheme.Dark)
                {
                    textC = (Color)Application.Current.Resources["IWTextReadOnly0Dark"];
                    backC = (Color)Application.Current.Resources["IWBackReadOnly0Dark"];
                }
                else
                {
                    textC = (Color)Application.Current.Resources["IWTextReadOnly0Light"];
                    backC = (Color)Application.Current.Resources["IWBackReadOnly0Light"];
                }
            }
            ve.TextColor = textC;
            ve.BackgroundColor = backC;
        }

        /// <summary>
        /// Hacer una pequeña pausa para refrescar.
        /// </summary>
        /// <param name="intervalo">El tiempo que hay que esperar (en milisegundos).</param>
        public static async Task Refrescar(int intervalo = 300)
        {
            await Task.Delay(intervalo);
        }
    }
}