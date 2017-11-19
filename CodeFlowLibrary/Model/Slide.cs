using CodeFlowLibrary.History;
using System;
using System.Collections.Generic;

namespace CodeFlowLibrary.Model {

    [Serializable]
    public class Slide : Node {

        #region Ctor

        public Slide() { }
        public Slide(HistoryController hc) : base(hc) { }

        #endregion

        protected override bool CanAddChild(Node child) {
            return child.GetType().IsSubclassOf(typeof(VisualNode))
                || child.GetType() == typeof(VisualNode)
                || child.GetType().IsSubclassOf(typeof(Link))
                || child.GetType() == typeof(Link);
        }
    }
}
