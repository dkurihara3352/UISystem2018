using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility;

[TestFixture, Ignore]
public class CalculatorTest {
	[Test, TestCaseSource(typeof(CalcSineAndCosine_TestCase), "cases"), Ignore]
	public void CalcSineAndCosine_Demo(Vector2 deltaPos){

		float sine;
		float cosine;
		Calculator.CalcSineAndCosine(deltaPos, out sine, out cosine);

		Debug.Log("deltaPos: " + deltaPos.ToString() + ", sine: " + sine.ToString() + ", cosine: " + cosine.ToString());
	}
	public class CalcSineAndCosine_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(1f, 0f)},
			new object[]{new Vector2(1f, 1f)},
			new object[]{new Vector2(0f, 1f)},
			new object[]{new Vector2(-1f, 1f)},
			new object[]{new Vector2(-1f, 0f)},
			new object[]{new Vector2(-1f, -1f)},
			new object[]{new Vector2(0f, -1f)},
			new object[]{new Vector2(1f, -1f)},
			
			new object[]{new Vector2(10f, 0f)},
			new object[]{new Vector2(10f, 10f)},
			new object[]{new Vector2(0f, 10f)},
			new object[]{new Vector2(-10f, 10f)},
			new object[]{new Vector2(-10f, 0f)},
			new object[]{new Vector2(-10f, -10f)},
			new object[]{new Vector2(0f, -10f)},
			new object[]{new Vector2(10f, -10f)},
		};
	}
	
}
