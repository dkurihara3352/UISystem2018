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
	public void Construction_ThisRelativeCursorPosition_IsForcedInRange(Vector2 relativeCursorPosition, Vector2 expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.relativeCursorPosition.Returns(relativeCursorPosition);
		TestScroller scroller = new TestScroller(arg);


		Assert.That(scroller.thisRelativeCursorPosition_Test, Is.EqualTo(expected));
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
	public void Construction_ThisRubberBandMultiplier_IsForcedInRage(Vector2 rubberBandLimitMultiplier, Vector2 expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.rubberBandLimitMultiplier.Returns(rubberBandLimitMultiplier);
		TestScroller scroller = new TestScroller(arg);

		Assert.That(scroller.thisRubberBandLimitMultiplier_Test, Is.EqualTo(expected));
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
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetRect().Returns(thisRect);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("rect has at least one dimension not set right"), 
			() => {
				new TestScroller(arg);
			}
		);
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
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetRect().Returns(scrollerRect);

		TestScroller testScroller = new TestScroller(arg);

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
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
		arg.cursorLength.Returns(cursorLength);
		TestScroller testScroller = new TestScroller(arg);

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
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(relativeCursorPos);
		TestScroller testScroller = new TestScroller(arg);

		testScroller.ActivateImple();

		Vector2 actual = testScroller.thisCursorLocalPosition_Test;
		Assert.That(actual, Is.EqualTo(expectedPos));
	}
	public class ActivateImple_RelativeCursorPos_Various_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(200f, 100f), new Vector2(200f, 100f), Vector2.zero, Vector2.zero},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), Vector2.one, new Vector2(100f, 50f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(.5f, .5f), new Vector2(50f, 25f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(0f, 0f), new Vector2(0f, 0f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(1f, 1f), new Vector2(100f, 50f)},
			new object[]{new Vector2(200f, 100f), new Vector2(100f, 50f), new Vector2(.2f, .8f), new Vector2(20f, 40f)},
			new object[]{new Vector2(200f, 100f), new Vector2(50f, 50f), new Vector2(.5f, .5f), new Vector2(75f, 25f)},
		};
	}
	[Test]
	public void ActivateImple_ChildrenNull_ThrowsError(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		List<IUIElement> returnedList = null;
		arg.uia.GetChildUIEs().Returns(returnedList);
		TestScroller scroller = new TestScroller(arg);

		Assert.Throws(Is.TypeOf(typeof(System.NullReferenceException)).And.Message.EqualTo("childUIEs must not be null"), ()=> {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_ChildrenCountNotOne_ThrowsError([Values(0, 2, 5)]int childCount){
		ITestScrollerConstArg arg = CreateMockConstArg();
		List<IUIElement> returnedList = new List<IUIElement>();
		for(int i = 0; i < childCount; i++)
			returnedList.Add(Substitute.For<IUIElement>());
		arg.uia.GetChildUIEs().Returns(returnedList);
		TestScroller scroller = new TestScroller(arg);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller must have only one UIE child as Scroller Element"), () => {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_FirstChildIsNull_ThrowsError(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		IUIElement returnedChild = null;
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{returnedChild});
		arg.uia.GetChildUIEs().Returns(returnedList);
		TestScroller scroller = new TestScroller(arg);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller's only child must not be null"), () => {scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_OnlyChildNotNull_DoesNotThrowException(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		IUIElement child = Substitute.For<IUIElement>();
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
		arg.uia.GetChildUIEs().Returns(returnedList);
		TestScroller scroller = new TestScroller(arg);

		Assert.DoesNotThrow(()=>{scroller.ActivateImple();});
	}
	[Test]
	public void ActivateImple_SetsScrollerElementRectFields(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Vector2 actualLength = new Vector2(100f, 200f);
		Rect actualRect = new Rect(Vector2.zero, actualLength);
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(actualRect);
		TestScroller testScroller = new TestScroller(arg);

		testScroller.ActivateImple();

		Assert.That(testScroller.thisScrollerElementRect_Test, Is.EqualTo(actualRect));
		Assert.That(testScroller.thisScrollerElementLength_Test, Is.EqualTo(actualLength));
	}
	/* Dragging */
	[Test]
	public void ThisHasDoneDragEvaluation_Initially_IsFalse(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		TestScroller scroller = new TestScroller(arg);

		Assert.That(scroller.thisHasDoneDragEvaluation_Test, Is.False);
	}
	[Test]
	public void ThisShouldProcessDrag_Initially_IsFalse(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		TestScroller scroller = new TestScroller(arg);

		Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
	}
	[Test, TestCaseSource(typeof(OnDragImple_WhenCalled_SetsShouldProcessDragVarious_TestCase), "cases")]
	public void OnDragImple_WhenCalled_SetsShouldProcessDragVarious(ScrollerAxis scrollerAxis, Vector2 dragDelta, bool expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(scrollerAxis);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();

		scroller.OnDragImple_Test(CreateCustomEventDataFromDelta(dragDelta));

		Assert.That(scroller.thisShouldProcessDrag_Test, Is.EqualTo(expected));
	}
	public class OnDragImple_WhenCalled_SetsShouldProcessDragVarious_TestCase{
		public static object[] cases = {
			new object[]{ScrollerAxis.Both, new Vector2(.1f, 0f), true},
			new object[]{ScrollerAxis.Both, new Vector2(0f, .1f), true},
			new object[]{ScrollerAxis.Horizontal, new Vector2(.1f, 0f), true},
			new object[]{ScrollerAxis.Horizontal, new Vector2(.1f, .1f), true},
			new object[]{ScrollerAxis.Horizontal, new Vector2(0f, .1f), false},
			new object[]{ScrollerAxis.Vertical, new Vector2(.1f, 0f), false},
			new object[]{ScrollerAxis.Vertical, new Vector2(.1f, .1f), false},
			new object[]{ScrollerAxis.Vertical, new Vector2(0f, .1f), true},
		};
	}
	[Test, TestCaseSource(typeof(ThisProcessedDrag_TestCase), "cases")]
	public void OnDragImple_Various_MakesThisShouldProcessDragVarious(ScrollerAxis scrollerAxis, Vector2 dragDelta, bool expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(scrollerAxis);
		TestScroller scroller = new TestScroller(arg);
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
	public void OnDragImple_NotShouldProcessDrag_CallsParentUIEOnDrag(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(ScrollerAxis.Horizontal);
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);
		Vector2 dragDelta = new Vector2(0f, 1f);
		ICustomEventData data = CreateCustomEventDataFromDelta(dragDelta);
		TestScroller testScroller = new TestScroller(arg);
		testScroller.ActivateImple();
		
		testScroller.OnDragImple_Test(data);
		
		Assert.That(testScroller.thisShouldProcessDrag_Test, Is.False);
		parentUIE.Received(1).OnDrag(data);
	}
	[Test, TestCaseSource(typeof(ElementIsUndersizedTo_Various_TestCase), "cases")]
	public void ElementIsUndersizedTo_Various(Vector2 referenceLength, int dimension, bool expectedBool){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(new Rect(Vector2.zero, new Vector2(100f, 100f)));
		TestScroller testScroller = new TestScroller(arg);
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
		TestScroller testScroller = CreateTestScrollerOversizedToCursor();

		testScroller.ActivateImple();
		
		for(int i = 0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.False);
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
		TestScroller testScroller = CreateTestScrollerUndersizedToCursor();
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.True);
			float actual = testScroller.GetElementPositionNormalizedToCursor_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	TestScroller CreateTestScrollerUndersizedToCursor(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		Vector2 cursorLength = new Vector2(50f, 50f);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(new Vector2(.5f, .5f));
		Rect elementRect = new Rect(Vector2.zero, new Vector2(45f, 45f));
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		TestScroller testScroller = new TestScroller(arg);
		return testScroller;
	}
	TestScroller CreateTestScrollerUndersizedToScroller(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		Vector2 cursorLength = new Vector2(50f, 50f);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(new Vector2(.5f, .5f));
		Rect elementRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		TestScroller testScroller = new TestScroller(arg);
		return testScroller;
	}
	TestScroller CreateTestScrollerOversizedToScroller(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		Vector2 cursorLength = new Vector2(50f, 50f);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(new Vector2(.5f, .5f));
		Rect elementRect = new Rect(Vector2.zero, new Vector2(300f, 150f));
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		TestScroller testScroller = new TestScroller(arg);
		return testScroller;
	}
	TestScroller CreateTestScrollerOversizedToCursor(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		Vector2 cursorLength = new Vector2(50f, 50f);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(new Vector2(.5f, .5f));
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		TestScroller testScroller = new TestScroller(arg);
		return testScroller;
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "scrollerCases")]
	public void GetElementPositionNormalizedToScroller_ElementIsNotUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		TestScroller scroller = CreateTestScrollerOversizedToScroller();

		scroller.ActivateImple();
		
		for(int i = 0; i < 2; i++){
			Assert.That(scroller.ElementIsUndersizedTo_Test(scroller.thisRect_Test.size, i), Is.False);
			float actual = scroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);
			
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "scrollerZeroCases")]
	public void GetElementPositionNormalizedToScroller_ElementIsUndersized_ReturnsZero(Vector2 elementLocalPos){
		TestScroller testScroller = CreateTestScrollerUndersizedToScroller();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisRect_Test.size, i), Is.True);
			float actual = testScroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	[Test, TestCaseSource(typeof(GetElementCursorOffsetInPixel_TestCase), "cases")]
	public void GetElementCursorOffsetInPixel_ElementIsNotUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		TestScroller testScroller = CreateTestScrollerOversizedToCursor();
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.False);
			float actual = testScroller.GetElementCursorOffsetInPixel_Test(elementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetElementCursorOffsetInPixel_TestCase), "undersizedCases")]
	public void GetElementCursorOffsetInPixel_ElementIsUndersized_Various(Vector2 elementLocalPos, Vector2 expected){
		TestScroller testScroller = CreateTestScrollerUndersizedToCursor();
		testScroller.ActivateImple();

		for(int i =0; i < 2; i++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.True);
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
	[Test, TestCaseSource(typeof(GetNormalizedCursoredPosFromPosInElementSpace_TestCase), "cases")]
	public void GetNormalizedCursoredPosFromPosInElementSpace_ElementIsNotUndersized_Various(Vector2 positionInElementSpace, Vector2 expected){
		TestScroller testScroller = CreateTestScrollerOversizedToCursor();
		testScroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.False);
			float actual = testScroller.GetNormalizedCursoredPosFromPosInElementSpace_Test(positionInElementSpace[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(GetNormalizedCursoredPosFromPosInElementSpace_TestCase), "undersizedCases")]
	public void GetNormalizedCursoredPosFromPosInElementSpace_ElementIsUndersized_ReturnsZero(Vector2 positionInElementSpace){
		TestScroller testScroller = CreateTestScrollerUndersizedToCursor();
		testScroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.True);
			float actual = testScroller.GetNormalizedCursoredPosFromPosInElementSpace_Test(positionInElementSpace[i], i);

			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	public class GetNormalizedCursoredPosFromPosInElementSpace_TestCase{
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
		TestScroller testScroller = CreateTestScrollerOversizedToCursor();
		testScroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			bool actual = testScroller.ElementIsScrolledToIncreaseCursorOffset_Test(dragDelta[i], elementLocalPosition[i], i);

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
	[Test, TestCaseSource(typeof(CalcRubberBandedPosOnAxis_Demo_TestCase), "cases"), Ignore]
	public void CalcRubberBandedPosOnAxis_Demo(Vector2 localPos){
		TestScroller testScroller = CreateTestScrollerOversizedToCursor();

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
	[Test, TestCaseSource(typeof(CalcLocalPositionFromNormalizedCursoredPosition_TestCase), "cases")]
	public void CalcLocalPositionFromNormalizedCursoredPosition_Various(Vector2 scrollerLength, Vector2 cursorLength, Vector2 relativeCursorPos, Vector2 elementLength, Vector2 normalizedCursoredPosition, Vector2 expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(relativeCursorPos);
		Rect elementRect = new Rect(Vector2.zero, elementLength);
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();

		Vector2 actual = scroller.CalcLocalPositionFromNormalizedCursoredPosition_Test(normalizedCursoredPosition);

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class CalcLocalPositionFromNormalizedCursoredPosition_TestCase{
		public static object[] cases = {
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(100f, 100f), 
				new Vector2(0f, 0f), 
				new Vector2(75f, 25f)
			},
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(100f, 100f), 
				new Vector2(.5f, .5f), 
				new Vector2(50f, 0f)
			},
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(100f, 100f), 
				new Vector2(1f, 1f), 
				new Vector2(25f, -25f)
			},
			
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(50f, 50f), 
				new Vector2(0f, 0f), 
				new Vector2(75f, 25f)
			},
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(75f, 25f)
			},
			new object[]{
				new Vector2(200f, 100f), 
				new Vector2(50f, 50f), 
				new Vector2(.5f, .5f), 
				new Vector2(50f, 50f), 
				new Vector2(1f, 1f), 
				new Vector2(75f, 25f)
			},
		};
	}
	/* Release */
	[Test]
	public void OnReleaseImple_ThisProcessedDrag_MakesThisProcessedDragFalse(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		Vector2 dragDelta = new Vector2(1f, 0f);
		scroller.OnDragImple_Test(CreateCustomEventDataFromDelta(dragDelta));
		Assert.That(scroller.thisProcessedDrag_Test, Is.True);

		scroller.OnReleaseImple_Test();

		Assert.That(scroller.thisProcessedDrag_Test, Is.False);
	}
	[Test]
	public void OnReleaseImple_ThisNotProcessedDrag_CallsParentUIEOnRelease(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		Assert.That(scroller.thisProcessedDrag_Test, Is.False);

		scroller.OnReleaseImple_Test();

		parentUIE.Received(1).OnRelease();
	}
	[Test]
	public void OnReleaseImple_ResetsDrag(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		Vector2 dragDelta = new Vector2(1f, 0f);
		scroller.OnDragImple_Test(CreateCustomEventDataFromDelta(dragDelta));
		Assert.That(scroller.thisShouldProcessDrag_Test, Is.True);
		Assert.That(scroller.thisHasDoneDragEvaluation_Test, Is.True);

		scroller.OnReleaseImple_Test();

		Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
		Assert.That(scroller.thisHasDoneDragEvaluation_Test, Is.False);
	}
	/* Swipe */
	[Test]
	public void OnSwipeImple_NotProcessedDrag_CallsParentUIEOnSwipe(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		IUIElement parentUIE = Substitute.For<IUIElement>();
		arg.uia.GetParentUIE().Returns(parentUIE);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		Assert.That(scroller.thisProcessedDrag_Test, Is.False);
		ICustomEventData data = Substitute.For<ICustomEventData>();

		scroller.OnSwipeImple_Test(data);

		parentUIE.Received(1).OnSwipe(data);
	}
	[Test]
	public void OnSwipeImple_ResetsDrag(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		Vector2 dragDelta = new Vector2(1f, 0f);
		scroller.OnDragImple_Test(CreateCustomEventDataFromDelta(dragDelta));
		Assert.That(scroller.thisShouldProcessDrag_Test, Is.True);
		Assert.That(scroller.thisHasDoneDragEvaluation_Test, Is.True);

		scroller.OnSwipeImple_Test(Substitute.For<ICustomEventData>());

		Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
		Assert.That(scroller.thisHasDoneDragEvaluation_Test, Is.False);
	}
	[Test]
	public void OnSwipeImple_PrcessedDrag_MakesNotProcessedDrag(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();
		scroller.OnDragImple_Test(CreateCustomEventDataFromDelta(new Vector2(0f, 1f)));
		Assert.That(scroller.thisProcessedDrag_Test, Is.True);

		ICustomEventData data = Substitute.For<ICustomEventData>();
		scroller.OnSwipeImple_Test(data);

		Assert.That(scroller.thisProcessedDrag_Test, Is.False);
	}
	[Test, TestCaseSource(typeof(CheckForDynamicBoundarySnap_TestCase), "cases")]
	public void CheckForDynamicBoundarySnap_Various(Vector2 deltaPos, Vector2 elementLocalPos, bool[] expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(relativeCursorPos);
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		arg.uia.GetChildUIEs()[0].GetLocalPosition().Returns(elementLocalPos);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			bool actual = scroller.CheckForDynamicBoundarySnap_Test(deltaPos[i], i);
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class CheckForDynamicBoundarySnap_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(0f, 0f), new Vector2(75f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(1f, 0f), new Vector2(75f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(-1f, 0f), new Vector2(75f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(0f, 1f), new Vector2(75f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(0f, -1f), new Vector2(75f, 25f), new bool[]{false, false}},
			
			new object[]{new Vector2(1f, 0f), new Vector2(76f, 26f), new bool[]{true, false}},
			new object[]{new Vector2(.01f, 0f), new Vector2(75.0001f, 26f), new bool[]{true, false}},
			new object[]{new Vector2(1f, 1f), new Vector2(76f, 26f), new bool[]{true, true}},
			new object[]{new Vector2(-1f, -1f), new Vector2(76f, 26f), new bool[]{false, false}},
			
			new object[]{new Vector2(-1f, 1f), new Vector2(25f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(-1f, 1f), new Vector2(24f, 26f), new bool[]{true, true}},
			new object[]{new Vector2(1f, -1f), new Vector2(24f, 26f), new bool[]{false, false}},
			
			new object[]{new Vector2(-1f, -1f), new Vector2(25f, -25f), new bool[]{false, false}},
			new object[]{new Vector2(-1f, -1f), new Vector2(24f, -26f), new bool[]{true, true}},
			new object[]{new Vector2(1f, 1f), new Vector2(24f, -26f), new bool[]{false, false}},
			
			new object[]{new Vector2(1f, -1f), new Vector2(75f, -25f), new bool[]{false, false}},
			new object[]{new Vector2(1f, -1f), new Vector2(76f, -26f), new bool[]{true, true}},
			new object[]{new Vector2(-1f, 1f), new Vector2(76f, -26f), new bool[]{false, false}},
		};
	}
	[Test, TestCaseSource(typeof(CheckForStaticBoundarySnap_TestCase), "cases")]
	public void CheckForStaticBoundarySnap_Various(Vector2 elementLocalPosition, bool[] expected){
		ITestScrollerConstArg arg = CreateMockConstArg();
		Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
		Vector2 cursorLength = new Vector2(50f, 50f);
		Vector2 relativeCursorPos = new Vector2(.5f, .5f);
		Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
		arg.uia.GetRect().Returns(scrollerRect);
		arg.cursorLength.Returns(cursorLength);
		arg.relativeCursorPosition.Returns(relativeCursorPos);
		arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
		arg.uia.GetChildUIEs()[0].GetLocalPosition().Returns(elementLocalPosition);
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();

		for(int i = 0; i < 2; i ++){
			bool actual = scroller.CheckForStaticBoundarySnapOnAxis_Test(i);
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class CheckForStaticBoundarySnap_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(75f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(76f, 26f), new bool[]{true, true}},
			new object[]{new Vector2(25f, 25f), new bool[]{false, false}},
			new object[]{new Vector2(24f, 26f), new bool[]{true, true}},
			new object[]{new Vector2(25f, -25f), new bool[]{false, false}},
			new object[]{new Vector2(24f, -26f), new bool[]{true, true}},
			new object[]{new Vector2(75f, -25f), new bool[]{false, false}},
			new object[]{new Vector2(76f, -26f), new bool[]{true, true}},
		};
	}
	/* Process */
	[Test]
	public void ProcessSwitching_Demo(){
		ITestScrollerConstArg arg = CreateMockConstArg();
		IUIElement scrollerElement = arg.uia.GetChildUIEs()[0];
		TestScroller scroller = new TestScroller(arg);
		scroller.ActivateImple();

		IScrollerElementMotorProcess horProcessA = CreateMockProcess(scroller, 0);
		IScrollerElementMotorProcess horProcessB = CreateMockProcess(scroller, 0);

		horProcessA.Run();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[0], Is.SameAs(horProcessA));
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[1], Is.Null);
		scrollerElement.Received(1).DisableInputRecursively();

		horProcessB.Run();
		horProcessA.Received(1).Stop();
		scrollerElement.Received(1).EnableInputRecursively();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[0], Is.SameAs(horProcessB));
		scrollerElement.Received(2).DisableInputRecursively();

		IScrollerElementMotorProcess verProcessA = CreateMockProcess(scroller, 1);
		IScrollerElementMotorProcess verProcessB = CreateMockProcess(scroller, 1);
		
		verProcessA.Run();
		scrollerElement.Received(3).DisableInputRecursively();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[1], Is.SameAs(verProcessA));

		verProcessB.Run();
		verProcessA.Received(1).Stop();
		scrollerElement.Received(4).DisableInputRecursively();
		scrollerElement.Received(1).EnableInputRecursively();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[1], Is.SameAs(verProcessB));

		scroller.OnTouchImple_Test(1);
		horProcessB.Received(1).Stop();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[0], Is.Null);
		verProcessB.Received(1).Stop();
		Assert.That(scroller.thisRunningScrollerMotorProcess_Test[1], Is.Null);
		scrollerElement.Received(2).EnableInputRecursively();
	}
	IScrollerElementMotorProcess CreateMockProcess(IScroller scroller, int dimension){
		IScrollerElementMotorProcess process = Substitute.For<IScrollerElementMotorProcess>();
		process.When(
			x =>{x.Run();}
		).Do(
			x => {scroller.SwitchRunningElementMotorProcess(process, dimension);}
		);
		process.When(
			x =>{x.Stop();}
		).Do(
			x =>{scroller.ClearScrollerElementMotorProcess(process, dimension);}
		);
		return process;
	}
	/* Touch */






	public class TestScroller: AbsScroller, INonActivatorUIElement{
		public TestScroller(ITestScrollerConstArg arg): base(arg){
			thisTestCursorLength = arg.cursorLength;
		}
		readonly Vector2 thisTestCursorLength;
		protected override Vector2 CalcCursorLength(){
			return thisTestCursorLength;
		}
		protected override bool[] thisShouldApplyRubberBand{get{return new bool[]{true, true};}}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		protected override Vector2 GetInitialNormalizedCursoredPosition(){return Vector2.zero;}
		/* Test exposures */
		public Vector2 thisRelativeCursorPosition_Test{get{return thisRelativeCursorPosition;}}
		public Vector2 thisRubberBandLimitMultiplier_Test{get{return thisRubberBandLimitMultiplier;}}
		public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
		public Rect thisRect_Test{get{return thisRect;}}
		public Vector2 thisRectLength_Test{get{return thisRectLength;}}
		public Vector2 thisCursorLocalPosition_Test{get{return thisCursorLocalPosition;}}
		public Rect thisScrollerElementRect_Test{get{return thisScrollerElementRect;}}
		public Vector2 thisScrollerElementLength_Test{get{return thisScrollerElementLength;}}
		public bool thisHasDoneDragEvaluation_Test{get{return thisHasDoneDragEvaluation;}}
		public bool thisProcessedDrag_Test{get{return thisShouldProcessDrag;}}
		public void OnDragImple_Test(ICustomEventData eventData){
			this.OnDragImple(eventData);
		}
		public bool thisShouldProcessDrag_Test{get{return thisShouldProcessDrag;}}
		public bool ElementIsUndersizedTo_Test(Vector2 referenceLength, int dimension){
			return this.ElementIsUndersizedTo(referenceLength, dimension);
		}
		public float GetElementPositionNormalizedToCursor_Test(float elementLocalPosOnAxis, int dimension){
			return GetNormalizedCursoredPosition(elementLocalPosOnAxis, dimension);
		}
		public float GetElementPositionNormalizedToScroller_Test(float elementLocalPosOnAxis, int dimension){
			return GetNormalizedScrollerPosition(elementLocalPosOnAxis, dimension);
		}
		public float GetElementCursorOffsetInPixel_Test(float elementLocalPosOnAxis, int dimension){
			return GetElementCursorOffsetInPixel(elementLocalPosOnAxis, dimension);
		}
		public float GetNormalizedCursoredPosFromPosInElementSpace_Test(float positionInElementSpaceOnAxis, int dimension){
			return GetNormalizedCursoredPositionFromPosInElementSpace(positionInElementSpaceOnAxis, dimension);
		}
		public bool ElementIsScrolledToIncreaseCursorOffset_Test(float deltaPosOnAxis, float elementLocalPosOnAxis, int dimension){
			return this.ElementIsScrolledToIncreaseCursorOffset(deltaPosOnAxis, elementLocalPosOnAxis, dimension);
		}
		public float CalcRubberBandedPosOnAxis_Test(float localPosOnAxis, int dimension){
			return CalcRubberBandedPosOnAxis(localPosOnAxis, dimension);
		}
		public void OnReleaseImple_Test(){
			this.OnReleaseImple();
		}
		public Vector2 CalcLocalPositionFromNormalizedCursoredPosition_Test(Vector2 normalizedCursoredPosition){
			return CalcLocalPositionFromNormalizedCursoredPosition(normalizedCursoredPosition);
		}
		public void OnSwipeImple_Test(ICustomEventData data){
			this.OnSwipeImple(data);
		}
		public bool CheckForDynamicBoundarySnap_Test(float deltaPosOnAxis, int dimension){
			return this.CheckForDynamicBoundarySnap(deltaPosOnAxis, dimension);
		}
		public bool CheckForStaticBoundarySnapOnAxis_Test(int dimension){
			return this.CheckForStaticBoundarySnapOnAxis(dimension);
		}
		public IScrollerElementMotorProcess[] thisRunningScrollerMotorProcess_Test{
			get{return thisRunningScrollerMotorProcess;}
		}
		public void OnTouchImple_Test(int touchCount){
			this.OnTouchImple(touchCount);
		}
	}
	public interface ITestScrollerConstArg: IScrollerConstArg{
		Vector2 cursorLength{get;}
	}
	public class TestScrollerConstArg: ScrollerConstArg, ITestScrollerConstArg{
		public TestScrollerConstArg(Vector2 cursorLength, ScrollerAxis scrollerAxis, Vector2 relativeCursorPosition, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IScrollerAdaptor uia, IUIImage uiImage): base(scrollerAxis, relativeCursorPosition, rubberBandLimitMultiplier, isEnabledInertia ,uim, processFactory, uieFactory, uia, uiImage){
			thisCursorLength = cursorLength;
		}
		readonly Vector2 thisCursorLength;
		public Vector2 cursorLength{get{return thisCursorLength;}}
	}
	ITestScrollerConstArg CreateMockConstArg(){
		ITestScrollerConstArg arg = Substitute.For<ITestScrollerConstArg>();
		arg.cursorLength.Returns(Vector2.zero);
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		arg.relativeCursorPosition.Returns(Vector2.zero);
		arg.rubberBandLimitMultiplier.Returns(Vector2.zero);
		arg.isEnabledInertia.Returns(false);
		arg.uim.Returns(Substitute.For<IUIManager>());
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
		IScrollerAdaptor scrollerAdaptor = Substitute.For<IScrollerAdaptor>();
			scrollerAdaptor.GetRect().Returns(new Rect(Vector2.zero, new Vector2(100f, 100f)));
			IUIElement child = Substitute.For<IUIElement>();
			IUIAdaptor childUIA = Substitute.For<IUIAdaptor>();
			child.GetUIAdaptor().Returns(childUIA);
			List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
			scrollerAdaptor.GetChildUIEs().Returns(returnedList);
		arg.uia.Returns(scrollerAdaptor);
		arg.image.Returns(Substitute.For<IUIImage>());
		return arg;
	}

	public ICustomEventData CreateCustomEventDataFromDelta(Vector2 deltaPos){
		ICustomEventData data = new CustomEventData(Vector2.zero, deltaPos);
		return data;
	}

}
