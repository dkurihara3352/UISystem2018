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

	public class TestScrollerElementMotorProcess: AbsScrollerElementMotorProcess{
		public TestScrollerElementMotorProcess(ITestScrollerElementMotorProcessConstArg arg): base(arg.scroller, arg.scrollerElement, arg.dimension, arg.processManager){}
		public override void UpdateProcess(float deltaT){}
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
}
