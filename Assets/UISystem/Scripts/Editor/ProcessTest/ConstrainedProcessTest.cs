using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;

[TestFixture]
public class ConstrainedProcessTest {
	[Test][TestCaseSource(typeof(Run_TestCases), "variousCases")]
	public void Run_ValueDiffBigEnough_ConstraintVarious_SetsConstraintValuesAccordingly(ProcessConstraint processConstraint, float constraintValue, float expectedExpireT, float expectedRateOfChange){
		TestConstrainedProcess testProcess = CreateTestConstrainedProcess(true, processConstraint, constraintValue);
		
		testProcess.Run();

		Assert.That(testProcess.GetExpireT(), Is.EqualTo(expectedExpireT));
		Assert.That(testProcess.GetRateOfChange(), Is.EqualTo(expectedRateOfChange));
	}
	public class Run_TestCases{
		public static object[] variousCases = new object[]{
			new object[]{
				ProcessConstraint.none, 1f, 0f, 0f
			},
			new object[]{
				ProcessConstraint.none, .5f, 0f, 0f
			},
			new object[]{
				ProcessConstraint.none, -.5f, 0f, 0f
			},
			new object[]{
				ProcessConstraint.expireTime, 1f, 1f, 0f
			},
			new object[]{
				ProcessConstraint.expireTime, -1f, 1f, 0f
			},
			new object[]{
				ProcessConstraint.expireTime, 100f, 100f, 0f
			},
			new object[]{/* normalizedValDif = 1f */
				ProcessConstraint.rateOfChange, 1f, 1f, 1f
			},
			new object[]{
				ProcessConstraint.rateOfChange, .5f, 2f, .5f
			},
			new object[]{
				ProcessConstraint.rateOfChange, 4f, .25f, 4f
			},
			new object[]{
				ProcessConstraint.rateOfChange, -.1f, 10f, -.1f
			}
		};
	}
	[Test]
	public void Run_ValueDiffIsBigEnough_CallsProcessStateOnProcessUpdateWithZeroF(){
		IWaitAndExpireProcessState processState;
		TestConstrainedProcess testProcess = CreateTestConstrainedProcess(1f, out processState);

		testProcess.Run();

		processState.Received().OnProcessUpdate(0f);
	}
	[Test][TestCaseSource(typeof(ValueDifferenceIsBigEnough_TestCases), "variousCases")]
	public void ValueDifferenceIsBigEnough_Various_ReturnsAccordingly(float normalizedValueDiff, float diffThreshold, bool expectedBool){
		TestConstrainedProcess testProcess = CreateTestConstrainedProcess(normalizedValueDiff, diffThreshold);

		Assert.That(testProcess.TestValueDifferenceisBigEnough(), Is.EqualTo(expectedBool));
	}
	public class ValueDifferenceIsBigEnough_TestCases{
		public static object[] variousCases = {
			new object[]{0f, .05f, false},/* zero, over thresh */
			new object[]{.1f, .05f, true},/* positive, over thresh */
			new object[]{.1f, .1f, false},/* positive, equal thresh */
			new object[]{.1f, .15f, false},/* positive, under thresh */
			new object[]{-.1f, .05f, true},/* negative, over thresh */
			new object[]{-.1f, .1f, false},/* negative, equal thresh */
			new object[]{-.1f, .15f, false},/* negative, under thresh */
		};
	}
	[Test][ExpectedException(typeof(System.ArgumentException))][TestCaseSource(typeof(Construction_TestCases), "diffThreshUnderCases")]
	public void Construction_DiffThresholdIsLessThanZero_ThrowsException(float diffThreshold){
		IProcessManager processManager = Substitute.For<IProcessManager>();
		TestConstrainedProcess testProcess = new TestConstrainedProcess(processManager, ProcessConstraint.none, 0f, null, diffThreshold, 0f);
	}
	[Test]
	public void Construction_DiffThresholdZero_DoesNotThrowException(){
		float diffThreshold = 0f;
		IProcessManager processManager = Substitute.For<IProcessManager>();
		Assert.DoesNotThrow(() =>{ new TestConstrainedProcess(processManager, ProcessConstraint.none, 0f, null, diffThreshold, 0f);
		});
	}
	[Test]
	public void Construction_ProcessManagerNull_ThrowsException(){
		Assert.Throws(Is.TypeOf(typeof(System.ArgumentException)).And.Message.StringContaining("process never works without processManager"), () => {
			new TestConstrainedProcess(null, ProcessConstraint.none, 0f, null, 0f, 0f);
		});
	}
	public class Construction_TestCases{
		public static object[] diffThreshUnderCases = {
			new object[]{- .1f},
			new object[]{- 1f},
			new object[]{- 10f}
		};
	}
	[Test][TestCaseSource(typeof(UpdateProcess_TestCases), "cases")]
	public void UpdateProcess_ProcessConstraintNotNone_DeltaTimeVarious_CallsProcessStateOnProcessExpire(float expireT){
		IWaitAndExpireProcessState processState;
		IProcessManager processManager;
		TestConstrainedProcess testProcess = CreateTestConstrainedProcess(expireT, out processState, out processManager);

		testProcess.Run();
		processState.Received().OnProcessUpdate(0f);

		float deltaT = .1f;
		for(float f = deltaT; f < expireT; f += deltaT){
			float elapsedT = f;
			testProcess.UpdateProcess(deltaT);
			processState.Received().OnProcessUpdate(deltaT);
			processManager.DidNotReceive().RemoveRunningProcess(testProcess);
		}
		testProcess.UpdateProcess(deltaT);
		processState.Received(1).OnProcessExpire();
	}
	public class UpdateProcess_TestCases{
		public static object[] cases = {
			new object[]{.05f},
			new object[]{.1f},
			new object[]{1f}
		};
	}
	/* Test Classes */
	public class TestConstrainedProcess: AbsConstrainedProcess{
		public TestConstrainedProcess(IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, IWaitAndExpireProcessState processState, float diffThreshold, float normalizedValueDiff): base(processManager, processConstraint, constraintValue, processState, diffThreshold){
			thisLatestInitialValueDifference = normalizedValueDiff;
		}
		protected override void UpdateProcessImple(float deltaT){}
		readonly float thisLatestInitialValueDifference;
		protected override float GetLatestInitialValueDifference(){
			return thisLatestInitialValueDifference;
		}
		protected override void SetTerminalValue(){
			return;
		}
		public bool TestValueDifferenceisBigEnough(){
			return ValueDifferenceIsBigEnough();
		}
		public float GetRateOfChange(){
			return thisRateOfChange;
		}
		public float GetExpireT(){
			return thisExpireT;
		}
	}
	public TestConstrainedProcess CreateTestConstrainedProcess(float normalizedValueDiff, float diffThreshold){
		IProcessManager processManager = Substitute.For<IProcessManager>();
		TestConstrainedProcess testProcess = new TestConstrainedProcess(processManager, ProcessConstraint.none, 0f, null, diffThreshold, normalizedValueDiff);
		return testProcess;
	}
	public TestConstrainedProcess CreateTestConstrainedProcess(bool valueDiffIsBigEnough, ProcessConstraint processConstraint, float constraintValue){
		IProcessManager processManager = Substitute.For<IProcessManager>();
		TestConstrainedProcess testProcess = new TestConstrainedProcess(processManager, processConstraint, constraintValue, null, .5f, valueDiffIsBigEnough? 1f: 0f);
		return testProcess;
	}
	public TestConstrainedProcess CreateTestConstrainedProcess(float expireT, out IWaitAndExpireProcessState processState){
		IProcessManager processManager;
		IWaitAndExpireProcessState procSta;
		TestConstrainedProcess testProcess = CreateTestConstrainedProcess(expireT, out procSta, out processManager);
		processState = procSta;
		return testProcess;
	}
	public TestConstrainedProcess CreateTestConstrainedProcess(float expireT, out IWaitAndExpireProcessState processState, out IProcessManager processManager){
		IProcessManager procMan = Substitute.For<IProcessManager>();
		IWaitAndExpireProcessState procSta = Substitute.For<IWaitAndExpireProcessState>();
		TestConstrainedProcess testProcess = new TestConstrainedProcess(procMan, ProcessConstraint.expireTime, expireT, procSta, .5f, 1f);
		processState = procSta;
		processManager = procMan;
		return testProcess;
	}
}
