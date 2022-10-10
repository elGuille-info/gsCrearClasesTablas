//
// Clase para usar un CheckBox con etiqueta.                   (08/oct/22 15.20)
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.Converters;
using Microsoft.Maui.Controls;

namespace gsCrearClasesTablas_MAUI.Controles
{
    /// <summary>
    /// Una etiqueta y un CheckBox.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckLabel : StackLayout
    {
        private Color labelC;
        private readonly Color thumbC;

        public CheckLabel()
        {
            InitializeComponent();

            thumbC = chkButton.Color;
            labelC = LabelText.TextColor;

            // Para usar los nuevos colores de Android. (02/sep/22 19.55)
            //if (App.DevicePlatform == DevicePlatform.Android)
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                labelC = (Color)Application.Current.Resources["SwitchAndroid"];
                //thumbC = (Color)Application.Current.Resources["SwitchAndroidThumb"];
                thumbC = (Color)Application.Current.Resources["SwitchAndroid"];
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                thumbC = (Color)Application.Current.Resources["SwitchiOS"];
            }
            else if (DeviceInfo.Platform == DevicePlatform.WinUI)
            {
                thumbC = (Color)Application.Current.Resources["CheckUWP"];
            }
            BackColorCheck();
        }

        /// <summary>
        /// Evento para cuando cambia el estado de IsChecked.
        /// </summary>
        public event EventHandler<CheckedChangedEventArgs> CheckedChanged;

