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
        public static string AppFileVersion { get; } = "3.0.3.1";

        /// <summary>
        /// La fecha de última actualización
        /// </summary>
        public static string AppFechaVersion { get; } = "07-oct-2022";

        // Intentar no pasar de estas marcas: 60 caracteres. 2         3         4         5         6
        //                                ---------|---------|---------|---------|---------|---------|
        //[COPIAR]AppDescripcionCopia = " preparando el interfaz"


        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}