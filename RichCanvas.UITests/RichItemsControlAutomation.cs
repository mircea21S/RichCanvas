using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using RichCanvas.Automation;
using RichCanvas.Automation.ControlInformations;
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

        public RichItemsControlAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }
    }
}
