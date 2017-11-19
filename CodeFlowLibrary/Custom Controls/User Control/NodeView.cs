using CodeFlowLibrary.Model;
using System.ComponentModel;
using System.Windows.Controls;
using ReflectionExtensions;

using static ReflectionExtensions.ReflectionExtensions;
using System;
using System.Windows;
using System.Windows.Input;
using Verification;
using System.Collections.Generic;
using CodeFlowLibrary.ViewModel;

namespace CodeFlowLibrary.Custom_Controls {

    /// <summary>
    //++ NodeView
    ///
    //+  Purpose:
    ///     Base class for all model views. 
    ///     
    ///     Provides base functionality for model change notification via the protected OnModelPropertyChanged
    ///     method. The OnModelPropertyChanged method should be implemented by sub-classes to set model properties
    ///     based on the parameters passed to the method.
    ///     
    ///     Provides functionality to attach a view to a model via the AttachToModel method.
    ///     
    ///     Provides the functionality to add a child node view. This includes the functionality to read the location
    ///     properties of the child node and add set its location in the parent.
    /// </summary>
    public class NodeView : UserControl, IDisposable {

        #region Ctor

        public NodeView() { _disposed = false; }

        ~NodeView() {
            Verify.True(_disposed, "NodeView has not been disposed!");
        }

        #endregion

        #region Fields

        private Node _model;
        private bool _disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Read access to the _model property. To set the model, use the AttachToModel method.
        /// </summary>
        public Node Model {
            get { return _model; }
        }

        #endregion

        #region Public

        /// <summary>
        /// Adds a view as a child of this view. The main purpose of this method is to acquire the canvas
        /// of this view that serves as the container for children and add the child to this canvas, setting
        /// the location and size properties of the child in the canvas.
        /// </summary>
        public void AddChildView(NodeView view, Point location) {
            GetViewCanvas().Children.Add((UIElement)view);
            Canvas.SetTop(view, location.Y);
            Canvas.SetLeft(view, location.X);
        }

        /// <summary>
        /// Adds the view as a child of this view. The main purpose of this method is to acquire the canvas
        /// of this view that serves as the container for children and add the child to this canvas, setting
        /// the location and size properties of the child in the canvas.
        /// </summary>
        /// <remarks>
        /// This overload of AddChildView reads the location used to place the child within the canvas used
        /// by this object to contain it's children from the model that is attached to the child.
        /// </remarks>
        public void AddChildView(NodeView view) {
            var location = Reflect(view.Model).GetPropVal<Point>("Location");

            Verify.NotNull(
                location,
                $"The model attached to the view {view.Name} does not have a Location property. " +
                $"Use the overload of the AddChildView method that has a 'location' parameter instead."
            );

            Verify.True(
                typeof(Point) == location.GetType(),
                $"The model attached to the view {view.Name} has a property named 'location', " +
                $"but it's 'location' property is not of type 'Point'. Use the overload of AddChildView ",
                $"that has a 'location' parameter instead."
            );

            AddChildView(view, location);
        }

        /// <summary>
        /// Enumerate the child views.
        /// </summary>
        public IEnumerable<NodeView> GetChildViews() {
            foreach (var child in GetViewCanvas().Children)
                if (child is NodeView)
                    yield return (NodeView)child;
        }

        /// <summary>
        /// Call to attach a model to this view.
        /// </summary>
        public void AttachToModel(Node model) {
            _model = model;
            _model.PropertyChanged += OnModelPropertyChangedInternal;
            AttachModelProperties();
        }

        /// <summary>
        /// Detach a model from this view.
        /// </summary>
        public void DetachModel() {
            if (_model == null)
                return;
            _model.PropertyChanged -= OnModelPropertyChangedInternal;
        }

        /// <summary>
        /// Dispose. A sub-class that has resources that need to be deposed should override the virtual
        /// Dispose(bool) method.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            _disposed = true;
        }

        #endregion

        #region Protected
        
        /// <summary>
        /// Sub-class access to model property changed notification. The sub-class should pretty much always
        /// provide an implementation of this method to make sure that the view reflects the current state
        /// of the model.
        /// </summary>
        protected virtual void OnModelPropertyChanged(string propertyName, object propertyValue) {}
        
        /// <summary>
        /// May be removed... At this time there are no views that have disposable assets.
        /// </summary>
        protected virtual void Dispose(bool disposing) {}        

        /// <summary>
        /// Requires the sub-class to return the canvas that serves as the container to hold child
        /// nodes.
        /// </summary>
        protected virtual Canvas GetViewCanvas() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Any mouse up event that occurs from within this node should set the model as the selected
        /// object.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            CodeFlowViewModel.Instance.FireNodeSelected(this);
        }
        

        #endregion

        #region Private

        /// <summary>
        /// Fires the OnModelPropertyChanged virtual method. Unwraps the PropertyChangedEventArgs, passing
        /// just the property value to the virtual method.
        /// </summary>
        private void OnModelPropertyChangedInternal(object sender, PropertyChangedEventArgs e) {
            OnModelPropertyChanged(e.PropertyName, Reflect(sender).GetPropVal(e.PropertyName));
        }

        /// <summary>
        /// Cycles through each property on the model, calling OnModelPropertyChanged for each one. This
        /// method is called to allow the view to set each view representation of each model property. This
        /// method is valuable, for instance, after a view has been constructed. After a view has been constructed
        /// AttackModelProperties allows the new view to set up its representation of the model.
        /// </summary>
        private void AttachModelProperties() {
            foreach(var property in Reflect(_model).Properties) {
                OnModelPropertyChanged(property.Name, Reflect(_model).GetPropVal(property.Name));
            }
        }

        #endregion

    }
}
