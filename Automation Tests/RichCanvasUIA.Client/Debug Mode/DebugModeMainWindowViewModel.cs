using System.Collections.ObjectModel;
using System.Windows.Input;

using RichCanvasUIA.Client.Debug_Mode.Editor;

namespace RichCanvasUIA.Client.Debug_Mode
{
    public class DebugModeMainWindowViewModel
    {
        public ObservableCollection<EditorTabItem> RichCanvasEditors { get; } = [];

        public ICommand AddRichCanvasEditorCommand { get; }

        public DebugModeMainWindowViewModel()
        {
            AddRichCanvasEditorCommand = new RelayCommand(AddRichCanvasEditor);
            RichCanvasEditors.Add(new EditorTabItem
            {
                Name = $"RichCanvas {RichCanvasEditors.Count + 1}",
                Editor = new RichCanvasEditor()
            });
            // create RichCanvasEditor settings.
            // when mutliple RichCanvasEditors will be supported, each of them has its own settings, could be cached.

            // create RichCanvasEditor. Pass the settings.
            // when multiple will be supported, create on demand, cache.

            // create StatusBar. Pass the RichCanvasEditor. Listen to changes and show info.
            // when active RichCanvasEditor changes. it will just update inside the StatusBar. This will be single instance.
            // when multiple RichCanvasEditors will be supported, create on demand, cache.
        }

        private void AddRichCanvasEditor()
        {
            RichCanvasEditors.Add(new EditorTabItem
            {
                Name = $"RichCanvas {RichCanvasEditors.Count + 1}",
                Editor = new RichCanvasEditor()
            });
        }
    }

    public class EditorTabItem
    {
        public string Name { get; set; }
        public RichCanvasEditor Editor { get; set; }
    }
}
