using CodeFlowLibrary.History;
using CodeFlowLibrary.Model;
using System.IO;
using System.Xml.Serialization;

namespace CodeFlowLibrary.Controllers {

    /// <summary>
    //++ Serializer
    ///
    //+  Purpose:
    ///     To Serialize/Deserialize a ModelBase hierarchy to or from a disk file.
    /// </summary>
    public static class Serializer {

        #region Public Methods

        /// <summary>
        /// Serializes the hierarchy represented by the root parameter to a file named 'filename'.
        /// </summary>
        public static void Serialize(ModelBase root, string filename) {
            var serializer = new XmlSerializer(typeof(ModelBase));
            var textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, root);
            textWriter.Close();
        }

        /// <summary>
        /// De-serializes a model hierarchy from a file named 'filename'.
        /// </summary>
        public static ModelBase DeSerialize(string filename) {
            ModelBase result;
            var serializer = new XmlSerializer(typeof(ModelBase));
            var textReader = new StreamReader(filename);
            result = (ModelBase)serializer.Deserialize(textReader);
            result.RepairParentReferences();
            return result;
        }

        /// <summary>
        /// De-serializes a model hierarchy from a file name 'filename'. After the file is deserialized, each
        /// model in the hierarchy has its PropertyChanging event wired up to the history controller referenced
        /// by the 'historyController' parameter.
        /// 
        /// Calling this method will clear the contents of the history controller.
        /// </summary>
        public static ModelBase DeSerialize(string filename, HistoryController historyController) {
            var root = DeSerialize(filename);
            var casted = root as Node;
            if (casted != null) {
                casted.Apply(x => {
                    x.PropertyChanging += historyController.HandleModelPropertyChanging;
                });
            }
            historyController.Clear();
            return root;
        }

        #endregion
    }
}
