using RichCanvasDemo.Common;
using RichCanvasDemo.ViewModels;
using RichCanvasDemo.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasDemo
{
    public class MainWindowViewModel
    {
        public ObservableCollection<Drawable> Items { get; }
        public ICommand DrawRectCommand { get; }
        public ICommand DrawLines { get; }

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Drawable>();
            DrawRectCommand = new RelayCommand(OnDrawCommand);
        }
        private void OnDrawCommand()
        {
            Items.Add(new Line { Top = -10, Left = 100, Width = 40, Height = 40 });
            Items.Add(new Line { Top = 395, Left = 100, Width = 40, Height = 40 });
            //Items.Add(new Rectangle());
        }
        public void DrawConnectedLine(Point p)
        {
            Items.Add(new Line { Top = p.Y, Left = p.X });
        }
    }
}
