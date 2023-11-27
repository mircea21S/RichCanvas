using FlaUI.Core;
using System.Diagnostics;

namespace RichCanvas.UITests.Helpers
{
    public static class ApplicationHelper
    {
        public static Application AttachOrLaunchRichCanvasDemo() =>
            Application.AttachOrLaunch(new ProcessStartInfo
            {
                FileName = ApplicationInfo.RichCanvasDemoBinPath
            });
    }
}
