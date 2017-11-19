using System;

namespace CodeFlowLibrary.Exceptions {

    /// <summary>
    //++ PropertyNotFoundException
    ///
    //+  Purpose:
    ///     Indicates that an object does not have a requested property.
    /// </summary>
    [Serializable]
    public class PropertyNotFoundException : Exception {
        public PropertyNotFoundException() { }
        public PropertyNotFoundException(string message) : base(message) { }
        public PropertyNotFoundException(string message, Exception inner) : base(message, inner) { }
        protected PropertyNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
