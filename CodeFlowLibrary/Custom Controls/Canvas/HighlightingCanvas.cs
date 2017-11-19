using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CodeFlowLibrary.Custom_Controls {
    
    /// <summary>
    //++ HighlightingCanvas
    ///
    //+  Purpose:
    ///     A canvas that will highlight with a dotted border when the mouse enters the control.
    /// </summary>
    public class HighlightingCanvas : CanvasWithCanvasParent {

        #region Construction

        static HighlightingCanvas() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightingCanvas), new FrameworkPropertyMetadata(typeof(HighlightingCanvas)));
        }

        /// <summary>
        /// Set the HighlightEnabled property to the default of true.
        /// Create the _border object.
        /// </summary>
        public HighlightingCanvas() {
            HighlightEnabled = true;
            _border = new Rectangle();
            Loaded += OnLoaded;
            SizeChanged += OnSizeChanged;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The border that sits on the inside edge of the control that is used to
        /// highlight the control when the mouse enters it.
        /// </summary>
        Rectangle _border;

        /// <summary>
        /// The color of the border when the mouse is within the boundaries of the control.
        /// i.e. the border is Highlighting the control.
        /// </summary>
        Brush _onBrush;

        /// <summary>
        /// The color of the border when the mouse is outside of the control. i.e. the color
        /// of the control's border when the highlighting is off.
        /// </summary>
        Brush _offBrush;

        #endregion

        #region Properties

        /// <summary>
        /// Is Highlighting enabled for this control?
        /// Defaults to true.
        /// </summary>
        public bool HighlightEnabled { get; set;}

        #endregion

        #region Mouse Overrides

        protected override void OnMouseEnter(MouseEventArgs e) {
            _border.Stroke = _onBrush;
        }

        protected override void OnMouseLeave(MouseEventArgs e) {
            _border.Stroke = _offBrush;
        }

        #endregion

        #region Other Overrides

        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            _border.Height = ActualHeight;
            _border.Width = ActualWidth;
        }

        #endregion

        #region Loaded Event Handler

        private void OnLoaded(object sender, RoutedEventArgs e) {
            _border.Stroke = Background;
            _border.StrokeThickness = 1;
            _border.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });
            _border.Height = ActualHeight;
            _border.Width = ActualWidth;
            this.Children.Add(_border);

            _offBrush = Background;
            _onBrush = GetInvertedBrush(((SolidColorBrush)_offBrush).Color);
        }

        #endregion

        #region Private methods

        Brush GetInvertedBrush(Color c) {

            if (IsGray(c)) return Brushes.White;

            var newColor = new Color();           

            newColor.A = 255;
            newColor.R = (byte)(255 - c.R);
            newColor.B = (byte)(255 - c.B);
            newColor.G = (byte)(255 - c.G);

            return new SolidColorBrush(newColor);
        }

        bool IsGray(Color c) {

            var delta = Math.Abs(128 - c.R) +
                        Math.Abs(128 - c.G) +
                        Math.Abs(128 - c.B);

            return delta < 30;
        }

        #endregion

    }
}
