using System;
using System.IO;
using System.Linq;
using CodeFlowLibrary.Factory;
using CodeFlowLibrary.Interface;
using CodeFlowLibrary.Model;
using CodeFlowLibrary.View;
using CodeFlowLibrary.ViewModel;
using Common.PathHelp;
using Verification;

namespace CodeFlowLibrary.Controllers {

    /// <summary>
    //++ FileController
    ///
    //+  Purpose:
    ///     Handles the logical aspects of saving/loading files.
    /// </summary>
    public class FileController {

        #region Ctor

        public FileController(IStoreController storeController) {
            _storeController = storeController;
        }

        #endregion

        #region Finalize

        /// <summary>
        /// The FileController should not be destroyed without closing the current file.
        /// </summary>
        ~FileController() {
            Verify.True(_slideCollection == null, "FileController is being destroyed with a live SlideCollection reference.");
        }

        #endregion

        #region Fields

        /// <summary>
        /// The path, filename, and extension of the current file.
        /// </summary>
        private string _filename;
        private string _path;
        private string _extension;


        /// <summary>
        /// The active SlideCollection is owned by the FileController.
        /// </summary>
        private SlideCollection _slideCollection;

        private IStoreController _storeController;

        #endregion

        #region Properties

        public string Filename => _filename;

        public string Path => _path;

        public string Extension => _extension;

        public string FullFilename => CreateFullFilename();

        internal SlideCollection SlideCollection => _slideCollection;

        #endregion

        #region Public

        /// <summary>
        /// Create a new file. Sets the internal 
        /// </summary>
        /// <returns>
        /// True if a new file was created, false otherwise.
        /// </returns>
        public bool New() {
            if(_slideCollection != null && CodeFlowViewModel.Instance.IsDirty)
                if(ConfirmDestroyDirtyFile())
                    return false;
            if(_slideCollection != null)
                CloseFileInternal();
            return NewFileInternal();
        }

        /// <summary>
        /// Is there a current file that can be saved?
        /// </summary>
        /// <returns>
        /// True if there is a current file that can be saved, false otherwise.
        /// </returns>
        public bool CanSave() {
            return _slideCollection != null && CodeFlowViewModel.Instance.IsDirty;
        }

        /// <summary>
        /// Can a save as operation be initiated?
        /// </summary>
        /// <returns>
        /// True if a save as operation can be initiated, false otherwise.
        /// </returns>
        public bool CanSaveAs() {
            return _slideCollection != null;
        }

        /// <summary>
        /// Can a create new file operation be initiated?
        /// </summary>
        /// <returns>
        /// True if a new file operation can be initiated, false otherwise.
        /// </returns>
        public bool CanCreateNewFile() {
            return true;
        }

        /// <summary>
        /// Can a close file operation be initiated?
        /// </summary>
        /// <returns>
        /// True if a close file operation can be initiated, false otherwise.
        /// </returns>
        public bool CanCloseFile() {
            return _slideCollection != null;
        }

        /// <summary>
        /// Can an open file operation be initiated?
        /// </summary>
        /// <returns>
        /// True if an open file operation can be initiated, false otherwise.
        /// </returns>
        public bool CanOpenFile() {
            return true;
        }

        /// <summary>
        /// Initiates an open operation.
        /// </summary>
        /// <returns>
        /// True if a file was opened, otherwise the return value is false.
        /// </returns>
        public bool Open() {
            if(_slideCollection != null && CodeFlowViewModel.Instance.IsDirty)
                if(ConfirmDestroyDirtyFile()) return false;
            var result = CodeFlowViewModel.Instance.FireOpenFilenameRequest();
            if(result.Cancel)
                return false;
            return OpenFileInternal(result.Path, result.Filename, result.Ext);
        }

        /// <summary>
        /// Saves the current file. If a filename is currently stored in _path, _filename, and _ext,
        /// then that filename is used. Otherwise, a filename is requested via the SaveFilenameRequest
        /// event. Subscribers to that event have an opportunity to cancel the operation, in which
        /// case, this method does nothing.
        /// </summary>
        /// <returns>
        /// True if the file was saved, otherwise false.
        /// </returns>
        public bool Save() {
            Verify.True(_slideCollection!=null, $"Save method was called when there is not a slide collection available to save.");
            Verify.True(CodeFlowViewModel.Instance.IsDirty, "Do not call Save when the view model is not dirty.");
            if(string.IsNullOrEmpty(_filename)) {
                var args = CodeFlowViewModel.Instance.FireSaveFilenameRequest();
                if(args.Cancel)
                    return false;
                SetFilename(args.Path, args.Filename, args.Ext);
            }
            return SaveInternal();
        }

