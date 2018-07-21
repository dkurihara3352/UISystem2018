using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class UIEActivationStateEngineTest {
	[Test]
	public void CurState_Initially_IsDeactivationCompletedState(){
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngine();
		
		IUIEActivationState actualState = engine.GetCurState();

		Assert.That(actualState is IUIEDeactivationCompletedState, Is.True);
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
	[Test][TestCaseSource(typeof(IsActivated_TestCase), "cases")]
	public void IsActivated_WhenCurStateIsActivatingOrActivationCompleted_ReturnsTrue(System.Type stateType, bool expected){
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngineWithState(stateType);

		Assert.That(engine.IsActivated(), Is.EqualTo(expected));
	}
	public class IsActivated_TestCase{
		public static object[] cases = {
			new object[]{typeof(IUIEActivatingState), true},
			new object[]{typeof(IUIEActivationCompletedState), true},
			new object[]{typeof(IUIEDeactivatingState), false},
			new object[]{typeof(IUIEDeactivationCompletedState), false},
		};
	}
	/* Test Classes */
	public class TestUIEActivationStateEngine: AbsUIEActivationStateEngine{
		public TestUIEActivationStateEngine(IUISystemProcessFactory processFactory, IUIElement uiElement): base(processFactory, uiElement){

		}
		protected override IUIEActivatingState CreateUIEActivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			return Substitute.For<IUIEActivatingState>();
		}
		protected override IUIEDeactivatingState CreateUIEDeactivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			return Substitute.For<IUIEDeactivatingState>();
		}
		public IUIEActivationState GetCurState(){
			return thisCurState;
		}		
	}
	public TestUIEActivationStateEngine CreateTestUIEActivationStateEngine(){
		IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
		IUIElement uiElement = Substitute.For<IUIElement>();
		return new TestUIEActivationStateEngine(processFactory, uiElement);
	}
	public TestUIEActivationStateEngine CreateTestUIEActivationStateEngineWithState(System.Type stateType){
		TestUIEActivationStateEngine engine = CreateTestUIEActivationStateEngine();
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
