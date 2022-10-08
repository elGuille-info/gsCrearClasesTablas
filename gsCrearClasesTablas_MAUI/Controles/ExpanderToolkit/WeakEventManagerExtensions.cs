using System;
using System.Collections.Generic;
using System.Text;

//using Xamarin.Forms;

namespace gsCrearClasesTablas_MAUI.Controles.ExpanderToolkit
{
    //
    // Summary:
    //     Extensions for Xamarin.Forms.WeakEventManager
    public static class WeakEventManagerExtensions
    {
        //
        // Summary:
        //     Invokes the event EventHandler
        //
        // Parameters:
        //   weakEventManager:
        //     WeakEventManager
        //
        //   sender:
        //     Sender
        //
        //   eventArgs:
        //     Event arguments
        //
        //   eventName:
        //     Event name
        public static void RaiseEvent(this WeakEventManager weakEventManager, object sender, object eventArgs, string eventName)
        {
            if (weakEventManager == null)
            {
                throw new ArgumentNullException("weakEventManager");
            }

            weakEventManager.HandleEvent(sender, eventArgs, eventName);
        }
    }
}
