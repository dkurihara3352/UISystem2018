using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class PopUpManagerTest{
    [Test]
    public void RegisterPopUp_CallsPopUpToRegisterShowHidden(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IPopUp popUpToRegister = Substitute.For<IPopUp>();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);

        popUpManager.RegisterPopUp(popUpToRegister);

        popUpToRegister.Received(1).ShowHiddenProximateParentPopUpRecursively();
    }
    [Test]
    public void RegisterPopUp_ActivePopUpNull_CallsRootUIEPopUpDisableRecursively(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIE = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIE);
        IPopUp popUpToRegister = Substitute.For<IPopUp>();
        popUpManager.SetActivePopUp_Test(null);

        popUpManager.RegisterPopUp(popUpToRegister);

        rootUIE.Received(1).PopUpDisableRecursivelyDownTo(popUpToRegister);
    }
    [Test]
    public void RegisterPopUp_PopUpToRegIsAncestorOfActive_DoesNotCallActivePopUp(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIE = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIE);
        IPopUp popUpToRegister = Substitute.For<IPopUp>();
        IPopUp activePopUp = Substitute.For<IPopUp>();
        popUpToRegister.IsAncestorOf(activePopUp).Returns(true);
        popUpManager.SetActivePopUp_Test(activePopUp);

        popUpManager.RegisterPopUp(popUpToRegister);

        rootUIE.DidNotReceive().PopUpDisableRecursivelyDownTo(popUpToRegister);
        activePopUp.DidNotReceive().PopUpDisableRecursivelyDownTo(popUpToRegister);
    }
    [Test]
    public void RegisterPopUp_ActivePopUpIsNotChild_CallActivePopUp(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIE = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIE);
        IPopUp popUpToRegister = Substitute.For<IPopUp>();
        IPopUp activePopUp = Substitute.For<IPopUp>();
        IPopUp nullPopUp = null;
        activePopUp.GetProximateParentPopUp().Returns(nullPopUp);
        popUpManager.SetActivePopUp_Test(activePopUp);

        popUpManager.RegisterPopUp(popUpToRegister);

        activePopUp.Received(1).PopUpDisableRecursivelyDownTo(popUpToRegister);
    }
    [Test]
    public void RegisterPopUp_SetsActivePopUp(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIE = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIE);
        IPopUp popUpToRegister = Substitute.For<IPopUp>();

        popUpManager.RegisterPopUp(popUpToRegister);

        IPopUp actual = popUpManager.GetActivePopUp_Test();

        Assert.That(actual, Is.SameAs(popUpToRegister));
    }
    [Test]
    public void UnregiterPopUp_CallsPopUpToUnregHideShow(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);
        IPopUp popUpToUnreg = Substitute.For<IPopUp>();

        popUpManager.UnregisterPopUp(popUpToUnreg);

        popUpToUnreg.Received(1).HideShownChildPopUpsRecursively();
    }
    [Test]
    public void UnregiterPopUp_UnregedPopUpParentPopUpNotNull_CallsItInSequence(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);
        IPopUp popUpToUnreg = Substitute.For<IPopUp>();
        IPopUp popUpToUnregParentPopUp = Substitute.For<IPopUp>();
        popUpToUnreg.GetProximateParentPopUp().Returns(popUpToUnregParentPopUp);
        popUpToUnregParentPopUp.IsAncestorOf(popUpToUnreg).Returns(true);
        popUpManager.SetActivePopUp_Test(popUpToUnreg);//this, or any other offspring will do

        popUpManager.UnregisterPopUp(popUpToUnreg);

        popUpToUnregParentPopUp.Received(1).ReversePopUpDisableRecursively();
        popUpToUnregParentPopUp.Received(1).ShowHiddenProximateParentPopUpRecursively();
        popUpToUnreg.DidNotReceive().PopUpDisableRecursivelyDownTo(popUpToUnregParentPopUp);
    }
    [Test]
    public void UnregiterPopUp_UnregedPopUpParentPopUpNotNull_SetsParentPopUpActive(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);
        IPopUp popUpToUnreg = Substitute.For<IPopUp>();
        IPopUp popUpToUnregParentPopUp = Substitute.For<IPopUp>();
        popUpToUnreg.GetProximateParentPopUp().Returns(popUpToUnregParentPopUp);
        popUpManager.SetActivePopUp_Test(popUpToUnreg);

        popUpManager.UnregisterPopUp(popUpToUnreg);

        Assert.That(popUpManager.GetActivePopUp_Test(), Is.SameAs(popUpToUnregParentPopUp));
    }
    [Test]
    public void UnregiterPopUp_UnregedPopUpParentPopUpNull_CallsRootUIEReverse(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);
        IPopUp popUpToUnreg = Substitute.For<IPopUp>();
        IPopUp popUpToUnregParentPopUp = null;
        popUpToUnreg.GetProximateParentPopUp().Returns(popUpToUnregParentPopUp);
        popUpManager.SetActivePopUp_Test(popUpToUnreg);

        popUpManager.UnregisterPopUp(popUpToUnreg);
        
        rootUIElement.Received(1).ReversePopUpDisableRecursively();
    }
    [Test]
    public void UnregiterPopUp_UnregedPopUpParentPopUpNull_SetsActiveNull(){
        TestPopUpManager popUpManager = new TestPopUpManager();
        IUIElement rootUIElement = Substitute.For<IUIElement>();
        popUpManager.SetRootUIElement(rootUIElement);
        IPopUp popUpToUnreg = Substitute.For<IPopUp>();
        IPopUp popUpToUnregParentPopUp = null;
        popUpToUnreg.GetProximateParentPopUp().Returns(popUpToUnregParentPopUp);
        popUpManager.SetActivePopUp_Test(popUpToUnreg);

        popUpManager.UnregisterPopUp(popUpToUnreg);
        
        Assert.That(popUpManager.GetActivePopUp_Test(), Is.Null);
    }
    public class TestPopUpManager: PopUpManager{
        public IPopUp GetActivePopUp_Test(){
            return thisActivePopUp;
        }
        public void SetActivePopUp_Test(IPopUp popUp){
            SetActivePopUp(popUp);
        }
    }
}
