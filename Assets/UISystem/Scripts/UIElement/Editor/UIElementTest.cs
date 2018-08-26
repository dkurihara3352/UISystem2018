using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class UIElementTest {
	[Test]
	public void OnTap_IsActivated_IsDisabledForPopUp_CallsPopUpManagerHideActivePopUp(){
		ITestUIElementConstArg arg = CreateMockArg();
		TestUIElement testUIE = new TestUIElement(arg);
		testUIE.SetIsActivated_Test(true);
		testUIE.SetIsDisabledForPopUp_Test(true);
		IPopUpManager popUpManager = arg.popUpManager;

		testUIE.OnTap(1);


		popUpManager.Received(1).CheckAndHideActivePopUp();
	}
	[Test]
	public void OnTap_IsActivated_IsNotDisabledForPopUp_IsEnabledInput_CallsParentScrollerInSequence(){
		ITestUIElementConstArg arg = CreateMockArg();
		TestUIElement testUIE = new TestUIElement(arg);
		testUIE.SetIsActivated_Test(true);
		testUIE.SetIsDisabledForPopUp_Test(false);
		testUIE.SetIsEnabledInput_Test(true);
		IScroller parentScroller = testUIE.GetProximateParentScroller();

		testUIE.OnTap(1);

		parentScroller.Received(1).UpdateVelocity(0f, 0);
		parentScroller.Received(1).UpdateVelocity(0f, 1);
		parentScroller.Received(1).ResetDrag();
		parentScroller.Received(1).CheckAndPerformStaticBoundarySnap();
	}
	[Test]
	public void OnTap_IsActivated_IsNotDisabledForPopUp_IsEnabledInput_CallsParentUIEOnTap(){
		ITestUIElementConstArg arg = CreateMockArg();
		TestUIElement testUIE = new TestUIElement(arg);
		testUIE.SetIsActivated_Test(true);
		testUIE.SetIsDisabledForPopUp_Test(false);
		testUIE.SetIsEnabledInput_Test(true);
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);

		testUIE.OnTap(1);

		parentUIE.Received(1).OnTap(1);
	}
	[Test]
	public void OnTap_IsActivated_IsNotDisabledForPopUp_IsNotEnabledInput_CallsParentUIEOnTap(){
		ITestUIElementConstArg arg = CreateMockArg();
		TestUIElement testUIE = new TestUIElement(arg);
		testUIE.SetIsActivated_Test(true);
		testUIE.SetIsDisabledForPopUp_Test(false);
		testUIE.SetIsEnabledInput_Test(false);
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);

		testUIE.OnTap(1);

		parentUIE.Received(1).OnTap(1);
	}


	public class TestUIElement: UIElement{
		public TestUIElement(ITestUIElementConstArg arg): base(arg){
			thisUIEActivationStateEngine = arg.activationStateEngine;

		}
		public void SetIsActivated_Test(bool isActivated){
			thisUIEActivationStateEngine.IsActivated().Returns(isActivated);
		}
		public void SetIsDisabledForPopUp_Test(bool isDisabled){
			thisIsDisabledForPopUp = isDisabled;
		}
		public void SetIsEnabledInput_Test(bool isEnabled){
			thisIsEnabledInput = isEnabled;
		}
		protected override IScroller FindProximateParentScroller(){
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullScroller = null;
			parentScroller.GetParentUIE().Returns(nullScroller);
			parentScroller.GetProximateParentScroller().Returns(nullScroller);
			return parentScroller;
		}
	}
	public ITestUIElementConstArg CreateMockArg(){
		ITestUIElementConstArg arg = Substitute.For<ITestUIElementConstArg>();
		IUIManager uim = Substitute.For<IUIManager>();
		IPopUpManager popUpManager = Substitute.For<IPopUpManager>();
		uim.GetPopUpManager().Returns(popUpManager);
		arg.uim.Returns(uim);
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
		arg.uia.Returns(Substitute.For<IUIAdaptor>());
		arg.activationMode.Returns(ActivationMode.None);
		arg.activationStateEngine.Returns(Substitute.For<IUIEActivationStateEngine>());
		arg.popUpManager.Returns(popUpManager);

		return arg;
	}
	public interface ITestUIElementConstArg: IUIElementConstArg{
		IUIEActivationStateEngine activationStateEngine{get;}
		IPopUpManager popUpManager{get;}
	}
}
