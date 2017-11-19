using CodeFlowLibrary.Custom_Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System;

namespace CodeFlowLibrary.View {
    /// <summary>
    /// Interaction logic for VisualNodeView.xaml
    /// </summary>
    public partial class DefaultNodeView : DraggableNodeView {
        #region Construction

        /// <summary>
        /// Calls InitializeCompent, and then creates an ID for this node.
        /// </summary>
        public DefaultNodeView() {
            InitializeComponent();
        }

        #endregion

        #region Fields

        #endregion

        #region Protected

        /// <summary>
        /// All classes that derive from NodeView must return the canvas that represents the view area.
        /// </summary>
        protected override Canvas GetViewCanvas() {
            return cvsMain;
        }

        #endregion

        #region NodeView

        protected override void OnModelPropertyChanged(string propertyName, object propertyValue) {
            switch (propertyName) {
                case "Background":
                    var backgroundBrush = new SolidColorBrush((Color)propertyValue);
                    Background               = backgroundBrush;
                    dlName.Background        = backgroundBrush;
                    dlDescription.Background = backgroundBrush;
                    break;
                case "Foreground":
                    var foregroundBrush = new SolidColorBrush((Color)propertyValue);
                    Foreground               = foregroundBrush;
                    dlName.Foreground        = foregroundBrush;
                    dlDescription.Foreground = foregroundBrush;
                    break;
                case "Name":
                    dlName.Text = (string)propertyValue;
                    break;
                case "Description":
                    dlDescription.Text = (string)propertyValue;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Mouse Events

        private void cvsMain_LButtonDown(object sender, MouseButtonEventArgs e) {

        }

        private void cvsMain_LButtonUp(object sender, MouseButtonEventArgs e) {

        }

        private void cvsMain_MouseMove(object sender, MouseEventArgs e) {

        }

        /// <summary>
        /// Handle the mouse enter event.
        /// </summary>
        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Handle the mouse leave event.
        /// </summary>
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
        }

        #endregion

        #region Button Events

        #endregion

        #region Context Menu Event Handlers

        private void cmNewChild_Click(object sender, RoutedEventArgs e) {
        }

        private void cmNewLink_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region Private Methods
        

        #endregion


    }
}
