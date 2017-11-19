using CodeFlowLibrary.History;
using System;

namespace CodeFlowLibrary.Model
{
    [Serializable]
    public class Link : Node {

        #region Ctor

        public Link() { }
        public Link(HistoryController hc) : base(hc) { }

        #endregion

        protected override bool CanAddChild(Node child) {
            return child.GetType().IsSubclassOf(typeof(Link));
        }
    }
}
