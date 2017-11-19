using CodeFlowLibrary.Exceptions;
using CodeFlowLibrary.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace tCodeFlowBuilder.Unit_Tests {


    [TestClass]
    public class tModel {

        private bool _notifyDirtyHandlerCalled = false;

        [TestMethod]
        public void tNotifyDirty() {
            ModelBase.NotifyDirty += NotifyDirtyHandler;
            _notifyDirtyHandlerCalled = false;

            var test = new ModelBase();
            test.Name = "Test";

            Assert.IsTrue(_notifyDirtyHandlerCalled);

            _notifyDirtyHandlerCalled = false;
            test.Guid = Guid.NewGuid();

            Assert.IsTrue(_notifyDirtyHandlerCalled);
        }

        private void NotifyDirtyHandler(object sender, EventArgs e) {
            _notifyDirtyHandlerCalled = true;
        }
    }

}
