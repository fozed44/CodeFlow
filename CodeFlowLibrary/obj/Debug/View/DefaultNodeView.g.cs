﻿#pragma checksum "..\..\..\View\DefaultNodeView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AC39D5DF49F3C1EF19C661F57F39DA5B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CodeFlowLibrary.Custom_Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CodeFlowLibrary.View {
    
    
    /// <summary>
    /// DefaultNodeView
    /// </summary>
    public partial class DefaultNodeView : CodeFlowLibrary.Custom_Controls.DraggableNodeView, System.Windows.Markup.IComponentConnector {
        
        
        #line 10 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CodeFlowLibrary.View.DefaultNodeView usrControl;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem cmNewChild;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem cmNewLink;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border outerBorder;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Canvas cvsMain;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CodeFlowLibrary.Custom_Controls.DraggableLabel dlName;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\View\DefaultNodeView.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal CodeFlowLibrary.Custom_Controls.DraggableLabel dlDescription;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CodeFlowLibrary;component/view/defaultnodeview.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\View\DefaultNodeView.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.usrControl = ((CodeFlowLibrary.View.DefaultNodeView)(target));
            return;
            case 2:
            this.cmNewChild = ((System.Windows.Controls.MenuItem)(target));
            
            #line 27 "..\..\..\View\DefaultNodeView.xaml"
            this.cmNewChild.Click += new System.Windows.RoutedEventHandler(this.cmNewChild_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.cmNewLink = ((System.Windows.Controls.MenuItem)(target));
            
            #line 31 "..\..\..\View\DefaultNodeView.xaml"
            this.cmNewLink.Click += new System.Windows.RoutedEventHandler(this.cmNewLink_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.outerBorder = ((System.Windows.Controls.Border)(target));
            return;
            case 5:
            this.cvsMain = ((System.Windows.Controls.Canvas)(target));
            
            #line 48 "..\..\..\View\DefaultNodeView.xaml"
            this.cvsMain.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.cvsMain_LButtonDown);
            
            #line default
            #line hidden
            
            #line 49 "..\..\..\View\DefaultNodeView.xaml"
            this.cvsMain.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.cvsMain_LButtonUp);
            
            #line default
            #line hidden
            
            #line 50 "..\..\..\View\DefaultNodeView.xaml"
            this.cvsMain.MouseMove += new System.Windows.Input.MouseEventHandler(this.cvsMain_MouseMove);
            
            #line default
            #line hidden
            return;
            case 6:
            this.dlName = ((CodeFlowLibrary.Custom_Controls.DraggableLabel)(target));
            return;
            case 7:
            this.dlDescription = ((CodeFlowLibrary.Custom_Controls.DraggableLabel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

