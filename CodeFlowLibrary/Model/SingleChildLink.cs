using CodeFlowLibrary.History;
using System;

namespace CodeFlowLibrary.Model
{
    [Serializable]
    public class SingleChildLink : Link {

        #region Ctor

        public SingleChildLink() { }
        public SingleChildLink(HistoryController hc) : base(hc) { }

        #endregion
    }

}