using CodeFlowLibrary.Custom_Controls;
using CodeFlowLibrary.Exceptions;
using CodeFlowLibrary.Model;
using CodeFlowLibrary.View;
using System;
using System.Collections.Generic;
using Verification;
using System.Diagnostics;

namespace CodeFlowLibrary.Factory {

    /// <summary>
    //++ ViewFactory
    ///
    //+  Purpose
    ///     Instantiate and return a single view for a single view, or instantiate and return an
    ///     entire view hierarchy.
    /// </summary>
    public class ViewFactory {

        #region Construction

        /// <summary>
        /// Initialize the view cache.
        /// </summary>
        public ViewFactory() {
            _viewCache = new Dictionary<WeakReference, WeakReference>();
        }

        #endregion

        #region Fields

        private static ViewFactory _instance;

        /// <summary>
        /// Caches views that have been created by the ViewFactory for specific models. The
        /// Keys in the dictionary points to a models while the values point to views.
        /// </summary>
        private Dictionary<WeakReference,WeakReference> _viewCache;

        #endregion

        #region Public

        /// <summary>
        /// Returns the view for 'model', and all children below 'model' in its hierarchy.
        /// </summary>
        public NodeView GetHierarchy(ModelBase model) {
            var root = GetView(model);
            GetHierarchyRecursive(root);
            return root;
        }

        /// <summary>
        /// Returns the view for 'model'.
        /// </summary>
        public NodeView GetView(ModelBase model) {
            var cached = GetViewFromCache(model);
            if (cached != null)
                return cached;

            var view = InstantiateView(model);
            AttachModel(view, model);
            return view;
        }

        /// <summary>
        /// Return the view for 'model'.
        /// </summary>
        public T GetView<T>(ModelBase model) {
            return (T)(object)GetView(model);
        }

        /// <summary>
        /// Return the view for 'model'. If attachToModel is false, the model is not attached to the view.
        /// </summary>
        /// <remarks>
        /// If this overload of GetView is called with attachToModel is false, it is possible to return a 
        /// newly instantiated view when the model already has a view attached to it. Basically when
        /// attachToModel is false, a new view is forcibly created, and the cache is ignored.
        /// </remarks>
        public NodeView GetView(ModelBase model, bool attachToModel) {
            if(attachToModel)
                return GetView(model);
            else {
                dbTraceForcedInstantation(model);
                return InstantiateView(model);
            }
        }

        /// <summary>
        /// Does 'model' have a cached view?
        /// </summary>
        /// <param name="model">
        /// The model to test for an existing view.
        /// </param>
        /// <returns>
        /// True if the referenced model has a view in the view cache.
        /// </returns>
        public bool HasCachedView(ModelBase model) {
            return GetViewFromCache(model) != null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the instance of the ViewFactory.
        /// </summary>
        public static ViewFactory Instance {
            get {
                return _instance ?? (_instance = new ViewFactory());
            }
        }

        #endregion

        #region Private

            #region view cache

        /// <summary>
        /// Attempt to retrieve a view for the provided model from the _viewCache. If the view exists in the cache
        /// it is returned to the caller, otherwise the return value is null.
        /// 
        /// In addition to retrieving a view from the cache, this model detects situations where there is a view in
        /// the cache but it is no longer alive. If this is the case, then the view is removed from the cache and
        /// the method returns null.
        /// </summary>
        private NodeView GetViewFromCache(ModelBase model) {
            foreach (var pair in _viewCache) {
                if (pair.Key.Target == model) {
                    if (pair.Key.IsAlive)
                        return pair.Value.Target as NodeView;
                    else {
                        _viewCache.Remove(pair.Key);
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Caches the view/model pair.
        /// </summary>
        private void CacheView(ModelBase model, NodeView view) {
            _viewCache.Add(new WeakReference(model), new WeakReference(view));
        }

            #endregion

        /// <summary>
        /// Instantiates a new NodeView specific to the sub-class of 'model'. The instantiated view
        /// is cached in _viewCache.
        /// </summary>
        private NodeView InstantiateView(ModelBase model) {
            var modelType = model.GetType();
            NodeView view;

            if(modelType == typeof(DefaultNode))
                view = new DefaultNodeView();

            else if(modelType == typeof(Slide))
                view = new SlideView();

            else
                throw new ViewNotFoundException(
                    $"Could not find a view for model type {model.GetType().Name}"
                );

            CacheView(model, view);
            return view;
        }

        
        private void GetHierarchyRecursive(NodeView parentNodeView) {
            foreach(var child in parentNodeView.Model.Children) {
                var childView = GetView(child);
                parentNodeView.AddChildView(childView);
                GetHierarchyRecursive(childView);
            }
        }

        /// <summary>
        /// Attaches the model to the view.
        /// </summary>
        private void AttachModel(NodeView view, ModelBase model) {
            var casted = model as Node;
            Verify.NotNull(
                casted, 
                $"The model {model.Name} cannot be attached to a view, the model is not a subclass of Node."
            );
            view.AttachToModel(casted);
        }

        /// <summary>
        /// Creates a debug trace when the GetView method is called with AttachToModel = false.
        /// </summary>8
        [Conditional("DEBUG")]
        private void dbTraceForcedInstantation(ModelBase model) {
            Debug.WriteLine($"WARNING: Forcibly instantiating view for model {model.Name}.");
        }

        #endregion
    }
}
