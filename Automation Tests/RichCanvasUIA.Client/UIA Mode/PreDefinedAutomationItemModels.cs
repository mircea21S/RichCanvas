using System.Collections.Generic;

using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvasUIA.Client.UIA_Mode
{
    public class PreDefinedAutomationItemModels
    {
        public static RichCanvasContainerAutomationModel ImmutablePositionedRectangleWithoutSize = new()
        {
            AllowScaleChangeToUpdatePosition = false,
            Top = 300,
            Left = 300
        };

        public static RichCanvasContainerAutomationModel MutablePositionedRectangleWithSize = new()
        {
            Top = 100,
            Left = 100,
            Height = 100,
            Width = 100
        };

        public static RichCanvasContainerAutomationModel FullyDrawnRectangle = new()
        {
            Top = 100,
            Left = 100,
            Height = 100,
            Width = 100
        };

        public static List<RichCanvasContainerAutomationModel> SelectableItems =
       [
           new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 150,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 250,
                Left = 250,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 80,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 100,
                Left = 300,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 300,
                Left = 400,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 180,
                Left = 230,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 450,
                Left = 450,
                Height = 50,
                Width = 50
            }
       ];

        public static List<RichCanvasContainerAutomationModel> VisuallyConsecutiveItemsForRealTimeSelection =
        [
            new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 150,
                Left = 150,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 250,
                Left = 250,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 350,
                Left = 350,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 450,
                Left = 450,
                Height = 50,
                Width = 50
            }
        ];

        public static List<RichCanvasContainerAutomationModel> SelectableItemsForSingleSelection =>
        [
            new()
            {
                Top = 50,
                Left = 50,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 150,
                Left = 90,
                Height = 50,
                Width = 50
            },
            new()
            {
                Top = 250,
                Left = 150,
                Height = 50,
                Width = 50
            }
        ];
    }
}
