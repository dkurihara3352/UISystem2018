using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UISystem;
using NUnit.Framework;
using NSubstitute;
using DKUtility;


[TestFixture, Category("UISystem")]
public class UIAdaptorInputStateEngineTest {
    [Test]
    public void Construction_SetsAllStates(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);

        Assert.That(engine.StatesAreAllSet(), Is.True);
    }
	[Test]
    public void ThisCurState_Initially_IsWaitingForFirstTouch(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);

        Assert.That(engine.isWaitingForFirstTouch, Is.True);
    }
    [Test]
    public void TouchCount_Initially_IsZero(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);

        Assert.That(engine.touchCount, Is.EqualTo(0));
    }

    /* isWaitingForFirstTouch */
    [Test]
    public void WaitingForFirstTouch_OnPointerEnter_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        Assert.That(engine.isWaitingForFirstTouch, Is.True);

        engine.OnPointerEnter(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForFirstTouch, 0);
    }
    [Test]
    public void WaitingForFirstTouch_OnPointerExit_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        Assert.That(engine.isWaitingForFirstTouch, Is.True);

        engine.OnPointerEnter(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForFirstTouch, 0);
    }
    void AssertNoChange(TestUIAdaptorInputStateEngine engine, bool expectedStateBool, int expectedTouchCount){
        Assert.That(expectedStateBool, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(expectedTouchCount));
    }
    [Test]
    public void WaitingForFirstTouch_OnPointerDown_IncrementTouchCounter(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        Assert.That(engine.isWaitingForFirstTouch, Is.True);

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        Assert.That(engine.touchCount, Is.EqualTo(1));
    }
    [Test]
    public void WaitingForFirstTouch_OnPointerDown_CallsUIEOnTouchOne(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        Assert.That(engine.isWaitingForFirstTouch, Is.True);

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        uie.Received(1).OnTouch(1);
    }
    [Test]
    public void WaitingForFirstTouch_OnPointerDown_SwitchesToWaitingForTapState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        Assert.That(engine.isWaitingForFirstTouch, Is.True);

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        Assert.That(engine.isWaitingForTap, Is.True);
    }
    /* isWaitingForTap */
    [Test]
    public void WaitingForTap_OnEnter_RunsProcess(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IWaitAndExpireProcess process = arg.process;
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        process.DidNotReceive().Run();
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);

        process.Received(1).Run();
    }
    [Test]
    public void WaitingForTap_OnBeginDrag_CallsUIEOnBeginDrag(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        ICustomEventData data = Substitute.For<ICustomEventData>();
        engine.OnBeginDrag(data);

        uie.Received(1).OnBeginDrag(data);
    }
    [Test]
    public void WaitingForTap_OnDrag_CallsUIEOnDrag(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        ICustomEventData data = Substitute.For<ICustomEventData>();
        engine.OnDrag(data);

        uie.Received(1).OnDrag(data);
    }
    [Test]
    public void WaitingForTap_OnPointerUp_SwitchesToWaitingForNextTouchState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);

        engine.OnPointerUp(Substitute.For<ICustomEventData>());

        Assert.That(engine.isWaitingForNextTouch, Is.True);
    }
    [Test, TestCaseSource(typeof(WaitingForTap_OnPointerUp_Velocity_TestCase), "overCases")]
    public void WaitingForTap_OnPointerUp_VelocityOverThreshold_CallsUIEOnSwipe(Vector2 velocity){
        /* threshold = 5f */
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        ICustomEventData data = Substitute.For<ICustomEventData>();
        data.velocity.Returns(velocity);
        engine.OnPointerUp(data);

        uie.Received(1).OnSwipe(data);
        uie.DidNotReceive().OnTap(1);
    }
    [Test, TestCaseSource(typeof(WaitingForTap_OnPointerUp_Velocity_TestCase), "underCases")]
    public void WaitingForTap_OnPointerUp_VelocityUnderThreshold_CAllsUIEOnTapOne(Vector2 velocity){
        /* threshold = 5f */
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        ICustomEventData data = Substitute.For<ICustomEventData>();
        data.velocity.Returns(velocity);
        engine.OnPointerUp(data);

        uie.DidNotReceive().OnSwipe(data);
        uie.Received(1).OnTap(1);
    }
    public class WaitingForTap_OnPointerUp_Velocity_TestCase{
        public static object[] overCases = {
            new object[]{new Vector2(velocityThreshold, 0f)},
            new object[]{new Vector2(0f, velocityThreshold)},
            new object[]{new Vector2(-velocityThreshold, 0f)},
            new object[]{new Vector2(0f, -velocityThreshold)},
        };
        public static object[] underCases = {
            new object[]{new Vector2(velocityThreshold - 0.01f, 0f)},
            new object[]{new Vector2(0f, velocityThreshold - 0.01f)},
            new object[]{new Vector2(-(velocityThreshold - 0.01f), 0f)},
            new object[]{new Vector2(0f, -(velocityThreshold - 0.01f))},
        };
    }
    [Test]
    public void WaitingForTap_OnPointerEnter_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        
        engine.OnPointerEnter(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForTap, 1);
    }
    [Test]
    public void WaitingForTap_OnPointerExit_SwitchesToWaitingForReleaseState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        
        engine.OnPointerExit(Substitute.For<ICustomEventData>());

        Assert.That(engine.isWaitingForRelease, Is.True);
    }
    [Test]
    public void WaitingForTap_OnProcessExpire_SwitchesToWaitingForReleaseState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IWaitAndExpireProcess process = arg.process;
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        Assert.That(process.IsRunning(), Is.True);
        process.Received(1).Run();
        
        engine.ExpireProcess_Test();

        process.DidNotReceive().Expire();//Cleared when entered into WFReleaseState!
        Assert.That(process.IsRunning(), Is.True);//re-run
        Assert.That(engine.isWaitingForRelease, Is.True);
    }
    [Test]
    public void WaitingForTap_OnProcessExpire_CallsUIEOnDelayedTouch(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        
        engine.ExpireProcess_Test();

        uie.Received(1).OnDelayedTouch();
    }
    [Test]
    public void WaitingForTap_OnProcessUpdate_CallsUIEOnHold(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForTap, Is.True);
        
        float deltaT = .1f;
        for(int i = 0; i < 10; i ++){
            engine.UpdateProcess_Test(deltaT);
            uie.Received(i + 1).OnHold(deltaT);
        }
    }
    [Test]
    public void WaitingForRelease_OnEnter_RunsProcess(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IWaitAndExpireProcess process = arg.process;
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        process.Received(1).Run();
        Assert.That(process.IsRunning(), Is.True);
    }
    [Test]
    public void WaitingForRelease_OnEnter_SetTouchCountZero(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        Assert.That(engine.touchCount, Is.EqualTo(0));
    }
    [Test]
    public void WaitingForRelease_OnPointerUp_SwitchesToWaitingForNextTouchState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        engine.OnPointerUp(Substitute.For<ICustomEventData>());

        Assert.That(engine.isWaitingForNextTouch, Is.True);
    }
    [Test, TestCaseSource(typeof(WaitingForRelease_OnPointerUp_Velocity_TestCase), "overCases")]
    public void WaitingForRelease_OnPointerUp_VelocityOverThreshold_CallsUIEOnSwipe(Vector2 velocity){
        // threhold = 5f
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        ICustomEventData data = Substitute.For<ICustomEventData>();
        data.velocity.Returns(velocity);
        engine.OnPointerUp(data);

        uie.Received(1).OnSwipe(data);
        uie.DidNotReceive().OnRelease();
    }
    [Test, TestCaseSource(typeof(WaitingForRelease_OnPointerUp_Velocity_TestCase), "underCases")]
    public void WaitingForRelease_OnPointerUp_VelocityUnderThreshold_CallsUIEOnRelease(Vector2 velocity){
        // threhold = 5f
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        ICustomEventData data = Substitute.For<ICustomEventData>();
        data.velocity.Returns(velocity);
        engine.OnPointerUp(data);

        uie.DidNotReceive().OnSwipe(data);
        uie.Received(1).OnRelease();
    }
    const float velocityThreshold = 200f;
    public class WaitingForRelease_OnPointerUp_Velocity_TestCase{
        public static object[] overCases = {
            new object[]{new Vector2(velocityThreshold, 0f)},
            new object[]{new Vector2(0f, velocityThreshold)},
            new object[]{new Vector2(-velocityThreshold, 0f)},
            new object[]{new Vector2(0f, -velocityThreshold)},
        };
        public static object[] underCases = {
            new object[]{new Vector2(velocityThreshold - .01f, 0f)},
            new object[]{new Vector2(0f, velocityThreshold - .01f)},
            new object[]{new Vector2(-(velocityThreshold - .01f), 0f)},
            new object[]{new Vector2(0f, -(velocityThreshold - .01f))},
        };
    }
    [Test]
    public void WaitingForRelease_OnProcessUpdate_CallsUIEOnHold(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        float deltaT = .1f;
        for(int i = 0; i < 10; i ++){
            engine.UpdateProcess_Test(deltaT);
            uie.Received(i + 1).OnHold(deltaT);
        }   
    }
    [Test]
    public void WaitingForRelsease_OnPointerEnter_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        engine.OnPointerEnter(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForRelease, 0);
    }
    [Test]
    public void WaitingForRelsease_OnPointerExit_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);

        engine.OnPointerExit(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForRelease, 0);
    }
    [Test]
    public void WaitingForRelsease_OnProcessExpire_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerExit(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForRelease, Is.True);
        
        engine.ExpireProcess_Test();

        AssertNoChange(engine, engine.isWaitingForRelease, 0);
    }
    [Test]
    public void WaitingForNextTouch_OnPointerEnter_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(1));

        engine.OnPointerEnter(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForNextTouch, 1);
    }
    [Test]
    public void WaitingForNextTouch_OnPointerExit_DoesNothing(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(1));

        engine.OnPointerExit(Substitute.For<ICustomEventData>());

        AssertNoChange(engine, engine.isWaitingForNextTouch, 1);
    }
    [Test]
    public void WaitingForNextTouch_OnEnter_RunsProcess(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IWaitAndExpireProcess process = arg.process;
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);

        process.Received(1).Run();
        Assert.That(process.IsRunning(), Is.True);
    }
    [Test]
    public void WaitingForNextTouch_OnPointerDown_IncrementTouchCount(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(1));

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        Assert.That(engine.touchCount, Is.EqualTo(2));
    }
    [Test]
    public void WaitingForNextTouch_OnPointerDown_CallsUIEOnTouchTwo(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        uie.Received(1).OnTouch(1);
        Assert.That(engine.touchCount, Is.EqualTo(1));
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        uie.Received(1).OnTouch(2);
    }
    [Test]
    public void WaitingForNextTouch_OnPointerDown_SwitchesToWaitingForTapState(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);

        engine.OnPointerDown(Substitute.For<ICustomEventData>());

        Assert.That(engine.isWaitingForTap, Is.True);
    }
    [Test]
    public void WaitingForNextTouch_OnProcessExpire_SwitchesToWaitingForFirstTouch(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);

        engine.ExpireProcess_Test();

        Assert.That(engine.isWaitingForFirstTouch, Is.True);
    }
    [Test]
    public void WaitingForNextTouch_OnProcessExpire_CallsUIEOnDelayedRelease(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        engine.OnPointerDown(Substitute.For<ICustomEventData>());
        engine.OnPointerUp(Substitute.For<ICustomEventData>());
        Assert.That(engine.isWaitingForNextTouch, Is.True);

        engine.ExpireProcess_Test();

        uie.Received(1).OnDelayedRelease();
    }
    const float veloctiyThreshold = 200f;
    [Test]
    public void SequenceTest(){
        ITestUIAdaptorInputStateEngineConstArg arg = CreateMockConstArg();
        IWaitAndExpireProcess process = arg.process;
        IUIElement uie = arg.uia.GetUIElement();
        TestUIAdaptorInputStateEngine engine = new TestUIAdaptorInputStateEngine(arg);
        ICustomEventData someData = Substitute.For<ICustomEventData>();
        ICustomEventData overData = Substitute.For<ICustomEventData>();
            overData.velocity.Returns(new Vector2(velocityThreshold, 0f));
        ICustomEventData underData = Substitute.For<ICustomEventData>();
            underData.velocity.Returns(new Vector2(velocityThreshold - .01f, 0f));
        Assert.That(engine.isWaitingForFirstTouch, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(0));

        engine.OnPointerDown(someData);
        uie.Received(1).OnTouch(1);
        Assert.That(engine.isWaitingForTap, Is.True);
        Assert.That(engine.touchCount, Is.EqualTo(1));

        engine.OnPointerUp(underData);
        uie.Received(1).OnTap(1);
        AssertTrue(engine.isWaitingForNextTouch);
        AssertTouchCount(engine, 1);
        
        engine.ExpireProcess_Test();
        uie.Received(1).OnDelayedRelease();
        AssertTrue(engine.isWaitingForFirstTouch);
        AssertTouchCount(engine, 0);



        engine.OnPointerDown(someData);
        uie.Received(2).OnTouch(1);
        AssertTrue(engine.isWaitingForTap);
        AssertTouchCount(engine, 1);

        engine.OnPointerUp(overData);
        uie.Received(1).OnSwipe(overData);
        AssertTrue(engine.isWaitingForNextTouch);
        AssertTouchCount(engine, 1);

        engine.OnPointerDown(someData);
        uie.Received(1).OnTouch(2);
        AssertTrue(engine.isWaitingForTap);
        AssertTouchCount(engine, 2);

        engine.OnPointerUp(underData);
        uie.Received(1).OnTap(2);
        AssertTrue(engine.isWaitingForNextTouch);
        AssertTouchCount(engine, 2);

        engine.ExpireProcess_Test();
        uie.Received(2).OnDelayedRelease();
        AssertTrue(engine.isWaitingForFirstTouch);
        AssertTouchCount(engine, 0);

        
        
        engine.OnPointerDown(someData);
        uie.Received(3).OnTouch(1);
        AssertTrue(engine.isWaitingForTap);
        AssertTouchCount(engine, 1);

        engine.ExpireProcess_Test();
        uie.Received(1).OnDelayedTouch();
        AssertTrue(engine.isWaitingForRelease);
        AssertTouchCount(engine, 0);

        engine.OnPointerUp(overData);
        uie.Received(2).OnSwipe(overData);
        AssertTrue(engine.isWaitingForNextTouch);
        AssertTouchCount(engine, 0);

        engine.OnPointerDown(someData);
        uie.Received(4).OnTouch(1);
        AssertTrue(engine.isWaitingForTap);
        AssertTouchCount(engine, 1);

        engine.ExpireProcess_Test();
        uie.Received(2).OnDelayedTouch();
        AssertTrue(engine.isWaitingForRelease);
        AssertTouchCount(engine, 0);

        engine.OnPointerUp(underData);
        uie.Received(1).OnRelease();
        AssertTrue(engine.isWaitingForNextTouch);
        AssertTouchCount(engine, 0);

        engine.ExpireProcess_Test();
        uie.Received(3).OnDelayedRelease();
        AssertTrue(engine.isWaitingForFirstTouch);
        AssertTouchCount(engine, 0);

    }
    void AssertTrue(bool expected){
        Assert.That(expected, Is.True);
    }
    void AssertTouchCount(TestUIAdaptorInputStateEngine engine, int touchCount){
        Assert.That(engine.touchCount, Is.EqualTo(touchCount));
    }










    public interface ITestUIAdaptorInputStateEngineConstArg{
        IUIManager uim{get;}
        IUIAdaptor uia{get;}
        IUISystemProcessFactory processFactory{get;}
        IWaitAndExpireProcess process{get;}
    }
    public ITestUIAdaptorInputStateEngineConstArg CreateMockConstArg(){
        ITestUIAdaptorInputStateEngineConstArg arg = Substitute.For<ITestUIAdaptorInputStateEngineConstArg>();
        IUIManager uim = Substitute.For<IUIManager>();
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
            IUIElement uie = Substitute.For<IUIElement>();
            uia.GetUIElement().Returns(uie);
        IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
        IWaitAndExpireProcess process = Substitute.For<IWaitAndExpireProcess>();
        process.When(
            x => {
                x.Run();
            }
        ).Do(
            x =>{
                process.IsRunning().Returns(true);
            }
        );
        process.When(
            x => {
                x.Expire();
            }
        ).Do(
            x =>{
                process.IsRunning().Returns(false);
            }
        );
        processFactory.CreateWaitAndExpireProcess(Arg.Any<IWaitAndExpireProcessState>(), Arg.Any<float>()).Returns(process);
        processFactory.When(
            x => {
                x.CreateWaitAndExpireProcess(Arg.Any<IWaitAndExpireProcessState>(), Arg.Any<float>());
            }
        ).Do(
            x => {
                process.ClearReceivedCalls();
            }
        );


        arg.uim.Returns(uim);
        arg.uia.Returns(uia);
        arg.processFactory.Returns(processFactory);
        arg.process.Returns(process);

        return arg; 
    }
    public class TestUIAdaptorInputStateEngine: UIAdaptorInputStateEngine{
        public TestUIAdaptorInputStateEngine(ITestUIAdaptorInputStateEngineConstArg arg): base(arg.uim, arg.uia, arg.processFactory){}
        public bool StatesAreAllSet(){
            bool result = true;
            result = thisWaitingForFirstTouchState != null;
            result = thisWaitingForTapState != null;
            result = thisWaitingForReleaseState != null;
            result = thisWaitingForNextTouchState != null;
            return result;
        }
        public bool isWaitingForFirstTouch{get{return thisCurState is WaitingForFirstTouchState;}}
        public bool isWaitingForTap{get{return thisCurState is WaitingForTapState;}}
        public bool isWaitingForRelease{get{return thisCurState is WaitingForReleaseState;}}
        public bool isWaitingForNextTouch{get{return thisCurState is WaitingForNextTouchState;}}
        public int touchCount{get{return thisTouchCount;}}
        public void ExpireProcess_Test(){
            if(thisCurState is IWaitAndExpireProcessState){
                ((IWaitAndExpireProcessState)thisCurState).ExpireProcess();
                ((IWaitAndExpireProcessState)thisCurState).OnProcessExpire();
            }
        }
        public void UpdateProcess_Test(float deltaT){
            if(thisCurState is IWaitAndExpireProcessState){
                ((IWaitAndExpireProcessState)thisCurState).OnProcessUpdate(deltaT);
            }
        }
    }
}
