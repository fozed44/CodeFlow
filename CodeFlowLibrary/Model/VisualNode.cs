
using CodeFlowLibrary.History;
using System;
using System.ComponentModel;

namespace CodeFlowLibrary.Model {
    public class VisualNode : Node {

        #region Ctor

        public VisualNode() { }
        public VisualNode(HistoryController hc) : base(hc) { }

        #endregion

        #region Fields

        private string _comment;
        private string _description;

        #endregion

        #region Properties

        [Category("Basic")]
        public string Comment {
            get { return _comment; }
            set {
                if (_comment == value) return;
                _comment = value;
                NotifyPropertyChanged(nameof(Comment));
            }
        }

        [Category("Basic")]
        public string Description {
            get { return _description; }
            set {
                if (_description == value)
                    return;
                _description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }
        #endregion

        protected override bool CanAddChild(Node child) {
            return child.GetType() == typeof(VisualNode)
                || child.GetType().IsSubclassOf(typeof(VisualNode));
        }
    }
}
