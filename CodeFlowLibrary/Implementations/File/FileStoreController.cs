
using System;
using CodeFlowLibrary.Controllers;
using CodeFlowLibrary.History;
using CodeFlowLibrary.Interface;
using CodeFlowLibrary.Model;

namespace CodeFlowLibrary.Implementations {

    /// <summary>
    //++ IStoreController
    ///
    //+  Purpose:
    ///     File based implementation of IStoreController. SlideCollection hierarchies are stored on
    ///     disk.
    ///     
    //+  Remarks:
    ///     The FileStoreController does not perform any existence checks before saving files. If the
    ///     PersistSlideCollection would destroy an existing file, it is expected that the caller has
    ///     already determined that the operation has been confirmed by the user.
    /// </summary>
    public class FileStoreController : IStoreController {

        #region Public

        /// <summary>
        /// Stores 'slideCollection' in a file called 'filename'.
        /// </summary>
        /// <param name="filename">
        /// The name of the file which the slideCollection will be serialized to.</param>
        /// <param name="slideCollection">
        /// The slideCollection that will be serialized.
        /// </param>
        /// <remarks>
        /// The FileStoreController does not perform any existence checks before saving files. If the
        /// PersistSlideCollection would destroy an existing file, it is expected that the caller has
        /// already determined that the operation has been confirmed by the user.
        /// </remarks>
        public void PersistSlideCollection(string filename, SlideCollection slideCollection) {
            Serializer.Serialize(slideCollection, filename);
        }

        /// <summary>
        /// Creates a SlideCollection by deserializing the file 'filename'.
        /// </summary>
        /// <param name="filename">
        /// The name of the file containing the serialized slide collection.
        /// </param>
        /// <param name="historyController">
        /// The history controller that will be wired up to each object within the slide collection.
        /// </param>
        /// <returns>
        /// A new SlideCollection object built by deserializing the file 'filename'
        /// </returns>
        public SlideCollection RestoreSlideCollection(string filename, HistoryController historyController) {
            var result = Serializer.DeSerialize(filename, historyController);
            if(result.GetType() != typeof(SlideCollection))
                throw new InvalidOperationException(
                    $"This file ({filename}) is invalid. The enclosing type ({result.GetType().Name}) is " +
                    $"not type CodeFlowLibrary.Model.ModelBase");
            return result as SlideCollection;
        }

        #endregion

    }
}
