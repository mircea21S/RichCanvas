using RichCanvasUITests.App.Models;
using RichCanvasUITests.App.TestMocks;
using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasUITests.App.States
{
    public class DrawingStateViewModel : ObservableObject
    {
        public MainWindowViewModel Parent { get; }

        private bool _shoudlExecuteDrawingEndedCommand = true;
        public bool ShouldExecuteDrawingEndedCommand
        {
            get => _shoudlExecuteDrawingEndedCommand;
            set => SetProperty(ref _shoudlExecuteDrawingEndedCommand, value);
        }

        public DrawingStateViewModel(MainWindowViewModel parent)
        {
            Parent = parent;
        }

        private RelayCommand<bool> _addPositionedRectangleCommand;
        public ICommand AddPositionedRectangleCommand => _addPositionedRectangleCommand ??= new RelayCommand<bool>(AddPositionedRectangle);

        private void AddPositionedRectangle(bool isImmutable)
        {
            Parent.Items.Add(isImmutable ? DrawingStateDataMocks.ImmutablePositionedRectangleMockWithoutSize : DrawingStateDataMocks.PositionedRectangleMockWithoutSize);
        }

        private RelayCommand _addDrawnRectangleCommand;
        public ICommand AddDrawnRectangleCommand => _addDrawnRectangleCommand ??= new RelayCommand(AddDrawnRectangle);

        private void AddDrawnRectangle()
        {
            Parent.Items.Add(DrawingStateDataMocks.DrawnRectangleMock);
        }

        private RelayCommand<Type> _addEmptyItemCommand;
        public ICommand AddEmptyItemCommand => _addEmptyItemCommand ??= new RelayCommand<Type>(AddEmptyRectangle);

        private void AddEmptyRectangle(Type itemType)
        {
            if (itemType == typeof(RichItemContainerModel))
            {
                Parent.Items.Add(new RichItemContainerModel());
            }
            else if (itemType == typeof(Line))
            {
                Parent.Items.Add(new Line());
            }
        }

        private RelayCommand<Point> _drawingEndedCommand;
        public ICommand DrawingEndedCommand => _drawingEndedCommand ??= new RelayCommand<Point>(DrawingEnded, () => ShouldExecuteDrawingEndedCommand);

        private void DrawingEnded(Point mousePositon)
        {
            Parent.Items.Add(new DrawingEndedRepresentation
            {
                Top = mousePositon.Y,
                Left = mousePositon.X,
                Width = 100,
                Height = 100
            });
        }

        private RelayCommand<Direction> _addItemOutsideViewportCommand;
        public ICommand AddItemOutsideViewportCommand => _addItemOutsideViewportCommand ??= new RelayCommand<Direction>(AddItemOutsideViewport);

        private void AddItemOutsideViewport(Direction direction)
        {
            switch (direction)
            {
                case Direction.Top:
                    Parent.Items.Add(new RichItemContainerModel
                    {
                        Top = Parent.ViewportLocation.Y - 100,
                        Left = Parent.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;
                case Direction.Left:
                    Parent.Items.Add(new RichItemContainerModel
                    {
                        Top = Parent.ViewportLocation.Y + 100,
                        Left = Parent.ViewportLocation.X - 100,
                        Width = 100,
                        Height = 100
                    });
                    break;
                case Direction.Bottom:
                    Parent.Items.Add(new RichItemContainerModel
                    {
                        Top = Parent.ViewportSize.Height,
                        Left = Parent.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;
                case Direction.Right:
                    Parent.Items.Add(new RichItemContainerModel
                    {
                        Top = Parent.ViewportLocation.Y + 10,
                        Left = Parent.ViewportSize.Width,
                        Width = 100,
                        Height = 100
                    });
                    break;
            }
        }
    }

    public enum Direction
    {
        Top,
        Left,
        Right,
        Bottom
    }
}
