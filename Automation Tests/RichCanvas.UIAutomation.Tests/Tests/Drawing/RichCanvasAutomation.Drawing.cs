using System;
using System.Drawing;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;
using RichCanvas.UIAutomation.Tests.Tests.Scrolling;

using RichCanvasUIA.Client.Automation;
using RichCanvasUIA.Client.IPC_Pipe;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public void AddEmptyRectangle()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyRectangle);
        }

        public void AddEmptyLine()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.AddEmptyLine);
        }

        public void RemoveFirstItem()
        {
            SendToAppWithDrawingPrefix(PipeHandlerNames.Drawing.RemoveFirstItem);
        }

        private void SendToAppWithDrawingPrefix(string operationName)
        {
            UITestsAppChannel.Send($"{nameof(PipeHandlerNames.Drawing)}.{operationName}");
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
