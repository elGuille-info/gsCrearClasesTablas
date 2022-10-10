//
// Funciones de apoyo para los controles.                    (08/oct/22 15.20)
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maui.ApplicationModel;

namespace gsCrearClasesTablas_MAUI.Controles
{
    public static class Funciones
    {
        // Esto solo se asigna una vez.

        private static Color _GrisDeshabilitadoClaro = (Color)Application.Current.Resources["GrisDeshabilitadoClaro"];

        /// <summary>
        /// El color gris deshabilitado más claro.
        /// </summary>
        public static Color GrisDeshabilitadoClaro { get => _GrisDeshabilitadoClaro; }


        private static Color _GrisDeshabilitado = (Color)Application.Current.Resources["GrisDeshabilitado"];
        /// <summary>
        /// El color gris para usar como deshabilitado (#7A7A7A).
        /// </summary>
        public static Color GrisDeshabilitado { get => _GrisDeshabilitado; }
    }
}
