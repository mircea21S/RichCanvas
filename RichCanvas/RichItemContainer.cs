using RichCanvas.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RichCanvas
{
    [TemplatePart(Name = ContentPresenterName, Type = typeof(ContentPresenter))]
    public class RichItemContainer : ContentControl
    {
        private const string ContentPresenterName = "PART_ContentPresenter";

        public static DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(OnIsSelectedChanged));
        /// <summary>
        /// Gets or sets a value that indicates whether this node is selected.
        /// Can only be set if <see cref="IsSelectable"/> is true.
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static DependencyProperty TopProperty = DependencyProperty.Register(nameof(Top), typeof(double), typeof(RichItemContainer), new FrameworkPropertyMetadata(OnPositionChanged));
        /// <summary>
        /// Gets or sets the Top position of this <see cref="RichItemContainer"/> on <see cref="RichItemsControl.ItemsHost"/>
        /// </summary>
        public double Top
        {
            get => (double)GetValue(TopProperty);
            set => SetValue(TopProperty, value);
        }

        public static DependencyProperty LeftProperty = DependencyProperty.Register(nameof(Left), typeof(double), typeof(RichItemContainer), new FrameworkPropertyMetadata(OnPositionChanged));
        /// <summary>
        /// Gets or sets the Left position of this <see cref="RichItemContainer"/> on <see cref="RichItemsControl.ItemsHost"/>
        /// </summary>
        public double Left
        {
            get => (double)GetValue(LeftProperty);
            set => SetValue(LeftProperty, value);
        }

        public static DependencyProperty IsSelectableProperty = DependencyProperty.Register(nameof(IsSelectable), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> can be selected.
        /// True by default
        /// </summary>
        public bool IsSelectable
        {
            get => (bool)GetValue(IsSelectableProperty);
            set => SetValue(IsSelectableProperty, value);
        }

        public static DependencyProperty IsDraggableProperty = DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether this <see cref="RichItemContainer"/> can be dragged on <see cref="RichItemsControl.ItemsHost"/>
        /// True by default
        /// </summary>
        public bool IsDraggable
        {
            get => (bool)GetValue(IsDraggableProperty);
            set => SetValue(IsDraggableProperty, value);
        }

        public static DependencyProperty ShouldBringIntoViewProperty = DependencyProperty.Register(nameof(ShouldBringIntoView), typeof(bool), typeof(RichItemContainer), new FrameworkPropertyMetadata(false, OnBringIntoViewChanged));
        /// <summary>
        /// Gets or sets whethere this <see cref="RichItemContainer"/> should be centered inside <see cref="RichItemsControl.ScrollContainer"/> viewport
        /// </summary>
        public bool ShouldBringIntoView
        {
            get => (bool)GetValue(ShouldBringIntoViewProperty);
            set => SetValue(ShouldBringIntoViewProperty, value);
        }

        static RichItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichItemContainer), new FrameworkPropertyMetadata(typeof(RichItemContainer)));
        }

        internal bool IsDrawn { get; set; }

        internal RichItemsControl Host { get; set; }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, true);
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (IsDraggable)
            {
                DragBehavior.SetIsDragging((RichItemContainer)e.OriginalSource, false);
            }
        }

        internal bool IsValid()
        {
            return Height != 0 && Width != 0 && !double.IsNaN(Height) && !double.IsNaN(Width);
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((RichItemContainer)d).UpdatePosition();

        private void UpdatePosition()
        {
            Host?.ItemsHost.InvalidateMeasure();
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((RichItemContainer)d).Host.AddSelection((RichItemContainer)d);
            }
        }

        private static void OnBringIntoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((RichItemContainer)d).BringIntoView();
            }
        }
    }
}
