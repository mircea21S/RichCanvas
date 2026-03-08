using RichCanvasUIA.Client.Debug_Mode.Editor;

namespace RichCanvasUIA.Client.Debug_Mode
{
    public class DebugModeMainWindowViewModel
    {
        public RichCanvasEditorHost FullMVVMRichCanvasEditor { get; }

        public DebugModeMainWindowViewModel()
        {
            FullMVVMRichCanvasEditor = new RichCanvasEditorHost();
            // create StatusBar. Pass the RichCanvasEditor. Listen to changes and show info.
            // when active RichCanvasEditor changes. it will just update inside the StatusBar. This will be single instance.
            // when multiple RichCanvasEditors will be supported, create on demand, cache.
        }
    }
}
