using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class MouseCustomKeyGesture : InputGesture
    {
        private MouseGesture _mouseGesture;
        private Key[] _customKeys = Array.Empty<Key>();

        [TypeConverter(typeof(MouseCustomKeyGestureConverter))]
        public Key[] CustomKeys
        {
            get => _customKeys;
            set => _customKeys = value;
        }

        public MouseGesture MouseGesture
        {
            get => _mouseGesture;
            set => _mouseGesture = value;
        }

        public MouseCustomKeyGesture()
        {

        }
        public MouseCustomKeyGesture(MouseGesture mouseGesture, params Key[] customKeys)
        {
            _mouseGesture = mouseGesture;
            _customKeys = customKeys;
        }
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (_customKeys.All(k => Keyboard.IsKeyDown(k)) && IsMouseGestureMatching(_mouseGesture.MouseAction) && Keyboard.Modifiers == _mouseGesture.Modifiers)
            {
                return true;
            }
            return false;
        }

        private bool IsMouseGestureMatching(MouseAction mouseAction)
        {
            return mouseAction switch
            {
                MouseAction.LeftClick => Mouse.LeftButton == MouseButtonState.Pressed,
                _ => false
            };
        }
    }

    internal class MouseCustomKeyGestureConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string key = (string)value;
            Enum.TryParse(key, out Key enumKey);
            return new[] { enumKey };
        }
    }
}
