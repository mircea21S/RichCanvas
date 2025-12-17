using System.Collections.ObjectModel;

using RichCanvasUIA.Client.Debug_Mode.RichCanvasItems;

namespace RichCanvasUIA.Client.Debug_Mode.Editor
{
    public class RichCanvasEditor
    {
        public RichCanvasSettings Settings { get; } = new RichCanvasSettings();
        public ObservableCollection<RichCanvasEditorItem> Items { get; } = [];
    }
}
