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
        /// <summary>
        /// Cambiar el color del CheckBox según el valor de isChecked (boolean).
        /// </summary>
        /// <param name="chk"></param>
        /// <param name="isChecked"></param>
        /// <param name="usarColorUnCheked">Si se debe usar el nuevo color para cuando no está seleccionado</param>
        /// <remarks>Usado desde CheckLabel.</remarks>
        public static void BackColorCheck(CheckBox chk, bool isChecked, bool usarColorUnCheked = false)
        {
            //if (comprobarEnabled && chk.IsEnabled == false)
            //    return;

            if (isChecked)
            {
                if (Application.Current.RequestedTheme == AppTheme.Light)
                {
                    if (App.DevicePlatform == DevicePlatform.Android)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchAndroid"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchiOS"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        chk.Color = (Color)Application.Current.Resources["CheckUWP"];
                    }
                    else
                    {
                        chk.Color = (Color)Application.Current.Resources["ColorBlanco"];
                    }
                }
                else
                {
                    if (App.DevicePlatform == DevicePlatform.Android)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchAndroid"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchiOS"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        chk.Color = (Color)Application.Current.Resources["CheckUWP"];
                    }
                    else
                    {
                        chk.Color = (Color)Application.Current.Resources["ColorAzul1"];
                    }
                }
            }
            else // UnChecked)
            {
                chk.IsChecked = false;

                if (usarColorUnCheked)
                {
                    if (App.DevicePlatform == DevicePlatform.Android)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchAndroid"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.iOS)
                    {
                        chk.Color = (Color)Application.Current.Resources["SwitchiOS"];
                    }
                    else if (DeviceInfo.Platform == DevicePlatform.WinUI)
                    {
                        chk.Color = (Color)Application.Current.Resources["CheckUWP"];
                    }
                }
                else
                {
                    // Tenía puesto BackColor en vez de Color. v1.20.0.15 (16/sep/22 23.00)
                    chk.Color = App.GrisDeshabilitado; // (Color)Application.Current.Resources["GrisDeshabilitado"];  //Color.FromHex("#7A7A7A"); // Color.Gray;
                }
            }
        }
    }
}
