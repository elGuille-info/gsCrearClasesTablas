using System;
using System.Collections.Generic;
using System.Text;

//using Xamarin.Forms;

namespace gsCrearClasesTablas_MAUI.Controles.ExpanderToolkit
{
    //
    // Summary:
    //     Abstract class that templated views should inherit
    //
    // Type parameters:
    //   TControl:
    //     The type of the control that this template will be used for
    public abstract class BaseTemplatedView<TControl> : TemplatedView where TControl : View, new()
    {
        protected TControl Control { get; private set; }

        //
        // Summary:
        //     Constructor of Xamarin.CommunityToolkit.UI.Views.Internals.BaseTemplatedView`1
        public BaseTemplatedView()
        {
            base.ControlTemplate = new ControlTemplate(typeof(TControl));
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Control != null)
            {
                Control!.BindingContext = base.BindingContext;
            }
        }

        protected override void OnChildAdded(Element child)
        {
            if (Control == null)
            {
                TControl val = child as TControl;
                if (val != null)
                {
                    Control = val;
                    OnControlInitialized(Control);
                }
            }

            base.OnChildAdded(child);
        }

        protected abstract void OnControlInitialized(TControl control);
    }
}
