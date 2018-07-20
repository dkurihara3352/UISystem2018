using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;

[TestFixture]
public class InterpolatorProcessTest{
    [Test]
    public void Run_ValueDifferenceIsBigEnough_CallsIrperInterpolateWithZero(){
        IProcessManager processManager = Substitute.For<IProcessManager>();
        TestInterpolatorProcess testProcess = new TestInterpolatorProcess(processManager, ProcessConstraint.expireTime, 1f, .1f, .5f, false, null);

        testProcess.Run();

        IInterpolator irper = testProcess.GetInterpolator();
        irper.Received(1).Interpolate(0f);
    }
    [Test][TestCaseSource(typeof(UpdateProcess_TestCases), "cases")]
    public void UpdateProcess_ValueDiffIsBigEnough_CallsIrperInterpolate(float expireT){
        IProcessManager processManager = Substitute.For<IProcessManager>();
        TestInterpolatorProcess testProcess = new TestInterpolatorProcess(processManager, ProcessConstraint.expireTime, expireT, .1f, .5f, false, null);

        testProcess.Run();
        IInterpolator irper = testProcess.GetInterpolator();
        irper.Received(1).Interpolate(0f);

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
        public TestInterpolatorProcess(IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, float diffThreshold, float normalizedValueDiff, bool useSpringT, IWaitAndExpireProcessState state): base(processManager, processConstraint, constraintValue, diffThreshold, useSpringT, state){
            thisLatestInitialValueDifference = normalizedValueDiff;
        }
        protected override IInterpolator InstantiateInterpolatorWithValues(){
            IInterpolator irper = Substitute.For<IInterpolator>();
            return irper;
        }
        public IInterpolator GetInterpolator(){
            return thisInterpolator;
        }
        protected override float GetLatestInitialValueDifference(){
            return thisLatestInitialValueDifference;
        }
        readonly float thisLatestInitialValueDifference;
    }
}
