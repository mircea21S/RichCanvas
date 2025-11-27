using System.Windows.Automation;
using System.Windows.Automation.Provider;

namespace RichCanvas.UIAutomation.Core
{
    public partial class RichCanvasAutomationPeer : IScrollProvider
    {
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
