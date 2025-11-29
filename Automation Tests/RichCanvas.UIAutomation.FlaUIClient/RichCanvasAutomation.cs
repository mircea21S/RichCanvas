using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Patterns;

using Newtonsoft.Json;

using RichCanvas.UIAutomation.Core.ControlInformations;
using RichCanvas.UIAutomation.FlaUIClient.Utilities;

namespace RichCanvas.UIAutomation.FlaUIClient
{
    public partial class RichCanvasAutomation(FrameworkAutomationElementBase frameworkAutomationElement) : AutomationElement(frameworkAutomationElement)
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
                if (Patterns.ItemContainer.TryGetPattern(out IItemContainerPattern? itemContainerPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
                    AutomationElement? item = null;
                    do
                    {
                        item = itemContainerPattern?.FindItemByProperty(item, null, null);
                        var itemAutomation = item?.AsRichCanvasContainerAutomation();
                        if (item != null && itemAutomation != null)
                        {
                            allItems.Add(itemAutomation);
                        }
                    }
                    while (item != null);
                    return [.. allItems];
                }
                return [];
            }
        }

        public RichCanvasContainerAutomation[] SelectedItems
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern? selectionPattern))
                {
                    var allItems = new List<RichCanvasContainerAutomation>();
                    foreach (AutomationElement selection in selectionPattern.Selection.ValueOrDefault ?? [])
                    {
                        var itemAutomation = selection?.AsRichCanvasContainerAutomation();

                        if (itemAutomation != null)
                        {
                            allItems.Add(itemAutomation);
                        }
                    }
                    return [.. allItems];
                }
                return [];
            }
        }

        public RichCanvasContainerAutomation? SelectedItem
        {
            get
            {
                if (Patterns.Selection.TryGetPattern(out ISelectionPattern? selectionPattern) && !selectionPattern?.CanSelectMultiple)
                {
                    return selectionPattern?.Selection.ValueOrDefault?.SingleOrDefault()?.AsRichCanvasContainerAutomation();
                }
                return null;
            }
        }

        public Window? ParentWindow { get; set; }

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

        public RichCanvasData GetRichCanvasSettings()
        => JsonConvert.DeserializeObject<RichCanvasData>(Patterns.Value.Pattern.Value.Value, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        }) ?? throw new InvalidOperationException($"Something went wrong when trying to deserialize {nameof(RichCanvasAutomation)} value.");

        private void SetValue(object value, [CallerMemberName] string? propertyName = default)
        {
            var containerInfoClone = (RichCanvasData)GetRichCanvasSettings().Clone();
            PropertyInfo? property = (containerInfoClone.GetType()?.GetProperty(propertyName ?? string.Empty))
                ?? throw new ArgumentException($"Property {propertyName} not found.");
            property.SetValue(containerInfoClone, value);
            Patterns.Value.Pattern.SetValue(JsonConvert.SerializeObject(containerInfoClone));
        }
    }
}
