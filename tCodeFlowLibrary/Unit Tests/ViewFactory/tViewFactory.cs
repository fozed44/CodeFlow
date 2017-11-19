using CodeFlowLibrary.Custom_Controls;
using CodeFlowLibrary.Factory;
using CodeFlowLibrary.Model;
using CodeFlowLibrary.View;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace tCodeFlowBuilder.Unit_Tests {

    [TestClass]
    public class tViewFactory {

        [TestMethod]
        public void tDefaultNodeView() {
            var testNode = new DefaultNode();

            var testView = (new ViewFactory()).GetView(testNode);

            Assert.AreEqual(typeof(DefaultNodeView), testView.GetType());
        }

        [TestMethod]
        public void tHasCachedView_true() {
            var testNode = new DefaultNode();
            var testView = ViewFactory.Instance.GetView(testNode);
            Assert.IsTrue(ViewFactory.Instance.HasCachedView(testNode));
        }

        [TestMethod]
        public void tHasCachedView_false() {
            var testNode = new DefaultNode();
            Assert.IsFalse(ViewFactory.Instance.HasCachedView(testNode));
        }

        [TestMethod]
        public void tSlideView() {
            var testSlide = new Slide();

            var testView = (new ViewFactory()).GetView(testSlide);

            Assert.AreEqual(typeof(SlideView), testView.GetType());
        }

        [TestMethod]
        public void tGetHierarchy() {
            var testSlide = new Slide();
            var testNode = new DefaultNode();
            testSlide.AddChild(testNode);

            var testView = ViewFactory.Instance.GetHierarchy(testSlide);

            Assert.AreEqual(typeof(SlideView), testView.GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(0).GetType());
        }

        [TestMethod]
        public void tGetHeirarcyDeep() {
            var testSlide = new Slide();
            var testNodeA1 = new DefaultNode();
            var testNodeA2 = new DefaultNode();
            var testNodeA1A1 = new DefaultNode();
            var testNodeA1A2 = new DefaultNode();
            var testNodeA2A1 = new DefaultNode();
            var testNodeA2A2 = new DefaultNode();

            testSlide.AddChild(testNodeA1);
            testSlide.AddChild(testNodeA2);
            testNodeA1.AddChild(testNodeA1A1);
            testNodeA1.AddChild(testNodeA1A2);
            testNodeA2.AddChild(testNodeA2A1);
            testNodeA2.AddChild(testNodeA2A2);

            testNodeA1A2.Description = "test description";

            var testView = ViewFactory.Instance.GetHierarchy(testSlide);

            Assert.AreEqual(typeof(SlideView), testView.GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(0).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(1).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(0).GetChildViews().ElementAt(0).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(1).GetChildViews().ElementAt(1).GetType());

            var testNode = testView.GetChildViews().ElementAt(0).GetChildViews().ElementAt(1);

            var ptestNode = new PrivateObject(testNode);
            var descriptionControl = (DraggableLabel)ptestNode.GetField("dlDescription");

            // test that the description property of testNodeA1A2 was applied by GetHierarchy
            Assert.AreEqual("test description", descriptionControl.Text);

        }

        /// <summary>
        /// Test that the get view method pulls from the view cache if a view for a particular model
        /// has already been created.
        /// </summary>
        [TestMethod]
        public void tTestGetViewReferences() {
            var testSlide = new Slide();
            var testNodeA1 = new DefaultNode();
            var testNodeA2 = new DefaultNode();
            var testNodeA1A1 = new DefaultNode();
            var testNodeA1A2 = new DefaultNode();
            var testNodeA2A1 = new DefaultNode();
            var testNodeA2A2 = new DefaultNode();

            testSlide.AddChild(testNodeA1);
            testSlide.AddChild(testNodeA2);
            testNodeA1.AddChild(testNodeA1A1);
            testNodeA1.AddChild(testNodeA1A2);
            testNodeA2.AddChild(testNodeA2A1);
            testNodeA2.AddChild(testNodeA2A2);

            testNodeA1A2.Description = "test description";

            var testView = ViewFactory.Instance.GetHierarchy(testSlide);

            Assert.AreEqual(typeof(SlideView), testView.GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(0).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(1).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(0).GetChildViews().ElementAt(0).GetType());
            Assert.AreEqual(typeof(DefaultNodeView), testView.GetChildViews().ElementAt(1).GetChildViews().ElementAt(1).GetType());

            var fromHierarchy = testView.GetChildViews().ElementAt(0);
            var fromNode = testNodeA1.GetView();

            Assert.IsTrue(object.ReferenceEquals(fromHierarchy, fromNode));

            var testNode = testView.GetChildViews().ElementAt(0).GetChildViews().ElementAt(1);

            var ptestNode = new PrivateObject(testNode);
            var descriptionControl = (DraggableLabel)ptestNode.GetField("dlDescription");

            // test that the description property of testNodeA1A2 was applied by GetHierarchy
            Assert.AreEqual("test description", descriptionControl.Text);

        }
    }
}
