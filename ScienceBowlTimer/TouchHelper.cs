using System.Windows;

namespace ScienceBowlTimer
{
    public static class TouchHelper
    {
        public static readonly DependencyProperty IsRealMouseOverProperty =
            DependencyProperty.RegisterAttached(
                "IsRealMouseOver",
                typeof(bool),
                typeof(TouchHelper),
                new FrameworkPropertyMetadata(false));

        public static bool GetIsRealMouseOver(UIElement element) =>
            (bool)element.GetValue(IsRealMouseOverProperty);

        public static void SetIsRealMouseOver(UIElement element, bool value) =>
            element.SetValue(IsRealMouseOverProperty, value);
        public static readonly DependencyProperty IsPressedRealProperty =
            DependencyProperty.RegisterAttached(
                "IsPressedReal",
                typeof(bool),
                typeof(TouchHelper),
                new FrameworkPropertyMetadata(false));

        public static bool GetIsPressedReal(UIElement element) =>
            (bool)element.GetValue(IsPressedRealProperty);

        public static void SetIsPressedReal(UIElement element, bool value) =>
            element.SetValue(IsPressedRealProperty, value);
        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached(
                "Enabled",
                typeof(bool),
                typeof(TouchHelper),
                new PropertyMetadata(false, OnEnabledChanged));

        public static bool GetEnabled(UIElement element) =>
            (bool)element.GetValue(EnabledProperty);

        public static void SetEnabled(UIElement element, bool value) =>
            element.SetValue(EnabledProperty, value);

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement el && (bool)e.NewValue)
            {
                // Mouse events
                el.MouseEnter += (s, ev) =>
                {
                    if (ev.StylusDevice == null)
                        SetIsRealMouseOver((UIElement)s, true);
                };
                el.MouseLeave += (s, ev) =>
                {
                    SetIsRealMouseOver((UIElement)s, false);
                    SetIsPressedReal((UIElement)s, false);
                };
                el.PreviewMouseLeftButtonDown += (s, ev) =>
                {
                    if (ev.StylusDevice == null)
                        SetIsPressedReal((UIElement)s, true);
                };
                el.PreviewMouseLeftButtonUp += (s, ev) =>
                {
                    SetIsPressedReal((UIElement)s, false);
                };
                // Touch events — handle directly, bypass mouse promotion
                el.PreviewTouchDown += (s, ev) =>
                {
                    SetIsPressedReal((UIElement)s!, true);
                    SetIsRealMouseOver((UIElement)s!, false);
                };
                el.PreviewTouchUp += (s, ev) =>
                {
                    SetIsPressedReal((UIElement)s!, false);
                    SetIsRealMouseOver((UIElement)s!, false);
                };
                el.TouchEnter += (s, ev) =>
                {
                    SetIsRealMouseOver((UIElement)s!, false);
                };
                el.TouchLeave += (s, ev) =>
                {
                    SetIsPressedReal((UIElement)s!, false);
                    SetIsRealMouseOver((UIElement)s!, false);
                };
            }
        }
    }
}
