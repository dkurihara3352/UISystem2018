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

		Assert.That(testScroller.TestThisRelativeCursorPos(), Is.EqualTo(expected));
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

		Assert.That(testScroller.TestThisRubberBandMultiplier(), Is.EqualTo(expected));
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

		Assert.That(testScroller.TestThisRect(), Is.EqualTo(scrollerRect));
		Assert.That(testScroller.TestThisRectLength(), Is.EqualTo(scrollerLength));
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

		Vector2 actual = testScroller.TestThisCursorLength();
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

		Vector2 actual = testScroller.TestThisCursorLocalPos();
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
	/*  */
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
	[Test, Ignore, TestCaseSource(typeof(ElementIsUndersizedToCursor_TestCase), "cases")]
	public void ElementIsUndersizedToCursor_Various(Vector2 cursorRectSize, Vector2 elementRectSize){

	}
	public class ElementIsUndersizedToCursor_TestCase{
		public static object[] cases = {};
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
		public bool ElementIsUndersizedToTest(Vector2 referenceLength, int dimension){
			return ElementIsUndersizedTo(referenceLength, dimension);
		}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		protected override Vector2 GetInitialPositionNormalizedToCursor(){return Vector2.zero;}
		/* Test exposures */
		public Vector2 TestThisRelativeCursorPos(){
			return thisRelativeCursorPosition;
		}
		public Vector2 TestThisRubberBandMultiplier(){
			return thisRubberBandLimitMultiplier;
		}
		public Vector2 TestThisCursorLength(){
			return thisCursorLength;
		}
		public Rect TestThisRect(){return thisRect;}
		public Vector2 TestThisRectLength(){return thisRectLength;}
		public Vector2 TestThisCursorLocalPos(){
			return thisCursorLocalPosition;
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
}