        /// <summary>
        /// Saves the current file, requesting a filename via the SaveAsFilenameRequest event. If
        /// a subscriber to the event cancels the operation, then this method does nothing.
        /// </summary>
        /// <returns>
        /// True if the file is saved, false otherwise.
        /// </returns>
        public bool SaveAs() {
            Verify.True(_slideCollection!=null, "SaveAs method was called when there is not a slide collection available to save.");
            var args = CodeFlowViewModel.Instance.FireSaveAsFilenameRequest();
            if(args.Cancel)
                return false;
            SetFilename(args.Path, args.Filename, args.Ext);
            return SaveInternal();
        }

        /// <summary>
        /// Closes the current file. This method fires the FileClosing event on the CodeFlowViewModel,
        /// giving any subscribers a chance to cancel the operation.
        /// </summary>
        /// <returns>
        /// True if the current file was closed. Otherwise false.
        /// </returns>
        public bool CloseFile() {
            Verify.True(_slideCollection!=null, "Close method was called when there is not a slide collection available to close.");
            if(CodeFlowViewModel.Instance.IsDirty)
                if(ConfirmDestroyDirtyFile())
                    return false;
            return CloseFileInternal();
        }

        #endregion

        #region Private

            #region Helpers


        /// <summary>
        /// Fires the CodeFlowViewModel.FireConfirmNewFile event, and returns the value of the
        /// Cancel property on the CancelableEventArgs that event subscribers can use to cancel 
        /// the operation.
        /// </summary>
        bool ConfirmDestroyDirtyFile() {
            var result = CodeFlowViewModel.Instance.FireDestroyDirtyFile();
            return result.Cancel;
        }

        /// <summary>
        /// Normalize and combine _path, _filename, and _extension;
        /// </summary>
        /// <returns>
        /// A string created by combining _path, _filename, and _extension.
        /// </returns>
        private string CreateFullFilename() {
            return $"{PathHelper.NormalizePath(_path)}{PathHelper.NormalizeFilename(_filename)}{_extension}";
        }
        
        /// <summary>
        /// Update _path, _filename, and _ext.
        /// </summary>
        /// <param name="path">
        /// The new file path.
        /// </param>
        /// <param name="filename">
        /// The new filename
        /// </param>
        /// <param name="extension">
        /// The new extension.
        /// </param>
        private void SetFilename(string path, string filename, string extension) {
            _path      = path;
            _filename  = filename;
            _extension = extension;
        }

        /// <summary>
        /// Sets the _path, _filename, and _extension fields to null.
        /// </summary>
        private void ClearFilename() {
            _path      = null;
            _filename  = null;
            _extension = null;
        }

        /// <summary>
        /// Returns the entire hierarchy for default slide in the collection.
        /// </summary>
        /// <returns>
        /// ATM there is no meta data or anything that determines which view in the slide collection
        /// is the 'default' slide. For now just return the first slide in the collection.
        /// </returns>        
        private SlideView GetDefaultSlideView(SlideCollection slideCollection) {
            // TODO:
            // Implement some way of determining a 'default' slide in a slide collection.
            Verify.NotNull(slideCollection?.Children[0], "The slide collection in the file does not contain any slides!");
            return (SlideView)ViewFactory.Instance.GetHierarchy(slideCollection.Children[0]);
        }

        #endregion

            #region Disk Access

        private bool SaveInternal() {
            _storeController.PersistSlideCollection(CreateFullFilename(), _slideCollection);
            CodeFlowViewModel.Instance.ClearDirtyFlag();
            return true;
        }

        private bool CloseFileInternal() {
            Verify.NotNull(_slideCollection, "_slideCollection is null.");
            CodeFlowViewModel.Instance.DetachCurrentSlide();
            _slideCollection.Dispose();
            _slideCollection = null;
            CodeFlowViewModel.Instance.ClearDirtyFlag();
            ClearFilename();
            return true;
        }

        private bool NewFileInternal() {
            // Clear the filename
            ClearFilename();  

            // Create a new SlideCollection
            _slideCollection = new Model.SlideCollection(CodeFlowViewModel.Instance.HistoryController);

            // Create a new slide for the collection.
            var newSlide = new Slide { Name = "New Slide" };

            // Add the new slide to the slide collection.
            _slideCollection.AddChild(newSlide);

            // Hopefully the UI has subscribed to the SlideSelected event.
            CodeFlowViewModel.Instance.SetCurrentSlide(newSlide.GetView<SlideView>());

            return true;
        }

        private bool OpenFileInternal(string path, string filename, string ext) {
            if(_slideCollection != null)
                CloseFileInternal();
            SetFilename(path, filename, ext);
            _slideCollection = 
                _storeController.RestoreSlideCollection(
                                     CreateFullFilename(), 
                                     CodeFlowViewModel.Instance.HistoryController
                                 );
            var slide = _slideCollection.GetFirstOrNew();
            CodeFlowViewModel.Instance.SetCurrentSlide((SlideView)ViewFactory.Instance.GetHierarchy(slide));
            return true;
        }

            #endregion

        #endregion

    }
}
