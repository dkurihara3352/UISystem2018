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
        IAlphaActivatorUIElement activatorUIE = arg.alphaActivatorUIE;
        activatorUIE.GetNormalizedGroupAlphaForActivation().Returns(currentGroupAlpha);

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
        public TestAlphaActivatorUIEActivationProcess(IProcessManager processManager, float expireT, bool doesActivate, IUIEActivationProcessState state, IAlphaActivatorUIElement alphaActivatorUIElement): base(processManager, expireT, doesActivate, state, alphaActivatorUIElement){}
        
        public float TestGetLatestInitialValueDifference(){
            return this.GetLatestInitialValueDifference();
        }
    }
    public TestAlphaActivatorUIEActivationProcess CreateTestAlphaActivatorUIEActivationProcess(out ITestAlphaActivatorUIEActivationProcessConstArg arg, float expireT, bool doesActivate){
        ITestAlphaActivatorUIEActivationProcessConstArg thisArg = Substitute.For<ITestAlphaActivatorUIEActivationProcessConstArg>();
        thisArg.processManager.Returns(Substitute.For<IProcessManager>());
        thisArg.expireT.Returns(expireT);
        thisArg.doesActivate.Returns(doesActivate);
        thisArg.state.Returns(Substitute.For<IUIEActivationProcessState>());
        thisArg.alphaActivatorUIE.Returns(Substitute.For<IAlphaActivatorUIElement>());
        arg = thisArg;
        return new TestAlphaActivatorUIEActivationProcess(thisArg.processManager, thisArg.expireT, thisArg.doesActivate, thisArg.state, thisArg.alphaActivatorUIE);
    }
    public interface ITestAlphaActivatorUIEActivationProcessConstArg{
        IProcessManager processManager{get;}
        float expireT{get;}
        bool doesActivate{get;}
        IUIEActivationProcessState state{get;}
        IAlphaActivatorUIElement alphaActivatorUIE{get;}
    }
}
