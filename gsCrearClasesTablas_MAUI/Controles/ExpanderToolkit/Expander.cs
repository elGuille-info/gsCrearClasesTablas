//#region Assembly Xamarin.CommunityToolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
//// C:\Users\Guille\.nuget\packages\xamarin.communitytoolkit\1.3.0\lib\netstandard2.0\Xamarin.CommunityToolkit.dll
//// Decompiled with ICSharpCode.Decompiler 7.1.0.6543
//#endregion

using System;
using System.Windows.Input;

//using Xamarin.CommunityToolkit.Helpers;
//using Xamarin.CommunityToolkit.UI.Views.Internals;
//using Xamarin.Forms;

namespace gsCrearClasesTablas_MAUI.Controles.ExpanderToolkit
{
    [ContentProperty("Content")]
    public class Expander : BaseTemplatedView<StackLayout>
    {
        private const string expandAnimationName = "expandAnimationName";

        private const uint defaultAnimationLength = 250u;

        private readonly WeakEventManager tappedEventManager = new WeakEventManager();

        private ContentView contentHolder;

        private GestureRecognizer headerTapGestureRecognizer;

        private DataTemplate previousTemplate;

        private double lastVisibleSize = -1.0;

        private Size previousSize = new Size(-1.0, -1.0);

        private bool shouldIgnoreContentSetting;

        private readonly object contentSetLocker = new object();

        public static readonly BindableProperty HeaderProperty = BindableProperty.Create("Header", typeof(View), typeof(Expander), null, BindingMode.OneWay, null, OnHeaderPropertyChanged);

        public static readonly BindableProperty ContentProperty = BindableProperty.Create("Content", typeof(View), typeof(Expander), null, BindingMode.OneWay, null, OnContentPropertyChanged);

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create("ContentTemplate", typeof(DataTemplate), typeof(Expander), null, BindingMode.OneWay, null, OnContentTemplatePropertyChanged);

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create("IsExpanded", typeof(bool), typeof(Expander), false, BindingMode.TwoWay, null, OnIsExpandedPropertyChanged);

        public static readonly BindableProperty DirectionProperty = BindableProperty.Create("Direction", typeof(ExpandDirection), typeof(Expander), ExpandDirection.Down, BindingMode.OneWay, null, OnDirectionPropertyChanged);

        public static readonly BindableProperty TouchCaptureViewProperty = BindableProperty.Create("TouchCaptureView", typeof(View), typeof(Expander), null, BindingMode.OneWay, null, OnTouchCaptureViewPropertyChanged);

        public static readonly BindableProperty AnimationLengthProperty = BindableProperty.Create("AnimationLength", typeof(uint), typeof(Expander), 250u);

        public static readonly BindableProperty ExpandAnimationLengthProperty = BindableProperty.Create("ExpandAnimationLength", typeof(uint), typeof(Expander), uint.MaxValue);

        public static readonly BindableProperty CollapseAnimationLengthProperty = BindableProperty.Create("CollapseAnimationLength", typeof(uint), typeof(Expander), uint.MaxValue);

        public static readonly BindableProperty AnimationEasingProperty = BindableProperty.Create("AnimationEasing", typeof(Easing), typeof(Expander));

        public static readonly BindableProperty ExpandAnimationEasingProperty = BindableProperty.Create("ExpandAnimationEasing", typeof(Easing), typeof(Expander));

        public static readonly BindableProperty CollapseAnimationEasingProperty = BindableProperty.Create("CollapseAnimationEasing", typeof(Easing), typeof(Expander));

        public static readonly BindableProperty StateProperty = BindableProperty.Create("State", typeof(ExpandState), typeof(Expander), ExpandState.Collapsed, BindingMode.OneWayToSource);

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create("CommandParameter", typeof(object), typeof(Expander));

        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(Expander));

        public static readonly BindableProperty ForceUpdateSizeCommandProperty = BindableProperty.Create("ForceUpdateSizeCommand", typeof(ICommand), typeof(Expander), null, BindingMode.OneWayToSource, null, null, null, null, GetDefaultForceUpdateSizeCommand);

        private double Size
        {
            get
            {
                if (!Direction.IsVertical())
                {
                    return base.Width;
                }

                return base.Height;
            }
        }

