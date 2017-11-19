using System;

namespace CodeFlowLibrary.Exceptions {

    /// <summary>
    //++ GuidMismatchException
    ///
    //+  Purpose:
    ///     Thrown when a comparison is made between two ModelBase sub-class instances, and the Guids
    ///     of the object references are different.
    /// </summary>
    [Serializable]
    public class GuidMismatchException : Exception {
        public GuidMismatchException() { }
        public GuidMismatchException(string message) : base(message) { }
        public GuidMismatchException(string message, Exception inner) : base(message, inner) { }
        protected GuidMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
