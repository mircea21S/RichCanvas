using System;
using System.Drawing;
using System.Windows.Input;

namespace RichCanvas.UITests.Helpers
{
    internal class KeyGestureHandler : GestureHandler
    {
        private readonly KeyGesture _keyGesture;

        public KeyGestureHandler(KeyGesture keyGesture)
        {
            _keyGesture = keyGesture;
        }

        internal override void Click(Point point) => throw new System.NotImplementedException();
        internal override void DefferedDrag(Point startPoint, params (Point Position, Action StepAction)[] dragStepPoints) => throw new NotImplementedException();
        internal override void Drag(Point startPoint, Point endPoint) => throw new System.NotImplementedException();
    }
}