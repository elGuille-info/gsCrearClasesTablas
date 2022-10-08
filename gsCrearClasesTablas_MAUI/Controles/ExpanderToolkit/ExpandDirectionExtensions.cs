using System;
using System.Collections.Generic;
using System.Text;

namespace gsCrearClasesTablas_MAUI.Controles.ExpanderToolkit
{
    internal static class ExpandDirectionExtensions
    {
        public static bool IsVertical(this ExpandDirection orientation)
        {
            if (orientation != 0)
            {
                return orientation == ExpandDirection.Up;
            }

            return true;
        }

        public static bool IsRegularOrder(this ExpandDirection orientation)
        {
            if (orientation != 0)
            {
                return orientation == ExpandDirection.Right;
            }

            return true;
        }
    }
}
