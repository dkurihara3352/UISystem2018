using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class AbsPointerDownInputProcessStateTest{
    [Test]
    public void Construction_CreateAndSetEmptyVelocityStack(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);

        Assert.That(state.GetVelocityStack_Test(), Is.EqualTo(new Vector2[arg.velocityStackSize]));
    }
    [Test]
    public void OnEnter_CreateAndSetNewVelocityStack(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);

        state.SetVelocityStack_Test(
            new Vector2[]{
                new Vector2(1f, 1f), 
                new Vector2(2f, 2f),
                new Vector2(3f, 3f)
            }
        );
        state.OnEnter();

        Assert.That(state.GetVelocityStack_Test(), Is.EqualTo(new Vector2[arg.velocityStackSize]));
    }
    [Test]
    public void OnExit_SetsProcessNull(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);

        state.SetProcess(Substitute.For<IUIAdaptorInputProcess>());
        Assert.That(state.GetProcess_Test(), Is.Not.Null);

        state.OnExit();

        Assert.That(state.GetProcess_Test(), Is.Null);
    }
    [Test]
    public void OnExit_ProcessIsRunning_StopsIt(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);
        IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
        process.IsRunning().Returns(true);
        state.SetProcess(process);

        state.OnExit();

        process.Received(1).Stop();
    }
    [Test]
    public void OnExit_ProcessIsNotRunning_DoesNotStopIt(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);
        IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
        process.IsRunning().Returns(false);
        state.SetProcess(process);

        state.OnExit();

        process.DidNotReceive().Stop();
    }
    [Test]
    public void ExpireProcess_ProcessIsRunning_ExpiresIt(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);
        IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
        process.IsRunning().Returns(true);
        state.SetProcess(process);

        state.ExpireProcess();

        process.Received(1).Expire();
    }
    [Test]
    public void ExpireProcess_ProcessIsNotRunning_DoesNotExpireIt(){
        IPointerDownInputProcessStateConstArg arg = CreateMockArg();
        TestAbsPointerDownInputProcessState state = new TestAbsPointerDownInputProcessState(arg);
        IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
        process.IsRunning().Returns(false);
        state.SetProcess(process);

        state.ExpireProcess();

        process.DidNotReceive().Expire();
    }
    



    class TestAbsPointerDownInputProcessState: AbsPointerDownInputProcessState<IUIAdaptorInputProcess>{
        public TestAbsPointerDownInputProcessState(IPointerDownInputProcessStateConstArg arg):base(arg){}
        protected override IUIAdaptorInputProcess CreateProcess(){
            return Substitute.For<IUIAdaptorInputProcess>();
        }
        public IUIAdaptorInputProcess GetProcess_Test(){
            return thisProcess;
        }
        public override void OnPointerUp(ICustomEventData eventData){}
        public override void OnPointerEnter(ICustomEventData eventData){}
        public override void OnPointerExit(ICustomEventData eventData){}
        public Vector2[] GetVelocityStack_Test(){
            return thisVelocityStack;
        }
        public void SetVelocityStack_Test(Vector2[] stack){
            thisVelocityStack = stack;
        }
        public void SetProcess(IUIAdaptorInputProcess process){
            thisProcess = process;
        }
    }
    IPointerDownInputProcessStateConstArg CreateMockArg(){
        IPointerDownInputProcessStateConstArg arg = Substitute.For<IPointerDownInputProcessStateConstArg>();
        arg.engine.Returns(Substitute.For<IUIAdaptorInputStateEngine>());
        arg.uiManager.Returns(Substitute.For<IUIManager>());
        arg.velocityStackSize.Returns(3);
        arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());

        return arg;
    }
}
