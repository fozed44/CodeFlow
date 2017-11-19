using System;

namespace CodeFlowLibrary.Exceptions {

    /// <summary>
    //++ IgnoredPropertyException
    ///
    //+  Purpose:
    ///     Thrown when creating a memento on a property that has the MementoIngore attribute.
    /// </summary>
    [Serializable]
    public class IgnoredPropertyException : Exception {
        public IgnoredPropertyException() { }
        public IgnoredPropertyException(string message) : base(message) { }
        public IgnoredPropertyException(string message, Exception inner) : base(message, inner) { }
        protected IgnoredPropertyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
