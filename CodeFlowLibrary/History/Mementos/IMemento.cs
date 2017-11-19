using CodeFlowLibrary.Model;
using System;

namespace CodeFlowLibrary.History {

    /// <summary>
    //++ IMemento
    ///
    //+  Purpose:
    ///     Interface for memento implementations.
    ///     
    ///     The memento interface and its implementations facilitate the undo/redo functionality of Code Flow.
    ///     The idea is that whenever a model object is created/destroyed/has a property set, an IMemento implementation
    ///     is created and stored in the history controller. The history controller can then use the IMemento objects
    ///     in it's Undo and Redo methods to restore the state of objects.
    ///     
    ///     The Model property references the model that was used to create the memento. The model that was used to
    ///     create the memento is the model that will be restored by the Restore method.
    /// </summary>
    public interface IMemento {

        // Should be the reference of the model that created the memento.
        ModelBase Model { get; }

        // Restores the properties of the model referenced by the model property to the state of those
        // properties as they were before the memento was created.
        IMemento Restore();
    }
}
