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
		IScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IScroller scroller = arg.scroller;
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		process.Run();

		scroller.Received(1).SwitchRunningElementMotorProcess(process, 0);
	}
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
		public TestScrollerElementMotorProcess(IScrollerElementMotorProcessConstArg arg): base(arg){}
		public override void UpdateProcess(float deltaT){}
	}
	public IScrollerElementMotorProcessConstArg CreateMockConstArg(){
		IScrollerElementMotorProcessConstArg arg = Substitute.For<IScrollerElementMotorProcessConstArg>();
		arg.scroller.Returns(Substitute.For<IScroller>());
		arg.scrollerElement.Returns(Substitute.For<IUIElement>());
		arg.dimension.Returns(0);
		arg.processManager.Returns(Substitute.For<IProcessManager>());
		return arg;
	}
}
