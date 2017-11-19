using System;

namespace CodeFlowLibrary.Event {

    /// <summary>
    //++ FileEventArgs
    ///
    //+  Purpose:
    ///     Used with the FileEventHandler delegates for file related events.
    /// </summary>
    public class FileEventArgs : EventArgs {
        public string Filename { get; set; }
        public string Ext      { get; set; }
        public string Path     { get; set; }
        public bool   Cancel   { get; set; }
    }
}
