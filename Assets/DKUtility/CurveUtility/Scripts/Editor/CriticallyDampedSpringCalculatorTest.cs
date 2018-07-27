using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using DKUtility.CurveUtility;
using DKUtility;

[TestFixture]
public class CriticallyDampedSpringCalculatorTest{
    [Test][TestCaseSource(typeof(NormalizedSpringValueCalculator_Demo_TestCases), "cases"), Ignore]
    public void NormalizedSpringValueCalculator_Demo(int resolution){
        NormalizedSpringValueCalculator calculator = new NormalizedSpringValueCalculator(resolution);
        DebugHelper.PrintInRed("Resolution: " + resolution.ToString());
        int steps = 20;
        float t = 0f;
        float prev = 0f;
        float springValue;
        float aproxValue;
        float delta;
        while(t < 1f){
            springValue = calculator.GetSpringValue(t);
            aproxValue = calculator.GetApproximateSpringValue(t);
            delta = springValue - prev;
            Debug.Log("t: " + t.ToString() + ", springValue: " + springValue.ToString() + ", aprox: " + aproxValue.ToString() + ", delta: " + delta.ToString());
            t += /* UnityEngine.Random.Range(.9f, 1.1f) *  */(1f/steps);
            prev = springValue;
        }
        springValue = calculator.GetSpringValue(t);
        aproxValue = calculator.GetApproximateSpringValue(t);
        delta = springValue - prev;
        Debug.Log("t: " + t.ToString() + ", springValue: " + springValue.ToString() + ", aprox: " + aproxValue.ToString() + ", delta: " + delta.ToString());
    }
    public class NormalizedSpringValueCalculator_Demo_TestCases{
        public static object[] cases = {
            new object[]{10},
            new object[]{50},
            new object[]{100}
        };
    }
    [Test][TestCaseSource(typeof(CriticallyDampedSpringValueCalculator_Demo_TestCases), "cases"), Ignore]
    public void CriticallyDampedSpringValueCalculator_Demo(float initVal, float termVal, float initVel, float coef, int resolution){
        FullCriticallyDampedSpringCalculator calculator = new FullCriticallyDampedSpringCalculator(initVal, termVal, initVel, coef, resolution);
        DebugHelper.PrintInRed("initVal: " + initVal.ToString() + ", termVal: " + termVal.ToString() + ", initVel: " + initVel.ToString() + ", coef: " + coef.ToString() + ", resolution: " + resolution.ToString());
        int steps = 15;
        float t = 0f;
        float prev = 0f;
        float springValue;
        float aproxValue;
        while(t < 1f){
            springValue = calculator.GetSpringValue(t);
            aproxValue = calculator.GetApproximateSpringValue(t);
            Debug.Log("t: " + t.ToString() + ", springValue: " + springValue.ToString() + ", aprox: " + aproxValue.ToString() + ", delta: " + (springValue - prev).ToString());
            prev = springValue;
            t += UnityEngine.Random.Range(.9f, 1.1f) * (1f/steps);
        }
            springValue = calculator.GetSpringValue(t);
            aproxValue = calculator.GetApproximateSpringValue(t);
            Debug.Log("t: " + t.ToString() + ", springValue: " + springValue.ToString() + ", aprox: " + aproxValue.ToString() + ", delta: " + (springValue - prev).ToString());
    }
    public class CriticallyDampedSpringValueCalculator_Demo_TestCases{
        public static object[] cases = {
            // new object[]{0f, 1f, 0f, 5f, 10},
            // new object[]{0f, 1f, 0f, 5f, 50},
            // new object[]{1f, 0f, 0f, 5f, 20},
            // new object[]{-1f, 0f, 0f, 5f, 20}
            // new object[]{-1f, -2f, 0f, 5f, 20}
            // new object[]{0f, 10f, 0f, 5f, 20},
            // new object[]{0f, 10f, 50f, 5f, 20},
            // new object[]{0f, 10f, 100f, 5f, 20},
            new object[]{0f, 10f, -100f, 5f, 20},
        };
    }
}
