using System;
using CodeFlowLibrary.Controllers;
using CodeFlowLibrary.Custom_Controls;
using CodeFlowLibrary.Event;
using CodeFlowLibrary.History;
using CodeFlowLibrary.Implementations;
using CodeFlowLibrary.Model;
using CodeFlowLibrary.View;
using Verification;

namespace CodeFlowLibrary.ViewModel {

    /// <summary>
    //++ CodeFlowViewModel
    ///
    //+  Purpose:
    ///     The CodeFlowViewModel is a singleton class that represents the view model that should be
    ///     wired up to the main window of the host application.
    /// </summary>
    public class CodeFlowViewModel {

        #region Ctor

        /// <summary>
        /// Ctor is private in a singleton implementation.
        /// </summary>
        private CodeFlowViewModel() {
            // Instantiate the file controller.
            _fileController = new FileController(new FileStoreController());

            // Instantiate the history controller.
            _historyController = new HistoryController();

            // Wire up the NotifyDirty event.
            ModelBase.NotifyDirty += NotifyDirtyHandler;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static CodeFlowViewModel _instance;

        /// <summary>
        /// Reference to the file controller. The FileController is owned by the CodeFlowViewModel and
        /// is instantiated in the constructor of this class.
        /// </summary>
        private FileController _fileController;

        /// <summary>
        /// True when the SlideCollection owned by the FileController is dirty, otherwise, false.
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// The history controller is owned by the view model.
        /// </summary>
        private HistoryController _historyController;

        /// <summary>
        /// Tracks the current slide.
        /// </summary>
        private SlideView _currentSlide;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton Instance access.
        /// </summary>
        public static CodeFlowViewModel Instance {
            get { return _instance ?? (_instance = new CodeFlowViewModel()); }
        }

        /// <summary>
        /// True if there are unsaved changes in the current hierarchy.
        /// </summary>
        public bool IsDirty {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        /// <summary>
        /// Access to the history controller.
        /// </summary>
        internal HistoryController HistoryController => _historyController;

        /// <summary>
        /// Access to the currently selected slide. Getting the value is a public operation, but setting
        /// the value is private and should only be done via the SetCurrentSlide method.
        /// </summary>
        public SlideView CurrentSlide {
            get { return _currentSlide; }
            private set { _currentSlide = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fired when a new file has been opened or created, or when a slide contained in the current
        /// slide collection has been selected.
        /// </summary>
        public event NodeSelectedEventHandler SlideSelected;

        /// <summary>
        /// Fired when the UI needs to detach the slide supplied in the DetachSlideRequestEventArgs needs to
        /// be detached from the UI. Usually this happens before a file is closed.
        /// </summary>
        public event DetachSlideRequestEventHandler DetachSlideRequest;

        /// <summary>
        /// Fired when the a new node has been selected.
        /// </summary>
        public event NodeSelectedEventHandler NodeSelected;

        /// <summary>
        /// Fired when the CodeFlowLibrary needs the host to determine a filename for which to save
        /// the current file. The host can set the Cancel property of the FileEventArgs to false to
        /// cancel the save operation.
        /// </summary>
        public event FileEventHandler SaveFilenameRequest;

        /// <summary>
        /// Fired when the CodeFlowLibrary needs the host to determine a filename for which to save
        /// the current file in a save as situation. The host can set the Cancel property of the
        /// FileEventArgs to false to cancel the save as operation.
        /// </summary>
        public event FileEventHandler SaveAsFilenameRequest;

        /// <summary>
        /// Fired when the CodeFlowLibrary needs the host to determine a filename for a file to be
        /// opened. The host has the option of setting the Cancel property of the FileEventArgs
        /// to false to cancel the open file operation.
        /// </summary>
        public event FileEventHandler OpenFilenameRequest;

        /// <summary>
        /// Fired when a new file operation has been triggered, but before the operation has been 
        /// started. Subscribers to this even have an opportunity to cancel the operation.
        /// </summary>
        public event CancelableEventHandler ConfirmDestroyDirtyFileRequest;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the NotifyDirty static event on the ModelBase class. This event is fired anytime
        /// any property in any ModelBase derived object that participates in change notification is
        /// changed.
        /// When this event is fired, the IsDirty flag is set.
        /// </summary>
        /// <param name="sender">
        /// Currently the sender is not tracked.
        /// </param>
        /// <param name="e">
        /// Currently the EventArgs are not used.
        /// </param>
        private void NotifyDirtyHandler(object sender, EventArgs e) {
            IsDirty = true;
        }

        #endregion

        #region Public

            #region Internal facing methods to fire events.

        /*
         * The following methods are internally facing methods that should only be called by parts of the
         * framework that are inside this assembly (not the UI). The events fired by these methods should 
         * be subscribed to by the UI.
         * */

        /// <summary>
        /// Fires the SlideSelected event, where slideView is passed as the NodeView property of the
        /// NodeSelectedEventArgs.
        /// </summary>
        internal void FireSlideSelected(SlideView slideView) {
            var e = SlideSelected;
            Verify.NotNull(e, $"The SlideSelected event does not have a subscriber.");
            e?.Invoke(this, new NodeSelectedEventArgs(slideView));
        }

        /// <summary>
        /// Fires the DetachSlideRequest event. This event signals the UI that the slide referenced
        /// in the DetachSlideRequestEventArgs needs to be detached from the UI.
        /// </summary>
        internal void FireDetachSlideRequest() {
            Verify.NotNull(_currentSlide, "FireDetachSlideRequest was called when _currentSlide == null.");
            var e = DetachSlideRequest;
            Verify.NotNull(e, "The DetachSlideRequested event does not have a subscriber.");
            e?.Invoke(this, new DetachSlideRequestEventArgs { SlideView = _currentSlide });
        }
        
        /// <summary>
        /// Called by NodeView objects when they are selected. Fires the NodeSelected event.
        /// </summary>
        internal void FireNodeSelected(NodeView node) {
            var e = NodeSelected;
            Verify.NotNull(e, $"The event NodeSelected does not have a subscriber.");
            e?.Invoke(this, new NodeSelectedEventArgs(node));
        }

        /// <summary>
        /// Called by the FileController when a save operation is requested, but there is not a
        /// current filename. The subscriber must populate a Filename, Path, and Extension.
        /// </summary>
        /// <returns>
        /// A FileEventArgs containing the Filename, Path, and extension generated by a subscriber
        /// of the event. If no subscriber exists or the user canceled the operation, the Cancel
        /// property of the FileEventArgs object should be true.
        /// </returns>
        internal FileEventArgs FireSaveFilenameRequest() {
            var e = SaveFilenameRequest;
            Verify.NotNull(e, $"The event SaveFilenameRequest does not have a subscriber.");
            if(e == null) return new FileEventArgs { Cancel = true };
            var args = new FileEventArgs();
            e(this, args);
            return args;
        }

        /// <summary>
        /// Called by the FileController when a save as operation is requested. The subscriber must
        /// populate a Filename, Path, and Extension.
        /// </summary>
        /// <returns>
        /// A FileEventArgs containing the Filename, Path, and extension generated by a subscriber
        /// of the event. If no subscriber exists or the user canceled the operation, the Cancel
        /// property of the FileEventArgs object should be true.
        /// </returns>
        internal FileEventArgs FireSaveAsFilenameRequest() {
            var e = SaveAsFilenameRequest;
            Verify.NotNull(e, "The event SaveAsFilenameRequest does not have a subscriber.");
            if(e == null) return new FileEventArgs { Cancel = true };
            var args = new FileEventArgs();
            e(this, args);
            return args;
        }

        /// <summary>
        /// Called by the FileController when a file operation has been requested that if completed
        /// will destroy the currently open file and that file is dirty.
        /// </summary>
        /// <returns>
        /// A CancelableEventArgs object containing a Canceled property. The property will be true if
        /// a subscriber of the ConfirmDestroyDirtyFile event canceled the operation.
        /// </returns>
        internal CancelableEventArgs FireDestroyDirtyFile() {
            var e = ConfirmDestroyDirtyFileRequest;
            Verify.NotNull(e, "The event ConfirmDestroyDirtyFileRequest does not have a subscriber.");
            if(e == null) return new CancelableEventArgs();
            var args = new CancelableEventArgs();
            e(this, args);
            return args;
        }

        /// <summary>
        /// Called by the FileController when an open file operation has been requested. The Subscribers
        /// have an opportunity to cancel the operation by setting the Cancel property of the FileEventArgs
        /// to false. Otherwise, the subscriber must supply a filename via the FileEventArgs.
        /// </summary>
        /// <returns>
        /// A FileEventArgs either containing a filename of a file to be opened, or noting if the operation
        /// was canceled (Cancel property will be true in that case).
        /// </returns>
        internal FileEventArgs FireOpenFilenameRequest() {
            var e = OpenFilenameRequest;
            Verify.NotNull(e, "The OpenFilenameRequest event does not have a subscriber.");
            if(e == null) return new Event.FileEventArgs { Cancel = true };
            var args = new FileEventArgs();
            e(this, args);
            return args;
        }

        /// <summary>
        /// Clear the IsDirty flag.
        /// </summary>
        internal void ClearDirtyFlag() {
            IsDirty = false;
        }

            #endregion

            #region Undo / Redo

        public void Undo()    => _historyController.Undo();

        public void Redo()    => _historyController.Redo();

        public bool CanUndo() => _historyController.CanUndo();

        public bool CanRedo() => _historyController.CanRedo();

            #endregion

            #region File

        /// <summary>
        /// Initiate a Save operation.
        /// </summary>
        /// <returns>
        /// True if the file was saved. False if the operation was canceled.
        /// </returns>
        public bool SaveFile() => _fileController.Save();

        /// <summary>
        /// Initiate a SaveAs operation.
        /// </summary>
        /// <returns>
        /// True if the file as saved. False if the operation was canceled.
        /// </returns>
        public bool SaveFileAs() => _fileController.SaveAs();

        /// <summary>
        /// Initiate a New file operation.
        /// </summary>
        /// <returns>
        /// True if a new file was created. False if the operation was canceled.
        /// </returns>
        public bool NewFile() {
            var result = _fileController.New();
            if(result)
                ClearDirtyFlag();
            return result;
        }

        /// <summary>
        /// Initiate an open file operation.
        /// </summary>
        /// <returns>
        /// True if a file was successfully opened, otherwise, false.
        /// </returns>
        public bool OpenFile() {
            if(!_fileController.Open())
                return false;
            return true;
        }

        /// <summary>
        /// Request that the file controller tell us if a save operation can be initiated.
        /// </summary>
        /// <returns>
        /// True if a save operation can be initiation, otherwise the return value is false.
        /// </returns>
        public bool CanSave() => _fileController.CanSave();

        /// <summary>
        /// Request that the file controller tell us if a save as operation can be initiated.
        /// </summary>
        /// <returns>
        /// True if a save as operation can be initiated, otherwise the return value is false.
        /// </returns>
        public bool CanSaveAs() => _fileController.CanSaveAs();

        /// <summary>
        /// Request that the file controller tell us if a create new file operation can be initiated.
        /// </summary>
        /// <returns>
        /// True if a create new file operation can be initiated, otherwise the return value is false.
        /// </returns>
        public bool CanCreateNewFile() => _fileController.CanCreateNewFile();

        /// <summary>
        /// Request that the file controller tell us if an open file operation can be initiated.
        /// </summary>
        /// <returns>
        /// True if an open file operation can be initiated, otherwise the return value is false.
        /// </returns>
        public bool CanOpenFile() => _fileController.CanOpenFile();

        /// <summary>
        /// Request that the file controller tell us if a close file operation can be initiated.
        /// </summary>
        /// <returns>
        /// True if an open file operation can be initiated, otherwise the return value is false.
        /// </returns>
        public bool CanCloseFile() => _fileController.CanCloseFile();

        /// <summary>
        /// Request that the file controller close the current file.
        /// </summary>
        /// <returns></returns>
        public bool CloseFile() {
            var result = _fileController.CloseFile();
            if(result)
                CodeFlowViewModel.Instance.ClearDirtyFlag();
            return result;
        }

            #endregion

            #region Current Slide

        /// <summary>
        /// If there is not a current slide, this method is a no-op. Otherwise the UI is sent the
        /// DetachSlideRequest event. _currentSlide is then set to NULL.
        /// </summary>
        internal void DetachCurrentSlide() {
            if(_currentSlide == null)
                return;
            FireDetachSlideRequest();
            _currentSlide = null;
        }

        /// <summary>
        /// Sets the value of the current slide reference (_currentSlide), and then fires the
        /// FieSlideSelected slide.
        /// </summary>
        internal void SetCurrentSlide(SlideView slide) {
            CurrentSlide = slide;
            FireSlideSelected(slide);
        }

            #endregion

        #endregion

        #region Private

        #endregion

    }
}
