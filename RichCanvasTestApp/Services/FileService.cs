using Microsoft.Win32;

namespace RichCanvasTestApp.Services
{
    public class FileService
    {
        public void OpenFileDialog(out string selectedPath)
        {
            selectedPath = null;
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*jpg)|*.png;*.jpeg;*jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                selectedPath = openFileDialog.FileName;
            }
        }
    }
}
