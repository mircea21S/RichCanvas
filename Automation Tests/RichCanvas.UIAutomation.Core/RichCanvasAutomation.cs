using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace RichCanvas.UIAutomation.Core
{
    public class RichCanvasAutomation : RichCanvas
    {
        public TranslateTransform ExposedTranslateTransform => TranslateTransform;
        public IScrollInfo ExposedScrollInfo => ScrollInfo;

        protected override AutomationPeer OnCreateAutomationPeer()
            => new RichCanvasAutomationPeer(this);
    }
}
