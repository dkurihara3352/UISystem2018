using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class PopUpTest{
	[Test]
	public void ShowHiddenProximateParentPopUpRecursively_ProxParNotNull_ProxParIsHidden_CallsItInSequence(){
		IPopUpConstArg arg = CreateMockArg();
		TestPopUp popUp = new TestPopUp(arg);
		popUp.SetUpPopUpHierarchy();
		IPopUp parentPopUp = popUp.GetProximateParentPopUp();
		parentPopUp.IsHidden().Returns(true);

		popUp.ShowHiddenProximateParentPopUpRecursively();

		parentPopUp.Received(1).Show(false);
		parentPopUp.Received(1).ShowHiddenProximateParentPopUpRecursively();
	}
	[Test]
	public void HideShownChildPopUpsRecursively_ChildIsActivated_ChildIsShown_CallsItInSequence(){
		IPopUpConstArg arg = CreateMockArg();
		TestPopUp popUp = new TestPopUp(arg);
		popUp.SetUpPopUpHierarchy();
		
		List<IPopUp> calledChildrent = new List<IPopUp>();
		for(int i = 0; i < 3 ; i ++){
			IPopUp childPopUp = Substitute.For<IPopUp>();
			childPopUp.IsActivated().Returns(true);
			childPopUp.IsShown().Returns(true);
			popUp.RegisterProximateChildPopUp(childPopUp);
			calledChildrent.Add(childPopUp);
		}
		List<IPopUp> notCalledChildren = new List<IPopUp>();
		for(int i = 0; i < 3 ; i ++){
			IPopUp childPopUp = Substitute.For<IPopUp>();
			childPopUp.IsActivated().Returns(false);
			childPopUp.IsShown().Returns(true);
			popUp.RegisterProximateChildPopUp(childPopUp);
			notCalledChildren.Add(childPopUp);
		}
		List<IPopUp> notCalledChildren2 = new List<IPopUp>();
		for(int i = 0; i < 3 ; i ++){
			IPopUp childPopUp = Substitute.For<IPopUp>();
			childPopUp.IsActivated().Returns(true);
			childPopUp.IsShown().Returns(false);
			popUp.RegisterProximateChildPopUp(childPopUp);
			notCalledChildren2.Add(childPopUp);
		}

		popUp.HideShownChildPopUpsRecursively();

		foreach(IPopUp childPopUp in calledChildrent){
			childPopUp.Received(1).Hide(false);
			childPopUp.Received(1).HideShownChildPopUpsRecursively();
		}
		foreach(IPopUp childPopUp in notCalledChildren){
			childPopUp.DidNotReceive().Hide(false);
			childPopUp.DidNotReceive().HideShownChildPopUpsRecursively();
		}
		foreach(IPopUp childPopUp in notCalledChildren2){
			childPopUp.DidNotReceive().Hide(false);
			childPopUp.DidNotReceive().HideShownChildPopUpsRecursively();
		}

	}
	[Test]
	public void IsAncestorOf_Child_ReturnsTrue(){
		IPopUpConstArg arg = CreateMockArg();
		TestPopUp popUp = new TestPopUp(arg);
		IPopUp childPopUp = Substitute.For<IPopUp>();
		childPopUp.GetProximateParentPopUp().Returns(popUp);

		Assert.That(popUp.IsAncestorOf(childPopUp), Is.True);
	}
	[Test]
	public void IsAncestorOf_GrandChild_ReturnsTrue(){
		IPopUpConstArg arg = CreateMockArg();
		TestPopUp popUp = new TestPopUp(arg);
		IPopUp childPopUp = Substitute.For<IPopUp>();
		childPopUp.GetProximateParentPopUp().Returns(popUp);
		IPopUp grandChildPopUp = Substitute.For<IPopUp>();
		grandChildPopUp.GetProximateParentPopUp().Returns(childPopUp);

		Assert.That(popUp.IsAncestorOf(grandChildPopUp), Is.True);
	}
	[Test]
	public void IsAncestorOf_Parent_ReturnsFalse(){
		IPopUpConstArg arg = CreateMockArg();
		TestPopUp popUp = new TestPopUp(arg);
		IPopUp childPopUp = Substitute.For<IPopUp>();
		childPopUp.GetProximateParentPopUp().Returns(popUp);

		Assert.That(childPopUp.IsAncestorOf(popUp), Is.False);
	}

	public class TestPopUp: PopUp{
		public TestPopUp(IPopUpConstArg arg): base(arg){
		}
		protected override IPopUp FindProximateParentPopUp(){
			IPopUp parentPopUp = Substitute.For<IPopUp>();
			IPopUp nullPopUp = null;
			parentPopUp.GetProximateParentPopUp().Returns(nullPopUp);
			return parentPopUp;
		}
		protected override IScroller FindProximateParentScroller(){
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullScroller = null;
			parentScroller.GetParentUIE().Returns(nullScroller);
			parentScroller.GetProximateParentScroller().Returns(nullScroller);
			
			return parentScroller;
		}
		protected override IPopUpAdaptor GetPopUpAdaptor(){
			return Substitute.For<IPopUpAdaptor>();
		}
	}
	public IPopUpConstArg CreateMockArg(){
		IPopUpConstArg arg = Substitute.For<IPopUpConstArg>();
		arg.popUpManager.Returns(Substitute.For<IPopUpManager>());
		arg.hidesOnTappingOthers.Returns(true);
		arg.popUpMode.Returns(PopUpMode.Alpha);

		return arg;
	}
}
