using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;


[TestFixture, Category("UISystem")]
public class GroupElementAtPositionInGroupSpaceCalculatorTest {
	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases")]
	public void Calculate_Various(int[] expectedArrayIndex){

		IUIElement[,] elementsArray = CreateElementsArray();
		IGroupElementAtPositionInGroupSpaceCalculator calculator = new GroupElementAtPositionInGroupSpaceCalculator(elementsArray, thisElementLength, thisPadding, thisGroupRectLength);

		Vector2 minPos = new Vector2(
			expectedArrayIndex[0] * (thisPadding.x + thisElementLength.x) + thisPadding.x,
			expectedArrayIndex[1] * (thisPadding.y + thisElementLength.y) + thisPadding.y
		);
		Vector2 maxPos = new Vector2(
			minPos.x + thisElementLength.x,
			minPos.y + thisElementLength.y
		);
		Vector2 slightOffset = new Vector2(.01f, .01f);

		IUIElement expected = elementsArray[expectedArrayIndex[0], expectedArrayIndex[1]];
		Assert.That(calculator.Calculate(minPos), Is.EqualTo(expected));
		Assert.That(calculator.Calculate(maxPos), Is.EqualTo(expected));
		Assert.That(calculator.Calculate(minPos - slightOffset), Is.Null);
		Assert.That(calculator.Calculate(maxPos + slightOffset), Is.Null);
		
	}
	Vector2 thisElementLength = new Vector2(100f, 50f);
	Vector2 thisPadding = new Vector2(10f, 10f);
	Vector2 thisGroupRectLength = new Vector2(340f, 190f);
	IUIElement[,] CreateElementsArray(){
		IUIElement[,] result = new IUIElement[3,3];
		for(int i = 0; i < 3; i ++){
			for(int j = 0; j < 3; j ++){
				result[i, j] = Substitute.For<IUIElement>();
			}
		}
		return result;
	}
	public class Calculate_TestCase{
		public static object[] cases = {
			new object[]{new int[]{0, 0}},
			new object[]{new int[]{0, 1}},
			new object[]{new int[]{0, 2}},
			new object[]{new int[]{1, 0}},
			new object[]{new int[]{1, 1}},
			new object[]{new int[]{1, 2}},
			new object[]{new int[]{2, 0}},
			new object[]{new int[]{2, 1}},
			new object[]{new int[]{2, 2}},
		};
	}
}
