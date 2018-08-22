using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class AbsPointerUpInputStateTest {
	[Test]
	public void OnPointerUp_ThrowsException(){
		IUIAdaptorInputStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputState state = new TestAbsPointerUpInputState(arg);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).
			And.Message.EqualTo("OnPointerUp should not be called while pointer is already held up"),
			() => {
				state.OnPointerUp(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test]
	public void OnBeginDrag_ThrowsException(){
		IUIAdaptorInputStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputState state = new TestAbsPointerUpInputState(arg);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).
			And.Message.EqualTo("OnBeginDrag should not be called while pointer is held up"),
			() => {
				state.OnBeginDrag(Substitute.For<ICustomEventData>());
			}
		);
	}
	[Test]
	public void OnDrag_ThrowsException(){
		IUIAdaptorInputStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputState state = new TestAbsPointerUpInputState(arg);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).
			And.Message.EqualTo("OnDrag should be impossible when pointer is held up, something's wrong"),
			() => {
				state.OnDrag(Substitute.For<ICustomEventData>());
			}
		);
	}

	public class TestAbsPointerUpInputState: AbsPointerUpInputState{
		public TestAbsPointerUpInputState(
			IUIAdaptorInputStateConstArg arg
		): base(
			arg
		){}

		public override void OnPointerDown(ICustomEventData eventData){}
		public override void OnEnter(){}
		public override void OnExit(){}
	}

	IUIAdaptorInputStateConstArg CreateMockArg(){
		IUIAdaptorInputStateConstArg arg = Substitute.For<IUIAdaptorInputStateConstArg>();
		arg.engine.Returns(Substitute.For<IUIAdaptorInputStateEngine>());
		return arg;
	}
}
