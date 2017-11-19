using System;
using CodeFlowLibrary.View;

namespace CodeFlowLibrary.Event {

    /// <summary>
    //++ DetachSlideRequestEventArgs
    ///
    //+  Purpose:
    ///     Event args for the DetatchSlideRequestEventHandler witch is the event handler for the DetatchSlide
    ///     event that is fired by the code flow view library when it needs to signal the UI that a slide
    ///     needs to be detached from the UI.
    /// </summary>
    public class DetachSlideRequestEventArgs : EventArgs {
        public SlideView SlideView { get; set; }
    }
}
