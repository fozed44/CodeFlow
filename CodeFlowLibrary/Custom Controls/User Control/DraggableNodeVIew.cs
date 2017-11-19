using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeFlowLibrary.Custom_Controls {
    
    /// <summary>
    //++ DraggableNodeView
    ///
    //+  Purpose:
    ///     A ResizableNodeView with the added ability to be dragged by the mouse.
    /// </summary>
    public class DraggableNodeView : ResizableNodeView {
        //static DraggableNodeView() {
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DraggableNodeView), new FrameworkPropertyMetadata(typeof(DraggableNodeView)));
        //}
        public DraggableNodeView() { }

        #region Fields

        /// <summary>
        /// The point at which the mouse was captured in parent space.
        /// </summary>
        Point _parentMouseCapPosition;

        /// <summary>
        /// The point it which the mouse was captured in this canvas's space.
        /// </summary>
        Point _thisMouseCapPosition;

        /// <summary>
        /// Set to true in the button down event. Remains true until the mouse is moved
        /// more than 5 pixels in the x or y direction. While _stillSticky is true, the
        /// DraggableCanvas is locked in position and wont move.
        /// This has the effect of making the user drag the mouse a small amount before
        /// the DraggableCanvas will actually start moving.
        /// </summary>
        bool _stillSticky;

        /// <summary>
        /// Is the mouse captured.
        /// </summary>
        bool _mouseCaptured;

        #endregion

        #region Mouse Events        

        /// <summary>
        /// Handles the Left Mouse Button Down event.
        /// </summary>
        /// <remarks>
        /// Captures the position of the mouse relative to the parent and the position of the mouse
        /// relative to this control and saves the values in _parentMouseCapPosition and
        /// _thisMouseCapPosition these values are needed by the OnMouseMove to calculate the new
        /// position of the control.
        /// </remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Don't respond to the event if it's already been handled.
            if (e.Handled) return;

            _mouseCaptured = true;
            _stillSticky = true;
            _parentMouseCapPosition = e.GetPosition(Parent as IInputElement);
            _thisMouseCapPosition   = e.GetPosition(this);
            e.Handled = true;
        }

        /// <summary>
        /// Release the mouse capture.
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);

            // Don't respond to the event if it's already been handled.
            if (e.Handled) return;

            // Don't respond to the event if we don't have control of the mouse.
            if (!_mouseCaptured) return;

            ReleaseMouseCapture();
            _mouseCaptured = false;
            e.Handled = true;
        }

        /// <summary>
        /// If the control is currently being dragged, OnMouseMove calculates the
        /// new position of the control.
        /// </summary>
        /// <remarks>
        /// The position of the control is set within the parent canvas via the SetLeft
        /// and SetTop static methods on the Canvas type.
        /// </remarks>
        protected override void OnMouseMove(MouseEventArgs e) {
            // Don't respond to the event if it's already been handled.
            if (e.Handled) return;

            // Don't respond to the event if we don't have control of the mouse.
            if (!_mouseCaptured) { base.OnMouseMove(e); return; }

            var currentPosition = e.GetPosition(Parent as IInputElement);
            if ((_stillSticky = IsStillSticky(currentPosition)))
                return;
            var newPosition = currentPosition - _thisMouseCapPosition;
            Canvas.SetLeft(this, ClipX(newPosition.X));
            Canvas.SetTop(this, ClipY(newPosition.Y));
            e.Handled = true;
        }

        #endregion

        #region Private Methods

            #region Is Sticky

        /// <summary>
        /// Return true if the mouse has not moved enough to 'unstick' the DraggableCanvas.
        /// Otherwise, return true;
        /// </summary>
        bool IsStillSticky(Point mousePosition) {
            if (Math.Abs(_parentMouseCapPosition.X - mousePosition.X) < 5
             && Math.Abs(_parentMouseCapPosition.Y - mousePosition.Y) < 5)
                return true;
            return false;
        }

            #endregion

            #region Clipping

        /// <summary>
        /// The input is the x position calculated as the new X position of the Canvas. The
        /// value is clipped so that this Canvas will not be rendered outside of its parent.
        /// </summary>
        double ClipX(double x) {
            if (x < 0) return 0;
            var parentWidth = ((FrameworkElement)Parent).ActualWidth;
            if (x + ActualWidth > parentWidth) return parentWidth - ActualWidth;
            return x;
        }

        /// <summary>
        /// The input is the y position calculated as the new Y position of the Canvas. The
        /// value is clipped so that this Canvas will not be rendered outside of its parent.
        /// </summary>
        double ClipY(double y) {
            if (y < 0) return 0;
            var parentHeight = ((FrameworkElement)Parent).ActualHeight;
            if (y + ActualHeight > parentHeight) return parentHeight - ActualHeight;
            return y;
        }

            #endregion

        #endregion
    }
}
