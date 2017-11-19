
using System;
using CodeFlowLibrary.Model;
using Verification;
using CodeFlowLibrary.Exceptions;

namespace CodeFlowLibrary.History {
    
    /// <summary>
    //++ Memento
    ///
    //+  Purpose:
    ///     Base implementation of IMemento.
    ///     
    ///     The functionality of this base implementation only includes storing the reference of the object
    ///     used to create the memento.
    /// </summary>
    public abstract class Memento : IMemento {

        #region Ctor

        /// <summary>
        /// Create a new Memento: Store the reference of the object used to create the memento.
        /// </summary>
        /// <param name="guid"></param>
        public Memento(ModelBase model) {
            _model = model;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Reference of the object used to create the memento.
        /// </summary>
        private ModelBase _model;

        #endregion

        #region Properties

        /// <summary>
        /// Returns a reference to the model that was used to create this memento.
        /// </summary>
        public ModelBase Model {
            get { return _model; }
        }

        #endregion

        #region Public

        /// <summary>
        /// Restores the object referenced by this memento to the state of the object before the memento 
        /// was created.
        /// </summary>
        public abstract IMemento Restore();

        #endregion
    }
}
