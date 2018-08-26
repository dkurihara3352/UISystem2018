using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NSubstitute;
using NUnit.Framework;
using UISystem;

[TestFixture, Category("UISystem")]
public class CorrectedCursoredElementIndexCalculatorTest {
	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases")]
	public void Calculate_Various(
		int sourceIndex,
		int[] arraySize,
		int[] sourceElementArrayIndex,
		int[] cursorSize,
		int[] expectedArrayIndex,
		int expectedIndex
	){

		IUIElement sourceElement = Substitute.For<IUIElement>();

		IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
		uieGroup.GetGroupElement(sourceIndex).Returns(sourceElement);
		uieGroup.GetArraySize(0).Returns(arraySize[0]);
		uieGroup.GetArraySize(1).Returns(arraySize[1]);
		uieGroup.GetGroupElementArrayIndex(sourceElement).Returns(sourceElementArrayIndex);
		IUIElement expectedMock = Substitute.For<IUIElement>();
		uieGroup.GetGroupElement(expectedArrayIndex[0], expectedArrayIndex[1]).Returns(expectedMock);
		uieGroup.GetGroupElementIndex(expectedMock).Returns(expectedIndex);

		ICorrectedCursoredElementIndexCalculator calculator = new CorrectedCursoredElementIndexCalculator(uieGroup, cursorSize);

		int actual = calculator.Calculate(sourceIndex);

		Assert.That(actual, Is.EqualTo(expectedIndex));
	}

	public class Calculate_TestCase{
		public static object[] cases = {
			new object[]{
				0,//sourceIndex, any
				new int[]{2, 2},
				new int[]{0, 0},//sourceArray
				new int[]{1, 1},//cursor
				new int[]{0, 0},//expected
				0// any, just make sure it is either same as source or not
			},
			//cursor == 1, anything goes
			new object[]{
				0,
				new int[]{2, 2},
				new int[]{1, 0},
				new int[]{1, 1},
				new int[]{1, 0},
				0
			},
			new object[]{
				0,
				new int[]{2, 2},
				new int[]{0, 1},
				new int[]{1, 1},
				new int[]{0, 1},
				0
			},
			//cursor == 2, corrected
			new object[]{// no correction needed
				0,
				new int[]{2, 2},
				new int[]{0, 0},
				new int[]{2, 1},
				new int[]{0, 0},
				0
			},
			new object[]{// corrected
				0,
				new int[]{2, 2},
				new int[]{1, 0},//this
				new int[]{2, 1},
				new int[]{0, 0},//is no longer this
				1
			},
			new object[]{
				0,
				new int[]{3, 3},
				new int[]{1, 0},//this
				new int[]{2, 1},
				new int[]{1, 0},//is is ok
				0
			},
			new object[]{
				0,
				new int[]{3, 3},
				new int[]{2, 0},//this
				new int[]{2, 1},
				new int[]{1, 0},//is is corrected
				1
			},
			//big num
			new object[]{
				0,
				new int[]{100, 3},
				new int[]{99, 0},
				new int[]{50, 1},
				new int[]{50, 0},
				1
			},
			//same for y
			new object[]{
				0,
				new int[]{3, 100},
				new int[]{2, 99},
				new int[]{2, 50},
				new int[]{1, 50},
				1
			},
		};
	}
}
