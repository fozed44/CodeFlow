using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CodeFlowLibrary.Custom_Controls {
         
    /// <summary>
    //++ CanvasWithCanvasParent
    /// 
    //+ Purpose:
    ///     A canvas that whose only allowed parent type is a canvas. This is enforced by
    ///     throwing an exception inside of the Loaded event handler if the parent of this
    ///     control is not a canvas.
    /// </summary>
    public class CanvasWithCanvasParent : Canvas {

        #region Static Constructor

        /// <summary>
        /// Auto Generated.
        /// </summary>
        static CanvasWithCanvasParent() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CanvasWithCanvasParent), new FrameworkPropertyMetadata(typeof(CanvasWithCanvasParent)));
        }

        #endregion

        #region Instance Constructor

        /// <summary>
        /// Construct a new CanvasWithConvasParent. 
        /// Hook the loaded event.
        /// </summary>
        public CanvasWithCanvasParent() {
            Loaded += CanvasWithConvasParent_Loaded;
        }

        #endregion

        #region Loaded Event Handler

        /// <summary>
        /// When the control has finished loading, check the Parent property to ensure that
        /// the parent is of type Canvas.
        /// </summary>
        private void CanvasWithConvasParent_Loaded(object sender, RoutedEventArgs e) {
            if (Parent.GetType() != typeof(Canvas))
                throw new InvalidOperationException(
                    "Any control that inherits from CanvasWithConvasParent can only " +
                    "be placed inside a canvas control."
                );
        }

        #endregion
    }
}
