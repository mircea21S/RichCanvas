using System.Windows;
using System.Windows.Automation.Provider;

namespace RichCanvas.UIAutomation
{
    public partial class RichCanvasAutomationPeer : ITransformProvider
    {
        public bool CanMove => true;

        public bool CanResize => false;

        public bool CanRotate => false;

        /// <summary>
        /// Simulates a panning on <see cref="RichCanvas"/>.
        /// </summary>
        /// <param name="x">Amount of panning to be applied on x axis.</param>
        /// <param name="y">Amount of panning to be applied on y axis.</param>
        public void Move(double x, double y)
        {
            OwnerRichCanvas.ViewportLocation -= new Vector(x, y) / OwnerRichCanvas.ViewportZoom;
        }

        public void Resize(double width, double height)
        {
            // do nothing
        }

        public void Rotate(double degrees)
        {
            // do nothing
        }
    }
}
