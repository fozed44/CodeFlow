using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeFlowLibrary.Custom_Controls {

    /// <summary>
    //++ ResizableNodeView
    ///
    //+  Purpose:
    ///     A user control with the added ability to be resized by dragging the edges of the control
    ///     with the mouse.
    /// </summary>
    public class ResizableNodeView : NodeView {

        #region Construction

        /// <summary>
        /// By default, a ResizableNodeView is re-sizable.
        /// </summary>
        public ResizableNodeView() {
            Resizable = true;
        }

        #endregion

        #region cursor

        enum cursor {
            NS,
            EW,
            NESW,
            NWSE
        }

        #endregion

        #region Fields

        /// <summary>
        /// Is the control re-sizable?
        /// </summary>
        bool _resizable;

        /// <summary>
        /// Needed to save the state of the cursor so that the OnMouseMoveCaptured knows
        /// which direction the control is being resized.
        /// </summary>
        cursor _currentCursor;

        #endregion

        #region Properties

        /// <summary>
        /// Set to false to disable the resizing functionality.
        /// </summary>
        bool Resizable {
            get { return _resizable; }
            set { VerifyAccess(); SetResizableProperty(value); }
        }

        /// <summary>
        /// Is the control currently resizing??? i.e. is the mouse
        /// currently capt
        /// </summary>
        protected bool Resizing { get; set; }

        /// <summary>
        /// Is true when the mouse is within the resizing zone.
        /// </summary>
        /// <remarks>
        /// The 'resizing zone' is the zone near the edge of the control where the cursor should be a resizing
        /// cursor.
        /// This property is set by the OnMouseMouseNotResizing method, the OnMouseLeftButtonUp event handler,
        /// and the setter for the Re-sizable property.
        /// </remarks>
        protected bool IsMouseInResizeZone { get; set; }

        #endregion

        #region Mouse Overrides

        #region MouseMove

        /// <summary>
        /// Process the mouse move event. If the mouse is currently captured, pass control
        /// to OnMouuseMoveCaptured, otherwise pass control to OnMouseMoveNotCaptured.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            // Don't respond to the event if it's already been handled.
            if (e.Handled) return;

            // Don't do anything if the control is not re-sizable.
            if (!Resizable) return;

            if (Resizing)
                OnMouseMoveResizing(e);
            else
                OnMouseMoveNotResizing(e);
        }

        /// <summary>
        /// If the mouse is not currently captured, check to see if the mouse is near an edge
        /// or corner. If it is near an edge or a corner, pass control to return SetCursor.
        /// </summary>
        private void OnMouseMoveNotResizing(MouseEventArgs e) {
            IsMouseInResizeZone = SetCursorIfInResizeZone(e.GetPosition(this));
            e.Handled = true;
        }

        /// <summary>
        /// Handles the MouseMove event when the control is resizing.
        /// </summary>
        private void OnMouseMoveResizing(MouseEventArgs e) {
            var currentPosition = e.GetPosition(this);
            var controlCenter = new Point(ActualWidth / 2, ActualHeight / 2);
            switch (_currentCursor) {
                case cursor.NS:
                    if (currentPosition.Y < controlCenter.Y)
                        ResizeTop();
                    else
                        ResizeBottom();
                    break;
                case cursor.EW:
                    if (currentPosition.X < controlCenter.X)
                        ResizeLeft();
                    else
                        ResizeRight();
                    break;
                case cursor.NESW:
                    if (currentPosition.Y < controlCenter.Y) {
                        ResizeTop();
                        ResizeRight();
                    } else {
                        ResizeBottom();
                        ResizeLeft();
                    }
                    break;
                case cursor.NWSE:
                    if (currentPosition.Y < controlCenter.Y) {
                        ResizeTop();
                        ResizeLeft();
                    } else {
                        ResizeBottom();
                        ResizeRight();
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
            SetCursorByCurrentCursor();
        }

        #endregion

        #region MouseUp / MouseDown

        /// <summary>
        /// Left button down event handler. If the mouse is in the resize zone and the
        /// control is re-sizable, capture the mouse and set Resizing = true.
        /// </summary>
        /// <remarks>
        /// Setting the Resizing property to true will cause the OnMouseMove event handler
        /// to call OnMouseMoveResizing instead of OnMouseMovingNotResizing.
        /// </remarks>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);

            // Don't respond to the event if its already been handled.
            if (e.Handled) return;

            // Don't do anything if the control is not re-sizable.
            if (!Resizable) return;

            if (!IsMouseInResizeZone) return;

            Resizing = true;
            CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// If the control is currently in the Resizing state, releases the mouse capture
        /// and sets the Resizing property to false.
        /// </summary>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);

            // Don't respond to the event if it's already been handled.
            if (e.Handled) return;

            // Don't respond to the event if the control is not currently resizing.
            if (!Resizing) return;

            ReleaseMouseCapture();
            Resizing = false;
            e.Handled = true;
        }

        #endregion

        #endregion

        #region Private Methods

            #region Re-sizable Property

        /// <summary>
        /// Sets the Re-sizable property to the input value, making sure that the internal state of
        /// the control is consistent.
        /// </summary>
        void SetResizableProperty(bool resize) {

            // Don't apply any logic if resize is not different than the current
            // value of the _resize field.
            if (_resizable == resize) return;

            _resizable = resize;

            // If the control is currently resizing, we have to make sure the mouse
            // capture is released.
            if (Resizing) {
                Resizing = false;
                ReleaseMouseCapture();
            }

            // The MouseInResizeZone will always return false when the control is not Re-sizable.
            IsMouseInResizeZone = false;

        }

            #endregion

            #region Cursor methods

        /// <summary>
        /// Set the cursor for this control. The new cursor is also saved in _currentCursor.
        /// </summary>
        /// <remarks>
        /// Mouse.SetCursor does not have a counterpart, such as Mouse.GetCursor.... So in
        /// order to keep track of the current cursor, this method saves this input in
        /// _currentCursor before calling Mouse.SetCursor.
        /// </remarks>
        /// <param name="cursor">
        /// The new cursor to use for the control.
        /// </param>
        /// <returns>
        /// Always returns true;
        /// </returns>
        bool SetCursor(cursor cursor) {
            _currentCursor = cursor;
            Mouse.SetCursor(GetCursor(cursor));
            return true;
        }

        /// <summary>
        /// Return a one of the cursors from the Cursors enumeration. The new cursor
        /// matches the input cursor according to direction.
        /// </summary>
        /// <param name="c">
        /// The cursor that is to be converted to a new Cursor object.
        /// </param>
        /// <returns>
        /// A new Cursor object matching the direction of the input cursor.
        /// </returns>
        Cursor GetCursor(cursor c) {
            switch (c) {
                case ResizableNodeView.cursor.EW:
                    return Cursors.SizeWE;
                case ResizableNodeView.cursor.NESW:
                    return Cursors.SizeNESW;
                case ResizableNodeView.cursor.NS:
                    return Cursors.SizeNS;
                case ResizableNodeView.cursor.NWSE:
                    return Cursors.SizeNWSE;
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Sets the cursor to the correct cursor if the input Point is within the 'resizing zone'.
        /// </summary>
        /// <remarks>
        /// The input point is within the 'resizing zone' if the point is close enough to the edge
        /// of the control.
        /// </remarks>
        /// <param name="currentPosition">
        /// The current position of the mouse.
        /// </param>
        /// <returns>
        /// True if a resize has been started.
        /// </returns>
        bool SetCursorIfInResizeZone(Point currentPosition) {

            if (currentPosition.Y < 7) {
                if (currentPosition.X < 7)
                    return SetCursor(cursor.NWSE);
                else if (ActualWidth - currentPosition.X < 7)
                    return SetCursor(cursor.NESW);
                else
                    return SetCursor(cursor.NS);
            } else if (Math.Abs(ActualHeight - currentPosition.Y) < 7) {
                if (currentPosition.X < 7)
                    return SetCursor(cursor.NESW);
                else if (ActualWidth - currentPosition.X < 7)
                    return SetCursor(cursor.NWSE);
                else
                    return SetCursor(cursor.NS);
            } else if (currentPosition.X < 7)
                return SetCursor(cursor.EW);

            else if (ActualWidth - currentPosition.X < 7)
                return SetCursor(cursor.EW);

            return false;
        }

        /// <summary>
        /// Sets the mouse cursor applied to the mouse according to the value of _currentCursor.
        /// </summary>
        void SetCursorByCurrentCursor() {
            switch (_currentCursor) {
                case cursor.EW:
                    Mouse.SetCursor(Cursors.SizeWE);
                    break;
                case cursor.NS:
                    Mouse.SetCursor(Cursors.SizeNS);
                    break;
                case cursor.NESW:
                    Mouse.SetCursor(Cursors.SizeNESW);
                    break;
                case cursor.NWSE:
                    Mouse.SetCursor(Cursors.SizeNWSE);
                    break;
            }
        }

            #endregion

            #region Resize Methods

        /// <summary>
        /// Changes both the top and the height properties of the control. The top property is altered so
        /// that it drags along with the mouse. The Height property is adjusted so that the right side of
        /// the control holds the same position.
        /// </summary>
        void ResizeTop() {
            var currentY   = Mouse.GetPosition(this).Y;
            var currentTop = Canvas.GetTop(this);
            var newTop = currentTop + currentY;
            newTop = newTop < 0 ? 0 : newTop;
            var change = currentTop - newTop;
            Canvas.SetTop(this, newTop);
            Height += change;
        }

        /// <summary>
        /// Change the height of the control using the distance from the top side of the control to the
        /// current position of the mouse as the new height. This is easy, the distance from the top
        /// side of the control to the current position of the mouse is the same as the Y value of the
        /// current mouse position, when the mouse position is retrieved relative to this control.
        /// </summary>
        void ResizeBottom() {
            var newHeight = Mouse.GetPosition(this).Y;
            var top = Canvas.GetTop(this);
            var parentHeight = ((Canvas)Parent).ActualHeight;

            if (top + newHeight > parentHeight)
                newHeight = parentHeight - top;

            Height = newHeight;
        }

        /// <summary>
        /// Change the width of the control using the distance from the left side of the control to the
        /// current position of the mouse as the new width. This is easy, the distance from the right
        /// side of the control to the current position of the mouse is the same as the X value of the
        /// current mouse position, when the mouse position is retrieved relative to this control.
        /// </summary>
        void ResizeRight() {
            var newWidth = Mouse.GetPosition(this).X;
            var left = Canvas.GetLeft(this);
            var parentWidth = ((Canvas)Parent).ActualWidth;

            if (left + newWidth > parentWidth)
                newWidth = parentWidth - left;

            Width = newWidth;
        }

        /// <summary>
        /// Changes both the width and the left properties of the control. The left property is altered so
        /// that it drags along with the mouse. The Width property is adjusted so that the right side
        /// of the control holds the same position.
        /// </summary>
        void ResizeLeft() {
            var currentX = Mouse.GetPosition(this).X;
            var currentLeft = Canvas.GetLeft(this);
            var newLeft = currentLeft + currentX;
            newLeft = newLeft < 0 ? 0 : newLeft;
            var change = currentLeft - newLeft;
            Canvas.SetLeft(this, newLeft);
            Width += change;
        }

            #endregion

        #endregion
    }
}
