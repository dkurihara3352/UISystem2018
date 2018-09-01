using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;

[TestFixture][Category("DKUtility")]
public class ConstrainedProcessTest {
	[Test][TestCaseSource(typeof(Run_TestCases), "variousCases")]
	public void Run_ValueDiffBigEnough_ConstraintVarious_SetsConstraintValuesAccordingly(
		ProcessConstraint processConstraint, 
		float constraintValue, 
		float expectedExpireT, 
		float expectedRateOfChange
	){
		IConstrainedProcessConstArg arg = CreateMockArg();
		arg.processConstraint.Returns(processConstraint);
		arg.constraintValue.Returns(constraintValue);
		TestConstrainedProcess testProcess = new TestConstrainedProcess(arg);
		testProcess.SetLatestInitialValueDifference_Test(1f);
		
		testProcess.Run();

		Assert.That(testProcess.GetExpireT(), Is.EqualTo(expectedExpireT));
		Assert.That(testProcess.GetRateOfChange_Test(), Is.EqualTo(expectedRateOfChange));
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
				ProcessConstraint.ExpireTime, 1f, 1f, 0f
			},
			new object[]{
				ProcessConstraint.ExpireTime, -1f, 1f, 0f
			},
			new object[]{
				ProcessConstraint.ExpireTime, 100f, 100f, 0f
			},
			new object[]{/* normalizedValDif = 1f */
				ProcessConstraint.RateOfChange, 1f, 1f, 1f
			},
			new object[]{
				ProcessConstraint.RateOfChange, .5f, 2f, .5f
			},
			new object[]{
				ProcessConstraint.RateOfChange, 4f, .25f, 4f
			},
			new object[]{
				ProcessConstraint.RateOfChange, -.1f, 10f, -.1f
			}
		};
	}
	/* Test Classes */
	public class TestConstrainedProcess: AbsConstrainedProcess{
		public TestConstrainedProcess(
			IConstrainedProcessConstArg arg
		): base(
			arg
		){}
		protected override void UpdateProcessImple(float deltaT){}
		float thisLatestInitialValueDifference;
		protected override float GetLatestInitialValueDifference(){
			return thisLatestInitialValueDifference;
		}
		public void SetLatestInitialValueDifference_Test(float value){
			thisLatestInitialValueDifference = value;
		}
		public float GetRateOfChange_Test(){
			return thisRateOfChange;
		}
		public float GetExpireT(){
			return thisExpireT;
		}
	}
	public IConstrainedProcessConstArg CreateMockArg(){
		IConstrainedProcessConstArg arg = Substitute.For<IConstrainedProcessConstArg>();
		arg.processManager.Returns(Substitute.For<IProcessManager>());
		arg.processConstraint.Returns(ProcessConstraint.ExpireTime);
		arg.constraintValue.Returns(1f);
		
		return arg;
	}
}
