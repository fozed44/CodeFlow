using CodeFlowLibrary.History;
using CodeFlowLibrary.Interface;
using CodeFlowLibrary.Model;

namespace tCodeFlowBuilder.Implementations {

    /// <summary>
    //++ FakeFileController
    ///
    //+  Purpose:
    ///     The purpose of the FakeFileController is to provide a file controller for use when running
    ///     unit tests. The PersistSlideCollection implementation does not actually save a file but it
    ///     does set the PersistTriggered property to true so that the unit test can confirm that the
    ///     method was called. The RestoreSlideCollection has a similar RestoreTriggered property so 
    ///     that the unit test can confirm that the method was called, and also returns a valid 
    ///     SlideCollection.
    ///     
    ///     This controller is only used to test the logic in the file controller related to the opening
    ///     closing, saving and creating new file operations.
    /// </summary>
    public class FakeStoreController : IStoreController {

        public bool PersistTriggered { get; set; }
        public bool RestoreTriggered { get; set; }

        public void PersistSlideCollection(string filename, SlideCollection slideCollection) {
            PersistTriggered = true;
        }

        public SlideCollection RestoreSlideCollection(string filename, HistoryController historyController) {
            RestoreTriggered = true;
            return new SlideCollection(historyController);
        }
    }
}
