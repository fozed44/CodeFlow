using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.Exceptions {

    /// <summary>
    //++ TypeMismatchException
    ///
    //+  Purpose:
    ///     Indication that an object or property is not of the expected type.
    /// </summary>
    [Serializable]
    public class TypeMismatchException : Exception {
        public TypeMismatchException() { }
        public TypeMismatchException(string message) : base(message) { }
        public TypeMismatchException(string message, Exception inner) : base(message, inner) { }
        protected TypeMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
