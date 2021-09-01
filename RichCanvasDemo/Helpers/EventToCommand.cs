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
        private static ICommand _command;

        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event", typeof(RoutedEvent), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnEventChanged));

        public static void SetEvent(UIElement element, RoutedEvent value) => element.SetValue(EventProperty, value);
        public static RoutedEvent GetEvent(UIElement element) => (RoutedEvent)element.GetValue(EventProperty);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, ICommand value) => element.SetValue(CommandProperty, value);
        public static ICommand GetCommand(UIElement element) => (ICommand)element.GetValue(CommandProperty);

        public static readonly DependencyProperty CanExecuteProperty = DependencyProperty.RegisterAttached("CanExecute", typeof(bool), typeof(EventToCommand),
          new FrameworkPropertyMetadata(false, OnCanExecuteChanged));

        public static void SetCanExecute(UIElement element, bool value) => element.SetValue(CanExecuteProperty, value);
        public static bool GetCanExecute(UIElement element) => (bool)element.GetValue(CanExecuteProperty);

        private static void OnCanExecuteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                SubscribeToEvent((RichItemContainer)d, _command);
            }
            else
            {
                //((RichItemContainer)d).RemoveHandler(_registeredEvent, HandleEvent(_command));
                ((RichItemContainer)d).LeftChanged -= Container_LeftChanged;
            }
        }
        private static void SubscribeToEvent(RichItemContainer container, ICommand command)
        {
            container.LeftChanged += Container_LeftChanged;
            var context = (Drawable)container.DataContext;
            //container.AddHandler(_registeredEvent, HandleEvent(command));
        }

        private static void Container_LeftChanged(object sender, RoutedEventArgs e)
        {
            var delta = (double)e.OriginalSource;
            _command.Execute(delta);
        }

        private static RoutedEventHandler HandleEvent(ICommand command)
        {
            return (sender, x) =>
            {
                var delta = (double)x.OriginalSource;
                command.Execute(delta);
            };
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => _registeredEvent = (RoutedEvent)e.NewValue;
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => _command = (ICommand)e.NewValue;

    }
}
