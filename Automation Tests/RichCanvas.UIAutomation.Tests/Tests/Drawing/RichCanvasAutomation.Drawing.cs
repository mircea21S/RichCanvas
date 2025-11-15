using System;
using System.Drawing;

using FlaUI.Core.Tools;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;
using RichCanvas.UIAutomation.Tests.Tests.Scrolling;

using RichCanvasUIA.Client.Automation;
using RichCanvasUIA.Client.IPC_Pipe;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        private static readonly Random _rand = new();

        public void Draw(Size containerSize, out Point containerLocation, int scaleX = 1, int scaleY = 1)
        {
            Point pointOnCanvas = GetRandomPointOnRichCanvas();
            FlaUIInputData flaUiInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drawing);
            Point endPoint = GetEndPointByScale(pointOnCanvas, containerSize, scaleX, scaleY);
            flaUiInput.Drag(pointOnCanvas.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
            containerLocation = pointOnCanvas;
        }

        public void DrawPositionedContainer(RichCanvasContainerAutomation container, Size containerSize, int scaleX = 1, int scaleY = 1)
        {
            Point startPoint = container.Location;
            FlaUIInputData flaUiInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Drawing);
            Point endPoint = GetEndPointByScale(startPoint, containerSize, scaleX, scaleY);
            flaUiInput.Drag(startPoint.ToCanvasDrawingPoint(), endPoint.ToCanvasDrawingPoint());
        }

        private Point GetEndPointByScale(Point pointOnCanvas, Size containerSize, int scaleX, int scaleY)
        {
            if (scaleX == 1 && scaleY == 1)
            {
                return new Point(pointOnCanvas.X + containerSize.Width, pointOnCanvas.Y + containerSize.Height);
            }
            else if (scaleX == -1 && scaleY == 1)
            {
                return new Point(pointOnCanvas.X - containerSize.Width, pointOnCanvas.Y + containerSize.Height);
            }
            else if (scaleX == 1 && scaleY == -1)
            {
                return new Point(pointOnCanvas.X + containerSize.Width, pointOnCanvas.Y - containerSize.Height);
            }
            return new Point(pointOnCanvas.X - containerSize.Width, pointOnCanvas.Y - containerSize.Height);
        }

        internal void AddDrawnRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddDrawnRectangle);
        }

        internal void AddPositionedRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddPositionedRectangle);
        }

        public void AddImmutableRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddImmutableRectangle);
        }

        public void MoveFirstItemToTheEnd()
        {
            SendToAppWithItemsSourcePrefix(PipeHandlerNames.ItemsSource.MoveFirstItemToTheEnd);
        }

        public void RemoveFirstItem()
        {
            SendToAppWithItemsSourcePrefix(PipeHandlerNames.ItemsSource.RemoveFirstItem);
        }

        public void AddEmptyRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyRectangle);
        }

        public void AddEmptyLine()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyLine);
        }

        public void DisableDrawingEndedCommandExecution()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.DisableDrawingEndedCommandExecution);
        }

        public void EnableDrawingEndedCommandExecution()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.EnableDrawingEndedCommandExecution);
        }

        private void SendToAppWithDrawingPrefix(string operationName)
        {
            UITestsAppChannel.Send($"{nameof(PipeHandlerNames.Drawing)}.{operationName}");
        }

        private void SendToAppWithItemsSourcePrefix(string operationName)
        {
            UITestsAppChannel.Send($"{nameof(PipeHandlerNames.ItemsSource)}.{operationName}");
        }

        private Point GetRandomPointOnRichCanvas()
        {
            // add a tolerance to not get a point on the edge and not being able to draw due to not having enough space
            int viewportSizeTolerance = 50;
            double width = ActualWidth - viewportSizeTolerance;
            double height = ActualHeight - viewportSizeTolerance;

            if (width <= 0 || height <= 0)
            {
                throw new InvalidOperationException("Canvas has no rendered size. Ensure it's loaded and visible.");
            }

            double x = _rand.NextDouble() * width;
            double y = _rand.NextDouble() * height;
            return new Point(x.ToInt(), y.ToInt());
        }

        public void DrawEmptyContainer(System.Windows.Size visualViewportSize, Direction direction, int offset, Action assertCallbackAction)
        {
            ParentWindow.InvokeButton(AutomationIds.AddEmptyRectangleButtonId);
            var viewportCenter = new Point((int)visualViewportSize.Width / 2, (int)visualViewportSize.Height / 2);
            Point draggingEndPoint = direction switch
            {
                Direction.Left => new Point(-offset, viewportCenter.Y),
                Direction.Right => new Point((int)visualViewportSize.Width + offset, viewportCenter.Y),
                Direction.Up => new Point(viewportCenter.X, -offset),
                Direction.Down => new Point(viewportCenter.X, (int)visualViewportSize.Height + offset),
                _ => throw new NotImplementedException(),
            };
            Input.WithGesture(RichCanvasGestures.Drawing).DefferedDrag(viewportCenter, (draggingEndPoint.ToCanvasDrawingPoint(), assertCallbackAction));
        }
    }
}
