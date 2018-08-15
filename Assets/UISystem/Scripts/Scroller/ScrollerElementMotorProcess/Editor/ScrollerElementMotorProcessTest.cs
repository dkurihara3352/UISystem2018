using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ScrollerElementMotorProcessTest {
	[Test]
	public void Run_CallsScrollerSetRunningMotorProcessThis(){
		IScroller scroller;
		TestScrollerElementMotorProcess process = CreateProcessWithMockScroller(out scroller);

		process.Run();

		scroller.Received(1).SwitchRunningElementMotorProcess(process, 0);
	}
	// [Test]
	// public void Stop_CallsScrollerClearRunningElementMotorProcessThis(){
	// 	IScroller scroller;
	// 	TestScrollerElementMotorProcess process = CreateProcessWithMockScroller(out scroller);

	// 	process.Stop();

	// 	scroller.Received(1).ClearScrollerElementMotorProcess(process, 0);
	// }
	public class SetScrollerElementLocalPosOnAxis_TestCase{
		public static object[] cases = {
			new object[]{0f, 0, new Vector2(5f, 5f), new Vector2(0f, 5f)},
			new object[]{0f, 1, new Vector2(5f, 5f), new Vector2(5f, 0f)},
			new object[]{10f, 0, new Vector2(5f, 5f), new Vector2(10f, 5f)},
			new object[]{10f, 1, new Vector2(5f, 5f), new Vector2(5f, 10f)},
			new object[]{-10f, 0, new Vector2(5f, 5f), new Vector2(-10f, 5f)},
			new object[]{-10f, 1, new Vector2(5f, 5f), new Vector2(5f, -10f)},
		};
	}







	public class TestScrollerElementMotorProcess: AbsScrollerElementMotorProcess{
		public TestScrollerElementMotorProcess(ITestScrollerElementMotorProcessConstArg arg): base(arg.scroller, arg.dimension, arg.processManager){}
		public override void UpdateProcess(float deltaT){}
	}
	public interface ITestScrollerElementMotorProcessConstArg{
		IScroller scroller{get;}
		int dimension{get;}
		IProcessManager processManager{get;}
	}
	public ITestScrollerElementMotorProcessConstArg CreateMockConstArg(){
		ITestScrollerElementMotorProcessConstArg arg = Substitute.For<ITestScrollerElementMotorProcessConstArg>();
		arg.scroller.Returns(Substitute.For<IScroller>());
		arg.dimension.Returns(0);
		arg.processManager.Returns(Substitute.For<IProcessManager>());
		return arg;
	}
	public TestScrollerElementMotorProcess CreateProcessWithMockScroller(out IScroller scroller){
		ITestScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IScroller thisScroller = Substitute.For<IScroller>();
		arg.scroller.Returns(thisScroller);
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		scroller = thisScroller;
		return process;
	}
}
