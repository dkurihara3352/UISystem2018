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
	/* Constructioin */
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
		[Test, TestCaseSource(typeof(SetUpCursorTransform_CursorLengthGreaterThanScrollerLength_TestCase), "cases")]
		public void SetUpCursorTransform_CursorLengthGreaterThanScrollerLength_ClampTheLength(Vector2 scrollerLength, Vector2 cursorLength, Vector2 expected){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
			arg.cursorLength.Returns(cursorLength);
			TestScroller testScroller = new TestScroller(arg);

			testScroller.SetUpCursorTransform();

			Vector2 actual = testScroller.thisCursorLength_Test;
			Assert.That(actual, Is.EqualTo(expected));
		}
		public class SetUpCursorTransform_CursorLengthGreaterThanScrollerLength_TestCase{
			static object[] cases = {
				new object[]{new Vector2(200f, 100f), new Vector2(20f, 10f), new Vector2(20f, 10f)},
				new object[]{new Vector2(200f, 100f), new Vector2(210f, 110f), new Vector2(200f, 100f)}
			};
		}
		[Test, TestCaseSource(typeof(SetUpCursorTransform_RelativeCursorPos_Various_TestCase), "cases")]
		public void SetUpCursorTransform_RelativeCursorPos_Various(Vector2 scrollerLength, Vector2 cursorLength, Vector2 relativeCursorPos, Vector2 expectedPos){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
			arg.cursorLength.Returns(cursorLength);
			arg.relativeCursorPosition.Returns(relativeCursorPos);
			TestScroller testScroller = new TestScroller(arg);

			testScroller.SetUpCursorTransform();

			Vector2 actual = testScroller.thisCursorLocalPosition_Test;
			Assert.That(actual, Is.EqualTo(expectedPos));
		}
		public class SetUpCursorTransform_RelativeCursorPos_Various_TestCase{
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
		public void SetUpScrollerElement_ChildrenNull_ThrowsError(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			List<IUIElement> returnedList = null;
			arg.uia.GetChildUIEs().Returns(returnedList);
			TestScroller scroller = new TestScroller(arg);

			Assert.Throws(Is.TypeOf(typeof(System.NullReferenceException)).And.Message.EqualTo("childUIEs must not be null"), ()=> {scroller.SetUpScrollerElement();});
		}
		[Test]
		public void SetUpScrollerElement_ChildrenCountNotOne_ThrowsError([Values(0, 2, 5)]int childCount){
			ITestScrollerConstArg arg = CreateMockConstArg();
			List<IUIElement> returnedList = new List<IUIElement>();
			for(int i = 0; i < childCount; i++)
				returnedList.Add(Substitute.For<IUIElement>());
			arg.uia.GetChildUIEs().Returns(returnedList);
			TestScroller scroller = new TestScroller(arg);

			Assert.Throws(
				Is.TypeOf(typeof(System.InvalidOperationException))
				.And.Message.EqualTo("Scroller must have only one UIE child as Scroller Element"), 
				() => {
					scroller.SetUpScrollerElement();
				}
			);
		}
		[Test]
		public void SetUpScrollerElement_FirstChildIsNull_ThrowsError(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			IUIElement returnedChild = null;
			List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{returnedChild});
			arg.uia.GetChildUIEs().Returns(returnedList);
			TestScroller scroller = new TestScroller(arg);

			Assert.Throws(
				Is.TypeOf(typeof(System.InvalidOperationException))
				.And.Message.EqualTo("Scroller's only child must not be null"), 

				() => {
					scroller.SetUpScrollerElement();
				}
			);
		}
		[Test]
		public void SetUpScrollerElement_OnlyChildNotNull_DoesNotThrowException(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			IUIElement child = Substitute.For<IUIElement>();
			List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
			arg.uia.GetChildUIEs().Returns(returnedList);
			TestScroller scroller = new TestScroller(arg);

			Assert.DoesNotThrow(()=>{scroller.SetUpScrollerElement();});
		}
		[Test]
		public void SetUpScrollerElement_SetsScrollerElementRectFields(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			Vector2 actualLength = new Vector2(100f, 200f);
			Rect actualRect = new Rect(Vector2.zero, actualLength);
			arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(actualRect);
			TestScroller testScroller = new TestScroller(arg);

			testScroller.SetUpScrollerElement();

			Assert.That(testScroller.thisScrollerElementRect_Test, Is.EqualTo(actualRect));
			Assert.That(testScroller.thisScrollerElementLength_Test, Is.EqualTo(actualLength));
		}
	/* Dragging */
		[Test]
		public void OnBeginDragImple_TopmostScrollerInMotionNotNull_BecomeShouldProcessDrag(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			TestScroller scroller = new TestScroller(arg);
			scroller.SetTopmostScrollerInMotion(Substitute.For<IScroller>());

			scroller.OnBeginDragImple_Test(Substitute.For<ICustomEventData>());

			bool actual = scroller.thisShouldProcessDrag_Test;

			Assert.That(actual, Is.True);
		}
		[Test]
		public void OnBeginDrag_ThisIsTopmostScrollerInMotion_CachesTouchPosition(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			TestScroller testScroller = new TestScroller(arg);
			testScroller.SetTopmostScrollerInMotion(testScroller);

			IUIElement scrollerElement = Substitute.For<IUIElement>();
			Vector2 scrollerElementLocalPosition = new Vector2(30f, 20f);
			scrollerElement.GetLocalPosition().Returns(scrollerElementLocalPosition);
			testScroller.SetScrollerElement_Test(scrollerElement);

			Vector2 touchPosition = new Vector2(14f, 33f);
			ICustomEventData eventData = Substitute.For<ICustomEventData>();
			eventData.position.Returns(touchPosition);

			testScroller.OnBeginDragImple_Test(eventData);

			Vector2 actualTouchPos = testScroller.thisTouchPosition_Test;
			Vector2 actualElementLocalPos = testScroller.thisElementLocalPositionAtTouch_Test;

			Assert.That(actualTouchPos, Is.EqualTo(touchPosition));
			Assert.That(actualElementLocalPos, Is.EqualTo(scrollerElementLocalPosition));
		}
		[Test]
		public void OnBeginDragImple_TopmostScrollerNotNull_ThisIsNotTopmost_CallTopmostScrollerOnBeginDrag(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			TestScroller scroller = new TestScroller(arg);
			IScroller anotherTopmostScroller = Substitute.For<IScroller>();
			scroller.SetTopmostScrollerInMotion(anotherTopmostScroller);

			ICustomEventData data = Substitute.For<ICustomEventData>();
			scroller.OnBeginDragImple_Test(data);

			anotherTopmostScroller.Received(1).OnBeginDrag(data);
		}
		[Test]
		public void OnBeginDragImple_TopmostScrollerNull_ThisScrollerAxisIsBoth_SetsThisShouldProcessDragTrue(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.scrollerAxis.Returns(ScrollerAxis.Both);
			TestScroller scroller = new TestScroller(arg);
			scroller.SetScrollerElement_Test(Substitute.For<IUIElement>());//for cache
			scroller.SetTopmostScrollerInMotion(null);
			Assert.That(scroller.thisScrollerAxis_Test, Is.EqualTo(ScrollerAxis.Both));

			scroller.OnBeginDragImple_Test(Substitute.For<ICustomEventData>());

			Assert.That(scroller.thisShouldProcessDrag_Test, Is.True);
		}
		[Test, TestCaseSource(typeof(OnBeginDragImple_ThisScrollerAxis_DragDirection_TestCase), "cases")]
		public void OnBeginDragImple_TopmostScrollerNull_ThisScrollerAxis_DragDirection_SetsThisShouldProcessDragVarious(ScrollerAxis scrollerAxis, Vector2 dragDelta, bool expected){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.scrollerAxis.Returns(scrollerAxis);
			TestScroller scroller = new TestScroller(arg);
			scroller.SetScrollerElement_Test(Substitute.For<IUIElement>());
			scroller.SetTopmostScrollerInMotion(null);

			ICustomEventData data = Substitute.For<ICustomEventData>();
			data.deltaPos.Returns(dragDelta);
			
			scroller.OnBeginDragImple_Test(data);

			Assert.That(scroller.thisShouldProcessDrag_Test, Is.EqualTo(expected));
		}
		public class OnBeginDragImple_ThisScrollerAxis_DragDirection_TestCase{
			public static object[] cases = {
				new object[]{ScrollerAxis.Both, new Vector2(1f, 0f), true},
				new object[]{ScrollerAxis.Both, new Vector2(0f, 1f), true},
				new object[]{ScrollerAxis.Both, new Vector2(-1f, 0f), true},
				new object[]{ScrollerAxis.Both, new Vector2(0f, -1f), true},
				new object[]{ScrollerAxis.Both, new Vector2(1f, 1f), true},
				
				new object[]{ScrollerAxis.Horizontal, new Vector2(1f, 0f), true},
				new object[]{ScrollerAxis.Horizontal, new Vector2(0f, 1f), false},
				new object[]{ScrollerAxis.Horizontal, new Vector2(-1f, 0f), true},
				new object[]{ScrollerAxis.Horizontal, new Vector2(0f, -1f), false},
				new object[]{ScrollerAxis.Horizontal, new Vector2(1f, 1f), true},
				
				new object[]{ScrollerAxis.Vertical, new Vector2(1f, 0f), false},
				new object[]{ScrollerAxis.Vertical, new Vector2(0f, 1f), true},
				new object[]{ScrollerAxis.Vertical, new Vector2(-1f, 0f), false},
				new object[]{ScrollerAxis.Vertical, new Vector2(0f, -1f), true},
				new object[]{ScrollerAxis.Vertical, new Vector2(1f, 1f), false},
			};
		}
		[Test, TestCaseSource(typeof(OnBeginDragImple_ThisShouldProcessDrag_TestCase), "cases")]
		public void OnBeginDragImple_TopmostScrollerNull_ThisShouldProcessDrag_CacheTouchPosition(Vector2 dragPosition, Vector2 elementLocalPosAtTouch){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.scrollerAxis.Returns(ScrollerAxis.Both);
			TestScroller scroller = new TestScroller(arg);
			IUIElement scrollerElement = Substitute.For<IUIElement>();
			scrollerElement.GetLocalPosition().Returns(elementLocalPosAtTouch);
			scroller.SetScrollerElement_Test(scrollerElement);
			scroller.SetTopmostScrollerInMotion(null);
			ICustomEventData data = Substitute.For<ICustomEventData>();
			data.position.Returns(dragPosition);

			Assert.That(scroller.thisTouchPosition_Test, Is.EqualTo(Vector2.zero));
			Assert.That(scroller.thisElementLocalPositionAtTouch_Test, Is.EqualTo(Vector2.zero));

			scroller.OnBeginDragImple_Test(data);
			
			Assert.That(scroller.thisShouldProcessDrag_Test, Is.True);
			Assert.That(scroller.thisTouchPosition_Test, Is.EqualTo(dragPosition));
			Assert.That(scroller.thisElementLocalPositionAtTouch_Test, Is.EqualTo(elementLocalPosAtTouch));
		}
		public class OnBeginDragImple_ThisShouldProcessDrag_TestCase{
			public static object[] cases = {
				new object[]{new Vector2(10f, 10f), new Vector2(20f, 20f)},
			};
		}
		[Test]
		public void OnBeginDragImple_ThisNotShoudProcessDrag_CallsParentUIEOnBeginDrag(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			IUIElement parentUIE = Substitute.For<IUIElement>();
			arg.uia.GetParentUIE().Returns(parentUIE);
			arg.scrollerAxis.Returns(ScrollerAxis.Horizontal);
			TestScroller scroller = new TestScroller(arg);
			scroller.ActivateImple();
			ICustomEventData data = Substitute.For<ICustomEventData>();
			data.deltaPos.Returns(new Vector2(0f, 1f));

			scroller.OnBeginDragImple_Test(data);

			Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
			parentUIE.Received(1).OnBeginDrag(data);
		}
		/*  */
		[Test, TestCaseSource(typeof(CalcDragDeltaSinceTouch_TestCase), "cases")]
		public void CalcDragDeltaSinceTouch_Various(Vector2 touchPosition, Vector2 dragPosition, ScrollerAxis axis, Vector2 expected){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.scrollerAxis.Returns(axis);
			TestScroller scroller = new TestScroller(arg);

			ICustomEventData data = Substitute.For<ICustomEventData>();
			data.position.Returns(touchPosition);
			scroller.SetTouchPosition_Test(touchPosition);

			Vector2 actual = scroller.CalcDragDeltaSinceTouch_Test(dragPosition);

			Assert.That(actual, Is.EqualTo(expected));
		}
		public class CalcDragDeltaSinceTouch_TestCase{
			public static object[] cases = {
				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(30f, 50f), 
					ScrollerAxis.Both,
					new Vector2(20f, 40f)
				},
				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Both,
					new Vector2(-40f, -60f)
				},
				new object[]{
					new Vector2(-10f, -10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Both,
					new Vector2(-20f, -40f)
				},
				
				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(30f, 50f), 
					ScrollerAxis.Horizontal,
					new Vector2(20f, 0f)
				},
				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Horizontal,
					new Vector2(-40f, 0f)
				},
				new object[]{
					new Vector2(-10f, -10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Horizontal,
					new Vector2(-20f, 0f)
				},

				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(30f, 50f), 
					ScrollerAxis.Vertical,
					new Vector2(0f, 40f)
				},
				new object[]{
					new Vector2(10f, 10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Vertical,
					new Vector2(0f, -60f)
				},
				new object[]{
					new Vector2(-10f, -10f), 
					new Vector2(-30f, -50f), 
					ScrollerAxis.Vertical,
					new Vector2(0f, -40f)
				},
			};
		}
		[Test, TestCaseSource(typeof(GetScrollerElementRubberBandedLocalPosition_TestCase), "cases"), Ignore]
		public void GetScrollerElementRubberBandedLocalPosition_Demo(Vector2 elementLocalPosAtTouch, Vector2 touchPos, Vector2 dragPos){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.uia.GetRect().Returns(new Rect(Vector2.zero, new Vector2(300f, 300f)));
			arg.relativeCursorPosition.Returns(new Vector2(.5f, .5f));
			arg.cursorLength.Returns(new Vector2(100f, 100f));
			IUIElement scrollerElement = arg.uia.GetChildUIEs()[0];
			scrollerElement.GetUIAdaptor().GetRect().Returns(new Rect(Vector2.zero, new Vector2(200f, 200f)));
			arg.scrollerAxis.Returns(ScrollerAxis.Both);
			arg.rubberBandLimitMultiplier.Returns(new Vector2(.1f, .1f));
			TestScroller scroller = new TestScroller(arg);
			scroller.ActivateImple();
			ICustomEventData data = Substitute.For<ICustomEventData>();
			data.position.Returns(touchPos);
			scrollerElement.GetLocalPosition().Returns(elementLocalPosAtTouch);

			scroller.OnBeginDragImple_Test(data);
			Assert.That(scroller.thisTouchPosition_Test, Is.EqualTo(touchPos));
			Assert.That(scroller.thisElementLocalPositionAtTouch_Test, Is.EqualTo(elementLocalPosAtTouch));
			Assert.That(scroller.GetElementCursorOffsetInPixel_Test(0f, 0), Is.EqualTo(0f));
			Assert.That(scroller.GetElementCursorOffsetInPixel_Test(0f, 1), Is.EqualTo(0f));

			int steps = 10;
			Vector2 prev = elementLocalPosAtTouch;
			DKUtility.DebugHelper.PrintInRed(
				"elementLocalPosAtTouch: " + elementLocalPosAtTouch.ToString() + 
				", touchPos: " + touchPos.ToString() +
				", dragPos: " + dragPos.ToString()
			);
			for(int i = 0; i <= steps; i ++){
				Vector2 newDragPos = Vector2.Lerp(touchPos, dragPos, i/(steps * 1f));
				Vector2 displacement = newDragPos - touchPos;
				Assert.That(scroller.CalcDragDeltaSinceTouch_Test(newDragPos), Is.EqualTo(displacement));
				Vector2 rubberBandedLocalPosition = scroller.GetScrollerElementRubberBandedLocalPosition_Test(displacement);
				Vector2 delta = rubberBandedLocalPosition - prev;
				prev = rubberBandedLocalPosition;
				Vector2 cursorOffset = 
					new Vector2(
						scroller.GetElementCursorOffsetInPixel_Test(rubberBandedLocalPosition[0], 0), scroller.GetElementCursorOffsetInPixel_Test(rubberBandedLocalPosition[1], 1)
					);
				Debug.Log(
					"i: " + i.ToString() +
					", dragPos: " + newDragPos.ToString() +
					", localPos: " + rubberBandedLocalPosition.ToString() +
					", delta: " + delta.ToString() +
					", offset: " + cursorOffset.ToString()
				);
			}

		}
		public class GetScrollerElementRubberBandedLocalPosition_TestCase{
			public static object[] cases = {
				new object[]{new Vector2(100f, 100f), new Vector2(150f, 150f), new Vector2(300f, 300f)},
				new object[]{new Vector2(100f, 100f), new Vector2(150f, 150f), new Vector2(300f, 0f)},
				new object[]{new Vector2(100f, 100f), new Vector2(150f, 150f), new Vector2(0f, 300f)},
				new object[]{new Vector2(100f, 100f), new Vector2(150f, 150f), new Vector2(0f, 0f)},
			};
		}
		[Test]
		public void ThisShouldProcessDrag_Initially_IsFalse(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			TestScroller scroller = new TestScroller(arg);

			Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
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
		[Test]
		public void OnDragImple_ShouldProcessDrag_TopmostNotNull_IsNotTopMost_CallsTopmostOnDrag(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			TestScroller scroller = new TestScroller(arg);
			scroller.SetShouldProcessDrag_Test(true);
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullParentScroller = null;
			scroller.SetTopmostScrollerInMotion(parentScroller);
			parentScroller.GetProximateParentScroller().Returns(nullParentScroller);

			ICustomEventData data = Substitute.For<ICustomEventData>();
			scroller.OnDragImple_Test(data);

			parentScroller.Received(1).OnDrag(data);
		}
		[Test, TestCaseSource(typeof(ElementIsUndersizedTo_Various_TestCase), "cases")]
		public void ElementIsUndersizedTo_Various(Vector2 referenceLength, int dimension, bool expectedBool){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(new Rect(Vector2.zero, new Vector2(100f, 100f)));
			TestScroller testScroller = new TestScroller(arg);
			testScroller.SetUpScrollerElement();

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

			testScroller.SetUpCursorTransform();
			testScroller.SetUpScrollerElement();
			
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

			testScroller.SetUpCursorTransform();
			testScroller.SetUpScrollerElement();

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

			// scroller.ActivateImple();
			scroller.SetUpCursorTransform();
			scroller.SetUpScrollerElement();
			
			for(int i = 0; i < 2; i++){
				Assert.That(scroller.ElementIsUndersizedTo_Test(scroller.thisRect_Test.size, i), Is.False);
				float actual = scroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);
				
				Assert.That(actual, Is.EqualTo(expected[i]));
			}
		}
		[Test, TestCaseSource(typeof(GetElementNormalizedPosition_TestCase), "scrollerZeroCases")]
		public void GetElementPositionNormalizedToScroller_ElementIsUndersized_ReturnsZero(Vector2 elementLocalPos){
			TestScroller testScroller = CreateTestScrollerUndersizedToScroller();
			testScroller.SetUpCursorTransform();
			testScroller.SetUpScrollerElement();

			for(int i =0; i < 2; i++){
				Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisRect_Test.size, i), Is.True);
				float actual = testScroller.GetElementPositionNormalizedToScroller_Test(elementLocalPos[i], i);

				Assert.That(actual, Is.EqualTo(0f));
			}
		}
		[Test, TestCaseSource(typeof(GetNormalizedCursoredPosFromPosInElementSpace_TestCase), "cases")]
		public void GetNormalizedCursoredPosFromPosInElementSpace_ElementIsNotUndersized_Various(Vector2 positionInElementSpace, Vector2 expected){
			TestScroller testScroller = CreateTestScrollerOversizedToCursor();
			// testScroller.ActivateImple();
			testScroller.SetUpCursorTransform();
			testScroller.SetUpScrollerElement();

			for(int i = 0; i < 2; i ++){
				Assert.That(testScroller.ElementIsUndersizedTo_Test(testScroller.thisCursorLength_Test, i), Is.False);
				float actual = testScroller.GetNormalizedCursoredPosFromPosInElementSpace_Test(positionInElementSpace[i], i);

				Assert.That(actual, Is.EqualTo(expected[i]));
			}
		}
		[Test, TestCaseSource(typeof(GetNormalizedCursoredPosFromPosInElementSpace_TestCase), "undersizedCases")]
		public void GetNormalizedCursoredPosFromPosInElementSpace_ElementIsUndersized_ReturnsZero(Vector2 positionInElementSpace){
			TestScroller testScroller = CreateTestScrollerUndersizedToCursor();
			testScroller.SetUpCursorTransform();
			testScroller.SetUpScrollerElement();

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
		[Test, TestCaseSource(typeof(CalcLocalPositionFromNormalizedCursoredPosition_TestCase), "cases")]
		public void CalcLocalPositionFromNormalizedCursoredPosition_Various(Vector2 scrollerLength, Vector2 cursorLength, Vector2 relativeCursorPos, Vector2 elementLength, Vector2 normalizedCursoredPosition, Vector2 expected){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.uia.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
			arg.cursorLength.Returns(cursorLength);
			arg.relativeCursorPosition.Returns(relativeCursorPos);
			Rect elementRect = new Rect(Vector2.zero, elementLength);
			arg.uia.GetChildUIEs()[0].GetUIAdaptor().GetRect().Returns(elementRect);
			TestScroller scroller = new TestScroller(arg);
			// scroller.ActivateImple();
			scroller.SetUpCursorTransform();
			scroller.SetUpScrollerElement();

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
	/*  */
		void AssertResetDragIsCalled(TestScroller scroller){
			Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
			Assert.That(scroller.thisTouchPosition_Test, Is.EqualTo(new Vector2(10000f, 10000f)));
			Assert.That(scroller.thisElementLocalPositionAtTouch_Test, Is.EqualTo(new Vector2(10000f, 10000f)));
		}
	/* Swipe */
		[Test]
		public void OnSwipeImple_NotProcessedDrag_CallsParentUIEOnSwipe(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			IUIElement parentUIE = Substitute.For<IUIElement>();
			arg.uia.GetParentUIE().Returns(parentUIE);
			TestScroller scroller = new TestScroller(arg);
			scroller.SetUpCursorTransform();
			scroller.SetUpScrollerElement();
			Assert.That(scroller.thisShouldProcessDrag_Test, Is.False);
			ICustomEventData data = Substitute.For<ICustomEventData>();

			scroller.OnSwipeImple_Test(data);

			parentUIE.Received(1).OnSwipe(data);
		}
		[Test]
		public void OnSwipeImple_ResetsDrag(){
			ITestScrollerConstArg arg = CreateMockConstArg();
			arg.scrollerAxis.Returns(ScrollerAxis.Both);
			TestScroller scroller = new TestScroller(arg);
			scroller.SetUpCursorTransform();
			scroller.SetUpScrollerElement();
			Vector2 dragDelta = new Vector2(1f, 0f);
			scroller.OnBeginDragImple_Test(CreateCustomEventDataFromDelta(dragDelta));
			Assert.That(scroller.thisShouldProcessDrag_Test, Is.True);

			scroller.OnSwipeImple_Test(Substitute.For<ICustomEventData>());

			AssertResetDragIsCalled(scroller);
		}
	/* Process */
	/* Touch */






	public class TestScroller: AbsScroller{
		public TestScroller(ITestScrollerConstArg arg): base(arg){
			thisTestCursorLength = arg.cursorLength;
		}
		readonly Vector2 thisTestCursorLength;
		protected override Vector2 CalcCursorLength(){
			return thisTestCursorLength;
		}
		protected override IScroller FindProximateParentScroller(){
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullParent = null;
			parentScroller.GetProximateParentScroller().Returns(nullParent);
			return parentScroller;
		}
		protected override bool[] thisShouldApplyRubberBand{get{return new bool[]{true, true};}}
		public void SetTopmostScrollerInMotion(IScroller scroller){
			thisTopmostScrollerInMotion = scroller;
		}
		public void SetScrollerElement_Test(IUIElement scrollerElement){
			thisScrollerElement = scrollerElement;
		}
		public void SetTouchPosition_Test(Vector2 touchPosition){
			thisTouchPosition = touchPosition;
		}
		public void SetShouldProcessDrag_Test(bool should){
			thisShouldProcessDrag = should;
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
		// public bool thisHasDoneDragEvaluation_Test{get{return thisHasDoneDragEvaluation;}}
		public bool thisShouldProcessDrag_Test{get{return thisShouldProcessDrag;}}
		public ScrollerAxis thisScrollerAxis_Test{get{return thisScrollerAxis;}}
		public Vector2 thisTouchPosition_Test{get{return thisTouchPosition;}}
		public Vector2 thisElementLocalPositionAtTouch_Test{get{return thisElementLocalPositionAtTouch;}}
		public void OnDragImple_Test(ICustomEventData eventData){
			this.OnDragImple(eventData);
		}
		public bool ElementIsUndersizedTo_Test(Vector2 referenceLength, int dimension){
			return this.ScrollerElementIsUndersizedTo(referenceLength, dimension);
		}
		public float GetElementPositionNormalizedToCursor_Test(float elementLocalPosOnAxis, int dimension){
			return GetNormalizedCursoredPositionOnAxis(elementLocalPosOnAxis, dimension);
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
		public void OnReleaseImple_Test(){
			this.OnReleaseImple();
		}
		public Vector2 CalcLocalPositionFromNormalizedCursoredPosition_Test(Vector2 normalizedCursoredPosition){
			return CalcLocalPositionFromNormalizedCursoredPosition(normalizedCursoredPosition);
		}
		public void OnSwipeImple_Test(ICustomEventData data){
			this.OnSwipeImple(data);
		}
		public void CheckAndPerformStaticBoundarySnapOnAxis_Test(int dimension){
			this.CheckAndPerformStaticBoundarySnapOnAxis(dimension);
		}
		public IScrollerElementMotorProcess[] thisRunningScrollerMotorProcess_Test{
			get{return thisRunningScrollerMotorProcess;}
		}
		public void OnTouchImple_Test(int touchCount){
			this.OnTouchImple(touchCount);
		}
		public void OnBeginDragImple_Test(ICustomEventData eventData){
			this.OnBeginDragImple(eventData);
		}
		public Vector2 CalcDragDeltaSinceTouch_Test(Vector2 dragPosition){
			return CalcDragDeltaSinceTouch(dragPosition);
		}
		public Vector2 GetScrollerElementRubberBandedLocalPosition_Test(Vector2 displacement){
			return GetScrollerElementRubberBandedLocalPosition(displacement);
		}
	}
	public interface ITestScrollerConstArg: IScrollerConstArg{
		Vector2 cursorLength{get;}
	}
	public class TestScrollerConstArg: ScrollerConstArg, ITestScrollerConstArg{
		public TestScrollerConstArg(
			Vector2 cursorLength, 
			ScrollerAxis scrollerAxis, 
			Vector2 relativeCursorPosition, 
			Vector2 rubberBandLimitMultiplier, 
			bool isEnabledInertia, 
			float newScrollSpeedThreshold,

			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uieFactory, 
			IScrollerAdaptor uia, 
			IUIImage uiImage,
			ActivationMode activationMode
		): base(
			scrollerAxis, 
			relativeCursorPosition, 
			rubberBandLimitMultiplier, 
			isEnabledInertia ,
			newScrollSpeedThreshold,
			
			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			uiImage,
			activationMode
		){
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
		ICustomEventData data = new CustomEventData(Vector2.zero, deltaPos, .1f);
		return data;
	}

}
