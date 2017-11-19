using System;

namespace CodeFlowLibrary.Exceptions {

    /// <summary>
    //++ ViewNotFoundException
    ///
    //+  Purpose:
    ///     Thrown by the ViewFactory when a view cannot be found for a model. 
    /// </summary>
    [Serializable]
    public class ViewNotFoundException : Exception {
        public ViewNotFoundException() { }
        public ViewNotFoundException(string message) : base(message) { }
        public ViewNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected ViewNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
