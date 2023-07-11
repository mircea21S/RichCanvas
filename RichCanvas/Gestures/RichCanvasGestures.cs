using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class RichCanvasGestures
    {
        public static InputGesture Select { get; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drawing { get; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drag { get; } = new MouseGesture(MouseAction.LeftClick);
    }
}
