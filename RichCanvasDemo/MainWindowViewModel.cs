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
        private RelayCommand generateElementsCommand;
        private RelayCommand drawRectCommand;
        private RelayCommand drawBezierCommand;
        private ICommand drawEndedCommand;
        private bool enableSnapping;
        private bool disableCache;
        private bool disableZoom;
        private bool disableScroll;
        private bool disableAutoPanning;
        private string autoPanSpeed;
        private string autoPanTickRate;
        private string scrollFactor;
        private string zoomFactor;
        private string maxScale;
        private string minScale;

        public ICommand DrawEndedCommand => drawEndedCommand ??= new RelayCommand<RoutedEventArgs>(DrawEnded);
        public ObservableCollection<Drawable> Items { get; }
        public ObservableCollection<Drawable> SelectedItems { get; }
        public ICommand DrawRectCommand => drawRectCommand ??= new RelayCommand(OnDrawCommand);
        public ICommand GenerateElements => generateElementsCommand ??= new RelayCommand(OnGenerateElements);
        public ICommand DrawLineCommand => drawLineCommand ??= new RelayCommand(DrawLine);
        public ICommand ResizeCommand => resizeCommand ??= new RelayCommand(Resize);
        public ICommand DeleteCommand => deleteCommand ??= new RelayCommand(Delete);
        public ICommand DrawBezierCommand => drawBezierCommand ??= new RelayCommand(OnDrawBezier);

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

        public bool EnableSnapping { get => enableSnapping; set => SetProperty(ref enableSnapping, value); }


        public bool DisableCache { get => disableCache; set => SetProperty(ref disableCache, value); }


        public bool DisableZoom { get => disableZoom; set => SetProperty(ref disableZoom, value); }


        public bool DisableScroll { get => disableScroll; set => SetProperty(ref disableScroll, value); }


        public bool DisableAutoPanning { get => disableAutoPanning; set => SetProperty(ref disableAutoPanning, value); }


        public string AutoPanSpeed { get => autoPanSpeed; set => SetProperty(ref autoPanSpeed, value); }


        public string AutoPanTickRate { get => autoPanTickRate; set => SetProperty(ref autoPanTickRate, value); }


        public string ScrollFactor { get => scrollFactor; set => SetProperty(ref scrollFactor, value); }


        public string ZoomFactor { get => zoomFactor; set => SetProperty(ref zoomFactor, value); }


        public string MaxScale { get => maxScale; set => SetProperty(ref maxScale, value); }


        public string MinScale { get => minScale; set => SetProperty(ref minScale, value); }

        public MainWindowViewModel()
        {
            Items = new ObservableCollection<Drawable>();
            SelectedItems = new ObservableCollection<Drawable>();
        }

        private void OnDrawBezier()
        {
            var bezier = new Bezier
            {
                Points = new List<Point>(),
            };
            bezier.Point1 = new Point(bezier.Width - 20, 0);
            bezier.Point2 = new Point(bezier.Width - 10, 0);
            bezier.Point3 = new Point(bezier.Width, bezier.Height);
            Items.Add(bezier);
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

        private void DrawEnded(RoutedEventArgs args)
        {
            var element = (Drawable)args.OriginalSource;
            if (element is Bezier bezier)
            {
                bezier.Point1 = new Point(bezier.Width - 20, 0);
                bezier.Point2 = new Point(bezier.Width - 10, 0);
                bezier.Point3 = new Point(bezier.Width, bezier.Height);
            }
        }
    }
}
