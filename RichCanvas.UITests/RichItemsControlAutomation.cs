using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using Newtonsoft.Json;
using RichCanvas.Automation.ControlInformations;
using System.Collections.Generic;
using System.Drawing;

namespace RichCanvas.UITests
{
    public class RichItemsControlAutomation : AutomationElement
    {
        public RichItemsControlAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        public Size DemoControlSize => new Size(1187, 800);

        public Point ViewportCenter => new Point(DemoControlSize.Width / 2, DemoControlSize.Height / 2);

        public RichItemsControlData RichItemsControlData => JsonConvert.DeserializeObject<RichItemsControlData>(Patterns.Value.Pattern.Value.Value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        });

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

    }
}
