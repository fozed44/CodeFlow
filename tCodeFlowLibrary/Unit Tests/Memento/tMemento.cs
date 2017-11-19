using CodeFlowLibrary.Exceptions;
using CodeFlowLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace tCodeFlowBuilder.Unit_Tests {

    [TestClass]
    public class tMemento {
        
        [TestMethod]
        public void tRestore() {
            var testNode = new DefaultNode {
                Name = "testA"
            };
            var memento = testNode.GetMemento("Name");

            testNode.Name = "testB";
            Assert.AreEqual("testB", testNode.Name);

            memento.Restore();
            Assert.AreEqual("testA", testNode.Name);
        }
    }
}
