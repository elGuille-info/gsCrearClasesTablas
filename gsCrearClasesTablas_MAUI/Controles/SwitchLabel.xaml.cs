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
        public SwitchLabel()
        {
            InitializeComponent();
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
        
        private void ChkButton_Toggled(object sender, ToggledEventArgs e)
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

        //private FontAttributes _FontAttributes = FontAttributes.None;

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SwitchLabel), 
                FontAttributes.None,
                propertyChanged: OnFontAttributesChanged);

        private static void OnFontAttributesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (SwitchLabel)bindable;

            var nuevoValor = (FontAttributes)newValue;
            control.LabelText.FontAttributes = nuevoValor;
            control.FontAttributes = nuevoValor;
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

        /// <summary>
        /// El color de la etiqueta.
        /// </summary>
        public Color TextColor
        {
            get => LabelText.TextColor;
            set => LabelText.TextColor = value;
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