namespace RichCanvasUIA.Client.UIA_Mode.IPC_Pipe.Handlers
{
    internal class SelectionHandler(RichCanvasClientUIAModeViewModel clientUIAModeViewModel) : PipeHandlerDefinition(clientUIAModeViewModel)
    {
        internal override void Process(string pipeDataName)
        {
            if (pipeDataName == PipeHandlerNames.Selection.AddSelectableItems)
            {
                foreach (var item in PreDefinedAutomationItemModels.SelectableItems)
                {
                    RichCanvasAutomationModel.Items.Add(item);
                }
            }
            else if (pipeDataName == PipeHandlerNames.Selection.AddConsecutiveItemsForRealTimeSelection)
            {
                foreach (var item in PreDefinedAutomationItemModels.VisuallyConsecutiveItemsForRealTimeSelection)
                {
                    RichCanvasAutomationModel.Items.Add(item);
                }
            }
            else if (pipeDataName == PipeHandlerNames.Selection.AddSingleSelectionTestItems)
            {
                foreach (var item in PreDefinedAutomationItemModels.SelectableItemsForSingleSelection)
                {
                    RichCanvasAutomationModel.Items.Add(item);
                }
            }
        }
    }
}
