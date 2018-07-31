using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class UIElementGroupTest {
	[Test, TestCaseSource(typeof(CalcNumberOfColumnsToCreate_TestCase), "cases")]
	public void CalcNumberOfColumnsToCreate_Various(int columnCountConstraint, int rowCountConstraint, int elementsCount, int expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithConstraints(columnCountConstraint, rowCountConstraint);
		List<IUIElement> elements = CreateUIElements(elementsCount);
		uieGroup.SetElements(elements);

		int actual = uieGroup.CalcNumberOfColumnsToCreate_Test();

		Assert.That(actual, Is.EqualTo(expected));
	}
	List<IUIElement> CreateUIElements(int count){
		List<IUIElement> result = new List<IUIElement>();
		for(int i = 0; i < count; i ++)
			result.Add(Substitute.For<IUIElement>());
		return result;
	}
	public class CalcNumberOfColumnsToCreate_TestCase{
		public static object[] cases = {
			new object[]{1, 0, 20, 1},
			new object[]{5, 0, 20, 5},
			new object[]{10, 0, 20, 10},

			new object[]{0, 1, 20, 20},
			new object[]{0, 5, 20, 4},
			new object[]{0, 10, 20, 2},

			new object[]{0, 3, 20, 7},
			new object[]{0, 6, 20, 4},
		};
	}
	[Test, TestCaseSource(typeof(CalcNumberOfRowsToCreate_TestCase), "cases")]
	public void CalcNumberOfRowsToCreate_Various(int columnCountConstraint, int rowCountConstraint, int elementsCount, int expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithConstraints(columnCountConstraint, rowCountConstraint);
		List<IUIElement> elements = CreateUIElements(elementsCount);
		uieGroup.SetElements(elements);

		int actual = uieGroup.CalcNumberOfRowsToCreate_Test();

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcNumberOfRowsToCreate_TestCase{
		public static object[] cases = {
			new object[]{1, 0, 20, 20},
			new object[]{5, 0, 20, 4},
			new object[]{10, 0, 20, 2},

			new object[]{0, 1, 20, 1},
			new object[]{0, 5, 20, 5},
			new object[]{0, 10, 20, 10},

			new object[]{0, 3, 20, 3},
			new object[]{0, 6, 20, 6},
			
			new object[]{3, 0, 20, 7},
			new object[]{6, 0, 20, 4},
		};
	}
	[Test, TestCaseSource(typeof(CalcColumnIndex_TestCase), "cases")]
	public void CalcColumnIndex_Various(bool topToBottom, bool leftToRight, bool rowToColumn, int elementIndex, int numOfColumns, int numOfRows, int expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithAlignmentSetting(topToBottom, leftToRight, rowToColumn);

		int actual = uieGroup.CalcColumnIndex_Test(elementIndex, numOfColumns, numOfRows);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcColumnIndex_TestCase{
		public static object[] cases = {
			new object[]{true, true, true, 0, 4, 3, 0},
			new object[]{true, true, true, 1, 4, 3, 1},
			new object[]{true, true, true, 2, 4, 3, 2},
			new object[]{true, true, true, 3, 4, 3, 3},
			new object[]{true, true, true, 4, 4, 3, 0},
			new object[]{true, true, true, 5, 4, 3, 1},
			new object[]{true, true, true, 6, 4, 3, 2},
			new object[]{true, true, true, 7, 4, 3, 3},
			new object[]{true, true, true, 8, 4, 3, 0},
			new object[]{true, true, true, 9, 4, 3, 1},
			new object[]{true, true, true, 10, 4, 3, 2},
			new object[]{true, true, true, 11, 4, 3, 3},
			
			new object[]{false, true, true, 0, 4, 3, 0},
			new object[]{false, true, true, 1, 4, 3, 1},
			new object[]{false, true, true, 2, 4, 3, 2},
			new object[]{false, true, true, 3, 4, 3, 3},
			new object[]{false, true, true, 4, 4, 3, 0},
			new object[]{false, true, true, 5, 4, 3, 1},
			new object[]{false, true, true, 6, 4, 3, 2},
			new object[]{false, true, true, 7, 4, 3, 3},
			new object[]{false, true, true, 8, 4, 3, 0},
			new object[]{false, true, true, 9, 4, 3, 1},
			new object[]{false, true, true, 10, 4, 3, 2},
			new object[]{false, true, true, 11, 4, 3, 3},
			
			new object[]{true, false, true, 0, 4, 3, 3},
			new object[]{true, false, true, 1, 4, 3, 2},
			new object[]{true, false, true, 2, 4, 3, 1},
			new object[]{true, false, true, 3, 4, 3, 0},
			new object[]{true, false, true, 4, 4, 3, 3},
			new object[]{true, false, true, 5, 4, 3, 2},
			new object[]{true, false, true, 6, 4, 3, 1},
			new object[]{true, false, true, 7, 4, 3, 0},
			new object[]{true, false, true, 8, 4, 3, 3},
			new object[]{true, false, true, 9, 4, 3, 2},
			new object[]{true, false, true, 10, 4, 3, 1},
			new object[]{true, false, true, 11, 4, 3, 0},

			new object[]{false, false, true, 0, 4, 3, 3},
			new object[]{false, false, true, 1, 4, 3, 2},
			new object[]{false, false, true, 2, 4, 3, 1},
			new object[]{false, false, true, 3, 4, 3, 0},
			new object[]{false, false, true, 4, 4, 3, 3},
			new object[]{false, false, true, 5, 4, 3, 2},
			new object[]{false, false, true, 6, 4, 3, 1},
			new object[]{false, false, true, 7, 4, 3, 0},
			new object[]{false, false, true, 8, 4, 3, 3},
			new object[]{false, false, true, 9, 4, 3, 2},
			new object[]{false, false, true, 10, 4, 3, 1},
			new object[]{false, false, true, 11, 4, 3, 0},

			new object[]{true, true, false, 0, 4, 3, 0},
			new object[]{true, true, false, 1, 4, 3, 0},
			new object[]{true, true, false, 2, 4, 3, 0},
			new object[]{true, true, false, 3, 4, 3, 1},
			new object[]{true, true, false, 4, 4, 3, 1},
			new object[]{true, true, false, 5, 4, 3, 1},
			new object[]{true, true, false, 6, 4, 3, 2},
			new object[]{true, true, false, 7, 4, 3, 2},
			new object[]{true, true, false, 8, 4, 3, 2},
			new object[]{true, true, false, 9, 4, 3, 3},
			new object[]{true, true, false, 10, 4, 3, 3},
			new object[]{true, true, false, 11, 4, 3, 3},

			new object[]{false, true, false, 0, 4, 3, 0},
			new object[]{false, true, false, 1, 4, 3, 0},
			new object[]{false, true, false, 2, 4, 3, 0},
			new object[]{false, true, false, 3, 4, 3, 1},
			new object[]{false, true, false, 4, 4, 3, 1},
			new object[]{false, true, false, 5, 4, 3, 1},
			new object[]{false, true, false, 6, 4, 3, 2},
			new object[]{false, true, false, 7, 4, 3, 2},
			new object[]{false, true, false, 8, 4, 3, 2},
			new object[]{false, true, false, 9, 4, 3, 3},
			new object[]{false, true, false, 10, 4, 3, 3},
			new object[]{false, true, false, 11, 4, 3, 3},
			
			new object[]{true, false, false, 0, 4, 3, 3},
			new object[]{true, false, false, 1, 4, 3, 3},
			new object[]{true, false, false, 2, 4, 3, 3},
			new object[]{true, false, false, 3, 4, 3, 2},
			new object[]{true, false, false, 4, 4, 3, 2},
			new object[]{true, false, false, 5, 4, 3, 2},
			new object[]{true, false, false, 6, 4, 3, 1},
			new object[]{true, false, false, 7, 4, 3, 1},
			new object[]{true, false, false, 8, 4, 3, 1},
			new object[]{true, false, false, 9, 4, 3, 0},
			new object[]{true, false, false, 10, 4, 3, 0},
			new object[]{true, false, false, 11, 4, 3, 0},
			
			new object[]{false, false, false, 0, 4, 3, 3},
			new object[]{false, false, false, 1, 4, 3, 3},
			new object[]{false, false, false, 2, 4, 3, 3},
			new object[]{false, false, false, 3, 4, 3, 2},
			new object[]{false, false, false, 4, 4, 3, 2},
			new object[]{false, false, false, 5, 4, 3, 2},
			new object[]{false, false, false, 6, 4, 3, 1},
			new object[]{false, false, false, 7, 4, 3, 1},
			new object[]{false, false, false, 8, 4, 3, 1},
			new object[]{false, false, false, 9, 4, 3, 0},
			new object[]{false, false, false, 10, 4, 3, 0},
			new object[]{false, false, false, 11, 4, 3, 0},
		};
	}
	[Test, TestCaseSource(typeof(CalcRowIndex_TestCase), "cases")]
	public void CalcRowIndex_Various(bool topToBottom, bool leftToRight, bool rowToColumn, int elementIndex, int numOfColumns, int numOfRows, int expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithAlignmentSetting(topToBottom, leftToRight, rowToColumn);

		int actual = uieGroup.CalcRowIndex_Test(elementIndex, numOfColumns, numOfRows);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcRowIndex_TestCase{
		public static object[] cases = {
			new object[]{true, true, true, 0, 4, 3, 0},
			new object[]{true, true, true, 1, 4, 3, 0},
			new object[]{true, true, true, 2, 4, 3, 0},
			new object[]{true, true, true, 3, 4, 3, 0},
			new object[]{true, true, true, 4, 4, 3, 1},
			new object[]{true, true, true, 5, 4, 3, 1},
			new object[]{true, true, true, 6, 4, 3, 1},
			new object[]{true, true, true, 7, 4, 3, 1},
			new object[]{true, true, true, 8, 4, 3, 2},
			new object[]{true, true, true, 9, 4, 3, 2},
			new object[]{true, true, true, 10, 4, 3, 2},
			new object[]{true, true, true, 11, 4, 3, 2},
			
			new object[]{true, false, true, 0, 4, 3, 0},
			new object[]{true, false, true, 1, 4, 3, 0},
			new object[]{true, false, true, 2, 4, 3, 0},
			new object[]{true, false, true, 3, 4, 3, 0},
			new object[]{true, false, true, 4, 4, 3, 1},
			new object[]{true, false, true, 5, 4, 3, 1},
			new object[]{true, false, true, 6, 4, 3, 1},
			new object[]{true, false, true, 7, 4, 3, 1},
			new object[]{true, false, true, 8, 4, 3, 2},
			new object[]{true, false, true, 9, 4, 3, 2},
			new object[]{true, false, true, 10, 4, 3, 2},
			new object[]{true, false, true, 11, 4, 3, 2},
			
			new object[]{false, true, true, 0, 4, 3, 2},
			new object[]{false, true, true, 1, 4, 3, 2},
			new object[]{false, true, true, 2, 4, 3, 2},
			new object[]{false, true, true, 3, 4, 3, 2},
			new object[]{false, true, true, 4, 4, 3, 1},
			new object[]{false, true, true, 5, 4, 3, 1},
			new object[]{false, true, true, 6, 4, 3, 1},
			new object[]{false, true, true, 7, 4, 3, 1},
			new object[]{false, true, true, 8, 4, 3, 0},
			new object[]{false, true, true, 9, 4, 3, 0},
			new object[]{false, true, true, 10, 4, 3, 0},
			new object[]{false, true, true, 11, 4, 3, 0},
			
			new object[]{false, false, true, 0, 4, 3, 2},
			new object[]{false, false, true, 1, 4, 3, 2},
			new object[]{false, false, true, 2, 4, 3, 2},
			new object[]{false, false, true, 3, 4, 3, 2},
			new object[]{false, false, true, 4, 4, 3, 1},
			new object[]{false, false, true, 5, 4, 3, 1},
			new object[]{false, false, true, 6, 4, 3, 1},
			new object[]{false, false, true, 7, 4, 3, 1},
			new object[]{false, false, true, 8, 4, 3, 0},
			new object[]{false, false, true, 9, 4, 3, 0},
			new object[]{false, false, true, 10, 4, 3, 0},
			new object[]{false, false, true, 11, 4, 3, 0},

			new object[]{true, true, false, 0, 4, 3, 0},
			new object[]{true, true, false, 1, 4, 3, 1},
			new object[]{true, true, false, 2, 4, 3, 2},
			new object[]{true, true, false, 3, 4, 3, 0},
			new object[]{true, true, false, 4, 4, 3, 1},
			new object[]{true, true, false, 5, 4, 3, 2},
			new object[]{true, true, false, 6, 4, 3, 0},
			new object[]{true, true, false, 7, 4, 3, 1},
			new object[]{true, true, false, 8, 4, 3, 2},
			new object[]{true, true, false, 9, 4, 3, 0},
			new object[]{true, true, false, 10, 4, 3, 1},
			new object[]{true, true, false, 11, 4, 3, 2},
			
			new object[]{true, false, false, 0, 4, 3, 0},
			new object[]{true, false, false, 1, 4, 3, 1},
			new object[]{true, false, false, 2, 4, 3, 2},
			new object[]{true, false, false, 3, 4, 3, 0},
			new object[]{true, false, false, 4, 4, 3, 1},
			new object[]{true, false, false, 5, 4, 3, 2},
			new object[]{true, false, false, 6, 4, 3, 0},
			new object[]{true, false, false, 7, 4, 3, 1},
			new object[]{true, false, false, 8, 4, 3, 2},
			new object[]{true, false, false, 9, 4, 3, 0},
			new object[]{true, false, false, 10, 4, 3, 1},
			new object[]{true, false, false, 11, 4, 3, 2},
			
			new object[]{false, true, false, 0, 4, 3, 2},
			new object[]{false, true, false, 1, 4, 3, 1},
			new object[]{false, true, false, 2, 4, 3, 0},
			new object[]{false, true, false, 3, 4, 3, 2},
			new object[]{false, true, false, 4, 4, 3, 1},
			new object[]{false, true, false, 5, 4, 3, 0},
			new object[]{false, true, false, 6, 4, 3, 2},
			new object[]{false, true, false, 7, 4, 3, 1},
			new object[]{false, true, false, 8, 4, 3, 0},
			new object[]{false, true, false, 9, 4, 3, 2},
			new object[]{false, true, false, 10, 4, 3, 1},
			new object[]{false, true, false, 11, 4, 3, 0},
			
			new object[]{false, false, false, 0, 4, 3, 2},
			new object[]{false, false, false, 1, 4, 3, 1},
			new object[]{false, false, false, 2, 4, 3, 0},
			new object[]{false, false, false, 3, 4, 3, 2},
			new object[]{false, false, false, 4, 4, 3, 1},
			new object[]{false, false, false, 5, 4, 3, 0},
			new object[]{false, false, false, 6, 4, 3, 2},
			new object[]{false, false, false, 7, 4, 3, 1},
			new object[]{false, false, false, 8, 4, 3, 0},
			new object[]{false, false, false, 9, 4, 3, 2},
			new object[]{false, false, false, 10, 4, 3, 1},
			new object[]{false, false, false, 11, 4, 3, 0},
		};
	}
	[Test, TestCaseSource(typeof(CreateElements2DArray_TestCase), "cases")]
	public void CreateElements2DArray_Various(int columnCountConstraint, int rowCountConstraint, int elementsCount, int elementIndex, int expColumnIndex, int expRowIndex){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithConstraints(columnCountConstraint, rowCountConstraint);
		List<IUIElement> elements = CreateUIElements(elementsCount);
		uieGroup.SetElements(elements);
		uieGroup.thisElementsArray_Test = uieGroup.CreateElement2DArray_Test();

		int[] index = uieGroup.GetGroupElementArrayIndex(elements[elementIndex]);
		int actualColumnIndex = index[0];
		int actualRowIndex = index[1];

		Assert.That(actualColumnIndex, Is.EqualTo(expColumnIndex));
		Assert.That(actualRowIndex, Is.EqualTo(expRowIndex));
	}
	public class CreateElements2DArray_TestCase{
		public static object[] cases = {
			new object[]{1, 0, 3, 0, 0, 0},
			new object[]{1, 0, 3, 1, 0, 1},
			new object[]{1, 0, 3, 2, 0, 2},
			
			new object[]{0, 1, 3, 0, 0, 0},
			new object[]{0, 1, 3, 1, 1, 0},
			new object[]{0, 1, 3, 2, 2, 0},
			
			new object[]{2, 0, 6, 0, 0, 0},
			new object[]{2, 0, 6, 1, 1, 0},
			new object[]{2, 0, 6, 2, 0, 1},
			new object[]{2, 0, 6, 3, 1, 1},
			new object[]{2, 0, 6, 4, 0, 2},
			new object[]{2, 0, 6, 5, 1, 2},
			
			new object[]{0, 3, 6, 0, 0, 0},
			new object[]{0, 3, 6, 1, 0, 1},
			new object[]{0, 3, 6, 2, 0, 2},
			new object[]{0, 3, 6, 3, 1, 0},
			new object[]{0, 3, 6, 4, 1, 1},
			new object[]{0, 3, 6, 5, 1, 2},
		};
	}
	[Test, TestCaseSource(typeof(ResizeToFitElements_TestCase), "cases")]
	public void ResizeToFitElements_CallsUIASetRectLength(int columnCountConstraint, int rowCountConstraint, Vector2 elementLength, Vector2 padding, int elementsCount, Vector2 expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithConstraintsAndElementDimension(columnCountConstraint, rowCountConstraint, elementLength, padding);
		List<IUIElement> elements = CreateUIElements(elementsCount);
		uieGroup.SetElements(elements);
		uieGroup.thisElementsArray_Test = uieGroup.CreateElement2DArray_Test();

		uieGroup.ResizeToFitElements_Test();
		
		uieGroup.GetUIAdaptor().Received(1).SetRectLength(expected.x, expected.y);
	}
	public class ResizeToFitElements_TestCase{
		public static object[] cases = {
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, new Vector2(300f, 460f)},
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, new Vector2(1460f, 100f)},
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, new Vector2(590f, 280f)},
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, new Vector2(880f, 190f)},
		};
	}
	[Test, TestCaseSource(typeof(PlaceElements_TestCase), "cases")]
	public void PlaceElements_CallsElementsSetLocalPos(int columnCountConstraint, int rowCountConstraint, Vector2 elementLength, Vector2 padding, int elementsCount, int elementIndex, Vector2 expected){
		TestUIElementGroup uieGroup = CreateTestUIElementGroupWithConstraintsAndElementDimension(columnCountConstraint, rowCountConstraint, elementLength, padding);
		List<IUIElement> elements = CreateUIElements(elementsCount);
		uieGroup.SetElements(elements);
		uieGroup.thisElementsArray_Test = uieGroup.CreateElement2DArray_Test();
		
		uieGroup.PlaceElements_Test();
		
		IUIElement element = uieGroup.GetGroupElement(elementIndex);
		element.Received(1).SetLocalPosition(expected);
	}
	public class PlaceElements_TestCase{
		public static object[] cases = {
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 0, new Vector2(10f, 10f)},
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 1, new Vector2(10f, 100f)},
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 2, new Vector2(10f, 190f)},
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 3, new Vector2(10f, 280f)},
			new object[]{1, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 4, new Vector2(10f, 370f)},
			
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 0, new Vector2(10f, 10f)},
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 1, new Vector2(300f, 10f)},
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 2, new Vector2(590f, 10f)},
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 3, new Vector2(880f, 10f)},
			new object[]{0, 1, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 4, new Vector2(1170f, 10f)},
			
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 0, new Vector2(10f, 10f)},
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 1, new Vector2(300f, 10f)},
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 2, new Vector2(10f, 100f)},
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 3, new Vector2(300f, 100f)},
			new object[]{2, 0, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 4, new Vector2(10f, 190f)},
			
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 0, new Vector2(10f, 10f)},
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 1, new Vector2(10f, 100f)},
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 2, new Vector2(300f, 10f)},
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 3, new Vector2(300f, 100f)},
			new object[]{0, 2, new Vector2(280f, 80f), new Vector2(10f, 10f), 5, 4, new Vector2(590f, 10f)},
		};
	}
	[Test, TestCaseSource(typeof(GetUIElementsWithinIndexRange_TestCase), "cases")]
	public void GetUIElementsWithinIndexRange_Various(int minColumnIndex, int minRowIndex, int maxColumnIndex, int maxRowIndex, int[] expectedElementsIndex){
		IUIElementGroupConstArg arg = CreateMockConstArg();
		arg.columnCountConstraint.Returns(3);
		arg.rowCountConstraint.Returns(0);
		arg.elementLength.Returns(new Vector2(100f, 50f));
		arg.padding.Returns(new Vector2(10f, 10f));
		TestUIElementGroup uieGroup = new TestUIElementGroup(arg);

		List<IUIElement> elements = CreateUIElements(10);
		uieGroup.SetUpElements_Test(elements);

		IUIElement[] actual = uieGroup.GetGroupElementsWithinIndexRange(minColumnIndex, minRowIndex, maxColumnIndex, maxRowIndex);
		IUIElement[] expectedElements = GetUIElementsFromIndex(elements, expectedElementsIndex);

		Assert.That(actual, Is.EqualTo(expectedElements));
	}
	IUIElement[] GetUIElementsFromIndex(List<IUIElement> list, int[] index){
		List<IUIElement> result = new List<IUIElement>();
		foreach(int i in index)
			result.Add(list[i]);
		return result.ToArray();
	}
	public class GetUIElementsWithinIndexRange_TestCase{
		public static object[] cases = {
			new object[]{0, 0, 0, 0, new int[]{0}},
			new object[]{0, 0, 1, 1, new int[]{0, 1, 3, 4}},
			new object[]{0, 0, 2, 0, new int[]{0, 1, 2}},
			new object[]{0, 0, 2, 1, new int[]{0, 1, 2, 3, 4, 5}},
			new object[]{0, 0, 0, 2, new int[]{0, 3, 6}},
			new object[]{0, 0, 2, 2, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8}},
			new object[]{0, 0, 2, 3, new int[]{0, 1, 2, 3, 4, 5, 6, 7, 8, 9}},
			new object[]{1, 0, 1, 0, new int[]{1}},
			new object[]{1, 0, 2, 0, new int[]{1, 2}},
			new object[]{1, 1, 1, 1, new int[]{4}},
			new object[]{1, 1, 2, 2, new int[]{4, 5, 7, 8}},
			
			new object[]{1, 1, 0, 0, new int[]{}},
		};
	}
	[Test, TestCaseSource(typeof(GetUIElementAtPositionInGroupSpace_TestCase), "cases")]
	public void GetUIElementAtPositionInGroupSpace_Various(Vector2 elementLength, Vector2 padding, Vector2 posInGroupSpace, int expectedUIEIndex){
		IUIElementGroupConstArg arg = CreateMockConstArg();
		arg.columnCountConstraint.Returns(3);
		arg.rowCountConstraint.Returns(0);
		arg.elementLength.Returns(elementLength);
		arg.padding.Returns(padding);
		float scrollerRectLengthX = 3 * (elementLength[0] + padding[0]) + padding[0];
		float scrollerRectLengthY = 4 * (elementLength[1] + padding[1]) + padding[1];
		Vector2 scrollerRectLength = new Vector2(scrollerRectLengthX, scrollerRectLengthY);
		Rect thisRect = new Rect(Vector2.zero, scrollerRectLength);
		arg.uia.GetRect().Returns(thisRect);
		TestUIElementGroup uieGroup = new TestUIElementGroup(arg);
		List<IUIElement> elements = CreateUIElements(10);
		uieGroup.SetUpElements_Test(elements);

		IUIElement actual = uieGroup.GetGroupElementAtPositionInGroupSpace(posInGroupSpace);

		if(expectedUIEIndex == -1)
			Assert.That(actual, Is.Null);
		else
			Assert.That(actual, Is.EqualTo(elements[expectedUIEIndex]));
	}
	public class GetUIElementAtPositionInGroupSpace_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(0f, 0f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(-1f, -1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(340f, 250f), -1},
			
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(10f -.1f, 10f -.1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(10f, 10f), 0},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(60f, 35f), 0},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(110f, 60f), 0},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(110.1f, 60.1f), -1},

			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f -.1f, 10f -.1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f, 10f), 1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(170f, 35f), 1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(220f, 60f), 1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(220.1f, 60.1f), -1},
			
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(230f -.1f, 10f -.1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(230f, 10f), 2},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(280f, 35f), 2},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(330f, 60f), 2},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(330.1f, 60.1f), -1},
			
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f -.1f, 70f -.1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f, 70f), 4},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(170f, 95f), 4},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(220f, 120f), 4},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(220.1f, 120.1f), -1},
			
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(230f -.1f, 130f -.1f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(230f, 130f), 8},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(280f, 155f), 8},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(330f, 180f), 8},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(330.1f, 180.1f), -1},
			
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f, 190f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(170f, 215f), -1},
			new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(220f, 240f), -1},
			
		};
	}



	IUIElementGroupConstArg CreateMockConstArg(){
		IUIElementGroupConstArg arg = Substitute.For<IUIElementGroupConstArg>();
		arg.columnCountConstraint.Returns(3);
		arg.rowCountConstraint.Returns(0);
		arg.topToBottom.Returns(true);
		arg.leftToRight.Returns(true);
		arg.rowToColumn.Returns(true);
		arg.elementLength.Returns(new Vector2(100f, 50f));
		arg.padding.Returns(new Vector2(10f, 10f));
		arg.uim.Returns(Substitute.For<IUIManager>());
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
		arg.uia.Returns(Substitute.For<IUIElementGroupAdaptor>());
		arg.image.Returns(Substitute.For<IUIImage>());
		return arg;
	}

	public class TestUIElementGroup: AbsUIElementGroup<IUIElement>, INonActivatorUIElement{
		public TestUIElementGroup(IUIElementGroupConstArg arg): base(arg){

		}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			IUIEActivationStateEngine engine = new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
			return engine;
		}
		public void SetElements(List<IUIElement> elements){
			thisElements = elements;
		}
		/* Test Exposure */
		public IUIElement[, ] thisElementsArray_Test{
			get{return thisElementsArray;}
			set{thisElementsArray = value;}
		}
		public void SetUpElements_Test(List<IUIElement> elements){
			this.SetUpElements(elements);
		}
		public IUIElement[,] CreateElement2DArray_Test(){
			return this.CreateElements2DArray();
		}
		public int CalcNumberOfRowsToCreate_Test(){
			return this.CalcNumberOfRowsToCreate();
		}
		public int CalcNumberOfColumnsToCreate_Test(){
			return this.CalcNumberOfColumnsToCreate();
		}
		public int CalcColumnIndex_Test(int n, int numOfColumns, int numOfRows){
			return this.CalcColumnIndex(n, numOfColumns, numOfRows);
		}
		public int CalcRowIndex_Test(int n, int numOfColumns, int numOfRows){
			return this.CalcRowIndex(n, numOfColumns, numOfRows);
		}
		public int GetElementsArraySize_Test(int dimension){
			return this.GetGroupElementsArraySize(dimension);
		}
		public void ResizeToFitElements_Test(){
			this.ResizeToFitElements();
		}
		public void PlaceElements_Test(){
			this.PlaceElements();
		}
	}
	public TestUIElementGroup CreateTestUIElementGroupWithConstraints(int columnCountConstraint, int rowCountConstraint){
		IUIElementGroupConstArg arg = new UIElementGroupConstArg(columnCountConstraint, rowCountConstraint, true, true, true, Vector2.zero, Vector2.zero, Substitute.For<IUIManager>(), Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIElementFactory>(), Substitute.For<IUIElementGroupAdaptor>(), Substitute.For<IUIImage>());
		return new TestUIElementGroup(arg);
	}
	public TestUIElementGroup CreateTestUIElementGroupWithConstraintsAndElementDimension(int columnCountConstraint, int rowCountConstraint, Vector2 elementLength, Vector2 padding){
		IUIElementGroupConstArg arg = new UIElementGroupConstArg(columnCountConstraint, rowCountConstraint, true, true, true, elementLength, padding, Substitute.For<IUIManager>(), Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIElementFactory>(), Substitute.For<IUIElementGroupAdaptor>(), Substitute.For<IUIImage>());
		return new TestUIElementGroup(arg);
	}
	public TestUIElementGroup CreateTestUIElementGroupWithAlignmentSetting(bool topToBottom, bool leftToRight, bool rowToColumn){
		IUIElementGroupConstArg arg = new UIElementGroupConstArg(1, 1, topToBottom, leftToRight, rowToColumn, Vector2.zero, Vector2.zero, Substitute.For<IUIManager>(), Substitute.For<IUISystemProcessFactory>(), Substitute.For<IUIElementFactory>(), Substitute.For<IUIElementGroupAdaptor>(), Substitute.For<IUIImage>());
		return new TestUIElementGroup(arg);
	}
}
