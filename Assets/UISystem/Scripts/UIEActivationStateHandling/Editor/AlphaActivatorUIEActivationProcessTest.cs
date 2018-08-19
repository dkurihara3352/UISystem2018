﻿using System.Collections;
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
        ITestAlphaActivatorUIEActivationProcessConstArg arg = CreateMockConstArg();
        arg.doesActivate.Returns(doesActivate);
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
        uia.GetGroupAlpha().Returns(currentGroupAlpha);
        arg.uiElement.GetUIAdaptor().Returns(uia);
        TestAlphaActivatorUIEActivationProcess process = new TestAlphaActivatorUIEActivationProcess(arg);
        

        float actual = process.GetLatestInitialValueDifference_Test();

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
            ITestAlphaActivatorUIEActivationProcessConstArg arg
        ): base(
            arg.processManager, 
            arg.expireT, 
            arg.engine,
            arg.uiElement,
            arg.doesActivate
        ){}
        
        public float GetLatestInitialValueDifference_Test(){
            return this.GetLatestInitialValueDifference();
        }
    }
    public interface ITestAlphaActivatorUIEActivationProcessConstArg{
        IProcessManager processManager{get;}
        float expireT{get;}
        IUIEActivationStateEngine engine{get;}
        IUIElement uiElement{get;}
        bool doesActivate{get;}
    }
    public ITestAlphaActivatorUIEActivationProcessConstArg CreateMockConstArg(){
        ITestAlphaActivatorUIEActivationProcessConstArg arg = Substitute.For<ITestAlphaActivatorUIEActivationProcessConstArg>();
        arg.processManager.Returns(Substitute.For<IProcessManager>());
        arg.expireT.Returns(1f);
        arg.engine.Returns(Substitute.For<IUIEActivationStateEngine>());
        arg.uiElement.Returns(Substitute.For<IUIElement>());
        arg.doesActivate.Returns(true);
        return arg;
    }
}
