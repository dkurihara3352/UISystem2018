using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ScrollerTest{
	[Test, TestCaseSource(typeof(Construction_ThisRelativeCursorPosition_TestCase), "cases")]
	public void Construction_ThisRelativeCursorPosition_IsForcedInRange(Vector2 given, Vector2 expected){
		Rect thisRect = new Rect(Vector2.zero, new Vector2(100f, 200f));
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateTestScrollerFull(out arg, new Vector2(10f, 20f), ScrollerAxis.Both, given, new Vector2(.1f, .1f), thisRect);

		Assert.That(testScroller.thisRelativeCursorPosition_Test, Is.EqualTo(expected));
	}
	public class Construction_ThisRelativeCursorPosition_TestCase{
		public static object[] cases = {
			new object[]{ new Vector2(0f, 0f), new Vector2(0f, 0f)},
			new object[]{ new Vector2(1f, 1f), new Vector2(1f, 1f)},
			new object[]{ new Vector2(.5f, .5f), new Vector2(.5f, .5f)},
			new object[]{ new Vector2(2f, 2f), new Vector2(1f, 1f)},
			new object[]{ new Vector2(10f, 10f), new Vector2(1f, 1f)},
			new object[]{ new Vector2(-1f, -1f), new Vector2(0f, 0f)},
			new object[]{ new Vector2(-.5f, -.5f), new Vector2(0f, 0f)},
			new object[]{ new Vector2(-10f, -10f), new Vector2(0f, 0f)}
		};
	}
	[Test, TestCaseSource(typeof(Construction_ThisRubberBandMultiplier_TestCase), "cases")]
	public void Construction_ThisRubberBandMultiplier_IsForcedInRage(Vector2 given, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect thisRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		TestScroller testScroller = CreateTestScrollerFull(out arg, new Vector2(20f, 10f), ScrollerAxis.Both, Vector2.zero, given, thisRect);

		Assert.That(testScroller.thisRubberBandLimitMultiplier_Test, Is.EqualTo(expected));
	}
	public class Construction_ThisRubberBandMultiplier_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(0f, 0f), new Vector2(.01f, .01f)},
			new object[]{new Vector2(-2f, -2f), new Vector2(.01f, .01f)},
			new object[]{new Vector2(.1f, .1f), new Vector2(.1f, .1f)},
			new object[]{new Vector2(1f, 1f), new Vector2(1f, 1f)},
			new object[]{new Vector2(10f, 10f), new Vector2(1f, 1f)},
		};
	}
	[Test, TestCaseSource(typeof(Construction_RectLengthIsZero_TestCase), "cases")]
	public void Construction_RectLengthIsZero_ThrowsException(Vector2 length){
		Rect thisRect = new Rect(Vector2.zero, length);
		ITestScrollerConstArg arg;

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("rect has at least one dimension not set right"), () => {CreateTestScrollerFull(out arg, new Vector2(20f, 10f), ScrollerAxis.Both, Vector2.zero, Vector2.one, thisRect);});
	}
	public class Construction_RectLengthIsZero_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(0f, 0f)},
			new object[]{new Vector2(0f, 1f)},
			new object[]{new Vector2(1f, 0f)},
		};
	}
	[Test, TestCaseSource(typeof(Construction_SetsThisRect_TestCase), "cases")]
	public void Construction_SetsThisRect(Vector2 scrollerLength){
		Rect scrollerRect = new Rect(Vector2.zero, scrollerLength);
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateTestScrollerFull(out arg, new Vector2(10f, 10f), ScrollerAxis.Both, Vector2.zero, Vector2.one, scrollerRect);

		Assert.That(testScroller.thisRect_Test, Is.EqualTo(scrollerRect));
		Assert.That(testScroller.thisRectLength_Test, Is.EqualTo(scrollerLength));
	}
	public class Construction_SetsThisRect_TestCase{
		static object[] cases = {
			new object[]{new Vector2(200f, 100f)},
			new object[]{new Vector2(2000f, 1000f)},
		};
	}
	/* Activation */
	[Test, TestCaseSource(typeof(Activate_CursorLengthGreaterThanScrollerLength_TestCase), "cases")]
	public void ActivateImple_CursorLengthGreaterThanScrollerLength_ClampTheLength(Vector2 scrollerLength, Vector2 cursorLength, Vector2 expected){
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateTestScrollerWithRectLength(out arg, scrollerLength, cursorLength);

		testScroller.ActivateImple();

		Vector2 actual = testScroller.thisCursorLength_Test;
		Assert.That(actual, Is.EqualTo(expected));
	}
	public class Activate_CursorLengthGreaterThanScrollerLength_TestCase{
		static object[] cases = {
			new object[]{new Vector2(200f, 100f), new Vector2(20f, 10f), new Vector2(20f, 10f)},
			new object[]{new Vector2(200f, 100f), new Vector2(210f, 110f), new Vector2(200f, 100f)}
		};
	}
	[Test, TestCaseSource(typeof(ActivateImple_RelativeCursorPos_Various_TestCase), "cases")]
	public void ActivateImple_RelativeCursorPos_Various(Vector2 scrollerLength, Vector2 cursorLength, Vector2 relativeCursorPos, Vector2 expectedPos){
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateTestScrollerWithRectData(out arg, scrollerLength, cursorLength, relativeCursorPos);

		testScroller.ActivateImple();

		Vector2 actual = testScroller.thisCursorLocalPosition_Test;
		Assert.That(actual, Is.EqualTo(expectedPos));
	}
	public class ActivateImple_RelativeCursorPos_Various_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(200f, 100f), new Vector2(200f, 100f), Vector2.zero, Vector2.zero},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), Vector2.one, new Vector2(100f, 50f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(.5f, .5f), new Vector2(50f, 25f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(-1f, -10f), new Vector2(0f, 0f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(20f, 60f), new Vector2(100f, 50f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(.2f, .8f), new Vector2(20f, 40f)},
			new object[]{new Vector2(200f, 100f), new Vector2(50f, 50f), new Vector2(.5f, .5f), new Vector2(75f, 25f)},
		};
	}
	[Test]
	public void ActivateImple_ChildrenNull_ThrowsError(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForActivation(out arg);
		List<IUIElement> returnedList = null;
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.NullReferenceException)).And.Message.EqualTo("childUIEs must not be null"), ()=> {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_ChildrenCountNotOne_ThrowsError([Values(0, 2, 5)]int childCount){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForActivation(out arg);
		List<IUIElement> returnedList = new List<IUIElement>();
		for(int i = 0; i < childCount; i++)
			returnedList.Add(Substitute.For<IUIElement>());
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller must have only one UIE child as Scroller Element"), () => {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_FirstChildIsNull_ThrowsError(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForActivation(out arg);
		IUIElement returnedChild = null;
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{returnedChild});
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller's only child must not be null"), () => {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_OnlyChildNotNull_DoesNotThrowException(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForActivation(out arg);
		IUIElement child = Substitute.For<IUIElement>();
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.DoesNotThrow(()=>{scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_SetsScrollerElementRectFields(){
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateValidTestScrollerForDrag(out arg, ScrollerAxis.Both);
		Rect actualRect = arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect();
		Vector2 actualLength = new Vector2(100f, 100f);

		testScroller.ActivateImple();

		Assert.That(testScroller.thisScrollerElementRect_Test, Is.EqualTo(actualRect));
		Assert.That(testScroller.thisScrollerElementLength_Test, Is.EqualTo(actualLength));
	}
	/* Dragging */
	[Test]
	public void ThisIsNotDragged_Initially_IsTrue(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForDrag(out arg, ScrollerAxis.Both);
		scroller.ActivateImple();

		Assert.That(scroller.thisIsNotDragged_Test, Is.True);
	}
	[Test, TestCaseSource(typeof(OnDragImple_WhenCalled_SetsDragAxisVarious_TestCase), "cases")]
	public void OnDragImple_WhenCalled_SetsDragAxisVarious(Vector2 dragDelta, bool expIsNotDrag, bool expIsDraggedHor, bool expIsDraggedVer){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForDrag(out arg, ScrollerAxis.Both);
		scroller.ActivateImple();
		
		ICustomEventData eventData = CreateCustomEventDataFromDelta(dragDelta);
		scroller.OnDragImple_Test(eventData);

		Assert.That(scroller.thisIsNotDragged_Test, Is.EqualTo(expIsNotDrag));
		Assert.That(scroller.thisIsDraggedHorizontally_Test, Is.EqualTo(expIsDraggedHor));
		Assert.That(scroller.thisIsDraggedVertically_Test, Is.EqualTo(expIsDraggedVer));
	}
	public class OnDragImple_WhenCalled_SetsDragAxisVarious_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(.1f, 0f), false, true, false},
			new object[]{new Vector2(0f, .1f), false, false, true},
			new object[]{new Vector2(.1f, .1f), false, true, false}
		};
	}
	[Test, TestCaseSource(typeof(ThisProcessedDrag_TestCase), "cases")]
	public void OnDragImple_Various_MakesThisShouldProcessDragVarious(ScrollerAxis scrollerAxis, Vector2 dragDelta, bool expected){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateValidTestScrollerForDrag(out arg, scrollerAxis);
		scroller.ActivateImple();

		ICustomEventData data = CreateCustomEventDataFromDelta(dragDelta);
		scroller.OnDragImple_Test(data);

		Assert.That(scroller.thisShouldProcessDrag_Test, Is.EqualTo(expected));
	}
	public class ThisProcessedDrag_TestCase{
		public static object[] cases = {
			new object[]{ScrollerAxis.Both, new Vector2(.1f, .1f), true},
			new object[]{ScrollerAxis.Both, new Vector2(.1f, 0f), true},
			new object[]{ScrollerAxis.Both, new Vector2(0f, 10f), true},
			new object[]{ScrollerAxis.Horizontal, new Vector2(10f, 0f), true},
			new object[]{ScrollerAxis.Horizontal, new Vector2(0f, 10f), false},
			new object[]{ScrollerAxis.Vertical, new Vector2(10f, 0f), false},
			new object[]{ScrollerAxis.Vertical, new Vector2(0f, 10f), true},
		};
	}
	[Test]
	public void OnDragImple_NotShouldProcessImple_CallsParentUIEOnDrag(){
		ITestScrollerConstArg arg;
		ScrollerAxis scrollerAxis = ScrollerAxis.Horizontal;
		TestScroller testScroller = CreateValidTestScrollerForDrag(out arg, scrollerAxis);
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);
		Vector2 dragDelta = new Vector2(0f, 1f);
		ICustomEventData data = CreateCustomEventDataFromDelta(dragDelta);
		testScroller.ActivateImple();
		
		testScroller.OnDragImple_Test(data);
		
		Assert.That(testScroller.thisShouldProcessDrag_Test, Is.False);
		parentUIE.Received(1).OnDrag(data);
	}
	[Test, TestCaseSource(typeof(ElementIsUndersizedTo_Various_TestCase), "cases")]
	public void ElementIsUndersizedTo_Various(Vector2 referenceLength, int dimension, bool expectedBool){
		ITestScrollerConstArg arg;
		TestScroller testScroller = CreateValidTestScrollerForDrag(out arg, ScrollerAxis.Both);
		testScroller.ActivateImple();

		bool actual = testScroller.ElementIsUndersizedTo_Test(referenceLength, dimension);

		Assert.That(actual, Is.EqualTo(expectedBool));
	}
	public class ElementIsUndersizedTo_Various_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(100f, 100f), 0, true},
			new object[]{new Vector2(100f, 100f), 1, true},
			new object[]{new Vector2(99f, 99f), 0, false},
			new object[]{new Vector2(99f, 99f), 1, false},
			new object[]{new Vector2(101f, 101f), 0, true},
			new object[]{new Vector2(101f, 101f), 1, true}

		};
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "cursorCases")]
	public void GetElementPositionNormalizedToCursor_ElementNotUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);

		testScroller.ActivateImple();
		
		for(int i = 0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.False);
			float actual = testScroller.GetElementPositionNormalizedToCursor_Test(elementLocalPos[i], i);
			
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class GetElementNormalizedPosition_TestCase{
		public static object[] cursorCases = {
			new object[]{new Vector2(75f, 25f), new Vector2(0f, 0f)},
			new object[]{new Vector2(25f, -25f), new Vector2(1f, 1f)},
			new object[]{new Vector2(50f, 0f), new Vector2(.5f, .5f)},
			new object[]{new Vector2(75f, -25f), new Vector2(0f, 1f)},
			new object[]{new Vector2(25f, 25f), new Vector2(1f, 0f)},
			new object[]{new Vector2(75f, 50f), new Vector2(0f, -0.5f)},
			new object[]{new Vector2(100f, 50f), new Vector2(-0.5f, -0.5f)},
			new object[]{new Vector2(0f, -50f), new Vector2(1.5f, 1.5f)},
			new object[]{new Vector2(-25f, -75f), new Vector2(2f, 2f)},
			new object[]{new Vector2(125f, 75f), new Vector2(-1f, -1f)},
		};
		public static object[] cursorZeroCases = {
			new object[]{new Vector2(75f, 25f)},
			new object[]{new Vector2(25f, -25f)},
			new object[]{new Vector2(50f, 0f)},
			new object[]{new Vector2(75f, -25f)},
			new object[]{new Vector2(25f, 25f)},
			new object[]{new Vector2(75f, 50f)},
			new object[]{new Vector2(100f, 50f)},
			new object[]{new Vector2(0f, -50f)},
			new object[]{new Vector2(-25f, -75)},
			new object[]{new Vector2(125f, 75f)},
		};
		public static object[] scrollerCases = {
			new object[]{new Vector2(0f, 0f), new Vector2(0f, 0f)},
			new object[]{new Vector2(-100f, -50f), new Vector2(1f, 1f)},
			new object[]{new Vector2(-100f, 0f), new Vector2(1f, 0f)},
			new object[]{new Vector2(0f, -50f), new Vector2(0f, 1f)},
			new object[]{new Vector2(-50f, -25f), new Vector2(.5f, .5f)},
			new object[]{new Vector2(100f, 0f), new Vector2(-1f, 0f)},
			new object[]{new Vector2(200f, 0f), new Vector2(-2f, 0f)},
			new object[]{new Vector2(-100f, 0f), new Vector2(1f, 0f)},
			new object[]{new Vector2(-200f, 0f), new Vector2(2f, 0f)},
			new object[]{new Vector2(0f, 50f), new Vector2(0f, -1f)},
			new object[]{new Vector2(0f, 100f), new Vector2(0f, -2f)},
			new object[]{new Vector2(0f, -50f), new Vector2(0f, 1f)},
			new object[]{new Vector2(0f, -100f), new Vector2(0f, 2f)},
		};
		public static object[] scrollerZeroCases = {
			new object[]{new Vector2(0f, 0f)},
			new object[]{new Vector2(-100f, -50f)},
			new object[]{new Vector2(-100f, 0f)},
			new object[]{new Vector2(0f, -50f)},
			new object[]{new Vector2(-50f, -25f)},
			new object[]{new Vector2(100f, 0f)},
			new object[]{new Vector2(200f, 0f)},
			new object[]{new Vector2(-100f, 0f)},
			new object[]{new Vector2(-200f, 0f)},
			new object[]{new Vector2(0f, 50f)},
			new object[]{new Vector2(0f, 100f)},
			new object[]{new Vector2(0f, -50f)},
			new object[]{new Vector2(0f, -100f)},
		};
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "cursorZeroCases")]
	public void GetElementPositionNormalizedToCursor_ElementIsUndersized_ReturnsZero(Vector2 elementLocalPos){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(45f, 45f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.True);
			float actual = testScroller.GetElementPositionNormalizedToCursor_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "scrollerCases")]
	public void GetElementPositionNormalizedToScroller_ElementIsNotUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(300f, 150f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);

		testScroller.ActivateImple();
		
		for(int i = 0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(scrollerRect.size, i), Is.False);
			float actual = testScroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);
			
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "scrollerZeroCases")]
	public void GetElementPositionNormalizedToScroller_ElementIsUndersized_ReturnsZero(Vector2 elementLocalPos){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(scrollerRect.size, i), Is.True);
			float actual = testScroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	[Test, TestCaseSource(typeof(GetElementCursorOffsetInPixel_TestCase), "cases")]
	public void GetElementCursorOffsetInPixel_ElementIsNotUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.False);
			float actual = testScroller.GetElementCursorOffsetInPixel_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetElementCursorOffsetInPixel_TestCase), "undersizedCases")]
	public void GetElementCursorOffsetInPixel_ElementIsUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(50f, 50f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.True);
			float actual = testScroller.GetElementCursorOffsetInPixel_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class GetElementCursorOffsetInPixel_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(75f, 25f), new Vector2(0f, 0f)},
			new object[]{new Vector2(100f, 25f), new Vector2(-25f, 0f)},
			new object[]{new Vector2(75f, 50f), new Vector2(0f, -25f)},
			new object[]{new Vector2(100f, 50f), new Vector2(-25f, -25f)},
			new object[]{new Vector2(125f, 75f), new Vector2(-50f, -50f)},
			new object[]{new Vector2(50f, 0f), new Vector2(0f, 0f)},
			new object[]{new Vector2(25f, -25f), new Vector2(0f, 0f)},
			new object[]{new Vector2(0f, -25f), new Vector2(25f, 0f)},
			new object[]{new Vector2(0f, -50f), new Vector2(25f, 25f)},
			new object[]{new Vector2(-25f, -75f), new Vector2(50f, 50f)},
		};
		public static object[] undersizedCases = {
			new object[]{new Vector2(75f, 25f), new Vector2(0f, 0f)},
			new object[]{new Vector2(100f, 25f), new Vector2(-25f, 0f)},
			new object[]{new Vector2(125f, 25f), new Vector2(-50f, 0f)},
			new object[]{new Vector2(75f, 50f), new Vector2(0f, -25f)},
			new object[]{new Vector2(75f, 75f), new Vector2(0f, -50f)},
			new object[]{new Vector2(100f, 50f), new Vector2(-25f, -25f)},

			new object[]{new Vector2(50f, 25f), new Vector2(25f, 0f)},
			new object[]{new Vector2(25f, 25f), new Vector2(50f, 0f)},
			new object[]{new Vector2(50f, 0f), new Vector2(25f, 25f)},
			new object[]{new Vector2(75f, 0f), new Vector2(0f, 25f)},
			new object[]{new Vector2(75f, -25f), new Vector2(0f, 50f)},
			new object[]{new Vector2(25f, -25f), new Vector2(50f, 50f)},
		};
	}
	[Test, TestCaseSource(typeof(GetPosNormalizedToCursorFromPosInElementSpace_TestCase), "cases")]
	public void GetPosNormalizedToCursorFromPosInElementSpace_ElementIsNotUndersized_Various(Vector2 positionInElementSpace, Vector2 expected){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.False);
			float actual = testScroller.GetPosNormalizedToCursorFromPosInElementSpace_Test(positionInElementSpace[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetPosNormalizedToCursorFromPosInElementSpace_TestCase), "undersizedCases")]
	public void GetPosNormalizedToCursorFromPosInElementSpace_ElementIsUndersized_ReturnsZero(Vector2 positionInElementSpace){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(50f, 50f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(cursorLength, i), Is.True);
			float actual = testScroller.GetPosNormalizedToCursorFromPosInElementSpace_Test(positionInElementSpace[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	public class GetPosNormalizedToCursorFromPosInElementSpace_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(0f, 0f), new Vector2(0f, 0f)},
			new object[]{new Vector2(50f, 50f), new Vector2(1f, 1f)},
			new object[]{new Vector2(25f, 25f), new Vector2(.5f, .5f)},
			new object[]{new Vector2(50f, 0f), new Vector2(1f, 0f)},
			new object[]{new Vector2(0f, 50f), new Vector2(0f, 1f)},

			new object[]{new Vector2(-25f, -25f), new Vector2(-.5f, -.5f)},
			new object[]{new Vector2(75f, -25f), new Vector2(1.5f, -.5f)},
			new object[]{new Vector2(-25f, 75f), new Vector2(-.5f, 1.5f)},
			new object[]{new Vector2(75f, 75f), new Vector2(1.5f, 1.5f)},
		};
		public static object[] undersizedCases = {
			new object[]{new Vector2(0f, 0f)},
			new object[]{new Vector2(50f, 50f)},
			new object[]{new Vector2(25f, 25f)},
			new object[]{new Vector2(50f, 0f)},
			new object[]{new Vector2(0f, 50f)},

			new object[]{new Vector2(-25f, -25f)},
			new object[]{new Vector2(75f, -25f)},
			new object[]{new Vector2(-25f, 75f)},
			new object[]{new Vector2(75f, 75f)},

		};
	}
	[Test, TestCaseSource(typeof(ElementIsDisplacedInDragDirection_TestCase), "cases")]
	public void ElementIsDisplacedInDragDirection_Various(Vector2 elementLocalPosition, Vector2 dragDelta, bool[] expected){
		TestScroller testScroller = CreateOversizedTestScrollerReadyForDrag();

		for(int i = 0; i < 2; i ++){
			bool actual = testScroller.ElementIsDraggedToIncreaseCursorOffset_Test(dragDelta[i], elementLocalPosition[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class ElementIsDisplacedInDragDirection_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(76f, 26f), new Vector2(-1f, -1f), new bool[]{false, false}},
			new object[]{new Vector2(76f, 26f), new Vector2(1f, 1f), new bool[]{true, true}},
			new object[]{new Vector2(76f, 26f), new Vector2(1f, -1f), new bool[]{true, false}},
			new object[]{new Vector2(76f, 26f), new Vector2(-1f, 1f), new bool[]{false, true}},
			
			new object[]{new Vector2(50f, 0f), new Vector2(-1f, -1f), new bool[]{false, false}},
			new object[]{new Vector2(50f, 0f), new Vector2(1f, 1f), new bool[]{false, false}},
			new object[]{new Vector2(50f, 0f), new Vector2(1f, -1f), new bool[]{false, false}},
			new object[]{new Vector2(50f, 0f), new Vector2(-1f, 1f), new bool[]{false, false}},
			
			new object[]{new Vector2(24f, 26f), new Vector2(-1f, -1f), new bool[]{true, false}},
			new object[]{new Vector2(24f, 26f), new Vector2(1f, 1f), new bool[]{false, true}},
			new object[]{new Vector2(24f, 26f), new Vector2(1f, -1f), new bool[]{false, false}},
			new object[]{new Vector2(24f, 26f), new Vector2(-1f, 1f), new bool[]{true, true}},
			
			new object[]{new Vector2(24f, -26f), new Vector2(-1f, -1f), new bool[]{true, true}},
			new object[]{new Vector2(24f, -26f), new Vector2(1f, 1f), new bool[]{false, false}},
			new object[]{new Vector2(24f, -26f), new Vector2(1f, -1f), new bool[]{false, true}},
			new object[]{new Vector2(24f, -26f), new Vector2(-1f, 1f), new bool[]{true, false}},
			
			new object[]{new Vector2(76f, -26f), new Vector2(-1f, -1f), new bool[]{false, true}},
			new object[]{new Vector2(76f, -26f), new Vector2(1f, 1f), new bool[]{true, false}},
			new object[]{new Vector2(76f, -26f), new Vector2(1f, -1f), new bool[]{true, true}},
			new object[]{new Vector2(76f, -26f), new Vector2(-1f, 1f), new bool[]{false, false}},
		};
	}
	// [Test, TestCaseSource(typeof(CalcRubberBandedPosOnAxis_Demo_TestCase), "cases")]
	public void CalcRubberBandedPosOnAxis_Demo(Vector2 localPos){
		TestScroller testScroller = CreateOversizedTestScrollerReadyForDrag();

		Vector2 rubberBandedV2 = new Vector2();
		for(int i = 0; i < 2; i ++){
			float actual = testScroller.CalcRubberBandedPosOnAxis_Test(localPos[i], i);
			rubberBandedV2[i] = actual;
		}
		Debug.Log("source: " + localPos.ToString() + ", rubbered: " + rubberBandedV2.ToString());
	}
	public class CalcRubberBandedPosOnAxis_Demo_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(76f, 26f)},
			new object[]{new Vector2(100f, 50f)},
			new object[]{new Vector2(125f, 75f)},
			new object[]{new Vector2(175f, 125f)},
			new object[]{new Vector2(225f, 175f)},
			
			new object[]{new Vector2(76f, -26f)},
			new object[]{new Vector2(100f, -50f)},
			new object[]{new Vector2(150f, -100f)},
			new object[]{new Vector2(200f, -150f)},
			new object[]{new Vector2(250f, -200f)},
			
			new object[]{new Vector2(24f, 26f)},
			new object[]{new Vector2(-25f, 75f)},
			new object[]{new Vector2(-75f, 125f)},
			new object[]{new Vector2(-125f, 175f)},
			new object[]{new Vector2(-175f, 225f)},
			
			new object[]{new Vector2(24f, -26f)},
			new object[]{new Vector2(-25f, -75f)},
			new object[]{new Vector2(-75f, -125f)},
			new object[]{new Vector2(-125f, -175f)},
			new object[]{new Vector2(-175f, -225f)},
			new object[]{new Vector2(-1750f, -2250f)},
		};
	}






	public class TestScroller: AbsScroller, INonActivatorUIElement{
		public TestScroller(ITestScrollerConstArg arg): base(arg){
			thisTestCursorLength = arg.cursorLength;
		}
		readonly Vector2 thisTestCursorLength;
		protected override Vector2 CalcCursorLength(){
			return thisTestCursorLength;
		}
		protected override bool thisShouldApplyRubberBand{get{return true;}}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		protected override Vector2 GetInitialPositionNormalizedToCursor(){return Vector2.zero;}
		/* Test exposures */
		public Vector2 thisRelativeCursorPosition_Test{get{return thisRelativeCursorPosition;}}
		public Vector2 thisRubberBandLimitMultiplier_Test{get{return thisRubberBandLimitMultiplier;}}
		public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
		public Rect thisRect_Test{get{return thisRect;}}
		public Vector2 thisRectLength_Test{get{return thisRectLength;}}
		public Vector2 thisCursorLocalPosition_Test{get{return thisCursorLocalPosition;}}
		public Rect thisScrollerElementRect_Test{get{return thisScrollerElementRect;}}
		public Vector2 thisScrollerElementLength_Test{get{return thisScrollerElementLength;}}
		public bool thisIsNotDragged_Test{get{return thisIsNotDragged;}}
		public bool thisIsDraggedHorizontally_Test{get{return thisIsDraggedHorizontally;}}
		public bool thisIsDraggedVertically_Test{get{return thisIsDraggedVertically;}}
		public bool thisProcessedDrag_Test{get{return thisProcessedDrag;}}
		public void OnDragImple_Test(ICustomEventData eventData){
			this.OnDragImple(eventData);
		}
		public bool thisShouldProcessDrag_Test{get{return thisShouldProcessDrag;}}
		public bool ElementIsUndersizedTo_Test(Vector2 referenceLength, int dimension){
			return this.ElementIsUndersizedTo(referenceLength, dimension);
		}
		public float GetElementPositionNormalizedToCursor_Test(float elementLocalPosOnAxis, int dimension){
			return GetElementPositionNormalizedToCursor(elementLocalPosOnAxis, dimension);
		}
		public float GetElementPositionNormalizedToScroller_Test(float elementLocalPosOnAxis, int dimension){
			return GetElementPositionNormalizedToScroller(elementLocalPosOnAxis, dimension);
		}
		public float GetElementCursorOffsetInPixel_Test(float elementLocalPosOnAxis, int dimension){
			return GetElementCursorOffsetInPixel(elementLocalPosOnAxis, dimension);
		}
		public float GetPosNormalizedToCursorFromPosInElementSpace_Test(float positionInElementSpaceOnAxis, int dimension){
			return GetPosNormalizedToCursorFromPosInElementSpace(positionInElementSpaceOnAxis, dimension);
		}
		public bool ElementIsDraggedToIncreaseCursorOffset_Test(float deltaPosOnAxis, float elementLocalPosOnAxis, int dimension){
			return this.ElementIsDraggedToIncreaseCursorOffset(deltaPosOnAxis, elementLocalPosOnAxis, dimension);
		}
		public float CalcRubberBandedPosOnAxis_Test(float localPosOnAxis, int dimension){
			return CalcRubberBandedPosOnAxis(localPosOnAxis, dimension);
		}
	}
	public interface ITestScrollerConstArg: IScrollerConstArg{
		Vector2 cursorLength{get;}
	}
	public class TestScrollerConstArg: ScrollerConstArg, ITestScrollerConstArg{
		public TestScrollerConstArg(Vector2 cursorLength, ScrollerAxis scrollerAxis, Vector2 relativeCursorPosition, Vector2 rubberBandLimitMultiplier, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IScrollerAdaptor uia, IUIImage uiImage): base(scrollerAxis, relativeCursorPosition, rubberBandLimitMultiplier, uim, processFactory, uieFactory, uia, uiImage){
			thisCursorLength = cursorLength;
		}
		readonly Vector2 thisCursorLength;
		public Vector2 cursorLength{get{return thisCursorLength;}}
	}
	TestScroller CreateValidTestScrollerForActivation(out ITestScrollerConstArg arg){
		ITestScrollerConstArg thisArg;
		TestScroller testScroller = CreateTestScrollerWithRectData(out thisArg, new Vector2(200f, 100f), new Vector2(50f, 50f), new Vector2(.5f, .5f));
		arg = thisArg;
		return testScroller;
	}
	TestScroller CreateTestScrollerFull(out ITestScrollerConstArg arg, Vector2 cursorLength, ScrollerAxis scrollerAxis, Vector2 relativeCursorPosition, Vector2 rubberBandLimitMultiplier, Rect scrollerRect){
		IUIManager uim = Substitute.For<IUIManager>();
		IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
		IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
		IScrollerAdaptor scrollerAdaptor = Substitute.For<IScrollerAdaptor>();
			scrollerAdaptor.GetRect().Returns(scrollerRect);
		IUIImage image = Substitute.For<IUIImage>();
		
		ITestScrollerConstArg thisArg = new TestScrollerConstArg(cursorLength, scrollerAxis, relativeCursorPosition, rubberBandLimitMultiplier, uim, processFactory, uieFactory, scrollerAdaptor, image);
		arg = thisArg;
		TestScroller scroller = new TestScroller(thisArg);
		return scroller;
	}
	TestScroller CreateTestScrollerWithRectLength(out ITestScrollerConstArg arg, Vector2 scrollerLength, Vector2 cursorLength){
		Vector2 relativeCursorPos = Vector2.zero;
		ITestScrollerConstArg thisArg;
		TestScroller testScroller = CreateTestScrollerWithRectData(out thisArg, scrollerLength, cursorLength, relativeCursorPos);

		arg = thisArg;
		return testScroller;
	}
	TestScroller CreateTestScrollerWithRectData(out ITestScrollerConstArg arg, Vector2 scrollerLength, Vector2 cursorLength, Vector2 relativeCursorPos){
		Rect scrollerRect = new Rect(Vector2.zero, scrollerLength);
		ITestScrollerConstArg thisArg;
		TestScroller testScroller = CreateTestScrollerFull(out thisArg, cursorLength, ScrollerAxis.Both, relativeCursorPos, Vector2.one, scrollerRect);
		IScrollerAdaptor scrollerAdaptor = (IScrollerAdaptor)thisArg.uia;
		IUIElement child = Substitute.For<IUIElement>();
		List<IUIElement> returnedList = new List<IUIElement>( new IUIElement[]{child});
		scrollerAdaptor.GetChildUIEs().Returns(returnedList);

		arg = thisArg;
		return testScroller;
	}
	TestScroller CreateValidTestScrollerForDrag(out ITestScrollerConstArg arg, ScrollerAxis scrollerAxis){
		ITestScrollerConstArg thisArg;
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		TestScroller testScroller = CreateTestScrollerFull(out thisArg, cursorLength, scrollerAxis, relativeCursorPos, rubberBandLimitMultiplier, scrollerRect);
		IUIElement scrollerElement = Substitute.For<IUIElement>();
		IScrollerAdaptor scrollerElementAdaptor = Substitute.For<IScrollerAdaptor>();
		Rect scrollerElementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		scrollerElementAdaptor.GetRect().Returns(scrollerElementRect);
		scrollerElement.GetUIAdaptor().Returns(scrollerElementAdaptor);
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{scrollerElement});
		thisArg.uia.GetChildUIEs().Returns(returnedList);
		arg = thisArg;
		return testScroller;
	}
	public ICustomEventData CreateCustomEventDataFromDelta(Vector2 deltaPos){
		ICustomEventData data = new CustomEventData(Vector2.zero, deltaPos);
		return data;
	}
	TestScroller CreateValidTestScrollerForDragWithRectData(out ITestScrollerConstArg arg, ScrollerAxis scrollerAxis, Rect scrollerRect, Vector2 cursorLength, Vector2 relativeCursorPos, Rect elementRect){
		ITestScrollerConstArg thisArg;
		Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
		TestScroller testScroller = CreateTestScrollerFull(out thisArg, cursorLength, scrollerAxis, relativeCursorPos, rubberBandLimitMultiplier, scrollerRect);
		IUIElement scrollerElement = Substitute.For<IUIElement>();
		IScrollerAdaptor scrollerElementAdaptor = Substitute.For<IScrollerAdaptor>();
		scrollerElementAdaptor.GetRect().Returns(elementRect);
		scrollerElement.GetUIAdaptor().Returns(scrollerElementAdaptor);
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{scrollerElement});
		thisArg.uia.GetChildUIEs().Returns(returnedList);
		arg = thisArg;
		return testScroller;
	}
	TestScroller CreateOversizedTestScrollerReadyForDrag(){
		ITestScrollerConstArg arg;
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		TestScroller testScroller = CreateValidTestScrollerForDragWithRectData(out arg, ScrollerAxis.Both, scrollerRect, cursorLength, relativeCursorPos, elementRect);
		testScroller.ActivateImple();
		return testScroller;
	}
}
