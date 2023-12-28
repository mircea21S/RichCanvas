using RichCanvasTestApp.ViewModels.Base;

namespace RichCanvasTestApp.ViewModels
{
    public class TextVisual : Drawable
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
    }
}
