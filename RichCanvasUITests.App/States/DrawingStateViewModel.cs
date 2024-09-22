using RichCanvasUITests.App.Models;
using RichCanvasUITests.App.TestMocks;
using System;
using System.Windows;
using System.Windows.Input;

namespace RichCanvasUITests.App.States
{
    public class DrawingStateViewModel
    {
        public MainWindowViewModel Parent { get; }

        public DrawingStateViewModel(MainWindowViewModel parent)
        {
            Parent = parent;
        }

        private RelayCommand<bool> _addPositionedRectangleCommand;
        public ICommand AddPositionedRectangleCommand => _addPositionedRectangleCommand ??= new RelayCommand<bool>(AddPositionedRectangle);

        private void AddPositionedRectangle(bool isImmutable)
        {
            Parent.Items.Add(isImmutable ? DrawingStateDataMocks.ImmutablePositionedRectangleMock : DrawingStateDataMocks.PositionedRectangleMock);
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
        public ICommand DrawingEndedCommand => _drawingEndedCommand ??= new RelayCommand<Point>(DrawingEnded);

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
    }
}
