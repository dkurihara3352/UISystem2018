using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class GenericSingleElementScrollerTest{
	[Test, TestCaseSource(typeof(ActivateImple_CalcAndSetsCursorSize_TestCase), "cases")]
	public void ActivateImple_CalcAndSetsCursorSize(Vector2 relativeCursorLength, Vector2 expected){
		TestGenericSingleElementScroller scroller = CreateGenericSingleElementScroller(relativeCursorLength);
		
		scroller.ActivateImple();
		
		Vector2 actual = scroller.thisCursorLength_Test;
		Assert.That(actual, Is.EqualTo(expected));
	}
	public class ActivateImple_CalcAndSetsCursorSize_TestCase{
		public static object[] cases = {
			new object[]{new Vector2(1f, 1f), new Vector2(200f, 100f)},
			new object[]{new Vector2(.1f, .1f), new Vector2(20f, 10f)},
			new object[]{new Vector2(.5f, .5f), new Vector2(100f, 50f)},
		};
	}
	[Test, TestCaseSource(typeof(Constructor_TestCase), "lessThan0Cases")]
	public void Constructor_LessThan0_ThrowsException(Vector2 relativeCursorLength){
		ScrollerAxis scrollerAxis = ScrollerAxis.Both;
		Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
		Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
		IUIManager uim = Substitute.For<IUIManager>();
		IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
		IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
		IGenericSingleElementScrollerAdaptor uia = Substitute.For<IGenericSingleElementScrollerAdaptor>();
		IUIImage image = Substitute.For<IUIImage>();

		Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("relativeCursorLength must be greater than 0"), () => {new GenericSingleElementScrollerConstArg(relativeCursorLength, scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, true, uim, processFactory, uieFactory, uia, image);});
	}
	[Test, TestCaseSource(typeof(Constructor_TestCase), "greaterThan0Cases")]
	public void Constructor_GreaterThan0_ClampsValueDownToOne(Vector2 relativeCursorLength, Vector2 expected){
		TestGenericSingleElementScroller scroller = CreateGenericSingleElementScroller(relativeCursorLength);

		Vector2 actual = scroller.thisRelativeCursorLength_Test;

		Assert.That(actual, Is.EqualTo(expected));
	}
	public class Constructor_TestCase{
		public static object[] lessThan0Cases = {
			new object[]{new Vector2(0f, 0f)},
			new object[]{new Vector2(-.1f, -.1f)},
		};
		public static object[] greaterThan0Cases = {
			new object[]{new Vector2(1f, 1f), new Vector2(1f, 1f)},
			new object[]{new Vector2(.5f, .5f), new Vector2(.5f, .5f)},
			new object[]{new Vector2(10f, 10f), new Vector2(1f, 1f)},
		};
	}
	

	public class TestGenericSingleElementScroller: GenericSingleElementScroller{
		public TestGenericSingleElementScroller(IGenericSingleElementScrollerConstArg arg): base(arg){
		}
		public Vector2 thisRelativeCursorLength_Test{get{return thisRelativeCursorLength;}}
		public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
	}
	TestGenericSingleElementScroller CreateGenericSingleElementScroller(Vector2 relativeCursorLength){
		ScrollerAxis scrollerAxis = ScrollerAxis.Both;
		Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
		Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
		IUIManager uim = Substitute.For<IUIManager>();
		IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
		IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
		IGenericSingleElementScrollerAdaptor uia = Substitute.For<IGenericSingleElementScrollerAdaptor>();
			Rect scrollerRect = new Rect(Vector2.zero, new Vector2(200f, 100f));
			uia.GetRect().Returns(scrollerRect);
			IUIElement child = Substitute.For<IUIElement>();
			IUIAdaptor childUIA = Substitute.For<IUIAdaptor>();
			Rect elementRect = new Rect(Vector2.zero, new Vector2(100f, 100f));
				childUIA.GetRect().Returns(elementRect);
			child.GetUIAdaptor().Returns(childUIA);
			List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{child});
			uia.GetChildUIEs().Returns(returnedList);
		IUIImage image = Substitute.For<IUIImage>();

		IGenericSingleElementScrollerConstArg arg = new GenericSingleElementScrollerConstArg(relativeCursorLength, scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, true, uim, processFactory, uieFactory, uia, image);

		return new TestGenericSingleElementScroller(arg);
	}
}
