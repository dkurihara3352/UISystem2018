using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;

[TestFixture, Category("UISystem")]
public class GroupElementsArrayCalculatorTest {

	[Test, TestCaseSource(typeof(GetGroupElementsWithinIndexRange_TestCase), "cases")]
	public void GetGroupElementsWithinIndexRange_Various(int[] arraySize, int[] minIndex, int[] maxIndex, int[] expectedIndex){
		IUIElement[] elements = CreateElementsList(arraySize[0] * arraySize[1]);
		IUIElement[,] elementsArray = CreateElementsArray(arraySize[0], arraySize[1], elements);
		IGroupElementsArrayCalculator calculator = new GroupElementsArrayCalculator(elementsArray);

		IUIElement[] actual = calculator.GetGroupElementsWithinIndexRange(minIndex[0], minIndex[1], maxIndex[0], maxIndex[1]);

		IUIElement[] expected = CreateElementsListFromIndex(expectedIndex, elements);

		Assert.That(actual, Is.EqualTo(expected));
	}
	IUIElement[] CreateElementsList(int count){
		List<IUIElement> list = new List<IUIElement>();
		for(int i = 0; i < count; i ++)
			list.Add(Substitute.For<IUIElement>());
		return list.ToArray();
	}
	IUIElement[,] CreateElementsArray(int columnCount, int rowCount, IUIElement[] elementsList){
		IUIElement[,] result = new IUIElement[columnCount, rowCount];
		int index = 0;
		for(int j = 0; j < rowCount; j ++)
			for(int i = 0; i < columnCount; i ++)
				result[i, j] = elementsList[index++];
		return result;
	}
	IUIElement[] CreateElementsListFromIndex(int[] index, IUIElement[] source){
		List<IUIElement> list = new List<IUIElement>();
		foreach(int i in index)
			list.Add(source[i]);
		return list.ToArray();
	}
	public class GetGroupElementsWithinIndexRange_TestCase{
		public static object[] cases = {
			new object[]{
				new int[]{5, 5},
				new int[]{0, 0},
				new int[]{1, 1},
				new int[]{
					0, 1, 
					5, 6
				}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{0, 0},
				new int[]{2, 2},
				new int[]{
					0, 1, 2, 
					5, 6, 7, 
					10, 11, 12
				}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{1, 1},
				new int[]{4, 4},
				new int[]{
					6, 7, 8, 9,
					11, 12, 13, 14,
					16, 17, 18, 19,
					21, 22, 23, 24
				}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{2, 2},
				new int[]{0, 0},
				new int[]{}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{0, 0},
				new int[]{0, 0},
				new int[]{0}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{3, 4},
				new int[]{3, 4},
				new int[]{23}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{4, 4},
				new int[]{4, 4},
				new int[]{24}
			},
			new object[]{
				new int[]{5, 5},
				new int[]{4, 3},
				new int[]{4, 3},
				new int[]{19}
			},
		};
	}
}
