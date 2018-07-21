using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("DKUtility")]
public class SwitchableStateTest{
	[Test]
	public void Constructor_WhenCalled_CurStateIsNull(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		
		Assert.That(engine.GetCurState(), Is.Null);
	}
	[Test]
	public void TrySwitchState_FromNullToState_StateCalledOnEnter(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState state = Substitute.For<ITestSwitchableState>();

		engine.TrySwitchState(state);

		state.Received(1).OnEnter();
	}
	[Test]
	public void TrySwitchState_FromNullToState_CurStateIsState(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState state = Substitute.For<ITestSwitchableState>();

		engine.TrySwitchState(state);

		Assert.That(engine.GetCurState(), Is.SameAs(state));
	}
	[Test]
	public void TestSwitchState_FromAStateToSameState_StateCalledOnEnterOnce(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState state = Substitute.For<ITestSwitchableState>();
		
		engine.TrySwitchState(state);

		state.Received(1).OnEnter();
		state.DidNotReceive().OnExit();
	}
	[Test]
	public void TestSwitchState_FromAStateToSameState_CurStateRemainsState(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState state = Substitute.For<ITestSwitchableState>();
		
		engine.TrySwitchState(state);

		Assert.That(engine.GetCurState(), Is.SameAs(state));
	}
	[Test]
	public void TestSwitchState_FromStateToDiffStateOfSameType_TransitionNotSuccessful(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState stateA = Substitute.For<ITestSwitchableState>();
		ITestSwitchableState stateB = Substitute.For<ITestSwitchableState>();
		engine.TrySwitchState(stateA);

		engine.TrySwitchState(stateB);

		Assert.That(engine.GetCurState(), Is.SameAs(stateA));
		stateA.Received(1).OnEnter();
		stateA.DidNotReceive().OnExit();
		stateB.DidNotReceive().OnEnter();
	}
	[Test]
	public void TestSwitchState_FromStateToDiffStateOfDiffType_TransitionSuccessful(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState stateA = Substitute.For<ITestSwitchableStateA>();
		ITestSwitchableState stateB = Substitute.For<ITestSwitchableStateB>();
		engine.TrySwitchState(stateA);

		engine.TrySwitchState(stateB);

		Assert.That(engine.GetCurState(), Is.SameAs(stateB));
		stateA.Received(1).OnEnter();
		stateA.Received(1).OnExit();
		stateB.Received(1).OnEnter();
	}
	[Test][ExpectedException(typeof(System.ArgumentNullException))]
	public void TestSwitchState_FromStateToNull_ThrowsException(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		ITestSwitchableState state = Substitute.For<ITestSwitchableState>();
		engine.TrySwitchState(state);

		engine.TrySwitchState(null);
	}
	[Test][ExpectedException(typeof(System.ArgumentNullException))]
	public void TestSwitchState_FromNullToNull_ThrowsException(){
		TestSwitchableStateEngine engine = new TestSwitchableStateEngine();
		Assert.That(engine.GetCurState(), Is.Null);
		
		engine.TrySwitchState(null);
	}
	/* Test Support Classes */
		public interface ITestSwitchableState: ISwitchableState{}
		public interface ITestSwitchableStateA: ITestSwitchableState{}
		public interface ITestSwitchableStateB: ITestSwitchableState{}
		public class TestSwitchableStateEngine: AbsSwitchableStateEngine<ITestSwitchableState>{
			public ITestSwitchableState GetCurState(){
				return this.thisCurState;
			}
		}
	/*  */
}
