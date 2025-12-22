using System;
using System.Windows.Input;

using RichCanvasUIA.Client.Debug_Mode.Editor;
using RichCanvasUIA.Client.Debug_Mode.RichCanvasItems;

namespace RichCanvasUIA.Client.Debug_Mode.Overlay
{
    public class RichCanvasToolboxViewModel
    {
        public RichCanvasEditor RichCanvasEditor { get; }
        public ICommand AddNotDrawnItemCommand { get; }
        public ICommand AddDrawnItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RemoveAllCommand { get; }

        public RichCanvasToolboxViewModel(RichCanvasEditor richCanvasEditor)
        {
            RichCanvasEditor = richCanvasEditor;
            AddNotDrawnItemCommand = new RelayCommand<Type>(AddNotDrawnItem);
            AddDrawnItemCommand = new RelayCommand<Type>(AddDrawnItem);
            RemoveAllCommand = new RelayCommand(RichCanvasEditor.Items.Clear);
        }

        private void AddNotDrawnItem(Type type)
        {
            if (type == typeof(RichCanvasEditorItem))
            {
                RichCanvasEditor.Items.Add(new RichCanvasEditorItem());
            }
            else if (type == typeof(LineEditorItem))
            {
                RichCanvasEditor.Items.Add(new LineEditorItem());
            }
        }

        private void AddDrawnItem(Type type)
        {
            if (type == typeof(RichCanvasEditorItem))
            {
                RichCanvasEditor.Items.Add(new RichCanvasEditorItem
                {
                    Top = 100,
                    Left = 100,
                    Width = 50,
                    Height = 50
                });
            }
            else if (type == typeof(LineEditorItem))
            {
                RichCanvasEditor.Items.Add(new LineEditorItem
                {
                    Top = 100,
                    Left = 100,
                    Width = 100,
                    Height = 100
                });
            }
        }
    }
}
