using FlaUI.Core;
using FlaUI.Core.Input;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.UITests.Tests;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace RichCanvas.UITests
{
    [TestFixture]
    public class RichItemContainerTests
    {
        [Test]
        public void SelectItem_ContainerIsSelected()
        {
            using (var automation = new UIA3Automation())
            {
                using (var app = Application.Launch(new ProcessStartInfo
                {
                    FileName = RichCanvasTestAppTest.RichCanvasDemoBinPath
                }))
                {
                    app.WaitWhileMainHandleIsMissing();

                    // arrange
                    var abd = new System.Windows.Input.MouseGesture();
                    Mouse.Click(new Point(100, 100));
                }
            }
        }
    }
}
