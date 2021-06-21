using RichCanvasDemo.ViewModels.Base;

namespace RichCanvasDemo.ViewModels
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
