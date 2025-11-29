using System.Drawing;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.AutomationElements.Scrolling;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Patterns;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation
    {
        public IScrollPattern ScrollInfo => Patterns.Scroll.PatternOrDefault;

        public double ScrollFactor
        {
            get => RichCanvasSettings.ScrollFactor;
            set => SetValue(value);
        }

        public void ScrollByArrowKeyOrButton(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == Direction.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public void ScrollByPage(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == Direction.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public virtual void ScrollByScrollbarsDragging(Direction direction)
        {
            if (direction == Direction.Up || direction == Direction.Down)
            {
                VerticalScrollBar verticalScrollBar = ParentWindow.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsVerticalScrollBar();
                var verticalScrollbarLocation = new Point(verticalScrollBar.BoundingRectangle.Location.X + 2, (int)ViewportSize.Height / 2);
                Point verticalScrollbarMaxDragLocation = direction switch
                {
                    Direction.Down => new Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y - 1000),
                    _ => new Point(verticalScrollbarLocation.X, verticalScrollbarLocation.Y + 1000)
                };
                Mouse.Drag(verticalScrollbarLocation, verticalScrollbarMaxDragLocation);
            }
            else
            {
                HorizontalScrollBar horizontalScrollbar = ParentWindow.FindFirstDescendant(x => x.ByControlType(ControlType.ScrollBar)).AsHorizontalScrollBar();
                var horiontalScrollbarLocation = new Point((int)ViewportSize.Width / 2, horizontalScrollbar.BoundingRectangle.Location.Y + 2);
                Point horizontalScrollbarMaxDragLocation = direction switch
                {
                    Direction.Left => new Point(horiontalScrollbarLocation.X - 1000, horiontalScrollbarLocation.Y),
                    _ => new Point(horiontalScrollbarLocation.X + 1000, horiontalScrollbarLocation.Y),
                };
                Mouse.Drag(horiontalScrollbarLocation, horizontalScrollbarMaxDragLocation);
            }
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
