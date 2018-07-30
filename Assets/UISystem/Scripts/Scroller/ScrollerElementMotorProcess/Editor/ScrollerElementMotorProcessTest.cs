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
	[Test]
	public void Stop_CallsScrollerClearRunningElementMotorProcessThis(){
		IScroller scroller;
		TestScrollerElementMotorProcess process = CreateProcessWithMockScroller(out scroller);

		process.Stop();

		scroller.Received(1).ClearScrollerElementMotorProcess(process, 0);
	}
	[Test, TestCaseSource(typeof(SetScrollerElementLocalPosOnAxis_TestCase), "cases")]
	public void SetScrollerElementLocalPosOnAxis_CallsScrollerElementSetLocaPos(float elementLocalPosOnAxis, int dimension, Vector2 elementLocalPosition, Vector2 expected){
		IUIElement scrollerElement;
		TestScrollerElementMotorProcess process = CreateProcess_OutScrollerElement_Dimension_ElementLocalPosition(out scrollerElement, dimension, elementLocalPosition);

		process.SetScrollerElementLocalPosOnAxis_Test(elementLocalPosOnAxis);

		scrollerElement.Received(1).SetLocalPosition(expected);
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
		public TestScrollerElementMotorProcess(ITestScrollerElementMotorProcessConstArg arg): base(arg.scroller, arg.scrollerElement, arg.dimension, arg.processManager){}
		public override void UpdateProcess(float deltaT){}
		public void SetScrollerElementLocalPosOnAxis_Test(float newLocalPosOnAxis){
			this.SetScrollerElementLocalPosOnAxis(newLocalPosOnAxis);
		}
	}
	public interface ITestScrollerElementMotorProcessConstArg{
		IScroller scroller{get;}
		IUIElement scrollerElement{get;}
		int dimension{get;}
		IProcessManager processManager{get;}
	}
	public ITestScrollerElementMotorProcessConstArg CreateMockConstArg(){
		ITestScrollerElementMotorProcessConstArg arg = Substitute.For<ITestScrollerElementMotorProcessConstArg>();
		arg.scroller.Returns(Substitute.For<IScroller>());
		arg.scrollerElement.Returns(Substitute.For<IUIElement>());
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
	public TestScrollerElementMotorProcess CreateProcessWthMockScrollerElement(out IUIElement scrollerElement){
		ITestScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IUIElement thisScrollerElement = Substitute.For<IUIElement>();
		arg.scrollerElement.Returns(thisScrollerElement);
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		scrollerElement = thisScrollerElement;
		return process;
	}
	public TestScrollerElementMotorProcess CreateProcessWithMockScrollerAndDimension(out IScroller scroller, int dimension){
		ITestScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IScroller thisScroller = Substitute.For<IScroller>();
		arg.scroller.Returns(thisScroller);
		arg.dimension.Returns(dimension);
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		scroller = thisScroller;
		return process;
	}
	public TestScrollerElementMotorProcess CreateProcessWthMockScrollerElementAndDimension(out IUIElement scrollerElement, int dimension){
		ITestScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IUIElement thisScrollerElement = Substitute.For<IUIElement>();
		arg.scrollerElement.Returns(thisScrollerElement);
		arg.dimension.Returns(dimension);
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		scrollerElement = thisScrollerElement;
		return process;
	}
	public TestScrollerElementMotorProcess CreateProcess_OutScrollerElement_Dimension_ElementLocalPosition(out IUIElement scrollerElement, int dimension, Vector2 elementLocalPosition){
		ITestScrollerElementMotorProcessConstArg arg = CreateMockConstArg();
		IUIElement thisScrollerElement = Substitute.For<IUIElement>();
		arg.scrollerElement.Returns(thisScrollerElement);
		arg.dimension.Returns(dimension);
		thisScrollerElement.GetLocalPosition().Returns(elementLocalPosition);
		TestScrollerElementMotorProcess process = new TestScrollerElementMotorProcess(arg);

		scrollerElement = thisScrollerElement;
		return process;
	}
}
