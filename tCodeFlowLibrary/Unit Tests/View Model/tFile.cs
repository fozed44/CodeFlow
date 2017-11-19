using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeFlowLibrary.Event;
using CodeFlowLibrary.ViewModel;
using System.Reflection;
using CodeFlowLibrary.Interface;
using tCodeFlowBuilder.Implementations;
using CodeFlowLibrary.Controllers;

namespace tCodeFlowBuilder.Unit_Tests.View_Model {
    /// <summary>
    /// Summary description for tFile
    /// </summary>
    [TestClass]
    public class tFile {

        #region Internal Classes

        private class TriggeredFlags {
            public bool SlideSelected { get; set; }
            public bool DetachSlide { get; set; }
            public bool SaveFilenameRequest { get; set; }
            public bool SaveAsFilenameRequest { get; set; }
            public bool OpenFilenameRequest { get; set; }
            public bool ConfirmDestroyDirtyFileRequest { get; set; }
        }

        #endregion

        #region Fields

        private TriggeredFlags _triggeredFlags;

        private PrivateObject _pViewModel;
        private PrivateObject _pFileController;

        private const string _filename = "tFile";
        private const string _extension = ".cfl";
        private const string _path = @"c:\temp\";

        #endregion

        #region Additional test attributes

        [TestInitialize]
        public void tInitialize() {
            _triggeredFlags = new TriggeredFlags();
            FieldInfo fieldInfo = typeof(CodeFlowViewModel).GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
            fieldInfo.SetValue(null, null);
            _pViewModel = new PrivateObject(CodeFlowViewModel.Instance);

            fieldInfo = typeof(CodeFlowViewModel).GetField("_fileController", BindingFlags.NonPublic | BindingFlags.Instance);
            var fileController = (FileController)fieldInfo.GetValue(CodeFlowViewModel.Instance);

            fieldInfo =  typeof(FileController).GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo.SetValue(fileController, new FakeStoreController());
            var storeController = (FakeStoreController)fieldInfo.GetValue(fileController);

            _pFileController = new PrivateObject(fileController);
        }

        #endregion

        /// <summary>
        /// Calling the NewFile method on a new instance of the view model should
        /// 1) Fire the SlideSelected event when the new file is created.
        /// 2) Set the CurrentSlide property to the reference of the new Slide in
        ///    the new slide collection.
        /// </summary>
        [TestMethod]
        public void tNewFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();
            Assert.IsNotNull(CodeFlowViewModel.Instance.CurrentSlide);
            Assert.IsTrue(_triggeredFlags.SlideSelected);
        }

        /// <summary>
        /// Make sure that after a new file is opened that the file name stored in the File controller
        /// is clean.
        /// </summary>
        [TestMethod]
        public void tNewFile_NameIsClean() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Test what should happen when the NewFile method is called when a clean file is already open.
        /// This should cause a new file to open without any action. The ConfirmDestroyDirtyFileRequst
        /// event should not be fired since the currently open file is not dirty.
        /// 
        /// The OnSlideSelected event should be fired when NewFile is called the second time.
        /// 
        /// The CodeFileViewModel.Instance.CurrentSlide property should not reference the same object
        /// before and after the second call to NewFile since the second call should create a new file.
        /// </summary>
        [TestMethod]
        public void tNewFileOnCleanFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.NewFile();

            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Reset the SlideSelected flag so that we can make sure it is fired again during the second
            // call to NewFile.
            _triggeredFlags.SlideSelected = false;

            CodeFlowViewModel.Instance.NewFile();

