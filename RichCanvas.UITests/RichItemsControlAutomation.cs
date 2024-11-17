using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using RichCanvas.Automation;
using RichCanvas.Automation.ControlInformations;
using RichCanvas.UITests.Tests;
using System.Collections.Generic;
using System.Linq;

namespace RichCanvas.UITests
{
    public class RichItemsControlAutomation : AutomationElement
    {
        public RichItemContainerAutomation[] Items
        {
            get
            {
                if (Patterns.ItemContainer.TryGetPattern(out var itemContainerPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    AutomationElement item = null;
                    do
                    {
                        item = itemContainerPattern.FindItemByProperty(item, null, null);
                        if (item != null)
                        {
                            allItems.Add(item.AsRichItemContainerAutomation());
                        }
                    }
                    while (item != null);
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichItemContainerAutomation[] SelectedItems
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out var selectionPattern))
                {
                    var allItems = new List<RichItemContainerAutomation>();
                    foreach (var selection in selectionPattern.Selection.ValueOrDefault)
                    {
                        if (selection != null)
                        {
                            allItems.Add(selection.AsRichItemContainerAutomation());
                        }
                    }
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichItemContainerAutomation SelectedItem
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out var selectionPattern) && !selectionPattern.CanSelectMultiple)
                {
                    return selectionPattern.Selection.ValueOrDefault.SingleOrDefault().AsRichItemContainerAutomation();
                }
                return null;
            }
        }

        public RichItemsControlData RichItemsControlData => Patterns.Value.Pattern.Value.Value.AsRichItemsControlData();

        public IScrollPattern ScrollInfo => Patterns.Scroll.PatternOrDefault;

        public RichItemsControlAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public void ScrollByArrowKeyOrButton(ScrollingMode scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
            {
                if (scrollingMode == ScrollingMode.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                }
                else if (scrollingMode == ScrollingMode.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                }
                else if (scrollingMode == ScrollingMode.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == ScrollingMode.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public void ScrollByPage(ScrollingMode scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
            {
                if (scrollingMode == ScrollingMode.Up)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                }
                else if (scrollingMode == ScrollingMode.Down)
                {
                    scrollPattern.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                }
                else if (scrollingMode == ScrollingMode.Left)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
                }
                else if (scrollingMode == ScrollingMode.Right)
                {
                    scrollPattern.Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
                }
            }
        }

        public void ScrollByScrollbarsDragging(ScrollingMode scrollingMode)
        {
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
            {
                if (scrollingMode == ScrollingMode.Up)
                {
                    scrollPattern.SetScrollPercent(0, -1);
                }
                else if (scrollingMode == ScrollingMode.Down)
                {
                    scrollPattern.SetScrollPercent(0, 1);
                }
                else if (scrollingMode == ScrollingMode.Left)
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
            if (Patterns.Scroll.TryGetPattern(out var scrollPattern))
            {
                scrollPattern.SetScrollPercent(horizontalOffset, verticalOffset);
            }
        }
    }
}
