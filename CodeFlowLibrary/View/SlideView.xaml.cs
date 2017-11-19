using CodeFlowLibrary.Custom_Controls;
using CodeFlowLibrary.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using CodeFlowLibrary.Factory;

namespace CodeFlowLibrary.View {
    /// <summary>
    /// Interaction logic for SlideView.xaml
    /// </summary>
    public partial class SlideView : NodeView {
        public SlideView() {
            InitializeComponent();
        }


        #region Protected

        protected override Canvas GetViewCanvas() {
            return cvsMain;
        }

        #endregion


        #region Private

        #region Context Menu

        private void cmNewDefaultNode_Click(object sender, RoutedEventArgs e) {
            var location = Mouse.GetPosition(cvsMain);
            var newNode = new DefaultNode {
                Location = location,
                Size = new Size {
                    Width  = 200,
                    Height = 200
                }
            };
            Model.AddChild(newNode);
            AddChildView(ViewFactory.Instance.GetView<DefaultNodeView>(newNode), location);
        }

            #endregion

            #region Mouse
        

            #endregion

        #endregion

    }
}