        /// <summary>
        /// Evento por compatibilidad.
        /// </summary>
        [Obsolete("Por compatibilidad con SwitchLabel, se debe usar CheckedChanged")]
        public event EventHandler<ToggledEventArgs> Toggled;

        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(CheckLabel), null,
                defaultBindingMode: BindingMode.OneWay, propertyChanged: OnIsCheckedChanged);

        private static void OnIsCheckedChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckLabel)bindable;

            if (control.yaEstoy) return;
            control.yaEstoy = true;

            var anterior = (bool)oldValue;
            var value = (bool)newValue;
            // Esto solo si ha cambiado el valor, si no... ¿pa qué?
            if (anterior != value)
            {
                control.LabelText.Text = (value ? control.TextChecked : control.TextUnChecked) ?? control.LabelText.Text;

                control.chkButton.IsChecked = value;
                control.CheckedChanged?.Invoke(control, new CheckedChangedEventArgs(value));
                // Por compatibilidad.
                control.Toggled?.Invoke(control, new ToggledEventArgs(value));
            }
            control.BackColorCheck();

            control.yaEstoy = false;
        }

        protected bool yaEstoy = false;

        /// <summary>
        /// El valor del estado del control de tipo boolean.
        /// </summary>
        /// <remarks>Si están asignados los valores de TextCheked o TextUnChecked, se asignan.</remarks>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        // Para compatibilidad con switchLabel. v1.31.0.16 (22/sep/22 16.12)

        /// <summary>
        /// El valor del estado del control de tipo boolean.
        /// </summary>
        /// <remarks>Esta propiedad es por compatibilidad con SwitchLabel, pero debería usarse IsChecked.</remarks>
        [Obsolete("En este control es preferible usar IsChecked.")]
        public bool IsToggled
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            // Si no está habilitado, no permitir hacer click. v1.35.0.2 (26/sep/22 06.05)
            if (IsEnabled == false) return;

            IsChecked = !chkButton.IsChecked;
        }

        private void chkButton_Checked(object sender, CheckedChangedEventArgs e)
        {
            // Si no está habilitado, no permitir hacer click. v1.35.0.2 (26/sep/22 06.05)
            if (IsEnabled == false) return;

            IsChecked = e.Value;
        }

        /// <summary>
        /// Si se debe usar un color en vez del gris cuando está UnChecked.
        /// </summary>
        public bool UsarColorUnCheked { get; set; } = false;

        //
        // Cambiar el orden de la etiqueta y el checkbox
        //
        // El CheckBox estará a la izquierda de forma predeterminada. v1.34.0.3 (23/sep/22 19.22)
        public static readonly BindableProperty EtiquetaAlPrincipioProperty =
            BindableProperty.Create(nameof(EtiquetaAlPrincipio), typeof(bool), typeof(CheckLabel), false,
                propertyChanged: OnEtiquetaAlPrincipioChanged);

        private static void OnEtiquetaAlPrincipioChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckLabel)bindable;

            var nuevoValor = (bool)newValue;
            control.EtiquetaAlPrincipio = nuevoValor;

            var etiqueta = control.LabelText;
            var boton = control.chkButton;
            control.Children.Clear();
            if (nuevoValor)
            {
                control.Children.Add(etiqueta);
                control.Children.Add(boton);
                boton.Margin = new Thickness(0, 0, -6, 0);
                etiqueta.Margin = new Thickness(0, 0, 6, 0);
            }
            else
            {
                control.Children.Add(boton);
                control.Children.Add(etiqueta);
                // Si es android y tablet, asignar estos valores v1.4.17.48 (03/Ago/21)
                // comprobaré si en el teléfono también pasa lo mismo
                if (DeviceInfo.Platform == DevicePlatform.Android)
                {
                    etiqueta.Margin = new Thickness(0, 0, 6, 0);
                }
                else
                {
                    //etiqueta.Margin = new Thickness(-6, 0, 6, 0);
                    etiqueta.Margin = new Thickness(0, 0, 6, 0);
                }
                // no lo había quitado de aquí v1.4.17.50
                //etiqueta.Margin = new Thickness(-6, 0, 6, 0);
                boton.Margin = new Thickness(0, 0, -6, 0);
            }
        }

        /// <summary>
        /// Si se debe cambiar el orden de la etiqueta y el checkBox.
        /// </summary>
        /// <remarks>De forma predeterminada, la etiqueta está después del checkBox.</remarks>
        public bool EtiquetaAlPrincipio
        {
            get => (bool)GetValue(EtiquetaAlPrincipioProperty);
            set => SetValue(EtiquetaAlPrincipioProperty, value);
        }

        //
        // </Cambiar el orden de la etiqueta y el checkbox
        //



        // Para poder cambiar los márgenes de la etiqueta y el checkbox v1.4.17.49

        /// <summary>
        /// El marge del CheckBox.
        /// </summary>
        [TypeConverter(typeof(ThicknessTypeConverter))]
        public Thickness MargenCheckBox { get => chkButton.Margin; set => chkButton.Margin = value; }

        /// <summary>
        /// El margen de la etiqueta.
        /// </summary>
        [TypeConverter(typeof(ThicknessTypeConverter))]
        public Thickness MargenEtiqueta { get => LabelText.Margin; set => LabelText.Margin = value; }

        /// <summary>
        /// El tamaño de la letra de la etiqueta.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize { get { return LabelText.FontSize; } set { LabelText.FontSize = value; } }

        private FontAttributes _FontAttributes = FontAttributes.None;

        // Si se usa en <Style hay que definirlo así. 
        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(CheckLabel), 
                FontAttributes.None,
                propertyChanged: OnFontAttributesChanged);

        private static void OnFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckLabel)bindable;

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
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }

        /// <summary>
        /// El texto a mostrar (en la etiqueta).
        /// </summary>
        public string Text { get { return LabelText.Text; } set { LabelText.Text = value; } }

        /// <summary>
        /// Texto a mostrar cuando esté seleccionada.
        /// </summary>
        public string TextChecked { get; set; } = default; // El valor predeterminado de string es null.
        /// <summary>
        /// Texto a mostrar cuando no esté seleccionada.
        /// </summary>
        public string TextUnChecked { get; set; } = default; // El valor predeterminado de string es null.

        ///// <summary>
        ///// El botón asociado al que se le cambiará el texto.
        ///// </summary>
        ///// <remarks>Asignarlo en el contructor de la página que contiene este control.</remarks>
        //public Button Button2Text { get; set; } = null;


        /// <summary>
        /// El color de la etiqueta.
        /// </summary>
        public Color TextColor
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
            BindableProperty.Create(nameof(IsEnabled), typeof(bool), typeof(CheckLabel), true,
                propertyChanged: OnIsEnabledChanged);

        private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (CheckLabel)bindable;

            var nuevoValor = (bool)newValue;
            //if (nuevoValor)
            //{
            //    control.LabelText.TextColor = control.labelC;
            //    // Usar el valor que tuviera. v1.36.0.4 (27/sep/22 11.33)
            //    control.LabelText.FontAttributes = control._FontAttributes;
            //    //control.BackColorCheck();
            //}
            //else
            //{
            //    // Usar el color grisáceo (#7A7A7A) en todos los controles, no Gray. v1.30.0.2 (20/sep/22 07.31)
            //    control.chkButton.Color = App.GrisDeshabilitado;
            //    control.LabelText.TextColor = App.GrisDeshabilitado;
            //    control.LabelText.FontAttributes = FontAttributes.Italic;
            //}
            control.chkButton.IsEnabled = nuevoValor;
            control.LabelText.IsEnabled = nuevoValor;
            control.BackColorCheck();
        }

        /// <summary>
        /// Habilita o deshabilita el control.
        /// </summary>
        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public void BackColorCheck()
        {
            // Cambiar el color solo si está habilitado.
            //if (IsEnabled)
            //{
            //    // No comprobar si está habilitado, colorear de todas formas.
            //    Funciones.BackColorCheck(chkButton, IsChecked, UsarColorUnCheked);
            //}

            // Usar el valor original que será el de Android o el predeterminado del control.
            if (chkButton.IsEnabled)
            {
                LabelText.TextColor = labelC;
                chkButton.Color = thumbC;
                LabelText.FontAttributes = _FontAttributes;
            }
            else
            {
                LabelText.TextColor = Funciones.GrisDeshabilitado;
                chkButton.Color = Funciones.GrisDeshabilitado;
                LabelText.FontAttributes = FontAttributes.Italic;
            }
        }
    }
}