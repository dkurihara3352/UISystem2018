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
	public void CalcNumberOfColumnsToCreate_Various(
		int columnCountConstraint, 
		int rowCountConstraint, 
		int elementsCount, 
		int expected
	){
		IUIElementGroupConstArg arg = CreateMockConstArg();
		arg.columnCountConstraint.Returns(columnCountConstraint);
		arg.rowCountConstraint.Returns(rowCountConstraint);
		TestUIElementGroup uieGroup = new TestUIElementGroup(arg);

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
	public void CalcNumberOfRowsToCreate_Various(
		int columnCountConstraint, 
		int rowCountConstraint, 
		int elementsCount, 
		int expected
	){
		IUIElementGroupConstArg arg = CreateMockConstArg();
		arg.columnCountConstraint.Returns(columnCountConstraint);
		arg.rowCountConstraint.Returns(rowCountConstraint);
		TestUIElementGroup uieGroup = new TestUIElementGroup(arg);

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




	IUIElementGroupConstArg CreateMockConstArg(){
		IUIElementGroupConstArg arg = Substitute.For<IUIElementGroupConstArg>();
		arg.columnCountConstraint.Returns(3);
		arg.rowCountConstraint.Returns(0);
		arg.topToBottom.Returns(true);
		arg.leftToRight.Returns(true);
		arg.rowToColumn.Returns(true);
		arg.uim.Returns(Substitute.For<IUIManager>());
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
		arg.uia.Returns(Substitute.For<IUIElementGroupAdaptor>());
		arg.image.Returns(Substitute.For<IUIImage>());
		return arg;
	}

	public class TestUIElementGroup: AbsUIElementGroup<IUIElement>{
		public TestUIElementGroup(IUIElementGroupConstArg arg): base(arg){

		}
		protected override IScroller FindProximateParentScroller(){
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullScroller = null;
			parentScroller.GetProximateParentScroller().Returns(nullScroller);
			return parentScroller;
		}
		public void SetElements(List<IUIElement> elements){
			thisGroupElements = elements;
		}
		/* Test Exposure */
		public IUIElement[, ] thisElementsArray_Test{
			get{return thisElementsArray;}
			set{thisElementsArray = value;}
		}
		public void SetUpElements_Test(List<IUIElement> elements){
			this.SetUpElements(elements);
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
		public int GetArraySize_Test(int dimension){
			return this.GetArraySize(dimension);
		}
		public void PlaceElements_Test(){
			this.PlaceElements();
		}
	}

}
