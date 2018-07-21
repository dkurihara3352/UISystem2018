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
	[Test]
	public void Activate_ChildrenNull_ThrowsError(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateTestScrollerForActivation(out arg);
		List<IUIElement> returnedList = null;
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.NullReferenceException)).And.Message.EqualTo("childUIEs must not be null"), ()=> {scroller.ActivateImpleTest();});
	}
	[Test]
	public void Activate_ChildrenCountNotOne_ThrowsError([Values(0, 2, 5)]int childCount){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateTestScrollerForActivation(out arg);
		List<IUIElement> returnedList = new List<IUIElement>();
		for(int i = 0; i < childCount; i++)
			returnedList.Add(Substitute.For<IUIElement>());
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller must have only one UIE child as Scroller Element"), () => {scroller.ActivateImpleTest();});
	}
	[Test]
	public void Activate_FirstChildIsNull_ThrowsError(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateTestScrollerForActivation(out arg);
		IUIElement returnedChild = null;
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{returnedChild});
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("Scroller's only child must not be null"), () => {scroller.ActivateImpleTest();});
	}
	[Test]
	public void Activate_OnlyChildNotNull_DoesNotThrowException(){
		ITestScrollerConstArg arg;
		TestScroller scroller = CreateTestScrollerForActivation(out arg);
		IUIElement child = Substitute.For<IUIElement>();
		List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
		arg.uia.GetChildUIEs().Returns(returnedList);

		Assert.DoesNotThrow(()=>{scroller.ActivateImpleTest();});
	}
	[Test, TestCaseSource(typeof(ElementIsUndersizedToCursor_TestCase), "cases")]
	public void ElementIsUndersizedToCursor_Various(Vector2 cursorRectSize, Vector2 elementRectSize){

	}
	public class ElementIsUndersizedToCursor_TestCase{
		public static object[] cases = {};
	}
	public class TestScroller: AbsScroller, INonActivatorUIElement{
		public TestScroller(ITestScrollerConstArg arg): base(arg){
		}
		protected override bool thisShouldApplyRubberBand{get{return true;}}
		Vector2 thisCursorSize;
		public void SetCursorSize(Vector2 cursorSize){
			thisCursorSize = cursorSize;
		}
		protected override Vector2 CalcCursorDimension(IScrollerConstArg arg, Rect thisRect){
			return ((ITestScrollerConstArg)arg).cursorDimension;
		}
		public void ActivateImpleTest(){
			this.ActivateImple();
		}
		public bool ElementIsUndersizedToCursorTest(int dimension){
			return ElementIsUndersizedToCursor(dimension);
		}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
	}
	public interface ITestScrollerConstArg: IScrollerConstArg{
		Vector2 cursorDimension{get;}
	}
	TestScroller CreateTestScrollerForActivation(out ITestScrollerConstArg arg){
		ITestScrollerConstArg thisArg;
		TestScroller scroller = CreateTestScrollerFull(out thisArg, Vector2.zero, ScrollerAxis.Both, new float[2], new Rect(), Vector2.zero);
		arg = thisArg;
		return scroller;
	}
	TestScroller CreateTestScrollerFull(out ITestScrollerConstArg arg ,Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, float[] rubberBandLimitMultiplier, Rect scrollerRect, Vector2 cursorDimension){
		ITestScrollerConstArg thisArg = Substitute.For<ITestScrollerConstArg>();
		thisArg.uim.Returns(Substitute.For<IUIManager>());
		thisArg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		thisArg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
		IScrollerAdaptor scrollerAdaptor = Substitute.For<IScrollerAdaptor>();
			scrollerAdaptor.GetRect().Returns(scrollerRect);
		thisArg.uia.Returns(scrollerAdaptor);
		thisArg.image.Returns(Substitute.For<IUIImage>());
		thisArg.relativeCursorPosition.Returns(relativeCursorPosition);
		thisArg.scrollerAxis.Returns(scrollerAxis);
		thisArg.rubberBandLimitMultiplier.Returns(rubberBandLimitMultiplier);
		thisArg.cursorDimension.Returns(cursorDimension);
		TestScroller scroller = new TestScroller(thisArg);
		scroller.SetCursorSize(Vector2.zero);
		arg = thisArg;
		return scroller;
	}
}
