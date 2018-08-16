using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ElementIsScrolledToIncreaseCursorOffsetCalculatorTest {
    [Test]
    public void Calculate_DeltaPosZero_ReturnsFalse(){
        IScroller scroller = Substitute.For<IScroller>();
        scroller.GetElementCursorOffsetInPixel(Arg.Any<float>(), Arg.Any<int>()).Returns(0f);
        ElementIsScrolledToIncreaseCursorOffsetCalculator calculator = new ElementIsScrolledToIncreaseCursorOffsetCalculator(scroller);
        float anyLocalPosOnAxis = 222222222f;
        
        for(int i = 0; i < 2; i ++){
            bool actual = calculator.Calculate(0f, anyLocalPosOnAxis, i);
            Assert.That(actual, Is.False);
        }
    }

	[Test]
    public void Calculate_DeltaPosNonZero_ElementCursorOfffsetPixelIsZero_ReturnsFalse([Values(-1f, 1f, 1000f)]float deltaPosOnAxis){
        IScroller scroller = Substitute.For<IScroller>();
        scroller.GetElementCursorOffsetInPixel(Arg.Any<float>(), Arg.Any<int>()).Returns(0f);
        ElementIsScrolledToIncreaseCursorOffsetCalculator calculator = new ElementIsScrolledToIncreaseCursorOffsetCalculator(scroller);
        float anyLocalPosOnAxis = 200000000f;
        
        for(int i = 0; i < 2; i ++){
            bool actual = calculator.Calculate(deltaPosOnAxis, anyLocalPosOnAxis, i);

            Assert.That(actual, Is.False);
        }
    }
	[Test, TestCaseSource(typeof(Calculate_ElementCursorOfffsetPixelIsNotZero_TestCase), "cases")]
    public void Calculate_DeltaPosNonZero_ElementCursorOfffsetPixelIsNotZero_Various(float cursorOffsetInPixel, float deltaPosOnAxis, bool expected){
        IScroller scroller = Substitute.For<IScroller>();
        scroller.GetElementCursorOffsetInPixel(Arg.Any<float>(), Arg.Any<int>()).Returns(cursorOffsetInPixel);
        ElementIsScrolledToIncreaseCursorOffsetCalculator calculator = new ElementIsScrolledToIncreaseCursorOffsetCalculator(scroller);
        float anyLocalPosOnAxis = -1111111f;

        for(int i = 0; i < 2; i ++){
            bool actual = calculator.Calculate(deltaPosOnAxis, anyLocalPosOnAxis, i);

            Assert.That(actual, Is.EqualTo(expected));
        }

    }
    public class Calculate_ElementCursorOfffsetPixelIsNotZero_TestCase{
        public static object[] cases = {
            new object[]{
                -1f, 1f, true
            },
            new object[]{
                -1f, -1f, false
            },
            new object[]{
                1f, -1f, true
            },
            new object[]{
                1f, 1f, false
            },
        };
    }
}
