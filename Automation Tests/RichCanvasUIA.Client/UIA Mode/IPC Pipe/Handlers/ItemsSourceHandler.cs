using RichCanvasUIA.Client.UIA_Mode.Automation_Models;

namespace RichCanvasUIA.Client.UIA_Mode.IPC_Pipe.Handlers
{
    internal class ItemsSourceHandler(RichCanvasClientUIAModeViewModel clientUIAModeViewModel) : PipeHandlerDefinition(clientUIAModeViewModel)
    {
        internal override void Process(string pipeDataName)
        {
            switch (pipeDataName)
            {
                case PipeHandlerNames.ItemsSource.RemoveFirstItem:
                    RichCanvasAutomationModel.Items.RemoveAt(0);
                    break;

                case PipeHandlerNames.ItemsSource.MoveFirstItemToTheEnd:
                    RichCanvasAutomationModel.Items.Move(0, RichCanvasAutomationModel.Items.Count - 1);
                    break;

                case PipeHandlerNames.ItemsSource.AddItemTopOutsideViewport:
                    RichCanvasAutomationModel.Items.Add(new RichCanvasContainerAutomationModel
                    {
                        Top = RichCanvasAutomationModel.ViewportLocation.Y - 100,
                        Left = RichCanvasAutomationModel.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemLeftOutsideViewport:
                    RichCanvasAutomationModel.Items.Add(new RichCanvasContainerAutomationModel
                    {
                        Top = RichCanvasAutomationModel.ViewportLocation.Y + 100,
                        Left = RichCanvasAutomationModel.ViewportLocation.X - 100,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemBottomOutsideViewport:
                    RichCanvasAutomationModel.Items.Add(new RichCanvasContainerAutomationModel
                    {
                        Top = RichCanvasAutomationModel.ViewportSize.Height,
                        Left = RichCanvasAutomationModel.ViewportLocation.X + 10,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.AddItemRightOutsideViewport:
                    RichCanvasAutomationModel.Items.Add(new RichCanvasContainerAutomationModel
                    {
                        Top = RichCanvasAutomationModel.ViewportLocation.Y + 10,
                        Left = RichCanvasAutomationModel.ViewportSize.Width,
                        Width = 100,
                        Height = 100
                    });
                    break;

                case PipeHandlerNames.ItemsSource.ClearAllItems:
                    RichCanvasAutomationModel.Items.Clear();
                    break;
            }
        }
    }
}