        private double ContentSize
        {
            get
            {
                if (!Direction.IsVertical())
                {
                    return (contentHolder ?? throw new NullReferenceException())!.Width;
                }

                return (contentHolder ?? throw new NullReferenceException())!.Height;
            }
        }

        private double ContentSizeRequest
        {
            get
            {
                double num = (Direction.IsVertical() ? Content.HeightRequest : Content.WidthRequest);
                if (!(num < 0.0))
                {
                    Layout layout = Content as Layout;
                    if (layout != null)
                    {
                        return num + (Direction.IsVertical() ? layout.Padding.VerticalThickness : layout.Padding.HorizontalThickness);
                    }
                }

                return num;
            }
            set
            {
                if (contentHolder == null)
                {
                    throw new NullReferenceException();
                }

                if (Direction.IsVertical())
                {
                    contentHolder!.HeightRequest = value;
                }
                else
                {
                    contentHolder!.WidthRequest = value;
                }
            }
        }

        private double MeasuredContentSize
        {
            get
            {
                if (!Direction.IsVertical())
                {
                    return (contentHolder ?? throw new NullReferenceException())!.Measure(double.PositiveInfinity, base.Height).Request.Width;
                }

                return (contentHolder ?? throw new NullReferenceException())!.Measure(base.Width, double.PositiveInfinity).Request.Height;
            }
        }

        public View Header
        {
            get
            {
                return (View)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }

        public View Content
        {
            get
            {
                return (View)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(ContentTemplateProperty);
            }
            set
            {
                SetValue(ContentTemplateProperty, value);
            }
        }

        public bool IsExpanded
        {
            get
            {
                return (bool)GetValue(IsExpandedProperty);
            }
            set
            {
                SetValue(IsExpandedProperty, value);
            }
        }

        public ExpandDirection Direction
        {
            get
            {
                return (ExpandDirection)GetValue(DirectionProperty);
            }
            set
            {
                SetValue(DirectionProperty, value);
            }
        }

        public View TouchCaptureView
        {
            get
            {
                return (View)GetValue(TouchCaptureViewProperty);
            }
            set
            {
                SetValue(TouchCaptureViewProperty, value);
            }
        }

        public uint AnimationLength
        {
            get
            {
                return (uint)GetValue(AnimationLengthProperty);
            }
            set
            {
                SetValue(AnimationLengthProperty, value);
            }
        }

        public uint ExpandAnimationLength
        {
            get
            {
                return (uint)GetValue(ExpandAnimationLengthProperty);
            }
            set
            {
                SetValue(ExpandAnimationLengthProperty, value);
            }
        }

        public uint CollapseAnimationLength
        {
            get
            {
                return (uint)GetValue(CollapseAnimationLengthProperty);
            }
            set
            {
                SetValue(CollapseAnimationLengthProperty, value);
            }
        }

        public Easing AnimationEasing
        {
            get
            {
                return (Easing)GetValue(AnimationEasingProperty);
            }
            set
            {
                SetValue(AnimationEasingProperty, value);
            }
        }

        public Easing ExpandAnimationEasing
        {
            get
            {
                return (Easing)GetValue(ExpandAnimationEasingProperty);
            }
            set
            {
                SetValue(ExpandAnimationEasingProperty, value);
            }
        }

        public Easing CollapseAnimationEasing
        {
            get
            {
                return (Easing)GetValue(CollapseAnimationEasingProperty);
            }
            set
            {
                SetValue(CollapseAnimationEasingProperty, value);
            }
        }

        public ExpandState State
        {
            get
            {
                return (ExpandState)GetValue(StateProperty);
            }
            set
            {
                SetValue(StateProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public ICommand ForceUpdateSizeCommand
        {
            get
            {
                return (ICommand)GetValue(ForceUpdateSizeCommandProperty);
            }
            set
            {
                SetValue(ForceUpdateSizeCommandProperty, value);
            }
        }

        public event EventHandler Tapped
        {
            add
            {
                tappedEventManager.AddEventHandler(value, "Tapped");
            }
            remove
            {
                tappedEventManager.RemoveEventHandler(value, "Tapped");
            }
        }

        public void ForceUpdateSize()
        {
            lastVisibleSize = -1.0;
            OnIsExpandedChanged();
        }

        protected override void OnControlInitialized(StackLayout control)
        {
            ForceUpdateSizeCommand = new Command(ForceUpdateSize);
            headerTapGestureRecognizer = new TapGestureRecognizer
            {
                CommandParameter = this,
                Command = new Command(delegate (object parameter)
                {
                    Element parent = (parameter as View).Parent;
                    while (parent != null && !(parent is Page))
                    {
                        //Expander expander = (Expander)parent;
                        Expander expander = parent as Expander;
                        if (expander != null)
                        {
                            expander.ContentSizeRequest = -1.0;
                        }

                        parent = parent.Parent;
                    }

                    IsExpanded = !IsExpanded;
                    Command?.Execute(CommandParameter);
                    OnTapped();
                })
            };
            control.Spacing = 0.0;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            lastVisibleSize = -1.0;
            SetContent(isForceUpdate: true, shouldIgnoreAnimation: true);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if ((Math.Abs(width - previousSize.Width) >= double.Epsilon && Direction.IsVertical()) || (Math.Abs(height - previousSize.Height) >= double.Epsilon && !Direction.IsVertical()))
            {
                ForceUpdateSize();
            }

            previousSize = new Size(width, height);
        }

        private static void OnHeaderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnHeaderPropertyChanged((View)oldValue);
        }

        private static void OnContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnContentPropertyChanged();
        }

        private static void OnContentTemplatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnContentTemplatePropertyChanged();
        }

        private static void OnIsExpandedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnIsExpandedPropertyChanged();
        }

