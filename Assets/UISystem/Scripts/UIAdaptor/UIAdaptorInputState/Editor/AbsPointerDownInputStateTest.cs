using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class AbsPointerDownInputStateTest {

    class TestAbsPointerDownInputState: AbsPointerDownInputState{
        public TestAbsPointerDownInputState(IPointerDownInputStateConstArg arg): base(arg){
        }
        public override void OnExit(){}
        public override void OnPointerUp(ICustomEventData eventData){}
        public override void OnPointerEnter(ICustomEventData eventData){}
        public override void OnPointerExit(ICustomEventData eventData){}
    }
	IPointerDownInputStateConstArg CreateMockArg(){
        IPointerDownInputStateConstArg arg = Substitute.For<IPointerDownInputStateConstArg>();
        arg.engine.Returns(Substitute.For<IUIAdaptorInputStateEngine>());
        arg.uiManager.Returns(Substitute.For<IUIManager>());
        arg.velocityStackSize.Returns(3);

        return arg;
    }
}
