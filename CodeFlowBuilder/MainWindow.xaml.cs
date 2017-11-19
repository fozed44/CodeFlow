using CodeFlowLibrary.Model;
using System.Windows;
using System.Windows.Input;
using CodeFlowLibrary.Event;
using CodeFlowLibrary.ViewModel;
using Verification;
using System.Linq;
using CodeFlowLibrary.View;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System;

namespace CodeFlowBuilder
{
    /// <summary>
    //++ MainWindow
    ///
    //+  Purpose:
    ///     Main window that contains the slides, property grids, menus etc.
    ///     
    //+  Setting the selected object.
    ///     The 'selected object' refers to the object that is currently set as the selected object in the
    ///     property grid. It is up to the model views to set their model as the selected object via the 
    ///     SetSelectModel method of the main window. 
    ///     
    ///     The model views have access to the main window through the static 'MainWindow' property that
    ///     exists on the NodeView class. This property is set by the main window itself from within its 
    ///     constructor.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ctor

        public MainWindow() {

            InitializeComponent();

            SubscribeToLibraryEvents();         

        }

        #endregion

        #region Overrides / Window Event Handlers

        /// <summary>
        /// Update the size of the main canvas when the size of the main window has changed.
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            //if (_currentSlide == null) return;
            //_currentSlide.Width = cvsMain.ActualWidth;
            //_currentSlide.Height = cvsMain.ActualHeight;
        }

        #endregion

        #region Public

        public void OnNodeSelected(object sender, NodeSelectedEventArgs e) {
            propertyGrid.SelectedObject = e.Node.Model;
        }

        #endregion

        #region Commands

            #region Generic Can Execute

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

            #endregion

            #region File Commands        

                #region Can Execute

        private void FileSaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanSave();
        }

        private void FileSaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanSaveAs();
        }

        private void FileCloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanCloseFile();
        }

        private void FileNewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanCreateNewFile();
        }

        private void FileOpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanOpenFile();
        }

                #endregion

                #region Executed

        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            CodeFlowViewModel.Instance.OpenFile();            
        }

        private void FileNewCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            CodeFlowViewModel.Instance.NewFile();
        }

        private void FileCloseCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            CodeFlowViewModel.Instance.CloseFile();
        }

        /// <summary>
        /// Check _filename to see if it is null. If _filename is null, that means that the current
        /// _slides object has not been saved, so we call FileSaveAsCommand_Executed which forces
        /// the user to select a filename before trying to save.
        /// 
        /// If _filename already contains a filename, save the current state of _slides to disk
        /// using the SaveFile method.
        /// </summary>
        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            CodeFlowViewModel.Instance.SaveFile();
        }

        /// <summary>
        /// Call the GetSaveFilename method to get a filename from the user. If the user cancels out
        /// of the save dialog, GetSaveFilename returns null, in which case we just exit the method.
        /// If the user selected a filename, use the SaveFile method to actually save the file to
        /// disk.
        /// </summary>
        private void FileSaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
            CodeFlowViewModel.Instance.SaveFileAs();
        }

            #endregion

            #endregion

            #region Undo / Redo

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanUndo();
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = CodeFlowViewModel.Instance.CanRedo();
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (!CodeFlowViewModel.Instance.CanUndo()) return;
            CodeFlowViewModel.Instance.Undo();
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (!CodeFlowViewModel.Instance.CanRedo()) return;
            CodeFlowViewModel.Instance.Redo();
        }

        #endregion

        #endregion

        #region File Event Handlers

        /// <summary>
        /// Handles the ConfirmDestroyDirtyFile event fired by the code flow view model. The event
        /// is fired when the view model is going to destroy the current file, and the current file
        /// is dirty and hasn't been saved. The user is presented with a message box and given the
        /// chance to cancel the operation without losing the currently unsaved changes.
        /// </summary>
        private void OnConfirmDestroyDirtyFile(object sender, CancelableEventArgs e) {
            var result = MessageBox.Show(
                "The open file has unsaved changes. Are you sure you want to destroy those changes?", "Confirm", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.No)
                e.Cancel = true;
        }

        /// <summary>
        /// Handles the OpenFilenameRequest event fired by the code flow view model. The event
        /// is fired when the view model is in an open file operation and need a filename from
        /// the UI.
        /// </summary>
        private void OnOpenFilenameRequest(object sender, FileEventArgs e) {
            OpenFileDialog dg = new OpenFileDialog();
            dg.Filter = "Code Flow Files (*.cfl)|*.cfl";

            if(dg.ShowDialog() == false) {
                e.Cancel = true;
                return;
            }

            e.Filename = Path.GetFileNameWithoutExtension(dg.FileName);
            e.Path     = Path.GetDirectoryName(dg.FileName);
            e.Ext      = Path.GetExtension(dg.FileName);

            return;
        }

        /// <summary>
        /// Handle the SaveFilenameRequest event. This event is fired by the code flow view model when
        /// a save file operation is underway, but the file controller does not have a filename for the
        /// currently open file so the file controller is requesting a filename to use to save the file.
        /// </summary>
        private void OnSaveFilenameRequest(object sender, FileEventArgs e) {
            SaveFileDialog dg = new SaveFileDialog();
            dg.Filter = "Code Flow Files (*.cfl)|*.cfl";

            if(dg.ShowDialog() == false) {
                e.Cancel = true;
                return;
            }

            e.Filename = Path.GetFileNameWithoutExtension(dg.FileName);
            e.Path = Path.GetDirectoryName(dg.FileName);
            e.Ext = Path.GetExtension(dg.FileName);

            return;
        }

        /// <summary>
        /// Handle the SaveAsFilenameRequest event. This is the event that is fired by the code flow
        /// view model when it needs to know the new name of the file to use to save the currently open
        /// file.
        /// </summary>
        private void OnSaveAsFilenameRequest(object sender, FileEventArgs e) {
            SaveFileDialog dg = new SaveFileDialog();
            dg.Filter = "Code Flow Files (*.cfl)|*.cfl";

            if(dg.ShowDialog() == false) {
                e.Cancel = true;
                return;
            }

            e.Filename = Path.GetFileNameWithoutExtension(dg.FileName);
            e.Path = Path.GetDirectoryName(dg.FileName);
            e.Ext = Path.GetExtension(dg.FileName);

            return;
        }

        #endregion

        #region Private

            #region Event subscription

        /// <summary>
        /// Wires up event handlers to various events. The Method is divided into sub methods just
        /// to help keep things tidy and easier to manage.
        /// </summary>
        private void SubscribeToLibraryEvents() {
            SubscribeToSelectionEvents();
            SubscribeToFileEvents();
        }
        
        private void SubscribeToSelectionEvents() {
            CodeFlowViewModel.Instance.NodeSelected  += OnNodeSelected;
            CodeFlowViewModel.Instance.SlideSelected += OnSlideSelected;
            CodeFlowViewModel.Instance.DetachSlideRequest += OnDetachSlideRequest;
        }

        private void SubscribeToFileEvents() {
            CodeFlowViewModel.Instance.ConfirmDestroyDirtyFileRequest += OnConfirmDestroyDirtyFile;
            CodeFlowViewModel.Instance.OpenFilenameRequest += OnOpenFilenameRequest;
            CodeFlowViewModel.Instance.SaveAsFilenameRequest += OnSaveAsFilenameRequest;
            CodeFlowViewModel.Instance.SaveFilenameRequest += OnSaveFilenameRequest;
        }

            #endregion

            #region Slides

        /// <summary>
        /// Attaches 'slide' to the main canvas. Sets 'slide' as the current slide.
        /// </summary>
        private void OnSlideSelected(object sender, NodeSelectedEventArgs e) {
            Verify.True(cvsMain.Children.OfType<SlideView>().Count() == 0,
                "Cannot select a new slide while a slide is currently attached.");
            cvsMain.Children.Add(e.Node);
            Canvas.SetTop(e.Node, 1);
            Canvas.SetLeft(e.Node, 1);
            e.Node.Width = cvsMain.ActualWidth;
            e.Node.Height = cvsMain.ActualHeight;
        }
        
        /// <summary>
        /// Removes the slide that is referenced in the 'SlideView' property of the DetachSlideRequestEventArgs
        /// from the children of the cvsMain main window.
        /// </summary>
        private void OnDetachSlideRequest(object sender, DetachSlideRequestEventArgs e) {
            cvsMain.Children.Remove(e.SlideView);
        }

            #endregion

        #endregion                
        
    }
}
