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
        private KeyGesture[] _keyGestures;

        public MouseGesture MouseGesture
        {
            get => _mouseGesture;
            set => _mouseGesture = value;
        }

        [TypeConverter(typeof(MouseKeyGestureConverter))]
        public KeyGesture[] KeyGestures
        {
            get => _keyGestures;
            set => _keyGestures = value;
        }

        public MouseKeyGesture()
        {

        }
        public MouseKeyGesture(MouseGesture mouseGesture, params KeyGesture[] keyGestures)
        {
            _mouseGesture = mouseGesture;
            _keyGestures = keyGestures;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (_keyGestures.All(k => Keyboard.IsKeyDown(k.Key)) && _mouseGesture.Matches(targetElement, inputEventArgs))
            {
                return true;
            }
            return false;
        }
    }

    internal class MouseKeyGestureConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var key = (string)value;
            Enum.TryParse(key, out Key enumKey);
            var keyGesture = new KeyGesture(enumKey);
            return new[] { keyGesture };
        }
    }
}
