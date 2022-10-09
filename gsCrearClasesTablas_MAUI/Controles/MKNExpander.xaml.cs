using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gsCrearClasesTablas_MAUI.Controles.ExpanderToolkit;

using Microsoft.Maui.Graphics.Converters;

//using MKNReservasMobile.Controles.ExpanderToolkit;
//using Xamarin.CommunityToolkit.UI.Views;
//using Xamarin.Forms;
//using Xamarin.Forms.Xaml;

namespace gsCrearClasesTablas_MAUI.Controles
{
    public delegate void ExpandedDelegate(object sender, bool isExpanded);

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MKNExpander : Expander
    {
        public MKNExpander()
        {
            InitializeComponent();

            AsignarImagenExpander();
        }
        /// <summary>
        /// Evento para cuando está expandida o contraída.
        /// </summary>
        public event ExpandedDelegate Expanded;

        /// <summary>
        /// Refrescar el contenido.
        /// </summary>
        /// <param name="relanzarEvento">True para lanzar el evento (antes predeterminado = true).</param>
        /// <param name="asignarColores">Si se deben asignar los colores del fondo y texto.</param>
        /// <remarks>
        /// Llamar a este método desde el evento ContentPage_Appearing.
        /// Con la versión 1.9.0.89 del 20/Oct/21 pongo que el primer parámetro no sea opcional y le añado el segundo 
        /// (por ahora no opcionales para que me muestre dónde se hace la llamada a Refrescar)
        /// </remarks>
        public void Refrescar(bool relanzarEvento, bool asignarColores)
        {
            // Solo asignar los colores si se indica expresamente. v1.9.0.89 (20/Oct/21)
            if (asignarColores)
            {
                grbHeader.BackgroundColor = HeaderBackgroundColor;
                LabelHeader.TextColor = HeaderTextColor;
            }
            if (IsExpanded)
                LabelHeader.Text = HeaderTitleExpanded;
            else
                LabelHeader.Text = HeaderTitleCollapsed;

            AsignarImagenExpander();

            if (relanzarEvento)
                Expanded?.Invoke(this, IsExpanded);
        }

        /// <summary>
        /// Si se muestra las imágenes cuando está expandido o contraído.
        /// </summary>
        public bool MostrarExpandImage //=> imgExpander.IsVisible;
        {
            get { return ImgExpander.IsVisible; }
            set { ImgExpander.IsVisible = value; }
        }

        // Para usar una de las dos imágenes, la blanca o la negra. v1.10.28.6 (02/sep/22 21.01)

        /// <summary>
        /// Asignar la imagen del expander según esté expandido o no y según sea ImagenBlanca o no.
        /// </summary>
        public void AsignarImagenExpander()
        {
            // Si está expandido hay que mostrar collapse y al revés. v1.10.28.6 (02/sep/22 22.11)
            string imgSource;
            if (IsExpanded)
            {
                if (ImagenBlanca)
                {
                    imgSource = "collapse_white.png";
                }
                else
                {
                    imgSource = "collapse.png";
                }
            }
            else
            {
                if (ImagenBlanca)
                {
                    imgSource = "expand_white.png";
                }
                else
                {
                    imgSource = "expand.png";
                }
            }
            //ImgExpander.Source = FileImageSource.FromResource($"gsCrearClasesTablas_MAUI/Resources/Images/{imgSource}", typeof(MKNExpander).Assembly);
            //ImgExpander.Source = FileImageSource.FromResource($"{imgSource}", typeof(MKNExpander).Assembly);
            ImgExpander.Source = FileImageSource.FromFile($"{imgSource}");//, typeof(MKNExpander).Assembly);
        }

        private bool _ImagenBlanca = false;
        /// <summary>
        /// Si se usa la imagen blanca o la negra.
        /// </summary>
        public bool ImagenBlanca //{ get; set; }
        {
            get => _ImagenBlanca;
            set
            {
                _ImagenBlanca = value;
                AsignarImagenExpander();
            }
        }

        public static readonly BindableProperty HeaderTextColorProperty =
            BindableProperty.Create(nameof(HeaderTextColor), typeof(Color), typeof(MKNExpander), Colors.Navy,
                propertyChanged: OnHeaderTextColorChanged);

        private static void OnHeaderTextColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MKNExpander)bindable;
            control.LabelHeader.TextColor = (Color)newValue;
        }

        /// <summary>
        /// El color del texto de la etiqueta de cabecera.
        /// </summary>
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color HeaderTextColor
        {
            get => (Color)GetValue(HeaderTextColorProperty);
            set => SetValue(HeaderTextColorProperty, value);
        }

        public static readonly BindableProperty HeaderBackgroundColorProperty =
            BindableProperty.Create(nameof(HeaderBackgroundColor), typeof(Color), typeof(MKNExpander), Colors.LightSkyBlue,
                propertyChanged: OnHeaderBackgroundColorChanged);

        private static void OnHeaderBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MKNExpander)bindable;
            control.grbHeader.BackgroundColor = (Color)newValue;
        }

        /// <summary>
        /// El color de fondo de la cabecera.
        /// </summary>
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color HeaderBackgroundColor
        {
            get => (Color)GetValue(HeaderBackgroundColorProperty);
            set => SetValue(HeaderBackgroundColorProperty, value);
        }

        /// <summary>
        /// El título de la cabecera.
        /// </summary>
        /// <remarks>
        /// Si no están asignadas las propiedades de cuando está expandida o no, 
        /// se asignan con este título de la cabecera.
        /// </remarks>
        public string HeaderTitle
        {
            get { return LabelHeader.Text; }
            set
            {
                LabelHeader.Text = value;
                if (string.IsNullOrWhiteSpace(HeaderTitleExpanded))
                    HeaderTitleExpanded = value;
                if (string.IsNullOrWhiteSpace(HeaderTitleCollapsed))
                    HeaderTitleCollapsed = value;
            }
        }

        /// <summary>
        /// Asigna el texto a la cabecera y los valores para cuando está o no expandido.
        /// </summary>
        public string TextTodos
        {
            get { return LabelHeader.Text; }
            set
            {
                LabelHeader.Text = value;
                HeaderTitleExpanded = value;
                HeaderTitleCollapsed = value;
            }
        }

        /// <summary>
        /// El título de la cabecera (lo mismo que HeaderTitle).
        /// </summary>
        public string Text
        {
            get { return LabelHeader.Text; }
            set
            {
                LabelHeader.Text = value;
                if (string.IsNullOrWhiteSpace(HeaderTitleExpanded))
                    HeaderTitleExpanded = value;
                if (string.IsNullOrWhiteSpace(HeaderTitleCollapsed))
                    HeaderTitleCollapsed = value;
            }
        }

        public double HeaderHeightRequest
        {
            get { return LabelHeader.HeightRequest; }
            set { LabelHeader.HeightRequest = value; }
        }

        [TypeConverter(typeof(TextAlignmentConverter))]
        public TextAlignment HeaderHorizontalTextAlignment
        {
            get { return LabelHeader.HorizontalTextAlignment; }
            set { LabelHeader.HorizontalTextAlignment = value; }
        }

        [TypeConverter(typeof(LayoutOptionsConverter))]
        public LayoutOptions HeaderHorizontalOptions
        {
            get { return LabelHeader.HorizontalOptions; }
            set { LabelHeader.HorizontalOptions = value; }
        }

        [TypeConverter(typeof(LayoutOptionsConverter))]
        public LayoutOptions HeaderVerticalOptions
        {
            get { return LabelHeader.VerticalOptions; }
            set { LabelHeader.VerticalOptions = value; }
        }

        /// <summary>
        /// El tamaño de la letra de la cabecera.
        /// </summary>
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get { return LabelHeader.FontSize; }
            set { LabelHeader.FontSize = value; }
        }

        /// <summary>
        /// Los atributos del texto de la cabecera (Bold, Italic, None).
        /// </summary>
        [TypeConverter(typeof(FontAttributesConverter))]
        public FontAttributes FontAttibute
        {
            get { return LabelHeader.FontAttributes; }
            set { LabelHeader.FontAttributes = value; }
        }

        /// <summary>
        /// El color del texto de la cabecera.
        /// </summary>
        [TypeConverter(typeof(ColorTypeConverter))]
        public Color TextColor
        {
            get { return HeaderTextColor; }
            set { LabelHeader.TextColor = value; }
        }

        /// <summary>
        /// El título cuando está expandida.
        /// </summary>
        public string HeaderTitleExpanded { get; set; }

        /// <summary>
        /// El título cuando está contraída.
        /// </summary>
        /// <remarks>28-sep: v1.36.1.10</remarks>
        public string HeaderTitleCollapsed { get; set; }

        /// <summary>
        /// El título cuando está contraída.
        /// </summary>
        [Obsolete("Usar HeaderTitleCollapsed")]
        public string HeaderTitleNoExpanded
        { 
            get => HeaderTitleCollapsed; 
            set => HeaderTitleCollapsed = value; 
        }

        private void Expander_Tapped(object sender, EventArgs e)
        {
            Refrescar(relanzarEvento: true, asignarColores: true);
        }
    }
}
