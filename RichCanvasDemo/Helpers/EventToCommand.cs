using Microsoft.Xaml.Behaviors;
using RichCanvas;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasDemo.Helpers
{
    public class EventToCommand : Behavior<RichItemContainer>
    {
        private static List<RoutedEventHandler> _eventHandlers = new List<RoutedEventHandler>();
        private static readonly List<EventInfo> _events = new List<EventInfo>();
        private static List<ICommand> _commands;

        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached("Event", typeof(string), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnEventChanged));

        public static void SetEvent(UIElement element, string value) => element.SetValue(EventProperty, value);
        public static string GetEvent(UIElement element) => (string)element.GetValue(EventProperty);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(List<ICommand>), typeof(EventToCommand),
           new FrameworkPropertyMetadata(null, OnCommandChanged));

        public static void SetCommand(UIElement element, List<ICommand> value) => element.SetValue(CommandProperty, value);
        public static List<ICommand> GetCommand(UIElement element) => (List<ICommand>)element.GetValue(CommandProperty);

        public static readonly DependencyProperty CanExecuteProperty = DependencyProperty.RegisterAttached("CanExecute", typeof(bool), typeof(EventToCommand),
          new FrameworkPropertyMetadata(false, OnCanExecuteChanged));

        public static void SetCanExecute(UIElement element, bool value) => element.SetValue(CanExecuteProperty, value);
        public static bool GetCanExecute(UIElement element) => (bool)element.GetValue(CanExecuteProperty);

        private static void OnCanExecuteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                _commands = GetCommand((RichItemContainer)d);
                _eventHandlers.Clear();
                foreach (ICommand command in _commands)
                {
                    _eventHandlers.Add((s, e) =>
                    {
                        command?.Execute(e.OriginalSource);
                    });
                }
                for (int i = 0; i < _events.Count; i++)
                {
                    _events[i].AddEventHandler(d, _eventHandlers[i]);
                }

            }
            else
            {
                for (int i = 0; i < _events.Count; i++)
                {
                    _events[i].RemoveEventHandler(d, _eventHandlers[i]);
                }
            }
        }

        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _events.Clear();
            var container = (RichItemContainer)d;
            foreach (string ev in ((string)e.NewValue).Split(","))
            {
                EventInfo x = container.GetType().GetEvent(ev.Trim());
                _events.Add(x);
            }
        }
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _commands = (List<ICommand>)e.NewValue;
            foreach (ICommand command in _commands)
            {
                _eventHandlers.Add((s, e) =>
                {
                    command?.Execute(e.OriginalSource);
                });
            }
        }

    }
}
