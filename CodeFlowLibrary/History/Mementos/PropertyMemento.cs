
using CodeFlowLibrary.Model;
using System.Collections.Generic;
using System.Linq;

using static ReflectionExtensions.ReflectionExtensions;

namespace CodeFlowLibrary.History {

    /// <summary>
    //++ Memento
    ///
    //+  Purpose:
    ///     A memento implementation that is used specifically for restoring properties.
    ///     
    ///     When a PropertyMemento is created, the memento creates a snapshot of the values of the properties
    ///     whose names are listed in the propertyNames or propertyName parameters of the constructors.
    ///     
    ///     Calling restore will restore the values of these properties to the values of those properties at
    ///     the time the snapshot was taken (when the memento was constructed).
    /// </summary>
    public class PropertyMemento : Memento {

        #region Ctor

        /// <summary>
        /// Create a PropertyMemento that stores the values of the properties listed in 'propertyNames'
        /// on the object referenced by 'model'.
        /// </summary>
        public PropertyMemento(ModelBase model, IEnumerable<string> propertyNames) 
            : base(model) {
            _propertyDictionary = new Dictionary<string, object>();
            BuildDictionary(propertyNames);
        }

        /// <summary>
        /// Create a PropertyMemento that stores the value of the property named by 'propertyName' on 
        /// the object referenced by 'model'.
        /// </summary>
        public PropertyMemento(ModelBase model, string propertyName) 
            : base(model) {
            _propertyDictionary = new Dictionary<string, object>();
            BuildDictionary(propertyName);
        }

        #endregion

        #region Fields

        /// <summary>
        /// Stores the property names and the corresponding property values of the properties listed
        /// when the memento was constructed.
        /// </summary>
        private Dictionary<string, object> _propertyDictionary;

        #endregion

        #region Public

        /// <summary>
        /// Restores the values of the properties listed at the time of the memento's construction to
        /// the values of those properties at the time of the memento's construction.
        /// </summary>
        public override IMemento Restore() {
            var current = new PropertyMemento(Model, _propertyDictionary.Select(x => x.Key));
            RestoreProperties();
            return current;
        }

        #endregion

        #region Private

        /// <summary>
        /// Builds the dictionary storing the property names and property values on the properties contained
        /// in 'propertyNames'.
        /// </summary>
        private void BuildDictionary(IEnumerable<string> propertyNames) {
            foreach(var propertyName in propertyNames) {
                _propertyDictionary.Add(
                    propertyName,
                    Reflect(Model).GetPropVal(propertyName)
                );
            }
        }

        /// <summary>
        /// Builds the dictionary that will store the property name and property value of the single property
        /// named by 'propertyName'.
        /// </summary>
        private void BuildDictionary(string propertyName) {
            _propertyDictionary.Add(
                propertyName,
                Reflect(Model).GetPropVal(propertyName)
            );
        }

        /// <summary>
        /// Restores the values of the properties whose names and values were stored in the internal dictionary
        /// at the time the memento was constructed.
        /// </summary>
        private void RestoreProperties() {
            foreach(var property in _propertyDictionary) {
                Reflect(Model).SetPropVal(property.Key, property.Value);
            }
        }

        #endregion
    }
}
