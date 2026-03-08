using RichCanvasUIA.Client.Debug_Mode.Overlay;

namespace RichCanvasUIA.Client.Debug_Mode.Editor
{
    public class RichCanvasEditorHost
    {
        public RichCanvasEditor RichCanvasEditor { get; } = new RichCanvasEditor();
        public RichCanvasToolboxViewModel RichCanvasToolbox { get; }

        public RichCanvasEditorHost()
        {
            RichCanvasToolbox = new RichCanvasToolboxViewModel(RichCanvasEditor);
        }
    }
}
