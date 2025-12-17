namespace RichCanvasUIA.Client.Debug_Mode
{
    public class DebugModeMainWindowViewModel
    {
        public DebugModeMainWindowViewModel()
        {
            // create RichCanvasEditor settings.
            // when mutliple RichCanvasEditors will be supported, each of them has its own settings, could be cached.

            // create RichCanvasEditor. Pass the settings.
            // when multiple will be supported, create on demand, cache.

            // create StatusBar. Pass the RichCanvasEditor. Listen to changes and show info.
            // when active RichCanvasEditor changes. it will just update inside the StatusBar. This will be single instance.
            // when multiple RichCanvasEditors will be supported, create on demand, cache.
        }
    }
}
