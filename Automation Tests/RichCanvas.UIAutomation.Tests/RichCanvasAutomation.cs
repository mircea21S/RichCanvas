using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Patterns;

using Newtonsoft.Json;

using RichCanvas.UIAutomation.Core.ControlInformations;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation : AutomationElement
    {
        public Point ViewportLocation
        {
            get => RichCanvasSettings.ViewportLocation.AsDrawingPoint();
            set => SetValue(value.AsWindowsPoint());
        }

        public System.Windows.Size ViewportSize => RichCanvasSettings.ViewportSize;

        public RichCanvasContainerAutomation[] Items
        {
            get
            {
                if (Patterns.ItemContainer.TryGetPattern(out IItemContainerPattern itemContainerPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
                    AutomationElement item = null;
                    do
                    {
                        item = itemContainerPattern.FindItemByProperty(item, null, null);
                        if (item != null)
                        {
                            allItems.Add(item.AsRichCanvasContainerAutomation());
                        }
                    }
                    while (item != null);
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichCanvasContainerAutomation[] SelectedItems
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern selectionPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
                    foreach (AutomationElement selection in selectionPattern.Selection.ValueOrDefault)
                    {
                        if (selection != null)
                        {
                            allItems.Add(selection.AsRichCanvasContainerAutomation());
                        }
                    }
                    return allItems.ToArray();
                }
                return null;
            }
        }

        public RichCanvasContainerAutomation SelectedItem
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern selectionPattern) && !selectionPattern.CanSelectMultiple)
                {
                    return selectionPattern.Selection.ValueOrDefault.SingleOrDefault().AsRichCanvasContainerAutomation();
                }
                return null;
            }
        }

        public RichCanvasData RichCanvasSettings => Patterns.Value.Pattern.Value.Value.AsRichCanvasData();

        public Window ParentWindow { get; internal set; }

        public bool RealTimeDraggingEnabled
        {
            get => RichCanvasSettings.RealTimeDraggingEnabled;
            set => SetValue(value);
        }

        public RichCanvasAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        private void SetValue(object value, [CallerMemberName] string propertyName = default)
        {
            var containerInfoClone = (RichCanvasData)RichCanvasSettings.Clone();
            PropertyInfo property = containerInfoClone.GetType().GetProperty(propertyName);
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }
}
