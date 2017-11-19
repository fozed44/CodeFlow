using CodeFlowLibrary.Custom_Controls;
using System;

namespace CodeFlowLibrary.Event {
    
    /// <summary>
    //++ NodeSelectedEventArgs
    ///
    //+  Purpose:
    ///     Carries a reference to a NodeView. Used with the NodeSelected event of the CodeFlowViewModel.
    /// </summary>
    public class NodeSelectedEventArgs : EventArgs {
        public NodeSelectedEventArgs(NodeView node) { Node = node; }
        public NodeView Node { get; set; }
    }
}
