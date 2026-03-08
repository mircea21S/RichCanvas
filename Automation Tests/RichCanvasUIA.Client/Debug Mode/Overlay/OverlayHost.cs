using System.Windows;
using System.Windows.Controls;

namespace RichCanvasUIA.Client.Debug_Mode.Overlay
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RichCanvasUIA.Client.Debug_Mode.Overlay"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:RichCanvasUIA.Client.Debug_Mode.Overlay;assembly=RichCanvasUIA.Client.Debug_Mode.Overlay"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:OverlayHost/>
    ///
    /// </summary>

    [TemplatePart(Name = LayoutRootPart, Type = typeof(Grid))]
    [TemplatePart(Name = OverlayPart, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = ContentPart, Type = typeof(ContentPresenter))]
    public class OverlayHost : ContentControl
    {
        private const string ContentPart = "PART_Content";
        private const string OverlayPart = "PART_Overlay";
        private const string LayoutRootPart = "PART_LayoutRoot";

        public static DependencyProperty OverlayProperty = DependencyProperty.Register(nameof(Overlay), typeof(OverlayContent), typeof(OverlayHost), new FrameworkPropertyMetadata(default(OverlayContent)));

        public OverlayContent Overlay
        {
            get => (OverlayContent)GetValue(OverlayProperty);
            set => SetValue(OverlayProperty, value);
        }

        static OverlayHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverlayHost), new FrameworkPropertyMetadata(typeof(OverlayHost)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var contentPresenter = GetTemplateChild("PART_Content") as ContentPresenter;

            if (contentPresenter != null && Overlay != null)
            {
                Overlay.TargetElement = contentPresenter.Content as FrameworkElement;
            }
        }
    }

    public class OverlayContent : ContentControl
    {
        private static readonly DependencyPropertyKey TargetElementPropertyKey =
          DependencyProperty.RegisterReadOnly(
              nameof(TargetElement),
              typeof(FrameworkElement),
              typeof(OverlayContent),
              new PropertyMetadata(null));

        public static readonly DependencyProperty TargetElementProperty =
            TargetElementPropertyKey.DependencyProperty;

        public FrameworkElement TargetElement
        {
            get => (FrameworkElement)GetValue(TargetElementProperty);
            internal set => SetValue(TargetElementPropertyKey, value);
        }
    }
}
