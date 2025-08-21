﻿using RichCanvasTestApp.ViewModels.Base;

namespace RichCanvasTestApp.ViewModels
{
    public class ImageVisual : Drawable
    {
        private string _imageSource;

        public string ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }
    }
}
