using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ScrollerElementSnapProcessTest {
    [Test, TestCaseSource(typeof(Consruction_DiffThresholdIsEqualOrLessThanZero_TestCase), "cases")]
    public void Consruction_DiffThresholdIsEqualOrLessThanZero_ThrowsException(float diffThreshold){
        ITestScrollerElementSnapProcessConstArg arg = CreateMockConstArg();
        arg.diffThreshold.Returns(diffThreshold);

        Assert.Throws(
            Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("source threshold must be greater than 0"), 
            () => {
                new TestScrollerElementSnapProcess(arg);
            }
        );
    }
    public class Consruction_DiffThresholdIsEqualOrLessThanZero_TestCase{
        public static object[] cases = {
            new object[]{0f},
            new object[]{-.1f},
            new object[]{-100f},
        };
    }
    [Test, TestCaseSource(typeof(Construction_ValueDifferenceNotGreaterThanThreshold_TestCase), "cases")]
    public void Construction_ValueDifferenceNotGreaterThanThreshold_CallsScrollerSetScrollerElementLocalPosOnAxis(Vector2 initialLocalPos, float targetLocalPosOnAxis, int dimension, float diffThreshold, bool callExpected, Vector2 expectedLocalPos){
        ITestScrollerElementSnapProcessConstArg arg = CreateMockConstArg();
        IScroller scroller = Substitute.For<IScroller>();
        arg.scroller.Returns(scroller);
        IUIElement scrollerElement = Substitute.For<IUIElement>();
        arg.scrollerElement.Returns(scrollerElement);
        scrollerElement.GetLocalPosition().Returns(initialLocalPos);
        arg.diffThreshold.Returns(diffThreshold);
        arg.dimension.Returns(dimension);
        arg.targetElementLocalPositionOnAxis.Returns(targetLocalPosOnAxis);
        TestScrollerElementSnapProcess process = new TestScrollerElementSnapProcess(arg);

        if(callExpected)
            scroller.Received(1).SetScrollerElementLocalPosOnAxis(expectedLocalPos[dimension], dimension);
        else
            scroller.DidNotReceive().SetScrollerElementLocalPosOnAxis(expectedLocalPos[dimension], dimension);
    }
    public class Construction_ValueDifferenceNotGreaterThanThreshold_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(10f, 10f), 20f, 0, 10f, true, new Vector2(20f, 10f)},
            new object[]{new Vector2(10f, 10f), 20f, 1, 10f, true, new Vector2(10f, 20f)},
            new object[]{new Vector2(10f, 10f), 20f, 0, 9f, true, new Vector2(20f, 10f)},
            new object[]{new Vector2(10f, 10f), 20f, 1, 9f, true, new Vector2(10f, 20f)},
            new object[]{new Vector2(10f, 10f), 20f, 0, 11f, false, Vector2.zero},
        };
    }
    [Test, TestCaseSource(typeof(GetDeltaValue_TestCase), "cases")]
    public void GetDeltaValue_Various(float newValue, float deltaT, float expected){
        ITestScrollerElementSnapProcessConstArg arg = CreateMockConstArg();
        TestScrollerElementSnapProcess process = new TestScrollerElementSnapProcess(arg);

        float actual = process.GetDeltaValue_Test(newValue, deltaT);

        Assert.That(actual, Is.EqualTo(expected));

    }
    public class GetDeltaValue_TestCase{
        public static object[] cases = {
            new object[]{10f, .1f, 100f},
            new object[]{10f, 1f, 10f},
            new object[]{10f, 10f, 1f},
            
            new object[]{-10f, .1f, 100f},
            new object[]{-10f, 1f, 10f},
            new object[]{-10f, 10f, 1f},
        };
    }
    [Test, TestCaseSource(typeof(UpdateProcess_Demo_TestCase), "cases"), Ignore]
    public void UpdateProcess_Demo(Vector2 initialLocalPos, float targetLocalPosOnAxis, float initialVelOnAxis, int dimension, float diffThreshold, float stopVelocity){
        ITestScrollerElementSnapProcessConstArg arg = CreateMockConstArg();
        IUIElement scrollerElement = Substitute.For<IUIElement>();
        arg.scrollerElement.Returns(scrollerElement);
        scrollerElement.GetLocalPosition().Returns(initialLocalPos);
        arg.diffThreshold.Returns(diffThreshold);
        arg.dimension.Returns(dimension);
        arg.targetElementLocalPositionOnAxis.Returns(targetLocalPosOnAxis);
        arg.processManager.GetScrollerElementSnapSpringCoefficient().Returns(5f);
        arg.initialVelOnAxis.Returns(initialVelOnAxis);
        arg.stopVelocity.Returns(stopVelocity);
        TestScrollerElementSnapProcess process = new TestScrollerElementSnapProcess(arg);

        float deltaT = .02f;
        bool done = false;
        Vector2 passedLocalPos = Vector2.zero;
        scrollerElement.SetLocalPosition(Arg.Do<Vector2>(x => passedLocalPos = x));

        float prevLocalPosOnAxis = initialLocalPos[dimension];
        
        DKUtility.DebugHelper.PrintInRed("initPos: " + initialLocalPos.ToString() + ", targetPosOnAxis: " + targetLocalPosOnAxis.ToString() + ", initVel: " + initialVelOnAxis.ToString()+ ", dimension: " + dimension.ToString());
        
        float delta;

        while(!done){
            process.UpdateProcess(deltaT);
            delta = Mathf.Abs(passedLocalPos[dimension] - prevLocalPosOnAxis) /deltaT;
            Debug.Log("scrollerElementLocalPos: " + passedLocalPos.ToString() + ", delta: " + delta.ToString());

            if(delta <= stopVelocity)
                done = true;
            
            prevLocalPosOnAxis = passedLocalPos[dimension];

        }
    }
    public class UpdateProcess_Demo_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(0f, 0f), 20f, 0f, 0, .1f, .5f},
            new object[]{new Vector2(0f, 0f), 20f, 10f, 0, .1f, .5f},
            new object[]{new Vector2(0f, 0f), 20f, 100f, 0, .1f, .5f},
            new object[]{new Vector2(0f, 0f), 20f, -100f, 0, .1f, .5f},
        };
    }








    public class TestScrollerElementSnapProcess: ScrollerElementSnapProcess{
        public TestScrollerElementSnapProcess(ITestScrollerElementSnapProcessConstArg arg): base(arg.targetElementLocalPositionOnAxis, arg.initialVelOnAxis, arg.scroller, arg.scrollerElement, arg.dimension, arg.diffThreshold, arg.stopVelocity, arg.processManager){}
        public float GetDeltaValue_Test(float newValue, float deltaT){
            return 0f;
        }
    }
    public interface ITestScrollerElementSnapProcessConstArg{
        float targetElementLocalPositionOnAxis{get;}
        float initialVelOnAxis{get;}
        IScroller scroller{get;}
        IUIElement scrollerElement{get;}
        int dimension{get;}
        float diffThreshold{get;}
        float stopVelocity{get;}
        IProcessManager processManager{get;}
    }
    ITestScrollerElementSnapProcessConstArg CreateMockConstArg(){
        ITestScrollerElementSnapProcessConstArg arg = Substitute.For<ITestScrollerElementSnapProcessConstArg>();
        arg.targetElementLocalPositionOnAxis.Returns(0f);
        arg.initialVelOnAxis.Returns(0f);
        arg.scroller.Returns(Substitute.For<IScroller>());
        arg.scrollerElement.Returns(Substitute.For<IUIElement>());
        arg.dimension.Returns(0);
        arg.diffThreshold.Returns(.1f);
        arg.stopVelocity.Returns(0.05f);
        arg.processManager.Returns(Substitute.For<IProcessManager>());
        return arg;
    }
}
