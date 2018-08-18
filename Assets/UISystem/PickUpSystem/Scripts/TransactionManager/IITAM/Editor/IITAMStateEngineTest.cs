using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using UISystem.PickUpUISystem;

[TestFixture, Category("PickUpSystem"), Ignore]
public class IITAMStateEngineTest{
    [Test]
    public void SetIITAM_CallsBothStatesSetIITAM(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        IItemIconTransactionManager iiTAM = Substitute.For<IItemIconTransactionManager>();
        
        engine.SetIITAM(iiTAM);

        engine.GetPickedState().Received(1).SetIITAM(iiTAM);
        engine.GetDefaultState().Received(1).SetIITAM(iiTAM);
    }
    [Test]
    public void SetToPickedState_ThisBecomesPickedState(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        
        engine.SetToPickedState(Substitute.For<IItemIcon>());

        Assert.That(engine.IsInPickedUpState(), Is.True);
    }
    [Test]
    public void SetToPickedState_CallsPickedStateSetPickedII(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        IItemIcon pickedII = Substitute.For<IItemIcon>();
        
        engine.SetToPickedState(pickedII);

        engine.GetPickedState().Received(1).SetPickedII(pickedII);
    }
    [Test]
    public void SetToDefaultState_ThisBecomesDefaultState(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        
        engine.SetToDefaultState();

        Assert.That(engine.IsInDefaultState(), Is.True);
    }
    [Test]
    public void SetToPickedState_WhileInPickedState_DoesNotCallPickedStateOnEnterTwice(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        Assert.That(engine.IsInPickedUpState(), Is.False);
        engine.SetToPickedState(Substitute.For<IItemIcon>());
        Assert.That(engine.IsInPickedUpState(), Is.True);
        
        engine.SetToPickedState(Substitute.For<IItemIcon>());

        engine.GetPickedState().Received(1).OnEnter();
    }
    [Test]
    public void SetToDefaultState_WhileInDefaultState_DoesNotCallDefaultStateOnEnterTwice(){
        TestIITAMStateEngine engine = new TestIITAMStateEngine();
        Assert.That(engine.IsInDefaultState(), Is.False);
        engine.SetToDefaultState();
        Assert.That(engine.IsInDefaultState(), Is.True);
        
        engine.SetToDefaultState();

        engine.GetDefaultState().Received(1).OnEnter();
    }
    public class TestIITAMStateEngine: AbsItemIconTAManagerStateEngine{
        public TestIITAMStateEngine(){
            thisPickedState = Substitute.For<IIITAMPickedState>();
            thisDefaultState = Substitute.For<IIITAMDefaultState>();
        }
        public IIITAMPickedState GetPickedState(){return thisPickedState;}
        public IIITAMDefaultState GetDefaultState(){return thisDefaultState;}
    }
}

