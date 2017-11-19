using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace CodeFlowLibrary.Custom_Controls {

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CodeFlowBuilder.Custom_Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:CodeFlowBuilder.Custom_Controls;assembly=CodeFlowBuilder.Custom_Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ImageButton/>
    ///
    /// </summary>
    /// <remarks>
    /// 
    //++ ImageButton
    ///
    //+ Purpose:
    ///
    ///     Implementation of a button that uses three images for the three states of the button: the default state,
    ///     the pressed state, and the hover state.
    ///     
    //+ Notes:
    ///
    ///     The button has three string properties that can be set to set the images for the three recognized states
    ///     of the button.
    ///     
    ///     These properties are:
    ///     
    ///     1) DefaultImage
    ///     2) PressedImage
    ///     3) HoverImage
    ///     
    ///     Set the properties to the string representation of the image uri. Set the image uri strings and the ImageButton
    ///     does the rest.
    ///     
    /// </remarks>
    public class ImageButton : Button {

        #region Construction

            #region Static

        /// <summary>
        /// Type construction.
        /// </summary>
        static ImageButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

            #endregion

            #region Instance

        /// <summary>
        /// Instance construction
        /// </summary>
        public ImageButton() {
            SetEventHooks();
        }

            #endregion

        #endregion

        #region Fields

        /// <summary>
        /// Path to the three images for the three states of the button.
        /// </summary>
        string _defaultImageUri;
        string _pressedImageUri;
        string _hoverImageUri;

        /// <summary>
        /// The image control that displays the images for the ImageButton.
        /// </summary>
        Image _imageControl;

        /// <summary>
        /// The images that will be displayed in the image control based on 
        /// the state of the ImageButton
        /// </summary>
        BitmapImage _defaultImage;
        BitmapImage _pressedImage;
        BitmapImage _hoverImage;

        #endregion

        #region Properties

        /// <summary>
        /// Get/Set the value of the default image.
        /// </summary>
        public string DefaultImage {
            get { return _defaultImageUri; }
            set {
                VerifyAccess();
                _defaultImageUri = value;
                _defaultImage = LoadImage(value);
            }
        }

        /// <summary>
        /// Get/Set the image to use when the button is pressed.
        /// </summary>
        public string PressedImage {
            get { return _pressedImageUri; }
            set {
                VerifyAccess();
                _pressedImageUri = value;
                _pressedImage = LoadImage(value);
            }
        }

        /// <summary>
        /// Get/Set the image to user when the mouse is over the button.
        /// </summary>
        public string HoverImage {
            get { return _hoverImageUri; }
            set {
                VerifyAccess();
                _hoverImageUri = value;
                _hoverImage = LoadImage(value);
            }
        }

        #endregion

        #region Event Handlers

            #region Setup

        /// <summary>
        /// Sets up the event handlers that are hooked for an image button.
        /// </summary>
        private void SetEventHooks() {
            this.Loaded += Button_Loaded;            
        }

        /// <summary>
        /// Create a new image control, set alignment properties and add the image control
        /// to the Content property of the ImageButton.
        /// </summary>
        private void LoadImageControl() {
            _imageControl = new Image();

            _imageControl.VerticalAlignment = VerticalAlignment.Center;
            _imageControl.HorizontalAlignment = HorizontalAlignment.Center;

            _imageControl.Source = _defaultImage;

            this.AddChild(_imageControl);
        }

            #endregion

            #region Loaded

        /// <summary>
        /// Handles the Loaded event.
        /// </summary>
        /// <remarks>
        /// Sets the control template for the button. The control template for an ImageButton 
        /// is just like the control template for a normal button except that an ImageButton
        /// does not include a border outside of the content presenter.
        /// </remarks>
        private void Button_Loaded(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            string template =
            "<ControlTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'" +
                " TargetType=\"Button\">" +
                " <Border> " +
                     " <ContentPresenter/> " +
                " </Border> " +
            " </ControlTemplate> ";
            button.Template = (ControlTemplate)XamlReader.Parse(template);

            LoadImageControl();
        }

            #endregion

            #region Mouse Event Handlers

        /// <summary>
        /// Handle the mouse enter event.
        /// </summary>
        /// <remarks>
        /// When the mouse enters the Image button, set the image source to _hoverImage.
        /// </remarks>
        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            if (e.Handled) return;
            _imageControl.Source = _hoverImage;
            e.Handled = true;
        }

        /// <summary>
        /// Handle the mouse leave event.
        /// </summary>
        /// <remarks>
        /// When the mouse leaves the image button, set the image source to _defaultImage.
        /// </remarks>
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            _imageControl.Source = _defaultImage;            
        }

        /// <summary>
        /// Handle the mouse left button down event.
        /// </summary>
        /// <remarks>
        /// When the left button is pressed, capture the mouse and set the image source to _pressedImage.
        /// </remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (!e.Handled
             && CaptureMouse()) {
                _imageControl.Source = _pressedImage;
                e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Handle the left mouse button up event.
        /// </summary>
        /// <remarks>
        /// When the left mouse button is released, release the mouse capture if it is captured. 
        /// If the mouse is not captured, then exit the handler : the button wasn't pressed when the
        /// mouse was over the image button so the image source was never changed. The location of
        /// the mouse needs to be checked before setting the source image. If the mouse is over the
        /// Image button set the image source to _defaultImage, otherwise set the image source to
        /// _defaultImage.
        /// </remarks>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            _imageControl.Source = IsMouseOver
                                 ? _hoverImage
                                 : _defaultImage;
        }

            #endregion

        #endregion

        #region Private Methods

            #region Bitmap Images

        /// <summary>
        /// Return a new bitmap image loaded from the input uri.
        /// </summary>
        BitmapImage LoadImage(string uri) {
            var _uri   = new Uri(uri);
            var result = new BitmapImage(_uri);
            return result;
        }

            #endregion

        #endregion

    }

}