        private static void OnDirectionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnDirectionPropertyChanged((ExpandDirection)oldValue);
        }

        private static void OnTouchCaptureViewPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((Expander)bindable).OnTouchCaptureViewPropertyChanged((View)oldValue);
        }

        private static object GetDefaultForceUpdateSizeCommand(BindableObject bindable)
        {
            return new Command(((Expander)bindable).ForceUpdateSize);
        }

        private void OnHeaderPropertyChanged(View oldView)
        {
            SetHeader(oldView);
        }

        private void OnContentPropertyChanged()
        {
            SetContent();
        }

        private void OnContentTemplatePropertyChanged()
        {
            SetContent(isForceUpdate: true);
        }

        private void OnIsExpandedPropertyChanged()
        {
            SetContent(isForceUpdate: false);
        }

        private void OnDirectionPropertyChanged(ExpandDirection oldDirection)
        {
            SetDirection(oldDirection);
        }

        private void OnTouchCaptureViewPropertyChanged(View oldView)
        {
            SetTouchCaptureView(oldView);
        }

        private void OnIsExpandedChanged(bool shouldIgnoreAnimation = false)
        {
            if (contentHolder == null || (!IsExpanded && !contentHolder!.IsVisible))
            {
                return;
            }

            bool flag = contentHolder.AnimationIsRunning("expandAnimationName");
            contentHolder.AbortAnimation("expandAnimationName");
            double startSize = (contentHolder!.IsVisible ? Math.Max(ContentSize, 0.0) : 0.0);
            if (IsExpanded)
            {
                contentHolder!.IsVisible = true;
            }

            double num = ((ContentSizeRequest >= 0.0) ? ContentSizeRequest : lastVisibleSize);
            if (IsExpanded)
            {
                if (num <= 0.0)
                {
                    ContentSizeRequest = -1.0;
                    num = MeasuredContentSize;
                    ContentSizeRequest = 0.0;
                }
            }
            else
            {
                startSize = (lastVisibleSize = ((ContentSizeRequest >= 0.0) ? ContentSizeRequest : ((!flag) ? ContentSize : lastVisibleSize)));
                num = 0.0;
            }

            InvokeAnimation(startSize, num, shouldIgnoreAnimation);
        }

        private void SetHeader(View oldHeader)
        {
            if (oldHeader != null)
            {
                base.Control?.Children.Remove(oldHeader);
            }

            if (Header != null)
            {
                if (Direction.IsRegularOrder())
                {
                    base.Control?.Children.Insert(0, Header);
                }
                else
                {
                    base.Control?.Children.Add(Header);
                }
            }

            SetTouchCaptureView(oldHeader);
        }

        private void SetContent(bool isForceUpdate, bool shouldIgnoreAnimation = false, bool isForceContentReset = false)
        {
            if (IsExpanded && (Content == null || isForceUpdate || isForceContentReset))
            {
                lock (contentSetLocker)
                {
                    shouldIgnoreContentSetting = true;
                    View view = CreateContent();
                    if (view != null)
                    {
                        Content = view;
                    }
                    else if (isForceContentReset)
                    {
                        SetContent();
                    }

                    shouldIgnoreContentSetting = false;
                }
            }

            OnIsExpandedChanged(shouldIgnoreAnimation);
        }

        private void SetContent()
        {
            if (contentHolder != null)
            {
                contentHolder.AbortAnimation("expandAnimationName");
                base.Control?.Children.Remove(contentHolder);
                contentHolder = null;
            }

            if (Content != null)
            {
                contentHolder = new ContentView
                {
                    IsClippedToBounds = true,
                    IsVisible = false,
                    Content = Content
                };
                ContentSizeRequest = 0.0;
                if (Direction.IsRegularOrder())
                {
                    base.Control?.Children.Add(contentHolder);
                }
                else
                {
                    base.Control?.Children.Insert(0, contentHolder);
                }
            }

            if (!shouldIgnoreContentSetting)
            {
                SetContent(isForceUpdate: true);
            }
        }

        private View CreateContent()
        {
            DataTemplate dataTemplate = ContentTemplate;
            while (true)
            {
                DataTemplateSelector dataTemplateSelector = dataTemplate as DataTemplateSelector;
                if (dataTemplateSelector == null)
                {
                    break;
                }

                dataTemplate = dataTemplateSelector.SelectTemplate(base.BindingContext, this);
            }

            if (dataTemplate == previousTemplate && Content != null)
            {
                return null;
            }

            previousTemplate = dataTemplate;
            return (dataTemplate?.CreateContent()) as View;
        }

        private void SetDirection(ExpandDirection oldDirection)
        {
            if (oldDirection.IsVertical() == Direction.IsVertical())
            {
                SetHeader(Header);
                return;
            }

            if (base.Control != null)
            {
                base.Control!.Orientation = ((!Direction.IsVertical()) ? StackOrientation.Horizontal : StackOrientation.Vertical);
            }

            lastVisibleSize = -1.0;
            SetHeader(Header);
            SetContent(isForceUpdate: true, shouldIgnoreAnimation: true, isForceContentReset: true);
        }

        private void SetTouchCaptureView(View oldView)
        {
            oldView?.GestureRecognizers.Remove(headerTapGestureRecognizer);
            TouchCaptureView?.GestureRecognizers?.Remove(headerTapGestureRecognizer);
            Header?.GestureRecognizers.Remove(headerTapGestureRecognizer);
            (TouchCaptureView ?? Header)?.GestureRecognizers.Add(headerTapGestureRecognizer);
        }

        private void InvokeAnimation(double startSize, double endSize, bool shouldIgnoreAnimation)
        {
            State = ((!IsExpanded) ? ExpandState.Collapsing : ExpandState.Expanding);
            if (shouldIgnoreAnimation || Size < 0.0)
            {
                State = (IsExpanded ? ExpandState.Expanded : ExpandState.Collapsed);
                ContentSizeRequest = endSize;
                if (contentHolder == null)
                {
                    throw new NullReferenceException();
                }

                contentHolder!.IsVisible = IsExpanded;
                return;
            }

            uint num = CollapseAnimationLength;
            Easing easing = CollapseAnimationEasing;
            if (IsExpanded)
            {
                num = ExpandAnimationLength;
                easing = ExpandAnimationEasing;
            }

            if (num == uint.MaxValue)
            {
                num = AnimationLength;
            }

            if (easing == null)
            {
                easing = AnimationEasing;
            }

            if (lastVisibleSize > 0.0)
            {
                num = (uint)((double)num * (Math.Abs(endSize - startSize) / lastVisibleSize));
            }

            num = Math.Max(num, 1u);
            new Animation(delegate (double v)
            {
                ContentSizeRequest = v;
            }, startSize, endSize).Commit(contentHolder, "expandAnimationName", 16u, num, easing, delegate (double value, bool isInterrupted)
            {
                if (!isInterrupted)
                {
                    if (!IsExpanded)
                    {
                        if (contentHolder == null)
                        {
                            throw new NullReferenceException();
                        }

                        contentHolder!.IsVisible = false;
                        State = ExpandState.Collapsed;
                    }
                    else
                    {
                        State = ExpandState.Expanded;
                    }
                }
            });
        }

        private void OnTapped()
        {
            tappedEventManager.RaiseEvent(this, EventArgs.Empty, "Tapped");
        }
    }
}
