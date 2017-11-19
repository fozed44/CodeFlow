using System;

namespace CodeFlowLibrary.Exceptions {

    [Serializable]
    public class InvalidSubclassException : Exception {
        public InvalidSubclassException() { }
        public InvalidSubclassException(string message) : base(message) { }
        public InvalidSubclassException(string message, Exception inner) : base(message, inner) { }
        protected InvalidSubclassException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
