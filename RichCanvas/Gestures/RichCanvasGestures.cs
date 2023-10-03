using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class RichCanvasGestures
    {
        public static InputGesture Select { get; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drawing { get; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drag { get; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture ZoomIn { get; } = new KeyGesture(Key.OemPlus, ModifierKeys.Control);
        public static InputGesture ZoomOut { get; } = new KeyGesture(Key.OemMinus, ModifierKeys.Control);
        public static InputGesture Pan { get; set; } = new MouseKeyGesture(new MouseGesture(MouseAction.LeftClick), new KeyGesture(Key.Space));
        public static ModifierKeys Zoom { get; set; } = ModifierKeys.Control;
    }
}
