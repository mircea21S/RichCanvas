using FlaUI.Core;
using FlaUI.Core.Patterns;
using FlaUI.UIA3;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using RichCanvas.UITests.Helpers;
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
                using (var app = Application.Attach(ApplicationInfo.RichCanvasDemoProcessName))
                {
                    app.WaitWhileMainHandleIsMissing();

                    // arrange
                    var window = app.GetMainWindow(automation);
                    var richItemsControl = window.FindFirstDescendant(d => d.ByAutomationId("source")).AsRichItemsControlAutomation();
                    var pointInsideRichItemsControl = richItemsControl.GetClickablePoint();
                    var rectangleWidth = 100;
                    var rectangleHeight = 100;
                    var rectangleSize = new Point(pointInsideRichItemsControl.X + rectangleWidth, pointInsideRichItemsControl.Y + rectangleHeight);

                    // act
                    RichItemContainerAutomation existingContainer;
                    if (richItemsControl.Items.Any())
                    {
                        existingContainer = richItemsControl.Items[0].AsRichItemContainerAutomation();
                    }
                    else
                    {
                        //window.InvokeAddDrawnRectangleButton();
                        existingContainer = richItemsControl.Items[0].AsRichItemContainerAutomation();
                    }

                    ISelectionItemPattern selectionPattern;
                    if (existingContainer.Patterns.SelectionItem.TryGetPattern(out selectionPattern))
                    {
                        selectionPattern.AddToSelection();
                    }

                    // assert
                    using (new AssertionScope())
                    {
                        richItemsControl.SelectedItems.Length.Should().Be(1);
                        existingContainer.RichItemContainerData.IsSelected.Should().Be(selectionPattern.IsSelected);
                    }
                }
            }
        }
    }
}
