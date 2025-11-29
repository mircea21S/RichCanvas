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
using RichCanvas.UIAutomation.Tests.Extensions;

namespace RichCanvas.UIAutomation.Tests
{
    public partial class RichCanvasAutomation : AutomationElement
    {
        public Point ViewportLocation
        {
            get => GetRichCanvasSettings().ViewportLocation.AsDrawingPoint();
            set => SetValue(value.AsWindowsPoint());
        }

        public System.Windows.Size ViewportSize => GetRichCanvasSettings().ViewportSize;

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

        public RichCanvasData GetRichCanvasSettings()
            => JsonConvert.DeserializeObject<RichCanvasData>(Patterns.Value.Pattern.Value.Value, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

        public Window ParentWindow { get; internal set; }

        public bool RealTimeDraggingEnabled
        {
            get => GetRichCanvasSettings().RealTimeDraggingEnabled;
            set => SetValue(value);
        }

        public bool RealTimeSelectionEnabled
        {
            get => GetRichCanvasSettings().RealTimeSelectionEnabled;
            set => SetValue(value);
        }

        public bool CanSelectMultipleItems
        {
            get => GetRichCanvasSettings().CanSelectMultipleItems;
            set => SetValue(value);
        }

        public RichCanvasAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : base(frameworkAutomationElement)
        {
        }

        private void SetValue(object value, [CallerMemberName] string propertyName = default)
        {
            var containerInfoClone = (RichCanvasData)GetRichCanvasSettings().Clone();
            PropertyInfo property = containerInfoClone.GetType().GetProperty(propertyName);
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }
}
