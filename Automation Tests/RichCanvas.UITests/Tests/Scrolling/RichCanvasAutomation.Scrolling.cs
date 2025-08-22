using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;

using RichCanvas.UITests.Tests.Scrolling;

namespace RichCanvas.UITests
{
    public partial class RichCanvasAutomation
    {
        public IScrollPattern ScrollInfo => Patterns.Scroll.PatternOrDefault;

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

        public void ScrollByScrollbarsDragging(Direction scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                if (scrollingMode == Direction.Up)
                {
                    scrollPattern.SetScrollPercent(0, -1);
                }
                else if (scrollingMode == Direction.Down)
                {
                    scrollPattern.SetScrollPercent(0, 1);
                }
                else if (scrollingMode == Direction.Left)
                {
                    scrollPattern.SetScrollPercent(1, 0);
                }
                else
                {
                    scrollPattern.SetScrollPercent(-1, 0);
                }
            }
        }

        public void SetScrollPercent(double horizontalOffset, double verticalOffset)
        {
            if (Patterns.Scroll.TryGetPattern(out IScrollPattern scrollPattern))
            {
                scrollPattern.SetScrollPercent(horizontalOffset, verticalOffset);
            }
        }
    }
}
