using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Newtonsoft.Json;

using RichCanvas.UIAutomation.Core.ControlInformations;

namespace RichCanvas.UIAutomation.Core
{
    /// <summary>
    /// Exposes the <see cref="RichCanvas"/> to UI Automation project.
    /// </summary>
    /// <remarks>
    /// Initializes a new <see cref="RichCanvasAutomationPeer"/>.
    /// </remarks>
    /// <param name="owner"></param>
    public partial class RichCanvasAutomationPeer(RichCanvasAutomation owner) : SelectorAutomationPeer(owner), IValueProvider
    {
        /// <summary>
        /// Gets the <see cref="RichCanvas"/> that is associated with this <see cref="RichCanvasAutomationPeer"/>.
        /// </summary>
        protected RichCanvasAutomation OwnerRichCanvas => (RichCanvasAutomation)Owner;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets the serialized json value of <see cref="RichCanvasData"/> containing data about the associated <see cref="RichCanvas"/>.
        /// </summary>
        public string Value => JsonConvert.SerializeObject(new RichCanvasData
        {
            TranslateTransformX = OwnerRichCanvas.ExposedTranslateTransform.X,
            TranslateTransformY = OwnerRichCanvas.ExposedTranslateTransform.Y,
            ItemsExtent = OwnerRichCanvas.ItemsExtent,
            ScrollFactor = OwnerRichCanvas.ScrollFactor,
            ViewportLocation = OwnerRichCanvas.ViewportLocation,
            ViewportSize = OwnerRichCanvas.ViewportSize,
            ViewportExtent = new System.Windows.Size(OwnerRichCanvas.ExposedScrollInfo.ExtentWidth, OwnerRichCanvas.ExposedScrollInfo.ExtentHeight),
            ViewportZoom = OwnerRichCanvas.ViewportZoom,
            ScaleFactor = OwnerRichCanvas.ScaleFactor,
            MousePosition = OwnerRichCanvas.MousePosition,
            MaxZoom = OwnerRichCanvas.MaxScale,
            MinZoom = OwnerRichCanvas.MinScale,
            RealTimeDraggingEnabled = OwnerRichCanvas.RealTimeDraggingEnabled,
            RealTimeSelectionEnabled = OwnerRichCanvas.RealTimeSelectionEnabled,
            CanSelectMultipleItems = OwnerRichCanvas.CanSelectMultipleItems,
        });

        /// <inheritdoc/>
        public void SetValue(string value)
        {
            RichCanvasData? richCanvasData = JsonConvert.DeserializeObject<RichCanvasData>(value, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            if (richCanvasData == null)
            {
                return;
            }
            OwnerRichCanvas.RealTimeDraggingEnabled = richCanvasData.RealTimeDraggingEnabled;
            OwnerRichCanvas.ViewportLocation = richCanvasData.ViewportLocation;
            OwnerRichCanvas.ScrollFactor = richCanvasData.ScrollFactor;
            OwnerRichCanvas.RealTimeSelectionEnabled = richCanvasData.RealTimeSelectionEnabled;
            OwnerRichCanvas.CanSelectMultipleItems = richCanvasData.CanSelectMultipleItems;
        }

        /// <inheritdoc/>
        public override object GetPattern(PatternInterface patternInterface) => patternInterface switch
        {
            PatternInterface.Value => this,
            PatternInterface.Scroll => this,
            PatternInterface.Transform => this,
            _ => base.GetPattern(patternInterface)
        };

        /// <inheritdoc/>
        protected override AutomationControlType GetAutomationControlTypeCore()
            => AutomationControlType.Custom;

        /// <inheritdoc/>
        protected override string GetClassNameCore() => Owner.GetType().Name;

        /// <inheritdoc/>
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
            => new RichCanvasContainerAutomationPeer(item, this);
    }
}
