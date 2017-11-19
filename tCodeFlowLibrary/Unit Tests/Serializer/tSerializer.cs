using CodeFlowLibrary.Model;
using CodeFlowLibrary.Controllers;
using CodeFlowLibrary.History;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace tCodeFlowBuilder.Unit_Tests {

    [TestClass]
    public class tSerializer {

        [TestMethod]
        public void tSerializer_ModelBase() {
            var filename = @"C:\temp\UnitTest_tSerializer_ModelBase.xml";
            ModelBase o = new ModelBase{
                Name = "UnitTest"
            };

            Serializer.Serialize(o, filename);
            var result = Serializer.DeSerialize(filename);

            Assert.AreEqual(o.Name, result.Name);
            Assert.AreEqual(o.Guid, result.Guid);
        }

        [TestMethod]
        public void tSerializer_NodeBase() {
            var filename = @"C:\temp\UnitTest_tSerializer_NodeBase.xml";
            var vn = new VisualNode {
                Name = "UnitTest",
                Description = "Unit Test description.",
                Comment = "Unit Test comment.",
                Parent = null
            };

            Serializer.Serialize(vn, filename);
            var r = Serializer.DeSerialize(filename);

            Assert.AreEqual(r.GetType(), typeof(VisualNode));

            var result = (VisualNode)r;

            Assert.AreEqual(vn.Guid, result.Guid);
            Assert.AreEqual(vn.Name, result.Name);
            Assert.AreEqual(vn.Children.Count, result.Children.Count);
            Assert.AreEqual(vn.Description, result.Description);
            Assert.AreEqual(vn.Comment, result.Comment);
            Assert.AreEqual(vn.Parent, result.Parent);            
        }

        [TestMethod]
        public void tSerializer_Multiple_Nodes() {
            var root = new VisualNode {
                Name = "root",
                Description = "root_description",
                Comment = "root_comment",
                Parent = null
            };
            var childA = new VisualNode {
                Name = "root_childA",
                Description = "root_childA_description",
                Comment = "root_childA_comment",
                Parent = null
            };
            var childB = new VisualNode {
                Name = "root_childB",
                Description = "root_childB_description",
                Comment = "root_childB_comment",
                Parent = null
            };

            root.AddChild(childA);
            root.AddChild(childB);

            var filename = @"c:\temp\tSerializer_Multiple_Nodes.xml";
            Serializer.Serialize(root, filename);

            var r = Serializer.DeSerialize(filename);

            Assert.AreEqual(typeof(VisualNode), r.GetType());

            var result = (VisualNode)r;

            Assert.AreEqual(root.Name, result.Name);
            Assert.AreEqual(root.Children[0].Name, result.Children[0].Name);
            Assert.AreEqual(root.Children[1].Name, result.Children[1].Name);

            Assert.AreEqual(result.Children[0].Parent.Guid, result.Guid);
            Assert.AreEqual(result.Children[1].Parent.Guid, result.Guid);

        }

        [TestMethod]
        public void tSerializer_Slides() {
            var filename = @"c:\temp\tSerializer_Slides.xml";
            var slideCollection = new SlideCollection();
            var slide1 = new Slide { Name = "Slide1" };
            var slide2 = new Slide { Name = "Slide2" };
            var child1 = new VisualNode {
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

            Serializer.Serialize(slideCollection, filename);
            var result = Serializer.DeSerialize(filename);

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

            // The Parent properties of both children of the secode slide should reference the
            // second slide.
            firstChild = secondSlide.Children[0];
            secondChild = secondSlide.Children[1];
            Assert.AreEqual(secondSlide.Guid, firstChild.Parent.Guid);
            Assert.AreEqual(secondSlide.Guid, secondChild.Parent.Guid);
        }

        // Test the Serializer.DeSerialize(filename, HistoryController) overload of the DeSerialize
        // method. This overload of the DeSerialize method should hookup the HistoryController to
        // each object as it is deserialized. 
        [TestMethod]
        public void tSerializer_historyController() {
            var hc = new HistoryController();
            var filename = @"c:\temp\tSerializer_HistoryController.xml";
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

            Serializer.Serialize(slideCollection, filename);
            var result = Serializer.DeSerialize(filename,hc);

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

            // The Parent properties of both children of the secode slide should reference the
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

        // Test the Serializer.DeSerialize(filename, HistoryController) overload of the DeSerialize
        // method. This overload of the DeSerialize method should hookup the HistoryController to
        // each object as it is deserialized. 
        [TestMethod]
        public void tSerializer_DefaultNode() {
            var hc = new HistoryController();
            var filename = @"c:\temp\tSerializer_DefaultNode.xml";
            var slideCollection = new SlideCollection();
            var slide1 = new Slide { Name = "Slide1" };
            var child1 = new DefaultNode {
                Name = "child1",
                Description = "child1_description",
                Comment = "child1_comment",
                Background = new System.Windows.Media.Color { A = 1, B = 150, G = 160, R = 170 },
                BorderType = BorderType.normal,
                Foreground = new System.Windows.Media.Color { A = 1, R = 2, G = 3, B = 4},
                Location = new System.Windows.Point { X = 1, Y = 2},
                ShapeType = ShapeType.rectangle,
                Size = new System.Windows.Size { Height = 1, Width = 2},
                Parent = null
            };

            slideCollection.AddChild(slide1);

            slide1.AddChild(child1);

            Serializer.Serialize(slideCollection, filename);
            var result = Serializer.DeSerialize(filename,hc);

            Assert.AreEqual(typeof(SlideCollection), result.GetType());
            var r = (SlideCollection)result;
            
            var Slide = r.Children[0];
            Assert.AreEqual(r.Guid, Slide.Parent.Guid);
            
            var tNode = (DefaultNode)Slide.Children[0];
            Assert.AreEqual(Slide.Guid, tNode.Parent.Guid);            
            
            Assert.AreEqual("child1", tNode.Name);
            Assert.AreEqual("child1_description", tNode.Description);
            Assert.AreEqual("child1_comment", tNode.Comment);
            Assert.AreEqual(new System.Windows.Media.Color { A = 1, B = 150, G = 160, R = 170 }, tNode.Background);
            Assert.AreEqual(BorderType.normal, tNode.BorderType);
            Assert.AreEqual(new System.Windows.Media.Color { A = 1, R = 2, G = 3, B = 4 }, tNode.Foreground);
            Assert.AreEqual(new System.Windows.Point { X = 1, Y = 2 }, tNode.Location);
            Assert.AreEqual(ShapeType.rectangle, tNode.ShapeType);
            Assert.AreEqual(new System.Windows.Size { Height = 1, Width = 2 }, tNode.Size);
            
        }
    }
}
