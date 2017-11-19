using CodeFlowLibrary.History;
using System;
using System.Collections.Generic;

namespace CodeFlowLibrary.Model
{
    [Serializable]
    public class MultiChildLink : Link {

        #region Ctor

        public MultiChildLink() { }
        public MultiChildLink(HistoryController hc) : base(hc) { }

        #endregion

    }
}
