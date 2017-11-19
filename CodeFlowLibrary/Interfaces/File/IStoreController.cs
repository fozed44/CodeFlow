
using CodeFlowLibrary.History;
using CodeFlowLibrary.Model;

namespace CodeFlowLibrary.Interface {

    /// <summary>
    //++ IStoreController
    ///
    //+  Purpose:
    ///     Interface for an object that can persist a slide collection.
    /// </summary>
    public interface IStoreController {

        /// <summary>
        /// Persists a slide collection in a way that the collection can be retrieved by the 
        /// RestoreSlideCollection method.
        /// </summary>
        /// <param name="filename">
        /// The filename (including path and extension). The file does not exist it is created, if
        /// the already exists it is written over automatically.
        /// </param>
        /// <param name="slideCollection">
        /// The slide collection that is to be stored.
        /// </param>
        void PersistSlideCollection(string filename, SlideCollection slideCollection);

        /// <summary>
        /// Restores a slide collection that was persisted by the PersistSlideCollection.
        /// </summary>
        /// <param name="filename">
        /// The filename (including path and extension). If the file does not exists, an exception
        /// will be thrown.
        /// </param>
        /// <param name="historyController">
        /// Reference to the global history controller.
        /// </param>
        /// <returns>
        /// A SlideCollection object that is the root node of the hierarchy. The return value will
        /// never be null. If there is an error restoring the file, an exception must be thrown.
        /// </returns>
        SlideCollection RestoreSlideCollection(string filename, HistoryController historyController);
    }
}
