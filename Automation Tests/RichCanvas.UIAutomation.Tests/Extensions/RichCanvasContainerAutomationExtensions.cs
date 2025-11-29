using System;
using System.Drawing;

using FlaUI.Core.Tools;

using RichCanvas.UIAutomation.Tests.Utilities;

namespace RichCanvas.UIAutomation.Tests.Extensions
{
    internal static class RichCanvasContainerAutomationExtensions
    {
        internal static UIAClientAppPoint GetLocationPoint(this RichCanvasContainerAutomation container, StartPosition startPosition = StartPosition.TopLeft) => startPosition switch
        {
            StartPosition.TopLeft => Point.Add(container.Location, new Size(1, 1)),
            StartPosition.TopRight => Point.Add(container.Location, new Size(container.ActualWidth.ToInt(), 1)),
            StartPosition.BottomLeft => Point.Add(container.Location, new Size(1, container.ActualHeight.ToInt())),
            StartPosition.BottomRight => Point.Add(container.Location, new Size(container.ActualWidth.ToInt(), container.ActualHeight.ToInt())),
            _ => throw new NotImplementedException(),
        };
    }
}
