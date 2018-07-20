using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture]
public class UIEActivationStateEngineTest {
	[Test]
	public void SetInitializationFields_CallsAllStatesSetInitializationFields(){
		IUIEActivationStateEngineConstArg arg;
		IUIEActivationStateEngine engine = CreateUIEActivationStateEngine(out arg);
		IUIEActivationState[] states = new IUIEActivationState[4]{
			arg.activatingState,
			arg.activationCompletedState,
			arg.deactivatingState,
			arg.deactivationCompletedState
		};
		IUIElement uie = Substitute.For<IUIElement>();
		engine.SetInitializationFields(uie);
		foreach(IUIEActivationState state in states)
			state.Received(1).SetInitializationFields(engine, uie);
	}
	[Test]
	public void CurState_Initially_IsNull(){
		IUIEActivationStateEngineConstArg arg;
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngine(out arg);
		
		IUIEActivationState actualState = engine.GetCurState();

		Assert.That(actualState, Is.Null);
	}
	[Test][TestCaseSource(typeof(IsActivationComplete_TestCase), "cases")]
	public void IsActivationComplete_WhenCurStateIsActivationIsComplete_ReturnsTrue(System.Type stateType, bool expected){
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngineWithState(stateType);

		Assert.That(engine.IsActivationComplete(), Is.EqualTo(expected));
	}
	public class IsActivationComplete_TestCase{
		public static object[] cases = {
			new object[]{typeof(IUIEActivatingState), false},
			new object[]{typeof(IUIEActivationCompletedState), true},
			new object[]{typeof(IUIEDeactivatingState), false},
			new object[]{typeof(IUIEDeactivationCompletedState), false},
		};
	}
	/* Test Classes */
	public IUIEActivationStateEngine CreateUIEActivationStateEngine(out IUIEActivationStateEngineConstArg arg){
		IUIEActivationStateEngineConstArg thisArg = Substitute.For<IUIEActivationStateEngineConstArg>();
		thisArg.activatingState.Returns(Substitute.For<IUIEActivatingState>());
		thisArg.activationCompletedState.Returns(Substitute.For<IUIEActivationCompletedState>());
		thisArg.deactivatingState.Returns(Substitute.For<IUIEDeactivatingState>());
		thisArg.deactivationCompletedState.Returns(Substitute.For<IUIEDeactivationCompletedState>());
		arg = thisArg;
		return new UIEActivationStateEngine(thisArg);
	}
	public class TestUIEActivationStateEngine: UIEActivationStateEngine{
		public TestUIEActivationStateEngine(IUIEActivationStateEngineConstArg arg): base(arg){}
		public IUIEActivationState GetCurState(){
			return thisCurState;
		}
	}
	public TestUIEActivationStateEngine CreateTestUIEActivationStateEngine(out IUIEActivationStateEngineConstArg arg){
		IUIEActivationStateEngineConstArg thisArg = Substitute.For<IUIEActivationStateEngineConstArg>();
		thisArg.activatingState.Returns(Substitute.For<IUIEActivatingState>());
		thisArg.activationCompletedState.Returns(Substitute.For<IUIEActivationCompletedState>());
		thisArg.deactivatingState.Returns(Substitute.For<IUIEDeactivatingState>());
		thisArg.deactivationCompletedState.Returns(Substitute.For<IUIEDeactivationCompletedState>());
		arg = thisArg;
		return new TestUIEActivationStateEngine(thisArg);
	}
	public TestUIEActivationStateEngine CreateTestUIEActivationStateEngineWithState(System.Type stateType){
		IUIEActivationStateEngineConstArg thisArg;
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngine(out thisArg);
		if(stateType == typeof(IUIEActivatingState))
			engine.SetToActivatingState();
		else if(stateType == typeof(IUIEActivationCompletedState))
			engine.SetToActivationCompletedState();
		else if(stateType == typeof(IUIEDeactivatingState))
			engine.SetToDeactivatingState();
		else if(stateType == typeof(IUIEDeactivationCompletedState))
			engine.SetToDeactivationCompletedState();
		else
			throw new System.InvalidOperationException("T must be of any of four IUIEActivationState");
		return engine;
	}
	[Test][TestCaseSource(typeof(MetaTest_CreateTestUIEActivationStateEngineWithState_WorksFine_TestCase), "cases")]
	public void MetaTest_CreateTestUIEActivationStateEngineWithState_WorksFine(System.Type stateType){
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngineWithState(stateType);
		if(stateType == typeof(IUIEActivatingState))
			Assert.That(engine.GetCurState() is IUIEActivatingState, Is.True);
		else if(stateType == typeof(IUIEActivationCompletedState))
			Assert.That(engine.GetCurState() is IUIEActivationCompletedState, Is.True);
		else if(stateType == typeof(IUIEDeactivatingState))
			Assert.That(engine.GetCurState() is IUIEDeactivatingState, Is.True);
		else if(stateType == typeof(IUIEDeactivationCompletedState))
			Assert.That(engine.GetCurState() is IUIEDeactivationCompletedState, Is.True);
	}
	public class MetaTest_CreateTestUIEActivationStateEngineWithState_WorksFine_TestCase{
		public static object[] cases = {
			new object[]{typeof(IUIEActivatingState)},
			new object[]{typeof(IUIEActivationCompletedState)},
			new object[]{typeof(IUIEDeactivatingState)},
			new object[]{typeof(IUIEDeactivationCompletedState)}
		};
	}
}
