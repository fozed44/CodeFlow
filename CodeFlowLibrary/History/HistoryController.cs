using CodeFlowLibrary.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFlowLibrary.History {

    /// <summary>
    //++ HistoryController
    ///
    //+  Purpose:
    ///     Provides Undo and Redo stacks that are used by the system to store changes to the structure and
    ///     properties of the Node hierarchy.
    ///     
    //+  Change Notification
    ///     The history controller implements a HandleModelPropertyChanging method that can be wired up to the
    ///     PropertyChangeing event of ModelBase objects. If this method is wired up to a ModelBase object, a
    ///     memento is automatically created and added to the Undo stack every time a property is changed on
    ///     that ModelBase object.
    /// </summary>
    public class HistoryController {

        #region Ctor

        public HistoryController() {
            _undo = new Stack<History.IMemento>();
            _redo = new Stack<History.IMemento>();
        }

        #endregion

        #region Model Property Change Notification Handling
        
        public void HandleModelPropertyChanging(object sender, PropertyChangingEventArgs e) {
            if (_undoLock) return;
            Do((sender as ModelBase)?.GetMemento(e.PropertyName));
        }

        #endregion

        #region Fields

        private Stack<IMemento> _undo;
        private Stack<IMemento> _redo;

        private bool _undoLock = false;

        #endregion

        public bool CanUndo() {
            return _undo.Count > 0;
        }

        public bool CanRedo() {
            return _redo.Count > 0;
        }

        public void Undo() {
            if (_undo.Count() == 0) return;
            var topMemento = _undo.Pop();
            _undoLock = true;
            _redo.Push(topMemento.Restore());
            _undoLock = false;
        }
        public void Redo() {
            if (_redo.Count() == 0) return;
            var topMemento = _redo.Pop();
            _undoLock = true;
            _undo.Push(topMemento.Restore());
            _undoLock = false;
        }

        public void Do(IMemento memento) {
            _redo.Clear();
            _undo.Push(memento);
        }

        public void Clear() {
            _undo.Clear();
            _redo.Clear();
        }
    }
}
