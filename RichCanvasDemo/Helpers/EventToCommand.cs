using Microsoft.Xaml.Behaviors;
using RichCanvas;
using RichCanvasDemo.ViewModels.Base;
using RichCanvasDemo.ViewModels.Connections;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasDemo.Helpers
{
    public class EventToCommand : Behavior<RichItemContainer>
    {
        private static RoutedEvent _registeredEvent;
        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event", typeof(RoutedEvent), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnEventChanged));

        public static void SetEvent(UIElement element, RoutedEvent value) => element.SetValue(EventProperty, value);
        public static RoutedEvent GetEvent(UIElement element) => (RoutedEvent)element.GetValue(EventProperty);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, ICommand value) => element.SetValue(CommandProperty, value);
        public static ICommand GetCommand(UIElement element) => (ICommand)element.GetValue(CommandProperty);

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => _registeredEvent = (RoutedEvent)e.NewValue;
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = (RichItemContainer)d;
            var context = (Drawable)container.DataContext;
            if (context is IConnectable connection)
            {
                if (connection.IsParent)
                {
                    container.AddHandler(_registeredEvent, new RoutedEventHandler((sender, x) =>
                    {
                        var delta = (double)x.OriginalSource;
                        var command = (ICommand)e.NewValue;
                        command.Execute(delta);
                    }));
                }
            }
        }

    }
}
