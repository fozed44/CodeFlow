using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeFlowLibrary.Event {

    /// <summary>
    //++ CancelableEventHandler
    ///
    //+  Purpose:
    ///     Delegate for an event that represents a cancelable operation.
    /// </summary>
    public delegate void CancelableEventHandler(object sender, CancelableEventArgs e);
}
