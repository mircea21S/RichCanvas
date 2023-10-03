using System.Windows.Input;

namespace RichCanvas.Gestures
{
    public class RichCanvasGestures
    {
        public static InputGesture Select { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drawing { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture Drag { get; set; } = new MouseGesture(MouseAction.LeftClick);
        public static InputGesture ZoomIn { get; set; } = new KeyGesture(Key.OemPlus, ModifierKeys.Control);
        public static InputGesture ZoomOut { get; set; } = new KeyGesture(Key.OemMinus, ModifierKeys.Control);
        public static InputGesture Pan { get; set; } = new MouseKeyGesture(new MouseGesture(MouseAction.LeftClick), new KeyGesture(Key.Space));
        public static ModifierKeys Zoom { get; set; } = ModifierKeys.Control;
    }
}
