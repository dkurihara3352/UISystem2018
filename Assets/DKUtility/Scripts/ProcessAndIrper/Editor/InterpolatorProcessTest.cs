using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;

[TestFixture][Category("DKUtility")]
public class InterpolatorProcessTest{
    [Test]
    public void Run_CallsIrperInterpolateWithZero(){
        IInterpolatorProcesssConstArg arg = CreateMockArg();
        TestInterpolatorProcess testProcess = new TestInterpolatorProcess(arg);

        testProcess.Run();

        IInterpolator irper = testProcess.GetInterpolator();
        irper.Received(1).Interpolate(0f);
    }
    [Test][TestCaseSource(typeof(UpdateProcess_TestCases), "cases")]
    public void UpdateProcess_CallsIrperInterpolate(float expireT){
        IInterpolatorProcesssConstArg arg = CreateMockArg();
        arg.constraintValue.Returns(expireT);
        TestInterpolatorProcess testProcess = new TestInterpolatorProcess(arg);

        testProcess.Run();
        IInterpolator irper = testProcess.GetInterpolator();
        irper.Received(1).Interpolate(0f);

        IProcessManager processManager = arg.processManager;
        const float deltaT = .1f;
        for(float f = deltaT; f < expireT; f += deltaT){
            float elapsedT = f;
            testProcess.UpdateProcess(deltaT);
            irper.Received(1).Interpolate(elapsedT/ expireT);
            processManager.DidNotReceive().RemoveRunningProcess(testProcess);
        }

        testProcess.UpdateProcess(deltaT);

        irper.Received(1).Interpolate(1f);
        processManager.Received(1).RemoveRunningProcess(testProcess);
    }
    public bool ValueAlmostOne(float value){
        return value <= 1.0001f && value >= .999f;
    }
    public class UpdateProcess_TestCases{
        public static object[] cases = {
            new object[]{ 1f},
            new object[]{ .1f},
            new object[]{ .05f},
            new object[]{ 10f}
        };
    }
    public class TestInterpolatorProcess: AbsInterpolatorProcess<IInterpolator>{
        public TestInterpolatorProcess(
            IInterpolatorProcesssConstArg arg
        ): base(
            arg    
        ){
        }
        protected override IInterpolator InstantiateInterpolatorWithValues(){
            IInterpolator irper = Substitute.For<IInterpolator>();
            return irper;
        }
        public IInterpolator GetInterpolator(){
            return thisInterpolator;
        }
        protected override float GetLatestInitialValueDifference(){return 0f;}
    }
    public IInterpolatorProcesssConstArg CreateMockArg(){
        IInterpolatorProcesssConstArg arg = Substitute.For<IInterpolatorProcesssConstArg>();
        arg.processManager.Returns(Substitute.For<IProcessManager>());
        arg.processConstraint.Returns(ProcessConstraint.ExpireTime);
        arg.constraintValue.Returns(1f);
        arg.useSpringT.Returns(false);
        return arg;
    }
}
