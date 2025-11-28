using System;
using System.Drawing;

using FlaUI.Core.Input;

using RichCanvas.Gestures;
using RichCanvas.UIAutomation.Tests.Helpers;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        private bool _isSelecting;

        public void StartSelection(Point fromPoint)
        {
            if (_isSelecting)
            {
                throw new InvalidOperationException("Selection operation already in progress.");
            }

            _isSelecting = true;
            Mouse.Position = fromPoint;

            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Select);
            flaUIDragInput.Start();
        }

        public void Select(Point toPoint)
        {
            if (!_isSelecting)
            {
                throw new InvalidOperationException("Selection operation not started. Select input should be processed before selecting.");
            }
            Mouse.Position = toPoint;
            Wait.UntilInputIsProcessed();
        }

        public void EndSelection()
        {
            if (!_isSelecting)
            {
                throw new InvalidOperationException("Selection operation not in progress. Nothing to end.");
            }
            FlaUIInputData flaUIDragInput = InputMapper.MapToFlaUIInput(RichCanvasGestures.Select);
            flaUIDragInput.Stop();
            _isSelecting = false;
        }

        public void SelectAllItems()
        {
            foreach (RichCanvasContainerAutomation item in Items)
            {
                item.IsSelected = true;
            }
        }
    }
}
