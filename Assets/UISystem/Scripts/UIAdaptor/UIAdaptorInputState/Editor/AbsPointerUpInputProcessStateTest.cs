using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;
[TestFixture, Category("UISystem")]
public class AbsPointerUpInputProcessStateTest {
	[Test]
	public void OnEnter_CreatesAndRunsProcess(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);

		state.OnEnter();

		IUIAdaptorInputProcess process = state.GetProcess_Test();
		process.Received(1).Run();
	}
	[Test]
	public void OnExit_SetsProcessNull(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);
		state.SetProcess_Test(Substitute.For<IUIAdaptorInputProcess>());
		Assert.That(state.GetProcess_Test(), Is.Not.Null);

		state.OnExit();

		Assert.That(state.GetProcess_Test(), Is.Null);
	}
	[Test]
	public void OnExit_ProcessIsRunning_CallsItStop(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);
		IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
		process.IsRunning().Returns(true);
		state.SetProcess_Test(process);

		state.OnExit();

		process.Received(1).Stop();
	}
	[Test]
	public void OnExit_ProcessIsNotRunning_DoesNotCallItStop(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);
		IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
		process.IsRunning().Returns(false);
		state.SetProcess_Test(process);

		state.OnExit();

		process.DidNotReceive().Stop();
	}
	[Test]
	public void ExpireProcess_ProcessIsRunning_CallsItExpire(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);
		IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
		process.IsRunning().Returns(true);
		state.SetProcess_Test(process);

		state.ExpireProcess();

		process.Received(1).Expire();
	}
	[Test]
	public void ExpireProcess_ProcessIsNotRunning_DoesNotCallItExpire(){
		IPointerUpInputProcessStateConstArg arg = CreateMockArg();
		TestAbsPointerUpInputProcessState state = new TestAbsPointerUpInputProcessState(arg);
		IUIAdaptorInputProcess process = Substitute.For<IUIAdaptorInputProcess>();
		process.IsRunning().Returns(false);
		state.SetProcess_Test(process);

		state.ExpireProcess();

		process.DidNotReceive().Expire();
	}

	public class TestAbsPointerUpInputProcessState: AbsPointerUpInputProcessState<IUIAdaptorInputProcess>{
		public TestAbsPointerUpInputProcessState(
			IPointerUpInputProcessStateConstArg arg
		): base(
			arg
		){}
		public override void OnPointerDown(ICustomEventData eventData){}
		protected override IUIAdaptorInputProcess CreateProcess(){
			return Substitute.For<IUIAdaptorInputProcess>();
		}
		public IUIAdaptorInputProcess GetProcess_Test(){
			return thisProcess;
		}
		public void SetProcess_Test(IUIAdaptorInputProcess process){
			thisProcess = process;
		}
	}
	IPointerUpInputProcessStateConstArg CreateMockArg(){
		IPointerUpInputProcessStateConstArg arg = Substitute.For<IPointerUpInputProcessStateConstArg>();
		arg.engine.Returns(Substitute.For<IUIAdaptorInputStateEngine>());
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		
		return arg;
	}
}
