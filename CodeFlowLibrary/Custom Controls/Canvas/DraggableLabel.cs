using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeFlowLibrary.Custom_Controls
{    
     
    /// <summary>
    //+ DraggableLabel
    /// 
    //+ Purpose:
    ///     Implements a label that is both draggable ad re-sizable within the confines of
    ///     a parent Canvas.
    /// </summary>
    /// <remarks>
    /// Most of the functionality of this control is handled by the DraggableCanvas base class.
    /// DraggableLabel adds the Label, along with the style properties such as font, background, etc.
    /// </remarks>
    public class DraggableLabel : DraggableCanvas
    {
        #region Static Construction

        /// <summary>
        /// Auto Generated.
        /// </summary>
        static DraggableLabel() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DraggableLabel), new FrameworkPropertyMetadata(typeof(DraggableLabel)));
        }

        #endregion

        #region Instance Construction

        /// <summary>
        /// Create the children and hook the SizeChanged event.
        /// </summary>
        public DraggableLabel() {
            CreateChildren();
            SizeChanged += OnSizeChanged;
            _label.FontSize = 20;
            _label.Padding = new Thickness(0);
            _label.Margin  = new Thickness(0);
        }

        #endregion

        #region Fields

        /// <summary>
        /// The label that will display the text for this control.
        /// </summary>
        Label _label;

        Viewbox _viewBox;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the text displayed in the control.
        /// </summary>
        public string Text {
            get { return (string)_label.Content; }
            set { _label.Content = value; }
        }

        /// <summary>
        /// Get/Set the brush used as the text color.
        /// </summary>
        public Brush Foreground {
            get { return _label.Foreground; }
            set { _label.Foreground = value; }
        }

        /// <summary>
        /// Get/Set the font family of the used to draw the text in the
        /// control.
        /// </summary>
        public FontFamily Font {
            get { return _label.FontFamily; }
            set { _label.FontFamily = value; }
        }

        /// <summary>
        /// Get/Set the size of the text used.
        /// </summary>
        public double FontSize {
            get { return _label.FontSize; }
            set { _label.FontSize = value; }
        }

        /// <summary>
        /// Get/Set the FontWeight.
        /// </summary>
        public FontWeight FontWeight {
            get { return _label.FontWeight; }
            set { _label.FontWeight = value; }
        }

        #endregion

        #region Event Handlers
        
        /// <summary>
        /// To make the label always fill the canvas, hook the SizeChanged event,
        /// setting the size of the label to the actual width and height of the
        /// canvas every time the size of the canvas changes.
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e) {
            //_label.Width = ActualWidth;
            //_label.Height = ActualHeight;
            _viewBox.Height = ActualHeight;
            _viewBox.Width = ActualWidth;
            
        }

        /// <summary>
        /// In order to make sure that the options button is not shown when the
        /// control is fist loaded, hook the Initialized event an set the visibility
        /// of the button to collapsed.
        /// </summary>
        protected override void OnInitialized(EventArgs e) {
            OnSizeChanged(null, null);
        }

        #endregion

        #region Private Methods

            #region Create Children

        /// <summary>
        /// Create the button and the label.
        /// </summary>
        void CreateChildren() {
            InstantiateChildren();
            InitializeViewbox();
            InitializeLabel();
        }

        /// <summary>
        /// Create the button and the label controls.
        /// </summary>
        void InstantiateChildren() {
            _label = new Label();
            _viewBox = new Viewbox();
        } 

        void InitializeViewbox() {
            Children.Add(_viewBox);
            _viewBox.Stretch = Stretch.Uniform;
        }

        /// <summary>
        /// Initialize the label control.
        /// </summary>
        void InitializeLabel() {
            _viewBox.Child = _label;
            _label.HorizontalContentAlignment = HorizontalAlignment.Center;
            _label.VerticalContentAlignment   = VerticalAlignment.Center;            
            Canvas.SetTop(_label, 0);
            Canvas.SetLeft(_label, 0);
        }

            #endregion

        #endregion
    }
}
