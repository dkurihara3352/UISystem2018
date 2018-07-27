using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility.CurveUtility;
using DKUtility;

[TestFixture]
public class RubberBandCalculatorTest {
	[Test, TestCaseSource(typeof(CalcRubberBandValue_Demo_TestCase), "cases"), Ignore]
	public void CalcRubberBandValue_Demo(float suppleness, float limitLength){
		IRubberBandCalculator calculator = new RubberBandCalculator(suppleness, limitLength);
		float origDisp = 0f;
		int stepSize = Mathf.RoundToInt(limitLength);
		float prev = 0f;
		float diffThreshold = 0.5f;
		float rubberBandedValue = 0f;
		int iteration = 0;
		DebugHelper.PrintInRed("suppleness: " + suppleness.ToString() + ", limitLength: " + limitLength.ToString());
		while(rubberBandedValue + diffThreshold < limitLength){
			rubberBandedValue = calculator.CalcRubberBandValue(origDisp, false);
			float delta = rubberBandedValue - prev;
			Debug.Log("iteration: " + iteration.ToString() + ", origDisp: " + origDisp.ToString() + ", rubberValue: " + rubberBandedValue.ToString() + ", delta: " + delta.ToString());
			prev = rubberBandedValue;
			origDisp += stepSize;
			iteration ++;
		}
	}
	public class CalcRubberBandValue_Demo_TestCase{
		public static object[] cases = {
			new object[]{1f, 10f},
			new object[]{1f, 5f},
			new object[]{10f, 10f},
			new object[]{.5f, 10f},
		};
	}
}
