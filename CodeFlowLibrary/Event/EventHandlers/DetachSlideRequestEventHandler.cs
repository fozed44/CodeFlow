
namespace CodeFlowLibrary.Event {

    /// <summary>
    //++ DetachSlideRequestEventHandler
    ///
    //+  Purpose:
    ///     Event fired by the code flow library when the UI needs the detach a slide from the main
    ///     view.
    /// </summary>
    public delegate void DetachSlideRequestEventHandler(object sender, DetachSlideRequestEventArgs e);
}
