using RichCanvasUIA.Client.Debug_Mode.Editor;
using RichCanvasUIA.Client.Debug_Mode.Overlay;

namespace RichCanvasUIA.Client.Debug_Mode
{
    public class EditorContext
    {
        public RichCanvasEditor RichCanvasEditor { get; } = new RichCanvasEditor();
        public RichCanvasToolboxViewModel RichCanvasToolbox { get; }

        public EditorContext()
        {
            RichCanvasToolbox = new RichCanvasToolboxViewModel(RichCanvasEditor);
        }
    }
}
