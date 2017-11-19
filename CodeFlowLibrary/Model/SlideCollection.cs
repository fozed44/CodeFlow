using CodeFlowLibrary.History;
using System;
using System.Linq;

namespace CodeFlowLibrary.Model {

    [Serializable]
    public class SlideCollection : Node {

        #region Ctor

        public SlideCollection() { }
        public SlideCollection(HistoryController hc) : base(hc) { }

        #endregion

        #region Public

        /// <summary>
        /// Return true if 'child' can be added to this slide collection's children.
        /// </summary>
        /// <returns>
        /// True if 'child' can be added to this slide collection's list of children.
        /// </returns>
        /// <remarks>
        /// As long as 'child' is a slide, it can be added, otherwise it cannont.
        /// </remarks>
        protected override bool CanAddChild(Node child) {
            return child.GetType().IsSubclassOf(typeof(Slide))
                || child.GetType() == typeof(Slide);
        }

        public Slide GetFirstOrNew() {
            if(Children.Count > 0)
                return (Slide)Children.ElementAt(0);
            return new Model.Slide();
        }

        #endregion

        #region Private

        #endregion

    }
}
