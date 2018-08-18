using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture][Category("UISystem")]
public class AlphaActivatorUIEActivationProcessTest{
    [Test][TestCaseSource(typeof(GetLatestInitialValueDifference_TestCase), "cases")]
    public void GetLatestInitialValueDifference_Various(float currentGroupAlpha, bool doesActivate, float expectedValueDifference){
        ITestAlphaActivatorUIEActivationProcessConstArg arg;
        TestAlphaActivatorUIEActivationProcess process = CreateTestAlphaActivatorUIEActivationProcess(out arg, 0f, doesActivate);
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
        arg.uiElement.GetUIAdaptor().Returns(uia);

        uia.GetGroupAlpha().Returns(currentGroupAlpha);

        float actual = process.TestGetLatestInitialValueDifference();

        Assert.That(actual, Is.EqualTo(expectedValueDifference));
    }
    public class GetLatestInitialValueDifference_TestCase{
        public static object[] cases = {
            new object[]{0f, true, 1f},
            new object[]{1f, true, 0f},
            new object[]{.5f, true, .5f},
            new object[]{-10f, true, 1f},
            new object[]{10f, true, 0f},

            new object[]{0f, false, 0f},
            new object[]{1f, false, -1f},
            new object[]{.5f, false, -.5f},
            new object[]{-10f, false, 0f},
            new object[]{10f, false, -1f},

        };
    }
    public class TestAlphaActivatorUIEActivationProcess: AlphaActivatorUIEActivationProcess{
        public TestAlphaActivatorUIEActivationProcess(
            IProcessManager processManager, 
            float expireT, 
            IUIEActivationStateEngine engine,
            IUIElement uiElement,
            bool doesActivate
        ): base(
            processManager, 
            expireT, 
            engine,
            uiElement,
            doesActivate
        ){}
        
        public float TestGetLatestInitialValueDifference(){
            return this.GetLatestInitialValueDifference();
        }
    }
    public TestAlphaActivatorUIEActivationProcess CreateTestAlphaActivatorUIEActivationProcess(out ITestAlphaActivatorUIEActivationProcessConstArg arg, float expireT, bool doesActivate){
        ITestAlphaActivatorUIEActivationProcessConstArg thisArg = Substitute.For<ITestAlphaActivatorUIEActivationProcessConstArg>();
        thisArg.processManager.Returns(Substitute.For<IProcessManager>());
        thisArg.expireT.Returns(expireT);
        thisArg.engine.Returns(Substitute.For<IUIEActivationStateEngine>());
        thisArg.uiElement.Returns(Substitute.For<IUIElement>());
        thisArg.doesActivate.Returns(doesActivate);
        arg = thisArg;
        return new TestAlphaActivatorUIEActivationProcess(
            thisArg.processManager, 
            thisArg.expireT, 
            thisArg.engine,
            thisArg.uiElement,
            thisArg.doesActivate
        );
    }
    public interface ITestAlphaActivatorUIEActivationProcessConstArg{
        IProcessManager processManager{get;}
        float expireT{get;}
        IUIEActivationStateEngine engine{get;}
        IUIElement uiElement{get;}
        bool doesActivate{get;}
    }
}
