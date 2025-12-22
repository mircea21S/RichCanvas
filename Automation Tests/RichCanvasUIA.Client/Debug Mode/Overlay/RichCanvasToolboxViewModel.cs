using RichCanvasUIA.Client.Debug_Mode.Editor;

namespace RichCanvasUIA.Client.Debug_Mode.Overlay
{
    public class RichCanvasToolboxViewModel(RichCanvasEditor richCanvasEditor)
    {
        public RichCanvasEditor RichCanvasEditor { get; } = richCanvasEditor;
    }
}