            // Make sure that the ConfirmDestroyDirtyFIleRequest event was not fired during the second
            // call to NewFile().
            Assert.IsFalse(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure that the detach slide request was fired.
            Assert.IsTrue(_triggeredFlags.DetachSlide);

            // Make sure that the SlideSlected event was called during the second call to NewFile.
            Assert.IsTrue(_triggeredFlags.SlideSelected);
            // Make sure there is a slide referenced by the CurrentSlide property after the second
            // call to NewFile().
            Assert.IsNotNull(CodeFlowViewModel.Instance.CurrentSlide);

            // Make sure that the slide that exists in the view model after the second call to NewFile()
            // is not the same file that existed in the view model after the first call to NEwFile().
            Assert.IsFalse(object.ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Test what should happen when the NewFile method is called when a dirty file is already open.
        /// 
        /// This should cause the ConfirmDistroyDirtyFile event to fire. This test makes sure that the
        /// setting the cancel property on the event arguments of the ConfirmDistroyDirtyFile event
        /// must cancel the open operation and the CurrentSlide property of the view model should still
        /// hold a reference to the same slide that was referenced before the call to NewFile().
        /// </summary>
        [TestMethod]
        public void tNewFileOnDirtyFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to make the file dirty.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "ChangeMe";

            // Make sure that the dirty flag is true now.
            Assert.IsTrue(CodeFlowViewModel.Instance.IsDirty);

            // Try and open a new file.
            CodeFlowViewModel.Instance.NewFile();

            // Make sure that the ConfirmDestroyDirtyFIleRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure that the slide that was referenced before the second call to NewFile() is the 
            // same slide that is referenced now.
            Assert.IsTrue(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Calling CloseFile when no file is open throw an exception indicating that the CloseFIle
        /// method was called when the internal slide collection reference is null.
        /// </summary>
        [TestMethod]
        public void tCloseFile_NothingOpen() {
            var exceptionThrown = false;
            Assert.IsNull(CodeFlowViewModel.Instance.CurrentSlide);

            try {
                CodeFlowViewModel.Instance.CloseFile();
            } catch {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Calling the CloseFile method on the view model when the view model contains a clean (not
        /// dirty) slide collection should close the file. Also
        /// 1) The DetachSlide event should be called before the slide collection is destroyed.
        /// </summary>
        [TestMethod]
        public void tCloseFile_CleanFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.NewFile();

            // Close the file
            CodeFlowViewModel.Instance.CloseFile();

            // Make sure the DetachSlideRequest event was called.
            Assert.IsTrue(_triggeredFlags.DetachSlide);

            // Make sure there is not a valid slide reference in the view model.
            Assert.IsNull(CodeFlowViewModel.Instance.CurrentSlide);
        }

        /// <summary>
        /// Test the mechanics of closing a dirty file. When closing a dirty file, the ConfirmDestroyDirty
        /// file should be fired.
        /// 
        /// In this test, when the ConfirmDestroyDiryFile event is handled, the Cancel property is set
        /// so the dirty file should not be closed.
        /// </summary>
        [TestMethod]
        public void tCloseFile_DirtyFile_NoConfirm() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            // Get a copy of a reference to the current slide.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to make the file dirty.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Confirm that the file is dirty.
            Assert.IsTrue(CodeFlowViewModel.Instance.IsDirty);

            // Confirm that the OnConfirmDestroyDirtyFileRequest has not been called yet.
            Assert.IsFalse(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Attempt to close the file.
            CodeFlowViewModel.Instance.CloseFile();

            // Confirm that the OnConfirmDestroyDirtyFileRequest event handler was called.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Confirm that the original file is still open by comparing the current slide
            // reference to the reference that was saved before attempting to close the
            // file.
            Assert.IsTrue(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Test the mechanics of closing a dirty file. When cosing a dirty file, the ConfirmDestroyDirtyFile
        /// event should be fired.
        /// 
        /// In this test, the ConfirmDestroyDirtyFile event handler does not cancel the operation
        /// so the file should be closed after the call to CloseFile()
        /// </summary>
        [TestMethod]
        public void tCloseFile_DirtyFile_Confirm() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.NewFile();

            // Get a copy of the reference to the current slide.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to make the file dirty.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Confirm that the file is dirty.
            Assert.IsTrue(CodeFlowViewModel.Instance.IsDirty);

            // Confirm that the OnConfirmDestroyDirtyFIleRequest has not been called yet.
            Assert.IsFalse(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Attempt to close the file.
            CodeFlowViewModel.Instance.CloseFile();

            // Confirm that the OnConfirmDestroyDirtyFileRequest event handler was called.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Confirm that the file was actually closed
            Assert.IsNull(CodeFlowViewModel.Instance.CurrentSlide);
        }

        /// <summary>
        /// Calling OpenFile when there is already an open file and the open file is dirty should
        /// cause the ConfirmDestroyDirtyFile event to be called. If the ConfirmDestroyDirtyFIle
        /// handler cancels the operation, then the original file should still be open after the
        /// call to OpenFile.
        /// </summary>
        [TestMethod]
        public void tOpenFile_OnDirtyFile_Cancel() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            // Get a reference to the current slide so that it can be compared after the open
            // request is canceled.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to dirty the file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Try to open a new file.
            CodeFlowViewModel.Instance.OpenFile();

            // Make sure the OnConfirmDestroyDirtyFileRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure the operation was canceled by comparing the reference of the current slide
            // to the reference copied before the OpenFile request was made.
            Assert.IsTrue(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Calling OpenFile when there is already an open file and the open file is dirty should
        /// cause the ConfirmDestroyDirtyFile even to be fired. If the ConfirmDestroyDirtyFile
        /// handler does not cancel the operation, the user can still cancel the operation by canceling
        /// during the OpenFilenameRequest, which is tested here.
        /// </summary>
        [TestMethod]
        public void tOpenFile_OnDirtyFile_ConfirmNotCanceled_OpenCanceled() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.OpenFilenameRequest += OnOpenFilenameRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            // Get a reference to the current slide so that it can be compared after the open
            // request is canceled.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to dirty the file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Try to open a new file.
            CodeFlowViewModel.Instance.OpenFile();

            // Make sure the OnConfirmDestroyDirtyFileRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure the OpenFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.OpenFilenameRequest);

            // Make sure the operation was canceled by comparing the reference of the current slide
            // to the reference copied before the OpenFile request was made.
            Assert.IsTrue(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Calling OpenFile when there is already an open file and the open file is dirty should
        /// cause the ConfirmDestroyDirtyFile even to be fired. If the ConfirmDestroyDirtyFile
        /// handler does not cancel the operation, and the OpenFilenameRequest handler does not
        /// cancel the file, then the file should be opened.
        /// </summary>
        [TestMethod]
        public void tOpenFile_OnDirtyFile_ConfirmNotCanceled_OpenNotCanceled() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.OpenFilenameRequest += OnOpenFilenameRequest;
            CodeFlowViewModel.Instance.NewFile();

            // Get a reference to the current slide so that it can be compared after the open
            // request is canceled.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to dirty the file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Try to open a new file.
            CodeFlowViewModel.Instance.OpenFile();

            // Make sure the OnConfirmDestroyDirtyFileRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure the OpenFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.OpenFilenameRequest);

            // Make sure the DetachSlideEvent fired.
            Assert.IsTrue(_triggeredFlags.DetachSlide);

            // Make sure the operation was completed by making sure that the compareReference
            // reference that was acquired before the OpenFile method was called does not 
            // match the CurrentSlide reference that is currently in the view model.
            Assert.IsFalse(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// If the OpenFile method is called when there is already a file open, but the open file
        /// is not dirty, then make sure that the file opened without the ConfirmDestroyDirtyFile
        /// event being fired.
        /// </summary>
        [TestMethod]
        public void tOpenFile_OnCleanFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.OpenFilenameRequest += OnOpenFilenameRequest;
            CodeFlowViewModel.Instance.NewFile();

            // Get a reference to the current slide so that it can be compared after the open
            // request is canceled.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to dirty the file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Try to open a new file.
            CodeFlowViewModel.Instance.OpenFile();

            // Make sure the OnConfirmDestroyDirtyFileRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure the OpenFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.OpenFilenameRequest);

            // Make sure the DetachSlideRequest event was fired.
            Assert.IsTrue(_triggeredFlags.DetachSlide);

            // Make sure that the operation completed by comparing the compareReference that was copied
            // before the OpenFile request was made to the current reference in the currentSlide and
            // making sure that they are different.
            Assert.IsFalse(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// If the OpenFile method is called when there is already a file open, but the open file
        /// is not dirty, then make sure that the file opened without the ConfirmDestroyDirtyFile
        /// event being fired.
        /// </summary>
        [TestMethod]
        public void tOpenFile_OnCleanFile_User_Canceled() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFileRequest;
            CodeFlowViewModel.Instance.OpenFilenameRequest += OnOpenFilenameRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            // Get a reference to the current slide so that it can be compared after the open
            // request is canceled.
            var compareReference = CodeFlowViewModel.Instance.CurrentSlide;

            // Do something to dirty the file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // Try to open a new file.
            CodeFlowViewModel.Instance.OpenFile();

            // Make sure the OnConfirmDestroyDirtyFileRequest was fired.
            Assert.IsTrue(_triggeredFlags.ConfirmDestroyDirtyFileRequest);

            // Make sure the OpenFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.OpenFilenameRequest);

            // Make sure the DetachSlideRequest event was NOT fired since the operation was canceled
            // in the OnOpenFilenameRequest_Cancel handler.
            Assert.IsFalse(_triggeredFlags.DetachSlide);

            // Make sure that the operation did not complete and the Slide currently referenced by the
            // view model is the same slide that we copied the reference of earlier.
            Assert.IsTrue(ReferenceEquals(compareReference, CodeFlowViewModel.Instance.CurrentSlide));
        }

        /// <summary>
        /// Make sure that after a file is opened that the filename stored in the file controller reflects
        /// the name of the file that was just opened.
        /// </summary>
        [TestMethod]
        public void tOpenFile_FilenameIsCorrect() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Trying to save a file without a name should cause the Save operation to be treated as a
        /// SaveAs operation, even though the Save operation has its own 'save as' event, this event
        /// SaveFilenameRequest needs to be called.
        /// 
        /// This test will mock the user canceling the SaveAs dialog.
        /// </summary>
        [TestMethod]
        public void tSaveFile_OnUnnamedFile_Cancel() {
            CodeFlowViewModel.Instance.SaveFilenameRequest += OnSaveFilenameRequest_Cancel;
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // Dirty the open file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test Name";

            // Try to save the currently open file.
            CodeFlowViewModel.Instance.SaveFile();

            // Make sure that the SaveFielnameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.SaveFilenameRequest);

            // Make sure the PersistSlideCollection method on the storeController was NOT called.
            var storeController = (FakeStoreController)_pFileController.GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsFalse(storeController.PersistTriggered);
        }

        /// <summary>
        /// Trying to save a file without a name should cause the Save operation to be treated as a
        /// SaveAs operation, even though the Save operation has its own 'save as' event, this event
        /// SaveFilenameRequest needs to be called.
        /// </summary>
        [TestMethod]
        public void tSaveFile_OnUnnamedFile() {
            CodeFlowViewModel.Instance.SaveFilenameRequest += OnSaveFilenameRequest;
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // Dirty the open file.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test Name";

            // Try to save the currently open file.
            CodeFlowViewModel.Instance.SaveFile();

            // Make sure that the SaveFielnameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.SaveFilenameRequest);

            // Make sure the PersistSlideCollection method on the storeController was called.
            var storeController = (FakeStoreController)_pFileController.GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsTrue(storeController.PersistTriggered);
        }

        /// <summary>
        /// When a save file operation is initiated and the file controller does not yet have a name
        /// stored internally for the current file, the save operation will request a filename via
        /// the SaveFilenameRequest event. After the file is saved, make sure that the filename stored
        /// in the file controller reflects the name of the file that was just saved.
        /// </summary>
        [TestMethod]
        public void tSaveFile_OnUnnamedFile_FilenameCorrect() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If a file is open and already has a filename, then calling SaveFile should save the file.
        /// A file should be created and the dirty flag should be cleared.
        /// </summary>
        [TestMethod]
        public void tSaveFile_NamedFile() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // Set the name of the file.
            _pFileController.SetField("_filename", "filename");
            _pFileController.SetField("_extension", "ext");
            _pFileController.SetField("_path", "path");

            // Do something to make the file dirty.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test Name";

            CodeFlowViewModel.Instance.SaveFile();

            // Make sure the PersistSlideCollection method on the storeController was called.
            var storeController = (FakeStoreController)_pFileController.GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsTrue(storeController.PersistTriggered);
        }

        /// <summary>
        /// Attempting to save a file that is not dirty throws an exception. Normally The CanExecute
        /// handler should prevent the SaveFile from being called when the file is not dirty. Remember
        /// that the SaveAs functionality should be what is used in this situation.
        /// </summary>
        [TestMethod]
        public void tSaveFile_NotDirty() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // Set the name of the file.
            _pFileController.SetField("_filename", "filename");
            _pFileController.SetField("_extension", "ext");
            _pFileController.SetField("_path", "path");

            // Clear the dirty flag.
            _pViewModel.Invoke("ClearDirtyFlag", new object[] { });

            Exception e = null;

            try {
                CodeFlowViewModel.Instance.SaveFile();
            } catch(Exception ex) {
                e = ex;
            }

            // Make sure that an exception was thrown.
            Assert.IsNotNull(e);

            // Assert that the exception message is the correct message for attempting to save a file
            // when the dirty flag is clear.
            Assert.AreEqual(e.Message, "Do not call Save when the view model is not dirty.");

        }

        /// <summary>
        /// Calling the SaveAs method when no file is open causes an exception to be thrown. This
        /// should never happen because the CanSaveAs should be returning false when there is not
        /// a file open.
        /// </summary>
        [TestMethod]
        public void tSaveAs_NoFileOpen() {
            Exception e = null;
            try {
                CodeFlowViewModel.Instance.SaveFileAs();
            } catch(Exception ex) {
                e = ex;
            }

            // Make sure that an exception was thrown.
            Assert.IsNotNull(e);

            // Test the exception message the make sure that it is the message that goes along with
            // throwing an exception due to calling saveFileAs when there is no file to save.
            Assert.AreEqual(e.Message, "SaveAs method was called when there is not a slide collection available to save.");
        }

        /// <summary>
        /// Test canceling a save as operation. The SaveAsFilenameRequest event should be fired, at
        /// which point the Event handler should cancel the request. Make sure that the event was fired
        /// and that the PersistSlideCollection method on the store controller was NOT called.
        /// </summary>
        [TestMethod]
        public void tSaveAs_CanceledByUser() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.SaveAsFilenameRequest += OnSaveAsFilenameRequest_Cancel;
            CodeFlowViewModel.Instance.NewFile();

            CodeFlowViewModel.Instance.SaveFileAs();

            // Make sure the SaveAsFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.SaveAsFilenameRequest);

            // Make sure that the file was not saved.
            var storeController = (FakeStoreController)_pFileController.GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsFalse(storeController.PersistTriggered);
        }

        /// <summary>
        /// Test the SaveFileAs operation. The SaveAsFilenameRequest event should be fired, followed
        /// by the IStoreController.PersistSlideCollection method being called.
        /// </summary>
        [TestMethod]
        public void tSaveAs() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.SaveAsFilenameRequest += OnSaveAsFilenameRequest;
            CodeFlowViewModel.Instance.NewFile();

            CodeFlowViewModel.Instance.SaveFileAs();

            // Make sure the SaveAsFilenameRequest event was fired.
            Assert.IsTrue(_triggeredFlags.SaveAsFilenameRequest);

            // Make sure that the file was saved. (By making sure the PersistSlideCollection method
            // was called.
            var storeController = (FakeStoreController)_pFileController.GetField("_storeController", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsTrue(storeController.PersistTriggered);
        }

        /// <summary>
        /// After a save as operation, make sure that the filename stored internally in the file
        /// controller reflects the filename of the file that was just saved.
        /// </summary>
        [TestMethod]
        public void tSaveAs_FilenameCorrect() {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// CanOpenFile should return true under all circumstances.
        /// </summary>
        [TestMethod]
        public void tCanOpen_true() {
            Assert.IsTrue(CodeFlowViewModel.Instance.CanOpenFile());
        }
        
        /// <summary>
        /// CanSaveFile should return true when there is an open file that is dirty.
        /// </summary>
        [TestMethod]
        public void tCanSave_true() {
            // Open a new file.
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // Do something to make it dirty.
            CodeFlowViewModel.Instance.CurrentSlide.Model.Name = "Test";

            // CanSave should now return true.
            Assert.IsTrue(CodeFlowViewModel.Instance.CanSave());
        }

        /// <summary>
        /// CanSave should return false when no file is open, or when a file is open but is
        /// not dirty.
        /// </summary>
        [TestMethod]
        public void tCanSave_false() {
            // Should be false because no file is open.
            Assert.IsFalse(CodeFlowViewModel.Instance.CanSave());

            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();

            // There should be an open file.
            Assert.IsNotNull(CodeFlowViewModel.Instance.CurrentSlide);

            // But it should not be dirty.
            Assert.IsFalse(CodeFlowViewModel.Instance.IsDirty);

            // So CanSave should be false.
            Assert.IsFalse(CodeFlowViewModel.Instance.CanSave());
        }

        /// <summary>
        /// If there is an open file, the CanSaveAs method should return true.
        /// </summary>
        [TestMethod]
        public void tCanSaveAs_true() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();
            Assert.IsTrue(CodeFlowViewModel.Instance.CanSaveAs());
        }

        /// <summary>
        /// If there is not an open file, the CanSaveAs method should return false.
        /// </summary>
        [TestMethod]
        public void tCanSaveAs_false() {
            Assert.IsFalse(CodeFlowViewModel.Instance.CanSaveAs());
        }

        /// <summary>
        /// If there is an open file, CanClose should return true.
        /// </summary>
        [TestMethod]
        public void tCanClose_true() {
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.NewFile();
            Assert.IsTrue(CodeFlowViewModel.Instance.CanCloseFile());
        }

        /// <summary>
        /// If there is not an open file, CanCloseFile should return false.
        /// </summary>
        [TestMethod]
        public void tCanClose_false() {
            Assert.IsFalse(CodeFlowViewModel.Instance.CanCloseFile());
        }
        #region Event Handlers        

        private void OnSlideSelected(object sender, NodeSelectedEventArgs e) {
            _triggeredFlags.SlideSelected = true;
        }

        private void OnDetachSlideRequest(object sender, DetachSlideRequestEventArgs e) {
            _triggeredFlags.DetachSlide = true;
        }

        private void OnSaveFilenameRequest(object sender, FileEventArgs e) {
            _triggeredFlags.SaveFilenameRequest = true;
            e.Filename = _filename;
            e.Ext      = _extension;
            e.Path     = _path;
        }

        private void OnSaveFilenameRequest_Cancel(object sender, FileEventArgs e) {
            _triggeredFlags.SaveFilenameRequest = true;
            e.Cancel = true;
        }

        private void OnSaveAsFilenameRequest(object sender, FileEventArgs e) {
            _triggeredFlags.SaveAsFilenameRequest = true;
            e.Filename = _filename;
            e.Ext      = _extension;
            e.Path     = _path;
        }

        private void OnSaveAsFilenameRequest_Cancel(object sender, FileEventArgs e) {
            _triggeredFlags.SaveAsFilenameRequest = true;
            e.Cancel = true;
        }

        private void OnOpenFilenameRequest(object sender, FileEventArgs e) {
            _triggeredFlags.OpenFilenameRequest = true;
            e.Filename = _filename;
            e.Ext      = _extension;
            e.Path     = _path;
        }

        private void OnOpenFilenameRequest_Cancel(object sender, FileEventArgs e) {
            _triggeredFlags.OpenFilenameRequest = true;
            e.Cancel = true;
        }

        private void OnConfirmDestroyDirtyFileRequest(object sender, CancelableEventArgs e) {
            _triggeredFlags.ConfirmDestroyDirtyFileRequest = true;
        }

        private void OnConfirmDestroyDirtyFileRequest_Cancel(object sender, CancelableEventArgs e) {
            _triggeredFlags.ConfirmDestroyDirtyFileRequest = true;
            e.Cancel = true;
        }

        #endregion
    }
}
