using CodeFlowLibrary.Custom_Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeFlowLibrary.View
{
    /// <summary>
    /// Interaction logic for VisualNodeView.xaml
    /// </summary>
    public partial class VisualNodeView : DraggableNodeView
    {
        #region Construction

        /// <summary>
        /// Calls InitializeCompent, and then creates an ID for this node.
        /// </summary>
        public VisualNodeView() {
            InitializeComponent();

            _guid = Guid.NewGuid();

            _visualNodeChildren = new List<VisualNodeView>();
        }

        #endregion

        #region NodeViewMode Enumeration

        enum NodeViewMode {
            Edit,
            View
        }

        #endregion

        #region Fields
        
        /// <summary>
        /// Holds the current mode of the control.
        /// </summary>
        NodeViewMode _currentMode;

        /// <summary>
        /// The visual node children. These children display inside of this control when in
        /// view mode.
        /// </summary>
        List<VisualNodeView> _visualNodeChildren;

        /// <summary>
        /// The ID of this node
        /// </summary>
        Guid _guid;

        #endregion

        #region Protected

        /// <summary>
        /// All classes that derive from NodeView must return the canvas that represents the view area.
        /// </summary>
        protected override Canvas GetViewCanvas() {
            return cvsMain;
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
        /// <remarks>
        /// When the mouse enters the VisualNodeView control, the mode button becomes visible.
        /// </remarks>
        protected override void OnMouseEnter(MouseEventArgs e) {
            base.OnMouseEnter(e);
            btnMode.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handle the mouse leave event.
        /// </summary>
        /// <remarks>
        /// When the mouse leaves the VisualNodeView control, the mode button becomes hidden.
        /// </remarks>
        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            btnMode.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Flip the current mode.
        /// </summary>
        private void btnMode_Click(object sender, RoutedEventArgs e) {
            if (_currentMode == NodeViewMode.Edit)
                SetViewMode();
            else
                SetEditMode();
        }

        #endregion

        #region Text Changed Event Handlers

        private void txtName_TextChanged(object sender, TextChangedEventArgs e) {
            dlName.Text = txtName.Text;
        }

        private void txtDescription_TextChanged(object sender, TextChangedEventArgs e) {
            dlDescription.Text = txtDescription.Text;
        }

        #endregion

        #region Context Menu Event Handlers

        private void cmNewChild_Click(object sender, RoutedEventArgs e) {
            var childPosition = Mouse.GetPosition(this);
            AddNewVisualNodeChild(childPosition.X, childPosition.Y);
        }

        private void cmNewLink_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region Private Methods

        #region Mode

        /// <summary>
        /// Set the NodeViewMode to edit.
        /// </summary>
        /// <remarks>
        /// Setting the mode to edit hides the name and description draggable labels to Collapsed,
        /// while setting the visibility of the grdMain grid to Visible.
        /// </remarks>
        void SetEditMode() {
            grdMain.Visibility       = Visibility.Visible;
            dlDescription.Visibility = Visibility.Collapsed;
            dlName.Visibility        = Visibility.Collapsed;

            btnMode.DefaultImage = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/VButton.png";
            btnMode.HoverImage   = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/VButtonHover.png";
            btnMode.PressedImage = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/VButtonPressed.png";

            _currentMode = NodeViewMode.Edit;
        }

        /// <summary>
        /// Set the NodeViewMode to view.
        /// </summary>
        /// <remarks>
        /// Setting the mode to view hides the main grid while setting the visibility of the name
        /// and description draggable labels to Visible.
        /// </remarks>
        void SetViewMode() {
            grdMain.Visibility       = Visibility.Collapsed;
            dlDescription.Visibility = Visibility.Visible;
            dlName.Visibility        = Visibility.Visible;

            btnMode.DefaultImage = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/EButton.png";
            btnMode.HoverImage   = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/EButtonHover.png";
            btnMode.PressedImage = "pack://application:,,,/CodeFlowBuilder;component/Resources/Images/EButtonPressed.png";

            _currentMode = NodeViewMode.View;
        }

            #endregion

            #region Visual Node Children

        /// <summary>
        /// Add a node to the VisualNodeView children of this control. The position of the
        /// new child is specified by the top and left arguments of the method.
        /// </summary>
        protected void AddNewVisualNodeChild(double left, double top) {
            var child = new VisualNodeView();
            _visualNodeChildren.Add(child);
            child.Width = ActualWidth / 4;
            child.Height = ActualHeight / 4;
            cvsMain.Children.Add(child);
            Canvas.SetTop(child, top);
            Canvas.SetLeft(child, left);
        }

        /// <summary>
        /// Remove the specified child from the VisualNodeView children of this control.
        /// </summary>
        /// <remarks>
        /// To remove the correct child, the method searches the children for a child whose
        /// _guid field matches the _guid field of the input node.
        /// </remarks>
        protected void RemoveVisualNodeChild(VisualNodeView child) {
            _visualNodeChildren.RemoveAll(x => x._guid == child._guid);
        }

        #endregion

        #endregion

        
    }
}
