using System.Linq;
using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class MultiGesture : InputGesture
    {
        private readonly InputGesture[] _gestures;

        /// <summary>Constructs an instance of a <see cref="MultiGesture"/>.</summary>
        /// <param name="gestures">The input gestures.</param>
        public MultiGesture(params InputGesture[] gestures)
        {
            _gestures = gestures;
        }

        /// <inheritdoc />
        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            return MatchesAll(targetElement, inputEventArgs);
        }

        private bool MatchesAll(object targetElement, InputEventArgs inputEventArgs)
        {
            for (int i = 0; i < _gestures.Length; i++)
            {
                if (!_gestures[i].Matches(targetElement, inputEventArgs))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class MouseKeyGesture : InputGesture
    {
        private readonly MouseGesture _mouseGesture;
        private readonly KeyGesture[] _keyGestures;

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
}
