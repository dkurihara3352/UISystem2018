using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class UIElementGroupArrayIndexCalculatorTest {
	[Test, TestCaseSource(typeof(CalcColumnIndex_TestCase), "cases")]
	public void CalcColumnIndex_Various(bool topToBottom, bool leftToRight, bool rowToColumn, int numOfColumns, int numOfRows, int elementIndex, int expected){
		IUIElementGroupArrayIndexCalculator calculator = new UIElementGroupArrayIndexCalculator(topToBottom, leftToRight, rowToColumn);
		int actual = calculator.CalcColumnIndex(elementIndex, numOfColumns, numOfRows);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcColumnIndex_TestCase{
		public static object[] cases = {
			new object[]{true, true, true, 4, 3, 0, 0},
			new object[]{true, true, true, 4, 3, 1, 1},
			new object[]{true, true, true, 4, 3, 2, 2},
			new object[]{true, true, true, 4, 3, 3, 3},
			new object[]{true, true, true, 4, 3, 4, 0},
			new object[]{true, true, true, 4, 3, 5, 1},
			new object[]{true, true, true, 4, 3, 6, 2},
			new object[]{true, true, true, 4, 3, 7, 3},
			new object[]{true, true, true, 4, 3, 8, 0},
			new object[]{true, true, true, 4, 3, 9, 1},
			new object[]{true, true, true, 4, 3, 10, 2},
			new object[]{true, true, true, 4, 3, 11, 3},
			
			new object[]{false, true, true, 4, 3, 0, 0},
			new object[]{false, true, true, 4, 3, 1, 1},
			new object[]{false, true, true, 4, 3, 2, 2},
			new object[]{false, true, true, 4, 3, 3, 3},
			new object[]{false, true, true, 4, 3, 4, 0},
			new object[]{false, true, true, 4, 3, 5, 1},
			new object[]{false, true, true, 4, 3, 6, 2},
			new object[]{false, true, true, 4, 3, 7, 3},
			new object[]{false, true, true, 4, 3, 8, 0},
			new object[]{false, true, true, 4, 3, 9, 1},
			new object[]{false, true, true, 4, 3, 10, 2},
			new object[]{false, true, true, 4, 3, 11, 3},
			
			new object[]{true, false, true, 4, 3, 0, 3},
			new object[]{true, false, true, 4, 3, 1, 2},
			new object[]{true, false, true, 4, 3, 2, 1},
			new object[]{true, false, true, 4, 3, 3, 0},
			new object[]{true, false, true, 4, 3, 4, 3},
			new object[]{true, false, true, 4, 3, 5, 2},
			new object[]{true, false, true, 4, 3, 6, 1},
			new object[]{true, false, true, 4, 3, 7, 0},
			new object[]{true, false, true, 4, 3, 8, 3},
			new object[]{true, false, true, 4, 3, 9, 2},
			new object[]{true, false, true, 4, 3, 10, 1},
			new object[]{true, false, true, 4, 3, 11, 0},
			
			new object[]{false, false, true, 4, 3, 0, 3},
			new object[]{false, false, true, 4, 3, 1, 2},
			new object[]{false, false, true, 4, 3, 2, 1},
			new object[]{false, false, true, 4, 3, 3, 0},
			new object[]{false, false, true, 4, 3, 4, 3},
			new object[]{false, false, true, 4, 3, 5, 2},
			new object[]{false, false, true, 4, 3, 6, 1},
			new object[]{false, false, true, 4, 3, 7, 0},
			new object[]{false, false, true, 4, 3, 8, 3},
			new object[]{false, false, true, 4, 3, 9, 2},
			new object[]{false, false, true, 4, 3, 10, 1},
			new object[]{false, false, true, 4, 3, 11, 0},
			
			new object[]{true, true, false, 4, 3, 0, 0},
			new object[]{true, true, false, 4, 3, 1, 0},
			new object[]{true, true, false, 4, 3, 2, 0},
			new object[]{true, true, false, 4, 3, 3, 1},
			new object[]{true, true, false, 4, 3, 4, 1},
			new object[]{true, true, false, 4, 3, 5, 1},
			new object[]{true, true, false, 4, 3, 6, 2},
			new object[]{true, true, false, 4, 3, 7, 2},
			new object[]{true, true, false, 4, 3, 8, 2},
			new object[]{true, true, false, 4, 3, 9, 3},
			new object[]{true, true, false, 4, 3, 10, 3},
			new object[]{true, true, false, 4, 3, 11, 3},
			
			new object[]{false, true, false, 4, 3, 0, 0},
			new object[]{false, true, false, 4, 3, 1, 0},
			new object[]{false, true, false, 4, 3, 2, 0},
			new object[]{false, true, false, 4, 3, 3, 1},
			new object[]{false, true, false, 4, 3, 4, 1},
			new object[]{false, true, false, 4, 3, 5, 1},
			new object[]{false, true, false, 4, 3, 6, 2},
			new object[]{false, true, false, 4, 3, 7, 2},
			new object[]{false, true, false, 4, 3, 8, 2},
			new object[]{false, true, false, 4, 3, 9, 3},
			new object[]{false, true, false, 4, 3, 10, 3},
			new object[]{false, true, false, 4, 3, 11, 3},
			
			new object[]{true, false, false, 4, 3, 0, 3},
			new object[]{true, false, false, 4, 3, 1, 3},
			new object[]{true, false, false, 4, 3, 2, 3},
			new object[]{true, false, false, 4, 3, 3, 2},
			new object[]{true, false, false, 4, 3, 4, 2},
			new object[]{true, false, false, 4, 3, 5, 2},
			new object[]{true, false, false, 4, 3, 6, 1},
			new object[]{true, false, false, 4, 3, 7, 1},
			new object[]{true, false, false, 4, 3, 8, 1},
			new object[]{true, false, false, 4, 3, 9, 0},
			new object[]{true, false, false, 4, 3, 10, 0},
			new object[]{true, false, false, 4, 3, 11, 0},
			
			new object[]{false, false, false, 4, 3, 0, 3},
			new object[]{false, false, false, 4, 3, 1, 3},
			new object[]{false, false, false, 4, 3, 2, 3},
			new object[]{false, false, false, 4, 3, 3, 2},
			new object[]{false, false, false, 4, 3, 4, 2},
			new object[]{false, false, false, 4, 3, 5, 2},
			new object[]{false, false, false, 4, 3, 6, 1},
			new object[]{false, false, false, 4, 3, 7, 1},
			new object[]{false, false, false, 4, 3, 8, 1},
			new object[]{false, false, false, 4, 3, 9, 0},
			new object[]{false, false, false, 4, 3, 10, 0},
			new object[]{false, false, false, 4, 3, 11, 0},
		};
	}
	[Test, TestCaseSource(typeof(CalcRowIndex_TestCase), "cases")]
	public void CalcRowIndex_Various(bool topToBottom, bool leftToRight, bool rowToColumn, int numOfColumns, int numOfRows, int elementIndex, int expected){
		IUIElementGroupArrayIndexCalculator calculator = new UIElementGroupArrayIndexCalculator(topToBottom, leftToRight, rowToColumn);
		int actual = calculator.CalcRowIndex(elementIndex, numOfColumns, numOfRows);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcRowIndex_TestCase{
		public static object[] cases = {
			new object[]{false, true, true, 4, 3, 0, 0},
			new object[]{false, true, true, 4, 3, 1, 0},
			new object[]{false, true, true, 4, 3, 2, 0},
			new object[]{false, true, true, 4, 3, 3, 0},
			new object[]{false, true, true, 4, 3, 4, 1},
			new object[]{false, true, true, 4, 3, 5, 1},
			new object[]{false, true, true, 4, 3, 6, 1},
			new object[]{false, true, true, 4, 3, 7, 1},
			new object[]{false, true, true, 4, 3, 8, 2},
			new object[]{false, true, true, 4, 3, 9, 2},
			new object[]{false, true, true, 4, 3, 10, 2},
			new object[]{false, true, true, 4, 3, 11, 2},
			
			new object[]{false, false, true, 4, 3, 0, 0},
			new object[]{false, false, true, 4, 3, 1, 0},
			new object[]{false, false, true, 4, 3, 2, 0},
			new object[]{false, false, true, 4, 3, 3, 0},
			new object[]{false, false, true, 4, 3, 4, 1},
			new object[]{false, false, true, 4, 3, 5, 1},
			new object[]{false, false, true, 4, 3, 6, 1},
			new object[]{false, false, true, 4, 3, 7, 1},
			new object[]{false, false, true, 4, 3, 8, 2},
			new object[]{false, false, true, 4, 3, 9, 2},
			new object[]{false, false, true, 4, 3, 10, 2},
			new object[]{false, false, true, 4, 3, 11, 2},
			
			new object[]{true, true, true, 4, 3, 0, 2},
			new object[]{true, true, true, 4, 3, 1, 2},
			new object[]{true, true, true, 4, 3, 2, 2},
			new object[]{true, true, true, 4, 3, 3, 2},
			new object[]{true, true, true, 4, 3, 4, 1},
			new object[]{true, true, true, 4, 3, 5, 1},
			new object[]{true, true, true, 4, 3, 6, 1},
			new object[]{true, true, true, 4, 3, 7, 1},
			new object[]{true, true, true, 4, 3, 8, 0},
			new object[]{true, true, true, 4, 3, 9, 0},
			new object[]{true, true, true, 4, 3, 10, 0},
			new object[]{true, true, true, 4, 3, 11, 0},
			
			new object[]{true, false, true, 4, 3, 0, 2},
			new object[]{true, false, true, 4, 3, 1, 2},
			new object[]{true, false, true, 4, 3, 2, 2},
			new object[]{true, false, true, 4, 3, 3, 2},
			new object[]{true, false, true, 4, 3, 4, 1},
			new object[]{true, false, true, 4, 3, 5, 1},
			new object[]{true, false, true, 4, 3, 6, 1},
			new object[]{true, false, true, 4, 3, 7, 1},
			new object[]{true, false, true, 4, 3, 8, 0},
			new object[]{true, false, true, 4, 3, 9, 0},
			new object[]{true, false, true, 4, 3, 10, 0},
			new object[]{true, false, true, 4, 3, 11, 0},

			new object[]{false, true, false, 4, 3, 0, 0},
			new object[]{false, true, false, 4, 3, 1, 1},
			new object[]{false, true, false, 4, 3, 2, 2},
			new object[]{false, true, false, 4, 3, 3, 0},
			new object[]{false, true, false, 4, 3, 4, 1},
			new object[]{false, true, false, 4, 3, 5, 2},
			new object[]{false, true, false, 4, 3, 6, 0},
			new object[]{false, true, false, 4, 3, 7, 1},
			new object[]{false, true, false, 4, 3, 8, 2},
			new object[]{false, true, false, 4, 3, 9, 0},
			new object[]{false, true, false, 4, 3, 10, 1},
			new object[]{false, true, false, 4, 3, 11, 2},
			
			new object[]{false, false, false, 4, 3, 0, 0},
			new object[]{false, false, false, 4, 3, 1, 1},
			new object[]{false, false, false, 4, 3, 2, 2},
			new object[]{false, false, false, 4, 3, 3, 0},
			new object[]{false, false, false, 4, 3, 4, 1},
			new object[]{false, false, false, 4, 3, 5, 2},
			new object[]{false, false, false, 4, 3, 6, 0},
			new object[]{false, false, false, 4, 3, 7, 1},
			new object[]{false, false, false, 4, 3, 8, 2},
			new object[]{false, false, false, 4, 3, 9, 0},
			new object[]{false, false, false, 4, 3, 10, 1},
			new object[]{false, false, false, 4, 3, 11, 2},
			
			new object[]{true, true, false, 4, 3, 0, 2},
			new object[]{true, true, false, 4, 3, 1, 1},
			new object[]{true, true, false, 4, 3, 2, 0},
			new object[]{true, true, false, 4, 3, 3, 2},
			new object[]{true, true, false, 4, 3, 4, 1},
			new object[]{true, true, false, 4, 3, 5, 0},
			new object[]{true, true, false, 4, 3, 6, 2},
			new object[]{true, true, false, 4, 3, 7, 1},
			new object[]{true, true, false, 4, 3, 8, 0},
			new object[]{true, true, false, 4, 3, 9, 2},
			new object[]{true, true, false, 4, 3, 10, 1},
			new object[]{true, true, false, 4, 3, 11, 0},
			
			new object[]{true, false, false, 4, 3, 0, 2},
			new object[]{true, false, false, 4, 3, 1, 1},
			new object[]{true, false, false, 4, 3, 2, 0},
			new object[]{true, false, false, 4, 3, 3, 2},
			new object[]{true, false, false, 4, 3, 4, 1},
			new object[]{true, false, false, 4, 3, 5, 0},
			new object[]{true, false, false, 4, 3, 6, 2},
			new object[]{true, false, false, 4, 3, 7, 1},
			new object[]{true, false, false, 4, 3, 8, 0},
			new object[]{true, false, false, 4, 3, 9, 2},
			new object[]{true, false, false, 4, 3, 10, 1},
			new object[]{true, false, false, 4, 3, 11, 0},
		};
	}
}
