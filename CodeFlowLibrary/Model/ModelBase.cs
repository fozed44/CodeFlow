using System;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Diagnostics;
using CodeFlowLibrary.Factory;
using CodeFlowLibrary.History;
using ReflectionExtensions;

using static ReflectionExtensions.ReflectionExtensions;
using Verification;
using CodeFlowLibrary.Exceptions;
using CodeFlowLibrary.Attributes;
using CodeFlowLibrary.Custom_Controls;

namespace CodeFlowLibrary.Model
{

    /// <summary>
    //++ ModelBase
    ///
    //++ Purpose:
    ///     Base class for the model system that covers the various models that back the views that are presented
    ///     in the main canvas of the main window.
    ///     
    ///     The two properties provided here are Name and Guid. The name property just gives the model a human
    ///     readable name, while the guid gives the model a unique name that can be used to search the model
    ///     hierarchy using the Find methods.
    ///     
    ///     The PropertyChanged and PropertyChanging events are used to link the model to the view so that the
    ///     view can be updated when the model properties are changed. Further, these events are used by
    ///     the history controller to create mementos to support Undo and Redo functionality.
    ///     
    ///     The Undo/Redo functionality supported by the history controller is further supported by the
    ///     GetMemento method, which is implemented using reflection so that sub-classes of ModelBase to not
    ///     need to provide any further functionality to make the GetMemento method to work.
    ///     
    //+  Change Notification:
    ///     All sub-classes of ModelBase must make sure to call NotifyPropertyChanging and NotifyPropertyChanged
    ///     to make sure property notification is properly implemented.
    ///     
    //+  HistoryController:
    ///     The history controller is the object that supports the Undo/Redo functionality of CodeFlow. In order
    ///     to do this, each ModelBase sub-class object that is going to be participating in Undo/Redo operations
    ///     be connected to the history controller via the history controller wiring up to the NotifyPropertyChanging
    ///     and NotifyPropertyChanged events of the ModelBase sub-classed object.
    ///     
    ///     The ModelBase implementation provides a constructor that can automatically create these connections
    ///     upon construction. These connections are automatically created if the ModelBase constructor that
    ///     has a HistoryController parameter is used to construct the object.
    ///     
    //+  Undo Implementation
    ///     
    /// </summary>
    
    [Serializable]
    [XmlInclude(typeof(Node))]
    [XmlInclude(typeof(Link))]
    [XmlInclude(typeof(DefaultNode))]
    [XmlInclude(typeof(MultiChildLink))]
    [XmlInclude(typeof(SingleChildLink))]
    [XmlInclude(typeof(Slide))]
    [XmlInclude(typeof(SlideCollection))]
    public class ModelBase : INotifyPropertyChanged, INotifyPropertyChanging, IDisposable {

        #region Ctor

        public ModelBase() {
            Guid = Guid.NewGuid();
        }

        public ModelBase(HistoryController historyController) 
            : this() {
            PropertyChanging += historyController.HandleModelPropertyChanging;
        }

        ~ModelBase() {
            Verify.True(_disposed, "Object not disposed.");
        }

        #endregion

        #region fields 

        private string    _name;
        private Guid      _guid;
        private bool      _disposed;

        #endregion

        #region Properties

        [DisplayName("Name")]
        [Category("Basic")]
        [Description("Name of the node.")]
        public string Name {
            get { return _name; }
            set {
                if (_name == value) return;
                NotifyPropertyChanging(nameof(Name));
                _name = value;
                NotifyPropertyChanged(nameof(Name));
            }
        }

        [Browsable(false)]
        [MementoIgnore]
        public Guid Guid {
            get { return _guid; }
            set {
                if (_guid == value) return;
                NotifyPropertyChanging(nameof(Guid));
                _guid = value;
                NotifyPropertyChanged(nameof(Guid));
            }
        }        

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler  PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void NotifyPropertyChanged(string propertyName) {
            VerifyHasProperty(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Changing any property fires the NotifyDirty event.
            NotifyDirty?.Invoke(this, EventArgs.Empty);
        }

        protected void NotifyPropertyChanging(string propertyName) {
            VerifyHasProperty(propertyName);
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion

        #region Events

        /// <summary>
        /// Global event that is fired when any property on any ModelBase derived object that uses
        /// property change notification has changed. This event has the self evident purpose of
        /// allowing consumers to easily track whether or not the current hierarchy is dirty.
        /// </summary>
        public static event EventHandler NotifyDirty;

        #endregion

        #region IDisposable

        /// <summary>
        /// Make sure that any event subscriptions have been cleared.
        /// </summary>
        public virtual void Dispose() {
            PropertyChanged = null;
            PropertyChanging = null;
            if(ViewFactory.Instance.HasCachedView(this))
                ViewFactory.Instance.GetView(this).Dispose();
            _disposed = true;
        }        

        #endregion

        #region Public

        /// <summary>
        /// The purpose of this function is to repair the parent references of derived classes that have
        /// parent references after the serializer has deserialized a file.
        /// 
        /// This is required because the serializer uses the standard XmlSerializer to serialize/deserialize
        /// objects. The XmlSerializer does not support circular references and the Parent reference used
        /// by many of the classes derived from ModelBase does indeed create a circular reference.
        /// 
        /// To remedy this, each sub-class that uses a Parent reference declares the property as NonSerialized.
        /// After Serializer.DeSerialize has deserialized the xml file to an object hierarchy, Serializer.DeSerialize
        /// calls RepairParentReferences on the root object. This method then propagates through the object
        /// hierarchy, setting the Parent references to their correct values.
        /// </summary>
        
        public virtual void RepairParentReferences() { }
        
        /// <summary>
        /// Creates a memento that captures the state of the named property.
        /// </summary>
        public IMemento GetMemento(string propertyName) {
            var propertyValue = Reflect(this).GetPropVal(propertyName);
            return new PropertyMemento(this, propertyName);
        }

        /// <summary>
        /// Gets the view for this model from the View factory
        /// </summary>
        public T GetView<T>() {
            return ViewFactory.Instance.GetView<T>(this);
        }

        /// <summary>
        /// Get the view for this model from the view factory.
        /// </summary>
        public object GetView() {
            return ViewFactory.Instance.GetView(this);
        }

        #endregion

        #region Private

        [Conditional("DEBUG")]
        private void VerifyHasProperty(string propertyName) {
            Verify.NotNull<PropertyNotFoundException>(
                Reflect(this).Property(propertyName),
                $"{this.GetType().Name} does not have a property named {propertyName}");
        }

        #endregion
    }
}

