using CodeFlowLibrary.Attributes;
using CodeFlowLibrary.Exceptions;
using CodeFlowLibrary.History;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CodeFlowLibrary.Model
{
    /// <summary>
    //++ Node
    ///
    //+  Purpose:
    ///     The layer of the model system that implements the Parent/Child relationship of the model
    ///     system.
    /// </summary>
    [Serializable]
    public abstract class Node : ModelBase, INotifyCollectionChanged {

        #region Ctor

        public Node() {
            Children = new List<Node>();
        }

        public Node(HistoryController hc) : base(hc) {
            Children = new List<Node>();
        }

        #endregion

        #region Fields

        [XmlIgnore]
        private Node _parent;

        [XmlIgnore]
        private Node _root;

        #endregion

        #region Events

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Properties

        [XmlIgnore]
        [Browsable(false)]
        [MementoIgnore]
        public Node Parent {
            get { return _parent; }
            set {
                if (_parent == value) return;
                _parent = value;
                NotifyPropertyChanged(nameof(Parent));
            }
        }        

        [Browsable(false)]
        [MementoIgnore]
        public List<Node> Children { get; }

        [XmlIgnore]
        [Browsable(false)]
        [MementoIgnore]
        public Node Root {
            get { return _root ?? (_root = GetRoot()); }
        }

        #endregion

        #region IDisposable

        public override void Dispose() {
            foreach(var child in Children) {
                child.Dispose();
            }

            CollectionChanged = null;

            base.Dispose();
        }

        #endregion

        #region Public

        /// <summary>
        /// The RepairParentReferences should only need to be called by the serializer. The job of the method
        /// is to fix the parent references throughout the hierarchy. This is necessary because the serializer
        /// cannot handle recursive references and therefore does not serialize the parent property. This leads
        /// to the fact that when the hierarchy is deserialized, all of the parent references throughout the 
        /// hierarchy are null and need to be repaired so that they actually point to the correct parent.
        /// </summary>
        public override void RepairParentReferences() {
            if (Children == null) return;
            foreach(var child in Children) {
                child.Parent = this;
                child.RepairParentReferences();
            }
        }

        /// <summary>
        /// Adds a child node to this node.
        /// </summary>
        public void AddChild(Node child) {
            if (!CanAddChild(child))
                throw new InvalidSubclassException(
                    $"Cannot add a class of type {child.GetType().Name} to a class of type {this.GetType().Name}."
                );
            Children.Add(child);
            child.Parent = this;

            NotifyPropertyChanged(nameof(Children));
            NotifyCollectionChanged_Add(child);
        }

        /// <summary>
        /// Removes a child node from this node.
        /// </summary>
        public void RemoveChild(Node child) {
            Children.Remove(child);

            NotifyPropertyChanged(nameof(Children));
            NotifyCollectionChanged_Remove(child);
        }

        /// <summary>
        /// Return a node whose guid matches the guid parameter. This method searches the current
        /// node and all nodes below this one in the hierarchy.
        /// </summary>
        public Node Find(Guid guid) {
            if (Guid == guid) return this;
            foreach(var child in Children) {
                var result = child.Find(guid);
                if (result != null)
                    return result; 
            }
            return null;
        }

        /// <summary>
        /// Return the first node that returns true for the predicate. This method searches the
        /// current node and all nodes below this one in the hierarchy.
        /// </summary>
        public Node Find(Predicate<Node> pred) {
            if (pred(this))
                return this;
            foreach(var child in Children) {
                var result = child.Find(pred);
                if (result != null)
                    return result;
            }
            return null;
        }

        /// <summary>
        /// calls the 'action' parameter on this Node and all nodes below this node in the hierarchy.
        /// </summary>
        public void Apply(Action<Node> action) {
            action(this);
            foreach(var child in Children) {
                child.Apply(action);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// Called by the AddChild method, this method must be overridden by all sub-classes so that the
        /// sub-class can confirm that the child node being added is of an allowed type.
        /// </summary>
        protected abstract bool CanAddChild(Node child);

        protected void NotifyCollectionChanged_Add(Node child) {
            var eventCopy = CollectionChanged;
            if (eventCopy == null) return;

            eventCopy(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, child));
        }

        protected void NotifyCollectionChanged_Remove(Node child) {
            var eventCopy = CollectionChanged;
            if (eventCopy == null) return;

            eventCopy(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, child));
        }        

        #endregion

        #region Private

        /// <summary>
        /// Returns the root of the hierarchy.
        /// </summary>
        private Node GetRoot() {
            if (Parent == null)
                return this;
            return Parent.Root;
        }

        #endregion
    }
}
