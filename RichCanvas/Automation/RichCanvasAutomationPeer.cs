using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.Automation
{
    /// <summary>
    /// Exposes the <see cref="RichCanvas"/> to UI Automation project.
    /// </summary>
    public class RichCanvasAutomationPeer : SelectorAutomationPeer,
        IValueProvider,
        IScrollProvider
    //ITransformProvider
    {
        /// <summary>
        /// Gets the <see cref="RichCanvas"/> that is associated with this <see cref="RichCanvasAutomationPeer"/>.
        /// </summary>
        protected RichCanvas OwnerRichCanvas => (RichCanvas)Owner;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets the serialized json value of <see cref="RichCanvasData"/> containing data about the associated <see cref="RichCanvas"/>.
        /// </summary>
        public string Value => JsonConvert.SerializeObject(new RichCanvasData
        {
            TranslateTransformX = OwnerRichCanvas.TranslateTransform.X,
            TranslateTransformY = OwnerRichCanvas.TranslateTransform.Y,
            ItemsExtent = OwnerRichCanvas.ItemsExtent,
            ScrollFactor = OwnerRichCanvas.ScrollFactor,
            ViewportLocation = OwnerRichCanvas.ViewportLocation,
            ViewportSize = OwnerRichCanvas.ViewportSize,
            ViewportExtent = new System.Windows.Size(OwnerRichCanvas.ScrollInfo.ExtentWidth, OwnerRichCanvas.ScrollInfo.ExtentHeight),
            ViewportZoom = OwnerRichCanvas.ViewportZoom,
            ScaleFactor = OwnerRichCanvas.ScaleFactor,
            MousePosition = OwnerRichCanvas.MousePosition,
            MaxZoom = OwnerRichCanvas.MaxScale,
            MinZoom = OwnerRichCanvas.MinScale
        });

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// Returns: Always true.
        /// </summary>
        public bool HorizontallyScrollable => true;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.HorizontalOffset"/> value.
        /// </summary>
        public double HorizontalScrollPercent => OwnerRichCanvas.HorizontalOffset;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.ViewportSize"/>.Width value.
        /// </summary>
        public double HorizontalViewSize => OwnerRichCanvas.ViewportSize.Width;

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// Returns: Always true.
        /// </summary>
        public bool VerticallyScrollable => true;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.VerticalOffset"/> value.
        /// </summary>
        public double VerticalScrollPercent => OwnerRichCanvas.VerticalOffset;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.ViewportSize"/>.Height value.
        /// </summary>
        public double VerticalViewSize => OwnerRichCanvas.ViewportSize.Height;

        /// <summary>
        /// Initializes a new <see cref="RichCanvasAutomationPeer"/>.
        /// </summary>
        /// <param name="owner"></param>
        public RichCanvasAutomationPeer(RichCanvas owner) : base(owner)
        {
        }

        /// <inheritdoc/>
        public void SetValue(string value)
        {
            //TODO: maybe deserialize a json form a specific types with allowed dependency props
            //      that are modifiable and serializable
            throw new System.NotSupportedException("This control does not allow setting the value.");
        }

        /// <inheritdoc/>
        public override object GetPattern(PatternInterface patternInterface) => patternInterface switch
        {
            PatternInterface.Value => this,
            PatternInterface.Scroll => this,
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

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// Using <see cref="OwnerRichCanvas"/> implementation of <see cref="System.Windows.Controls.Primitives.IScrollInfo"/>.
        /// </summary>
        public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (verticalAmount == ScrollAmount.SmallIncrement)
            {
                OwnerRichCanvas.LineDown();
            }
            if (verticalAmount == ScrollAmount.SmallDecrement)
            {
                OwnerRichCanvas.LineUp();
            }
            if (verticalAmount == ScrollAmount.LargeIncrement)
            {
                OwnerRichCanvas.PageDown();
            }
            if (verticalAmount == ScrollAmount.LargeDecrement)
            {
                OwnerRichCanvas.PageUp();
            }

            if (horizontalAmount == ScrollAmount.SmallIncrement)
            {
                OwnerRichCanvas.LineLeft();
            }
            if (horizontalAmount == ScrollAmount.SmallDecrement)
            {
                OwnerRichCanvas.LineRight();
            }
            if (horizontalAmount == ScrollAmount.LargeIncrement)
            {
                OwnerRichCanvas.PageLeft();
            }
            if (horizontalAmount == ScrollAmount.LargeDecrement)
            {
                OwnerRichCanvas.PageRight();
            }
        }

        /// <summary>
        /// Sets the amount of vertical and horizontal offset on the <see cref="OwnerRichCanvas"/>.
        /// </summary>
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            OwnerRichCanvas.SetVerticalOffset(OwnerRichCanvas.VerticalOffset + verticalPercent);
            OwnerRichCanvas.SetHorizontalOffset(OwnerRichCanvas.HorizontalOffset + horizontalPercent);
        }
    }
}
