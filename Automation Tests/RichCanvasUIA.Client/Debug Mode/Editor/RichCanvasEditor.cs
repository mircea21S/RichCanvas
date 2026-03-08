using System.Collections.Generic;
using System.Collections.ObjectModel;

using RichCanvasUIA.Client.Debug_Mode.RichCanvasItems;

namespace RichCanvasUIA.Client.Debug_Mode.Editor
{
    public class RichCanvasEditor : ObservableObject
    {
        public RichCanvasSettings Settings { get; } = new RichCanvasSettings();
        public ObservableCollection<RichCanvasEditorItem> Items { get; } = [];
        public List<RichCanvasEditorItem> SelectedItems { get; } = [];

        private RichCanvasEditorItem _selectedItem;

        public RichCanvasEditorItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
    }
}
