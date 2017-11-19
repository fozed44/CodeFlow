using CodeFlowLibrary.Model;
using CodeFlowLibrary.Controllers;
using CodeFlowLibrary.History;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CodeFlowLibrary.Implementations;

namespace tCodeFlowBuilder.Unit_Tests {

    [TestClass]
    public class tFileStoreController {
                
        [TestMethod]
        public void tFileStoreContoller() {
            var hc = new HistoryController();
            var filename = @"c:\temp\tFileStoreContoller_HistoryController.xml";
            var slideCollection = new SlideCollection();
            var slide1 = new Slide { Name = "Slide1" };
            var slide2 = new Slide { Name = "Slide2" };
            var child1 = new DefaultNode {
                Name = "child1",
                Description = "child1_description",
                Comment = "child1_comment",
                Parent = null
            };
            var child2 = new VisualNode {
                Name = "child2",
                Description = "child2_description",
                Comment = "child2_comment",
                Parent = null
            };
            var child3 = new VisualNode {
                Name = "child3",
                Description = "child3_description",
                Comment = "child3_comment",
                Parent = null
            };
            var child4 = new VisualNode {
                Name = "child4",
                Description = "child4_description",
                Comment = "child4_comment",
                Parent = null
            };

            slideCollection.AddChild(slide1);
            slideCollection.AddChild(slide2);

            slide1.AddChild(child1);
            slide1.AddChild(child2);

            slide2.AddChild(child3);
            slide2.AddChild(child4);

            var fileStoreController = new FileStoreController();
            fileStoreController.PersistSlideCollection(filename, slideCollection);
            var result = fileStoreController.RestoreSlideCollection(filename, hc);

            Assert.AreEqual(typeof(SlideCollection), result.GetType());
            var r = (SlideCollection)result;

            // The Parent properties of both slides in the slide collection should reference the
            // slide collection.
            var firstSlide = r.Children[0];
            var secondSlide = r.Children[1];
            Assert.AreEqual(r.Guid, firstSlide.Parent.Guid);
            Assert.AreEqual(r.Guid, secondSlide.Parent.Guid);

            // The Parent properties of both children of the first slide should reference the
            // first slide.
            var firstChild = firstSlide.Children[0];
            var secondChild = firstSlide.Children[1];
            Assert.AreEqual(firstSlide.Guid, firstChild.Parent.Guid);
            Assert.AreEqual(firstSlide.Guid, secondChild.Parent.Guid);

            // The Parent properties of both children of the second slide should reference the
            // second slide.
            firstChild = secondSlide.Children[0];
            secondChild = secondSlide.Children[1];
            Assert.AreEqual(secondSlide.Guid, firstChild.Parent.Guid);
            Assert.AreEqual(secondSlide.Guid, secondChild.Parent.Guid);

            firstChild.Name = "testA";
            Assert.AreEqual("testA", firstChild.Name);

            firstChild.Name = "testB";
            Assert.AreEqual("testB", firstChild.Name);

            hc.Undo();
            Assert.AreEqual("testA", firstChild.Name);

            hc.Redo();
            Assert.AreEqual("testB", firstChild.Name);      

        }
    }
}
