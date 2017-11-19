using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeFlowBuilder.Helpers {

    /// <summary>
    /// The ObjectPropertyHelper class creates a list of ObjectProperty objects representing
    /// the object's public properties and the values of those public properties.
    /// </summary>
    /// <remarks>
    /// The list of NameValue pairs returned by this helper is used by the XmlHelper to create
    /// attribute collections that contain the property names and the values of those properties
    /// for an input object.
    /// </remarks>
    public static class ObjectPropertyHelper {
                
        #region Public Methods

        public static Dictionary<string,object> PropertyValues(this object o, List<string> propNames) {
            var result = new Dictionary<string,object>();
            foreach (var current in propNames)
                result[current] = GetPropertyValue(o, current);
            return result;
        }

        public static T PropertyValue<T>(this object o, string propertyName) {
            return (T)GetPropertyValue(o, propertyName);
        }

        /// <summary>
        /// Returns a list of the public properties on the specified object.
        /// </summary>
        public static List<string> Properties(this object o) {
            return o
                   .GetType()
                   .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                   .Select(x => x.Name)
                   .ToList<string>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the value of the property named by 'propertyName' on the object 'o'
        /// </summary>
        static object GetPropertyValue(object o, string propertyName) {
            var propInfo = o.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            return propInfo.GetValue(o);
        }

        #endregion

    }
}
