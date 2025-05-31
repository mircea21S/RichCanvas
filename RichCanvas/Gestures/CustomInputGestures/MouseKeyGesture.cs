using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class MouseKeyGesture : InputGesture
    {
        private MouseGesture _mouseGesture;
        public MouseGesture MouseGesture
        {
            get => _mouseGesture;
            set => _mouseGesture = value;
        }

        private KeyGesture[] _keyGestures;
        [TypeConverter(typeof(MouseKeyGestureConverter))]
        public KeyGesture[] KeyGestures
        {
            get => _keyGestures;
            set => _keyGestures = value;
        }

        private Key[] _keys;
        public Key[] Keys
        {
            get => _keys;
            set => _keys = value;
        }

        public MouseKeyGesture()
        {

        }
        public MouseKeyGesture(MouseGesture mouseGesture, params KeyGesture[] keyGestures)
        {
            _mouseGesture = mouseGesture;
            _keyGestures = keyGestures;
        }

        public MouseKeyGesture(MouseGesture mouseGesture, params Key[] keys)
        {
            _mouseGesture = mouseGesture;
            _keys = keys;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (_keyGestures != null)
            {
                if (_keyGestures.All(k => Keyboard.IsKeyDown(k.Key)) && _mouseGesture.Matches(targetElement, inputEventArgs))
                {
                    return true;
                }
            }
            if (_keys != null)
            {
                if (_keys.All(k => Keyboard.IsKeyDown(k)) && _mouseGesture.Matches(targetElement, inputEventArgs))
                {
                    return true;
                }
            }
            return false;
        }
    }

    internal class MouseKeyGestureConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string key = (string)value;
            Enum.TryParse(key, out Key enumKey);
            var keyGesture = new KeyGesture(enumKey);
            return new[] { keyGesture };
        }
    }
}
