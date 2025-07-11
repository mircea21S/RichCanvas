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
    public class RichItemsControlAutomationPeer : SelectorAutomationPeer,
        IValueProvider,
        IScrollProvider
    //ITransformProvider
    {
        /// <summary>
        /// Gets the <see cref="RichCanvas"/> that is associated with this <see cref="RichItemsControlAutomationPeer"/>.
        /// </summary>
        protected RichCanvas OwnerItemsControl => (RichCanvas)Owner;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <summary>
        /// Gets the serialized json value of <see cref="RichItemsControlData"/> containing data about the associated <see cref="RichCanvas"/>.
        /// </summary>
        public string Value => JsonConvert.SerializeObject(new RichItemsControlData
        {
            TranslateTransformX = OwnerItemsControl.TranslateTransform.X,
            TranslateTransformY = OwnerItemsControl.TranslateTransform.Y,
            ItemsExtent = OwnerItemsControl.ItemsExtent,
            ScrollFactor = OwnerItemsControl.ScrollFactor,
            ViewportLocation = OwnerItemsControl.ViewportLocation,
            ViewportSize = OwnerItemsControl.ViewportSize,
            ViewportExtent = new System.Windows.Size(OwnerItemsControl.ScrollInfo.ExtentWidth, OwnerItemsControl.ScrollInfo.ExtentHeight),
            ViewportZoom = OwnerItemsControl.ViewportZoom,
            ScaleFactor = OwnerItemsControl.ScaleFactor,
            MousePosition = OwnerItemsControl.MousePosition,
            MaxZoom = OwnerItemsControl.MaxScale,
            MinZoom = OwnerItemsControl.MinScale
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
        public double HorizontalScrollPercent => OwnerItemsControl.HorizontalOffset;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.ViewportSize"/>.Width value.
        /// </summary>
        public double HorizontalViewSize => OwnerItemsControl.ViewportSize.Width;

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// Returns: Always true.
        /// </summary>
        public bool VerticallyScrollable => true;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.VerticalOffset"/> value.
        /// </summary>
        public double VerticalScrollPercent => OwnerItemsControl.VerticalOffset;

        /// <summary>
        /// Gets associated <see cref="RichCanvas.ViewportSize"/>.Height value.
        /// </summary>
        public double VerticalViewSize => OwnerItemsControl.ViewportSize.Height;

        /// <summary>
        /// Initializes a new <see cref="RichItemsControlAutomationPeer"/>.
        /// </summary>
        /// <param name="owner"></param>
        public RichItemsControlAutomationPeer(RichCanvas owner) : base(owner)
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
            => new RichItemContainerAutomationPeer(item, this);

        /// <summary>
        /// <inheritdoc/>
        /// <br/>
        /// Using <see cref="OwnerItemsControl"/> implementation of <see cref="System.Windows.Controls.Primitives.IScrollInfo"/>.
        /// </summary>
        public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
        {
            if (verticalAmount == ScrollAmount.SmallIncrement)
            {
                OwnerItemsControl.LineDown();
            }
            if (verticalAmount == ScrollAmount.SmallDecrement)
            {
                OwnerItemsControl.LineUp();
            }
            if (verticalAmount == ScrollAmount.LargeIncrement)
            {
                OwnerItemsControl.PageDown();
            }
            if (verticalAmount == ScrollAmount.LargeDecrement)
            {
                OwnerItemsControl.PageUp();
            }

            if (horizontalAmount == ScrollAmount.SmallIncrement)
            {
                OwnerItemsControl.LineLeft();
            }
            if (horizontalAmount == ScrollAmount.SmallDecrement)
            {
                OwnerItemsControl.LineRight();
            }
            if (horizontalAmount == ScrollAmount.LargeIncrement)
            {
                OwnerItemsControl.PageLeft();
            }
            if (horizontalAmount == ScrollAmount.LargeDecrement)
            {
                OwnerItemsControl.PageRight();
            }
        }

        /// <summary>
        /// Sets the amount of vertical and horizontal offset on the <see cref="OwnerItemsControl"/>.
        /// </summary>
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            OwnerItemsControl.SetVerticalOffset(OwnerItemsControl.VerticalOffset + verticalPercent);
            OwnerItemsControl.SetHorizontalOffset(OwnerItemsControl.HorizontalOffset + horizontalPercent);
        }
    }
}
