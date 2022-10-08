//
// Clase para usar un Switch con etiqueta.                   (08/oct/22 15.20)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Converters;
using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace gsCrearClasesTablas_MAUI.Controles
{
    /// <summary>
    /// Una etiqueta y un Switch.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SwitchLabel : StackLayout
    {
        private Color thumbC;
        private Color labelC;
        public SwitchLabel()
        {
            InitializeComponent();
            // A ver si poniendo otra vez el nombre que tenía funciona. v1.20.0.10 (16/sep/22 19.16)
            // Es que al ejecutar da error que no se encuentra chkButton y muestra el código (interno) de esta clase.
            thumbC = chkButton.ThumbColor;
            labelC = LabelText.TextColor;

            // Para usar los nuevos colores de Android. v1.10.28.5 (02/sep/22 19.45)
            if (App.DevicePlatform == DevicePlatform.Android)
            {
                thumbC = (Color)Application.Current.Resources["SwitchAndroidThumb"];
                labelC = (Color)Application.Current.Resources["SwitchAndroid"];
            }
            BackColorCheck();
        }

        /// <summary>
        /// Evento para cuando se cambia IsToggled.
        /// </summary>
        public event EventHandler<ToggledEventArgs> Toggled;

        /// <summary>
        /// Evento para cuando cambia el estado de IsToggled.
        /// </summary>
        /// <remarks>Por Compatibilidad, pero es preferible usar Toggled.</remarks>
        [Obsolete("Por compatibilidad con CheckLabel, se debe usar Toggled.")]
        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

        //protected void OnToggled(bool value)
        //{
        //    Toggled?.Invoke(this, new ToggledEventArgs(value));
        //}

        //protected void OnToggled(ToggledEventArgs e)
        //{
        //    Toggled?.Invoke(this, e);
        //}

        // Para controlar si se debe lanzar el evento Toggled o no. v1.31.0.3 (21/sep/22 22.56)

        ///// <summary>
        ///// Asignarle false para que no lance el evento Toggled cuando se quiera asignar directamente.
        ///// </summary>
        ///// <remarks>Una vez producido el cambio se pone nuevamente en true.</remarks>
        //public bool LanzarEventoToggled { get; set; } = true;

        public static readonly BindableProperty IsToggledProperty =
            BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchLabel), null,
                defaultBindingMode: BindingMode.OneWay, propertyChanged: OnIsToggledChanged);

        private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchLabel)bindable;

            if (control.yaEstoy) return;
            control.yaEstoy = true;

            var anterior = (bool)oldValue;
            var value = (bool)newValue;
            // Esto solo si ha cambiado el valor, si no... ¿pa qué?
            if (anterior != value)
            {
                control.LabelText.Text = (value ? control.TextToggled : control.TextUnToggled) ?? control.LabelText.Text;

                control.chkButton.IsToggled = value;
                control.Toggled?.Invoke(control, new ToggledEventArgs(value));
                control.CheckedChanged?.Invoke(control, new CheckedChangedEventArgs(value));
            }
            control.BackColorCheck();

            control.yaEstoy = false;
        }

        protected bool yaEstoy = false;

        /// <summary>
        /// El valor del estado del control del tipo boolean.
        /// </summary>
        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        // Para compatibilidad con CheckLabel. v1.31.0.16 (22/sep/22 16.12)

        /// <summary>
        /// El valor del estado del control de tipo boolean.
        /// </summary>
        /// <remarks>Esta propiedad es por compatibilidad con CheckLabel, pero debería usarse IsToggled.</remarks>
        [Obsolete("En este control es preferible usar IsToggled.")]
        public bool IsChecked
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        


        //// El código anterior con BindableProperty sin usarlo en IsToggled.
        //// Dejar el "BindableProperty" pero no usarla en IsToggled.
        ////

        //// La pongo de forma simplificada, v1.20.0.8 (16/sep/22 17.46)
        //public static readonly BindableProperty IsToggledProperty =
        //    BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchLabel), false);

        //private bool _IsToggled = false;

        //private bool yaEstoy = false;

        ///// <summary>
        ///// Si está seleccionada la opción.
        ///// </summary>
        ///// <remarks>Al asignar el color se llama a BackColorCheck.</remarks>
        //public bool IsToggled
        //{
        //    //get { return chkButton.IsToggled; }
        //    get { return _IsToggled; }

        //    // Esto no funciona. v1.20.1.7 (17/sep/22 18.59)
        //    // get => (bool)GetValue(IsToggledProperty);
        //    // set => SetValue(IsToggledProperty, value);
        //    set
        //    {
        //        if (yaEstoy) return;

        //        if (_IsToggled == value) return;

        //        yaEstoy = true;

        //        _IsToggled = value;

        //        LabelText.Text = (value ? TextToggled : TextUnToggled) ?? LabelText.Text;

        //        // Si hay botón asociado, asignarle el texto que corresponda.
        //        if (Button2Text != null)
        //        {
        //            Button2Text.Text = LabelText.Text;
        //        }

        //        chkButton.IsToggled = value;
        //        //OnToggled(value);
        //        Toggled?.Invoke(this, new ToggledEventArgs(value));
        //        BackColorCheck();

        //        yaEstoy = false;
        //    }
        //}

        private void chkButton_Toggled(object sender, ToggledEventArgs e)
        {
            // Si no está habilitado, no permitir hacer click. v1.35.0.2 (26/sep/22 06.05)
            if (IsEnabled == false) return;

            if (yaEstoy) return;
            IsToggled = e.Value;
        }

        // Al pulsar en el control o la etiqueta. v1.10.5.20 (02/jul/22 15.30)
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // Si no está habilitado, no permitir hacer click. v1.35.0.2 (26/sep/22 06.05)
            if (IsEnabled == false) return;

            //IsToggled = !chkButton.IsToggled;
            IsToggled = !IsToggled;
        }

        /// <summary>
        /// El tamaño de la fuente se asigna en la etiqueta.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize { get => LabelText.FontSize; set => LabelText.FontSize = value; }

        private FontAttributes _FontAttributes = FontAttributes.None;

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SwitchLabel), 
                FontAttributes.None,
                propertyChanged: OnFontAttributesChanged);

        private static void OnFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchLabel)bindable;

            var nuevoValor = (FontAttributes)newValue;
            control.LabelText.FontAttributes = nuevoValor;
            control._FontAttributes = nuevoValor;
        }

        /// <summary>
        /// El atributo se asigna en la etiqueta.
        /// </summary>
        [TypeConverter(typeof(FontAttributesConverter))]
        public FontAttributes FontAttributes 
        {
            get => (FontAttributes)GetValue(FontAttributesProperty); // LabelText.FontAttributes; 
            set => SetValue(FontAttributesProperty, value); //LabelText.FontAttributes = value;
        }

        /*
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        */

        // Creo que no hace falta, se peude usar el valor interno de Orientation. v1.20.0.12 (16/sep/22 20.30)

        //
        // Para compatibilidad con CheckLabelTri. v1.10.28.12 (03/sep/22 14.56)
        //

        ///// <summary>
        ///// Si se debe cambiar el orden de la etiqueta y el switch.
        ///// </summary>
        ///// <remarks>Lo pongo por compatibilidad con CheckLabelTri.</remarks>
        //public StackOrientation Orientacion
        //{
        //    get => Orientation;
        //    set => Orientation = value;
        //}

        public void BackColorCheck()
        {
            // Asignar los colores si está o no habilitado, v1.11.2.43 (14/sep/22 06.03)
            // por si se llama a este método después de asignar IsEnabled.
            if (chkButton.IsEnabled)
            {
                chkButton.ThumbColor = thumbC;
                LabelText.TextColor = labelC;
                // Usar el color asignado. v1.36.0.4 (27/sep/22 11.47)
                LabelText.FontAttributes = _FontAttributes;
            }
            else
            {
                chkButton.ThumbColor = App.GrisDeshabilitado; // Color.FromHex("#7A7A7A"); // Color.Gray;
                LabelText.TextColor = App.GrisDeshabilitado; // Color.FromHex("#7A7A7A"); // Color.Gray;
                LabelText.FontAttributes = FontAttributes.Italic;
            }
        }

        //
        // Cambiar el orden de la etiqueta y el Switch.
        // Aunque no tiene sentido salvo que la orientación sea vertical.
        //

        // En el Switch, la etiqueta a la izquierda de forma predeterminada.
        public static readonly BindableProperty EtiquetaAlPrincipioProperty =
            BindableProperty.Create(nameof(EtiquetaAlPrincipio), typeof(bool), typeof(SwitchLabel), true,
                propertyChanged: OnEtiquetaAlPrincipioChanged);

        private static void OnEtiquetaAlPrincipioChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchLabel)bindable;

            var nuevoValor = (bool)newValue;
            //control.EtiquetaAlPrincipio = nuevoValor;

            var etiqueta = control.LabelText;
            var boton = control.chkButton;
            control.Children.Clear();
            if (nuevoValor)
            {
                control.Children.Add(etiqueta);
                control.Children.Add(boton);
            }
            else
            {
                control.Children.Add(boton);
                control.Children.Add(etiqueta);
            }
            control.BackColorCheck();
        }

        /// <summary>
        /// Si se debe mostrar el texto antes del switch.
        /// </summary>
        /// <remarks>No ponerla al final, el efecto visual no es consistente.</remarks>
        public bool EtiquetaAlPrincipio
        {
            get => (bool)GetValue(EtiquetaAlPrincipioProperty);
            set => SetValue(EtiquetaAlPrincipioProperty, value);
        }

        // Definirla bindable para usar en diseño. v1.10.5.20 (02/jul/22 14.59)
        public static readonly BindableProperty TextoAlPrincipioProperty =
            BindableProperty.Create(nameof(TextoAlPrincipio), typeof(bool), typeof(SwitchLabel), true,
                propertyChanged: OnEtiquetaAlPrincipioChanged);

        /// <summary>
        /// Otra forma de poner el texto al principio.
        /// </summary>
        /// <remarks>Se usa el valor de la propiedad EtiquetaAlPrincipio.</remarks>
        public bool TextoAlPrincipio
        {
            get => (bool)GetValue(TextoAlPrincipioProperty);
            set => SetValue(TextoAlPrincipioProperty, value);
        }


        //
        // </Cambiar el orden de la etiqueta y el checkbox
        //

        #region Cambiar la orientación del contenido no es necesaria, usar Orientation. v1.10.5.20 (02/jul/22 14.50)

        ////
        //// Cambiar la orientacion del contenido
        ////

        //public static readonly BindableProperty OrientacionProperty =
        //    BindableProperty.Create(nameof(Orientacion), typeof(StackOrientation), typeof(SwitchLabel), StackOrientation.Horizontal,
        //        propertyChanged: OnOrientacionChanged);

        //private static void OnOrientacionChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var control = (SwitchLabel)bindable;

        //    var nuevoValor = (StackOrientation)newValue;
        //    control.Orientacion = nuevoValor;

        //    control.Orientation = nuevoValor;
        //}

        ///// <summary>
        ///// Si se debe cambiar el orden de la etiqueta y el switch.
        ///// </summary>
        //[Obsolete("Usar la propiedad Orientation.")]
        //public StackOrientation Orientacion
        //{
        //    get => (StackOrientation)GetValue(OrientacionProperty);
        //    set => SetValue(OrientacionProperty, value);
        //}

        ////
        //// </Cambiar la orientacion del contenido
        ////

        #endregion

        /// <summary>
        /// Si se debe mostrar el Switch.
        /// </summary>
        public bool IsSwitchVisible { get { return chkButton.IsVisible; } set { chkButton.IsVisible = value; } }
        /// <summary>
        /// El texto a mostrar.
        /// </summary>
        public string Text { get { return LabelText.Text; } set { LabelText.Text = value; } }
        /// <summary>
        /// El botón asociado al que se le cambiará el texto.
        /// </summary>
        /// <remarks>Asignarlo en el contructor de la página que contiene este control.</remarks>
        public Button Button2Text { get; set; } = null;

        /// <summary>
        /// Texto a mostrar cuando esté marcada
        /// </summary>
        public string TextToggled { get; set; }
        /// <summary>
        /// Texto a mostrar cuando está desmarcada
        /// </summary>
        public string TextUnToggled { get; set; }

        //public static readonly BindableProperty TextColorProperty =
        //    BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SwitchLabel), Color.Black,
        //        propertyChanged: OnTextColorChanged);

        //private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var control = (SwitchLabel)bindable;
        //    var nuevoValor = (Color)newValue;
        //    //if (control != null && control.LabelText != null)
        //    //{
        //    //}
        //    control.LabelText.TextColor = nuevoValor;
        //    control.labelC = nuevoValor;
        //    control.BackColorCheck();
        //}

        /// <summary>
        /// El color de la etiqueta.
        /// </summary>
        public Color TextColor
        //{
        //    get => (Color)GetValue(TextColorProperty);
        //    set => SetValue(TextColorProperty, value);
        //}
        {
            get => LabelText.TextColor;
            set
            {
                LabelText.TextColor = value;
                labelC = value;
            }
        }

        // Habilitado de forma predeterminada.
        public static new readonly BindableProperty IsEnabledProperty =
            BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(SwitchLabel), true,
                propertyChanged: OnIsEnabledChanged);

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchLabel)bindable;

            var nuevoValor = (bool)newValue;
            control.chkButton.IsEnabled = nuevoValor;
            control.LabelText.IsEnabled = nuevoValor;
            if (nuevoValor)
            {
                control.chkButton.ThumbColor = control.thumbC;
                control.LabelText.TextColor = control.labelC;
                // Usar el valor que tuviera. v1.36.0.4 (27/sep/22 11.45)
                control.LabelText.FontAttributes = control._FontAttributes;
                control.BackColorCheck();
            }
            else
            {
                control.chkButton.ThumbColor = App.GrisDeshabilitado; // Color.FromHex("#7A7A7A"); // Color.Gray;
                control.LabelText.TextColor = App.GrisDeshabilitado; // Color.FromHex("#7A7A7A"); // Color.Gray;
                control.LabelText.FontAttributes = FontAttributes.Italic;
            }
        }

        /// <summary>
        /// Habilita o deshabilita el control.
        /// </summary>
        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }
    }
}