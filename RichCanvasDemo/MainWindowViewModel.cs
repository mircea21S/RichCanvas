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
    public class MainWindowViewModel : ObservableObject
    {
        private bool _enableGrid;
        private string _gridSpacing;
        private string _elementsCount;
        private bool _enableVirtualization;
        private ICommand drawLineCommand;
        private ICommand resizeCommand;
        private RelayCommand deleteCommand;

        public ObservableCollection<Drawable> Items { get; }
        public ObservableCollection<Drawable> SelectedItems { get; }
        public ICommand DrawRectCommand { get; }
        public ICommand GenerateElements { get; }
        public ICommand DrawLines { get; }
        public ICommand DrawLineCommand => drawLineCommand ??= new RelayCommand(DrawLine);
        public ICommand ResizeCommand => resizeCommand ??= new RelayCommand(Resize);
        public ICommand DeleteCommand => deleteCommand ??= new RelayCommand(Delete);

        public bool EnableGrid
        {
            get => _enableGrid;
            set => SetProperty(ref _enableGrid, value);
        }
        public string GridSpacing
        {
            get => _gridSpacing;
            set => SetProperty(ref _gridSpacing, value);
        }

        public string ElementsCount
        {
            get => _elementsCount;
            set => SetProperty(ref _elementsCount, value);
        }
        public bool EnableVirtualization
        {
            get => _enableVirtualization;
            set => SetProperty(ref _enableVirtualization, value);
        }

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Drawable>();
            DrawRectCommand = new RelayCommand(OnDrawCommand);
            SelectedItems = new ObservableCollection<Drawable>();
            GenerateElements = new RelayCommand(OnGenerateElements);
        }

        private void OnGenerateElements()
        {
            if (!string.IsNullOrEmpty(ElementsCount))
            {
                int elementsCount = int.Parse(ElementsCount);
                for (int i = 0; i < elementsCount; i++)
                {
                    Random rnd = new Random();
                    double left = rnd.Next(-1000, 1000);
                    double top = rnd.Next(-1000, 1000);
                    var item = new Line
                    {
                        Left = left,
                        Top = top,
                        Width = Math.Abs(left / 2 + 30),
                        Height = Math.Abs(top / 2 + 30)
                    };
                    if (item.Width == 0)
                    {
                        item.Width = 10;
                    }
                    if (item.Height == 0)
                    {
                        item.Height = 10;
                    }
                    Items.Add(item);
                }
            }
        }
        private void Delete()
        {
            Items.Remove(SelectedItems[0]);
        }

        private void OnDrawCommand()
        {
            Items.Add(new Rectangle());
        }
        public void DrawConnectedLine(Point p)
        {
            Items.Add(new Line { Top = p.Y, Left = p.X });
        }

        private void DrawLine()
        {
            Items.Add(new Line());
        }

        private void Resize()
        {
            var x = SelectedItems[0];
            x.Height += 20;
        }

    }
}
