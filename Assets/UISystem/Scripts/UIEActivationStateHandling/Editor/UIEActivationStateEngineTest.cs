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
		ITestUIEActivationStateEngineConstArg arg = CreateMockConstArg();
		TestUIEActivationStateEngine engine = new TestUIEActivationStateEngine(arg);

		Assert.That(engine.IsInDeactivationCompletedState_Test(), Is.True);
	}
	[Test]
	public void StartNewActivationProcess_ActivationModeIsNone_SwtchesProcessProperly(){
		ITestUIEActivationStateEngineConstArg arg = CreateMockConstArg();
		arg.activationMode.Returns(ActivationMode.None);
		TestUIEActivationStateEngine engine = new TestUIEActivationStateEngine(arg);

		IUISystemProcessFactory procFactory = arg.processFactory;
		INonActivatorUIEActivationProcess process = Substitute.For<INonActivatorUIEActivationProcess>();
		procFactory.CreateNonActivatorUIEActivationProcess(engine, true).Returns(process);
		IUIEActivationProcess prevProcess = Substitute.For<IUIEActivationProcess>();
		prevProcess.IsRunning().Returns(true);
		engine.SetRunningProcess(prevProcess);

		engine.StartNewActivateProcess();

		prevProcess.Received(1).Stop();
		process.Received(1).Run();
		Assert.That(engine.GetCurRunningProcess_Test(), Is.SameAs(process));
	}
	[Test]
	public void StartNewActivationProcess_ActivationModeIsAlpha_SwtchesProcessProperly(){
		ITestUIEActivationStateEngineConstArg arg = CreateMockConstArg();
		arg.activationMode.Returns(ActivationMode.Alpha);
		TestUIEActivationStateEngine engine = new TestUIEActivationStateEngine(arg);

		IUISystemProcessFactory procFactory = arg.processFactory;
		IAlphaActivatorUIEActivationProcess process = Substitute.For<IAlphaActivatorUIEActivationProcess>();
		procFactory.CreateAlphaActivatorUIEActivationProcess(arg.uiElement, engine, true).Returns(process);
		IUIEActivationProcess prevProcess = Substitute.For<IUIEActivationProcess>();
		prevProcess.IsRunning().Returns(true);
		engine.SetRunningProcess(prevProcess);

		engine.StartNewActivateProcess();

		prevProcess.Received(1).Stop();
		process.Received(1).Run();
		Assert.That(engine.GetCurRunningProcess_Test(), Is.SameAs(process));
	}


	/* Test Classes */
	public class TestUIEActivationStateEngine: UIEActivationStateEngine{
		public TestUIEActivationStateEngine(
			ITestUIEActivationStateEngineConstArg arg
		): base(
			arg.processFactory, 
			arg.uiElement,
			arg.activationMode
		){}
		public IUIEActivationState GetCurState(){
			return thisCurState;
		}
		public bool IsInDeactivationCompletedState_Test(){
			return thisCurState == thisDeactivationCompletedState;
		}
		public void SetToActivationCompletedState_Test(){
			thisCurState = thisActivationCompletedState;
		}
		public IUIEActivationProcess GetCurRunningProcess_Test(){
			return thisRunningProcess;
		}
		public void SetRunningProcess(IUIEActivationProcess process){
			thisRunningProcess = process;
		}
	}
	public interface ITestUIEActivationStateEngineConstArg{
		IUISystemProcessFactory processFactory{get;}
		IUIElement uiElement{get;}
		ActivationMode activationMode{get;}
	}
	public ITestUIEActivationStateEngineConstArg CreateMockConstArg(){
		ITestUIEActivationStateEngineConstArg arg = Substitute.For<ITestUIEActivationStateEngineConstArg>();
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElement.Returns(Substitute.For<IUIElement>());
		arg.activationMode.Returns(ActivationMode.None);
		return arg;
	}
}
