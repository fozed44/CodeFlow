using System;

namespace CodeFlowLibrary.Attributes {
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class MementoIgnoreAttribute : Attribute {}
}
