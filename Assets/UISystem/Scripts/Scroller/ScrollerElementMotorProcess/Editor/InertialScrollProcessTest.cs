using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class InertialScrollProcessTest {
    [Test, TestCaseSource(typeof(Construction_DecelerationNotGreaterThanZero_TestCase), "cases")]
    public void Construction_DecelerationNotGreaterThanZero_ThrowsException(float deceleration){
        ITestInertialScrollProcessConstArg arg = CreateMockConstArg();
        arg.deceleration.Returns(deceleration);
        
        Assert.Throws(
            Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("deceleration must be greater than zero"),
            () => {
                new TestInertialScrollProcess(arg);
            }
        );
    }
    public class Construction_DecelerationNotGreaterThanZero_TestCase{
        public static object[] cases = {
            new object[]{0f},
            new object[]{-1f},
        };
    }
    [Test, TestCaseSource(typeof(Construction_DecelAxisCompMultiplierLessThanZero_TestCase), "cases")]
    public void Construction_DecelAxisCompMultiplierLessThanZero_ThrowsException(float decelAxisCompMultiplier){
        ITestInertialScrollProcessConstArg arg = CreateMockConstArg();
        arg.decelerationAxisComponentMultiplier.Returns(decelAxisCompMultiplier);
        
        Assert.Throws(
            Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("deceleration axis component multiplier must not be less than zero"),
            () => {
                new TestInertialScrollProcess(arg);
            }
        );
    }
    public class Construction_DecelAxisCompMultiplierLessThanZero_TestCase{
        public static object[] cases = {
            new object[]{-1f},
            new object[]{-.02f},
        };
    }
    [Test, TestCaseSource(typeof(Construction_MultiplierNotLessThanZero_SetsExpireTime_TestCase), "cases")]
    public void Construction_MultiplierNotLessThanZero_SetsExpireTime(float initialDeltaPosOnAxis, float deceleration, float decelerationAxisComponentMultiplier, float expected){
        ITestInertialScrollProcessConstArg arg = CreateMockConstArg();
        arg.deceleration.Returns(deceleration);
        arg.initialDeltaPosOnAxis.Returns(initialDeltaPosOnAxis);
        arg.decelerationAxisComponentMultiplier.Returns(decelerationAxisComponentMultiplier);
        TestInertialScrollProcess process = new TestInertialScrollProcess(arg);

        float actual = process.thisExpireTime_Test;

        Assert.That(actual, Is.EqualTo(expected));
    }
    public class Construction_MultiplierNotLessThanZero_SetsExpireTime_TestCase{
        public static object[] cases = {
            new object[]{12f, 3f, .5f, 8f},
            new object[]{-12f, 3f, .5f, 8f},
            new object[]{-12f, 3f, 0f, 0f},
            new object[]{0f, 3f, 0f, 0f},
        };
    }
    [Test, TestCaseSource(typeof(UpadateProcess_Demo_TestCase), "cases"), Ignore]
    public void UpadateProcess_Demo(float initDeltaPosOnAxis, float deceleration, int dimension, Vector2 elementInitLocalPos){
        ITestInertialScrollProcessConstArg arg = CreateMockConstArg();
        arg.processManager.GetInertialScrollDeceleration().Returns(deceleration);
        arg.initialDeltaPosOnAxis.Returns(initDeltaPosOnAxis);
        arg.dimension.Returns(dimension);
        IUIElement scrollerElement = Substitute.For<IUIElement>();
        scrollerElement.GetLocalPosition().Returns(elementInitLocalPos);
        arg.scrollerElement.Returns(scrollerElement);
        TestInertialScrollProcess process = new TestInertialScrollProcess(arg);

        bool done = false;
        DebugHelper.PrintInRed("initVel: " + initDeltaPosOnAxis.ToString() + ", deceleration: " + deceleration.ToString());
        float deltaT = .1f;
        Vector2 localPos = elementInitLocalPos;
        scrollerElement.SetLocalPosition(Arg.Do<Vector2>(x => localPos = x));
        while(!done){
            process.UpdateProcess(deltaT);
            
            Debug.Log("elementLocalPos: " + localPos.ToString() + ", velocity: " + process.thisPrevVelocity_Test.ToString());
            if(process.thisElapsedTime_Test >= process.thisExpireTime_Test)
                done = true;
        }
    }
    public class UpadateProcess_Demo_TestCase{
        public static object[] cases = {
            new object[]{10f, 10f, 0, new Vector2(15f, 15f)},
            new object[]{-10f, 10f, 0, new Vector2(15f, 15f)},
        };
    }







    public class TestInertialScrollProcess: InertialScrollProcess{
        public TestInertialScrollProcess(ITestInertialScrollProcessConstArg arg): base(arg.initialDeltaPosOnAxis, arg.deceleration, arg.decelerationAxisComponentMultiplier, arg.scroller, arg.scrollerElement, arg.dimension, arg.processManager){}
        public float thisDeceleration_Test{get{return thisDeceleration;}}
        public float thisExpireTime_Test{get{return thisExpireTime;}}
        public float thisElapsedTime_Test{get{return thisElapsedTime;}}
        public float thisPrevVelocity_Test{get{return thisPrevVelocity;}}
    }
    public interface ITestInertialScrollProcessConstArg{
        float initialDeltaPosOnAxis{get;}
        float deceleration{get;}
        float decelerationAxisComponentMultiplier{get;}
        IScroller scroller{get;}
        IUIElement scrollerElement{get;}
        int dimension{get;}
        IProcessManager processManager{get;}
    }
    public ITestInertialScrollProcessConstArg CreateMockConstArg(){
        ITestInertialScrollProcessConstArg arg = Substitute.For<ITestInertialScrollProcessConstArg>();
        arg.initialDeltaPosOnAxis.Returns(0f);
        arg.deceleration.Returns(.1f);
        arg.decelerationAxisComponentMultiplier.Returns(.5f);
        arg.scroller.Returns(Substitute.For<IScroller>());
        arg.scrollerElement.Returns(Substitute.For<IUIElement>());
        arg.dimension.Returns(0);
        IProcessManager processManager = Substitute.For<IProcessManager>();
        arg.processManager.Returns(processManager);

        return arg;
    }
}   
