using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;
using UISystem;

[TestFixture, Category("UISystem")]
public class PopUpProcessTest {
    [Test, TestCaseSource(typeof(GetLatestInitialValueDifference_TestCase), "cases")]
    public void GetLatestInitialValueDifference_ReturnsVarious(
        float curGroupAlpha, 
        bool hides, 
        float expected
    ){
        IAlphaPopUpProcessConstArg arg = CreateMockArg();
            arg.hides.Returns(hides);
            IUIAdaptor uia = Substitute.For<IUIAdaptor>();
                uia.GetGroupAlpha().Returns(curGroupAlpha);
            arg.popUp.GetUIAdaptor().Returns(uia);
        TestAlphaPopUpProcess process = new TestAlphaPopUpProcess(arg);

        float actual = process.GetLatestInitialValueDifference_Test();

        Assert.That(actual, Is.EqualTo(expected));
    }
    public class GetLatestInitialValueDifference_TestCase{
        public static object[] cases = {
            /* showing */
            new object[]{
                0f,
                false,
                1f
            },
            new object[]{
                .5f,
                false,
                .5f
            },
            new object[]{
                .2f,
                false,
                .8f
            },
            new object[]{
                1f,
                false,
                0f
            },
            /* hiding */
            new object[]{
                1f,
                true, 
                -1f,
            },
            new object[]{
                .5f,
                true, 
                -.5f,
            },
            new object[]{
                .2f,
                true, 
                -.2f,
            },
            new object[]{
                0f,
                true, 
                0f,
            },
        };
    }
    [Test]
    public void Expire_Hides_CallsEngineSwitchToHiddenState(){
        IAlphaPopUpProcessConstArg arg = CreateMockArg();
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
            uia.GetGroupAlpha().Returns(1f);//required not to early break
        arg.popUp.GetUIAdaptor().Returns(uia);
        arg.hides.Returns(true);
        TestAlphaPopUpProcess process = new TestAlphaPopUpProcess(arg);
        process.Run();//required for interpolator set up

        process.Expire();

        arg.engine.Received(1).SwitchToHiddenState();
    }
    [Test]
    public void Expire_NotHides_CallsEngineSwitchToShownState(){
        IAlphaPopUpProcessConstArg arg = CreateMockArg();
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
            uia.GetGroupAlpha().Returns(0f);//required not to early break
        arg.popUp.GetUIAdaptor().Returns(uia);
        arg.hides.Returns(false);
        TestAlphaPopUpProcess process = new TestAlphaPopUpProcess(arg);
        process.Run();//required for interpolator set up

        process.Expire();

        arg.engine.Received(1).SwitchToShownState();
    }

    public class TestAlphaPopUpProcess: AlphaPopUpProcess{
        public TestAlphaPopUpProcess(
            IAlphaPopUpProcessConstArg arg
        ): base(
            arg
        ){}
        public float GetLatestInitialValueDifference_Test(){
            return this.GetLatestInitialValueDifference();
        }
    }
    
    public IAlphaPopUpProcessConstArg CreateMockArg(){
        IAlphaPopUpProcessConstArg arg = Substitute.For<IAlphaPopUpProcessConstArg>();
        arg.processManager.Returns(Substitute.For<IProcessManager>());
        arg.processConstraint.Returns(ProcessConstraint.ExpireTime);
        arg.constraintValue.Returns(1f);
        arg.popUp.Returns(Substitute.For<IPopUp>());
        arg.engine.Returns(Substitute.For<IPopUpStateEngine>());
        arg.hides.Returns(false);
        return arg;
    }
}
