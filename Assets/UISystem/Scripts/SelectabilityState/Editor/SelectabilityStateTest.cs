using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class SelectabilityStateTest {
    [Test]
    public void Constructor_WhenCalled_ThisStatesAreAllSet(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);

        Assert.That(engine.StatesAreAllSet(), Is.True);
    }
    [Test]
    public void Constructor_WhenCreated_ThisIsSelectable(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);

        Assert.That(engine.IsSelected(), Is.False);
        Assert.That(engine.IsSelectable(), Is.True);
    }
    [Test]
    public void BecomeSelectable_WhenCalledWhileIsSelectable_ThisStaysSelectable(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);

        engine.BecomeSelectable();

        Assert.That(engine.IsSelectable(), Is.True);
        Assert.That(engine.IsSelected(), Is.False);
    }
    [Test]
    public void BecomeUnselectable_WhenCalledWhileIsSelectable_ThisBecomesNotSelectable(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        Assert.That(engine.IsSelectable(), Is.True);
        
        engine.BecomeUnselectable();

        Assert.That(engine.IsSelectable(), Is.False);
    }
    [Test]
    public void BecomeUnselectable_WhenCalledWhileNotSelectable_ThisStaysNotSelectable(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        Assert.That(engine.IsSelectable(), Is.True);
        engine.BecomeUnselectable();
        Assert.That(engine.IsSelectable(), Is.False);

        engine.BecomeUnselectable();

        Assert.That(engine.IsSelectable(), Is.False);
    }
    [Test]
    public void BecomeSelected_WhenCalledWhileThisIsSelectable_ThisBecomesSelected(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        Assert.That(engine.IsSelectable(), Is.True);

        engine.BecomeSelected();

        Assert.That(engine.IsSelected(), Is.True);
        Assert.That(engine.IsSelectable(), Is.False);
    }
    [Test]
    [ExpectedException(typeof(System.InvalidOperationException))]
    public void BecomeSelected_WhenCalledWhileThisIsNotSelectable_ThrowsException(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        engine.BecomeUnselectable();
        Assert.That(engine.IsSelectable(), Is.False);

        engine.BecomeSelected();
    }
    [Test]
    public void BecomeSelectable_WhenCalledWhileThisIsSelected_ThisBecomesSelectableAndNotSelected(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        engine.BecomeSelected();
        Assert.That(engine.IsSelected(), Is.True);

        engine.BecomeSelectable();

        Assert.That(engine.IsSelectable(), Is.True);
        Assert.That(engine.IsSelected(), Is.False);
    }
    [Test]
    public void BecomeUnselectable_WhenCalledWhileThisIsSelected_ThisBecomesNotSelectableAndNotSelected(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        engine.BecomeSelected();
        Assert.That(engine.IsSelected(), Is.True);

        engine.BecomeUnselectable();

        Assert.That(engine.IsSelectable(), Is.False);
        Assert.That(engine.IsSelected(), Is.False);
    }
    [Test]
    public void BecomeSelected_WhenCalledWhileThisIsSelected_ThisStaysSelected(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        engine.BecomeSelected();
        Assert.That(engine.IsSelected(), Is.True);
        Assert.That(engine.IsSelectable(), Is.False);

        engine.BecomeSelected();

        Assert.That(engine.IsSelected(), Is.True);
        Assert.That(engine.IsSelectable(), Is.False);
    }
    [Test]
    public void BecomeSelectable_WhenCalledWhileThisIsNotSelectable_ThisBecomesSelectable(){
        TestSelStateEngineConstArg arg;
        TestSelectabilityStateEngine engine = CreateTestSelectabilityStateEngine(out arg);
        engine.BecomeUnselectable();
        Assert.That(engine.IsSelectable(), Is.False);

        engine.BecomeSelectable();

        Assert.That(engine.IsSelectable(), Is.True);
        Assert.That(engine.IsSelected(), Is.False);
    }
    /* Test Support Classes */
        class TestSelectabilityStateEngine: SelectabilityStateEngine{
            public TestSelectabilityStateEngine(IUIImage uiImage, IUIManager uim): base(uiImage, uim){}
            public bool StatesAreAllSet(){
                return this.selectableState != null && this.unselectableState != null && this.selectedState != null;
            }
        }
        TestSelectabilityStateEngine CreateTestSelectabilityStateEngine(out TestSelStateEngineConstArg arg){
            IUIImage image = Substitute.For<IUIImage>();
            IUIManager uim = Substitute.For<IUIManager>();
            TestSelectabilityStateEngine engine = new TestSelectabilityStateEngine(image, uim);
            TestSelStateEngineConstArg thisArg = new TestSelStateEngineConstArg();
            arg = thisArg;
            return engine;
        }
        class TestSelStateEngineConstArg{
            public TestSelStateEngineConstArg(){}
        }
}
