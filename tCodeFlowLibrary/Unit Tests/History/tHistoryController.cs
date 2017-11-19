using CodeFlowLibrary.History;
using CodeFlowLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tCodeFlowBuilder.Unit_Tests {

    [TestClass]
    public class tHistoryController {

        [TestMethod]
        public void tSingleDo() {
            var hc = new HistoryController();

            var o = new DefaultNode(hc);

            o.Name = "testA";
            Assert.AreEqual("testA", o.Name);

            o.Name = "testB";
            Assert.AreEqual("testB", o.Name);

            hc.Undo();
            Assert.AreEqual("testA", o.Name);
        }

        [TestMethod]
        public void tDoubleDo() {
            var hc = new HistoryController();

            var o = new DefaultNode(hc);

            o.Name = "testA";
            Assert.AreEqual("testA", o.Name);

            o.Name = "testB";
            Assert.AreEqual("testB", o.Name);

            o.Name = "testC";
            Assert.AreEqual("testC", o.Name);

            hc.Undo();
            Assert.AreEqual("testB", o.Name);

            hc.Undo();
            Assert.AreEqual("testA", o.Name);
        }

        [TestMethod]
        public void tSingleDoRedo() {
            var hc = new HistoryController();

            var o = new DefaultNode(hc);

            o.Name = "testA";
            Assert.AreEqual("testA", o.Name);

            o.Name = "testB";
            Assert.AreEqual("testB", o.Name);

            hc.Undo();
            Assert.AreEqual("testA", o.Name);

            hc.Redo();
            Assert.AreEqual("testB", o.Name);
        }

        [TestMethod]
        public void tDoubleDoRedo() {
            var hc = new HistoryController();

            var o = new DefaultNode(hc);

            o.Name = "testA";
            Assert.AreEqual("testA", o.Name);

            o.Name = "testB";
            Assert.AreEqual("testB", o.Name);

            o.Name = "testC";
            Assert.AreEqual("testC", o.Name);

            hc.Undo();
            Assert.AreEqual("testB", o.Name);

            hc.Undo();
            Assert.AreEqual("testA", o.Name);

            hc.Redo();
            Assert.AreEqual("testB", o.Name);

            hc.Redo();
            Assert.AreEqual("testC", o.Name);
        }
    }
}
