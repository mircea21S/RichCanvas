using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

using Newtonsoft.Json;

using RichCanvas.Automation.ControlInformations;

namespace RichCanvas.Automation
{
    public class RichItemsControlAutomationPeer : SelectorAutomationPeer,
        IValueProvider,
        IScrollProvider
    //ITransformProvider
    {
        protected RichItemsControl OwnerItemsControl => (RichItemsControl)Owner;

        /// <inheritdoc/>
        public bool IsReadOnly => true;

        /// <inheritdoc/>
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

        public bool HorizontallyScrollable => true;

        public double HorizontalScrollPercent => OwnerItemsControl.HorizontalOffset;

        public double HorizontalViewSize => OwnerItemsControl.ViewportSize.Width;

        public bool VerticallyScrollable => true;

        public double VerticalScrollPercent => OwnerItemsControl.VerticalOffset;

        public double VerticalViewSize => OwnerItemsControl.ViewportSize.Height;

        public RichItemsControlAutomationPeer(RichItemsControl owner) : base(owner)
        {
        }

        /// <inheritdoc/>
        public void SetValue(string value)
        {
            //TODO: maybe deserialize a json form a specific types with allowed dependency props
            //      that are modifiable and serializable
            throw new System.NotSupportedException("This control does not allow setting the value.");
        }

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

        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            OwnerItemsControl.SetVerticalOffset(OwnerItemsControl.VerticalOffset + verticalPercent);
            OwnerItemsControl.SetHorizontalOffset(OwnerItemsControl.HorizontalOffset + horizontalPercent);
        }
    }
}
