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
	[Test, TestCaseSource(typeof(Constructor_TestCase), "lessThan0Cases")]
	public void Constructor_RelativeCursorSizeNotGreaterThanZero_ThrowsException(Vector2 relativeCursorLength){
		IGenericSingleElementScrollerConstArg arg = CreateMockConstArg();
		arg.relativeCursorLength.Returns(relativeCursorLength);

		Assert.Throws(
			Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("relativeCursorLength must be greater than 0"),
			() => {
				new TestGenericSingleElementScroller(arg);
			}
		);
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
	[Test, TestCaseSource(typeof(Constructor_TestCase), "greaterThan0Cases")]
	public void Constructor_RelativeCursorLengthGreaterThanOne_ClampsValueDownToOne(Vector2 relativeCursorLength, Vector2 expected){
		IGenericSingleElementScrollerConstArg arg = CreateMockConstArg();
		arg.relativeCursorLength.Returns(relativeCursorLength);
		TestGenericSingleElementScroller scroller = new TestGenericSingleElementScroller(arg);
		scroller.ActivateImple();

		Vector2 actual = scroller.thisRelativeCursorLength_Test;

		Assert.That(actual, Is.EqualTo(expected));
	}
	
	IGenericSingleElementScrollerConstArg CreateMockConstArg(){
		IGenericSingleElementScrollerConstArg arg = Substitute.For<IGenericSingleElementScrollerConstArg>();
		arg.relativeCursorLength.Returns(new Vector2(.5f, .5f));
		arg.scrollerAxis.Returns(ScrollerAxis.Both);
		arg.rubberBandLimitMultiplier.Returns(new Vector2(.1f, .1f));
		arg.relativeCursorPosition.Returns(new Vector2(0f, 0f));
		arg.isEnabledInertia.Returns(true);
		arg.uim.Returns(Substitute.For<IUIManager>());
		arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
		arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
			IGenericSingleElementScrollerAdaptor uia = Substitute.For<IGenericSingleElementScrollerAdaptor>();
			uia.GetRect().Returns(new Rect(Vector2.zero, new Vector2(200f, 100f)));
			uia.GetChildUIEs().Returns(new List<IUIElement>(new IUIElement[]{Substitute.For<IUIElement>()}));
				IUIAdaptor childAdaptor = Substitute.For<IUIAdaptor>();
				childAdaptor.GetRect().Returns(new Rect(Vector2.zero, new Vector2(100f, 100f)));
				uia.GetChildUIEs()[0].GetUIAdaptor().Returns(childAdaptor);
		arg.uia.Returns(uia);
		arg.image.Returns(Substitute.For<IUIImage>());
		return arg;
	}
	public class TestGenericSingleElementScroller: GenericSingleElementScroller{
		public TestGenericSingleElementScroller(IGenericSingleElementScrollerConstArg arg): base(arg){
		}
		public Vector2 thisRelativeCursorLength_Test{get{return thisRelativeCursorLength;}}
		public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
		protected override IScroller FindProximateParentScroller(){
			IScroller parentScroller = Substitute.For<IScroller>();
			IScroller nullScroller = null;
			parentScroller.GetProximateParentScroller().Returns(nullScroller);
			return parentScroller;
		}
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
		float newScrollSpeedThreshold = 200f;

		IGenericSingleElementScrollerConstArg arg = new GenericSingleElementScrollerConstArg(
			relativeCursorLength, 
			scrollerAxis, 
			rubberBandLimitMultiplier, 
			relativeCursorPosition, 
			true, 
			newScrollSpeedThreshold,

			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			image,
			ActivationMode.None
		);

		return new TestGenericSingleElementScroller(arg);
	}
}
