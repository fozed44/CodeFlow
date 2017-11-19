using System;

namespace CodeFlowLibrary.Event {

    /// <summary>
    //++ CancelableEventArgs
    ///
    //+  Purpose:
    ///     EventArgs for CancelableEventHandler typed events.
    /// </summary>
    public class CancelableEventArgs : EventArgs {
        public bool Cancel { get; set; }
    }
}
