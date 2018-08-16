using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class ElementCursorOffsetInPixelCalculatorTest {
	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases1")]
	public void Calculate_ScrollerElementIsUndersizedToCursorLength_ReturnLocalPosDiff(Vector2 cursorLocalPos, Vector2 scrollerElementLocalPos, Vector2 expected){
		Vector2 anyCursorLength = new Vector2(1000f, 10000f);
		Vector2 anyScrollerElementLength = new Vector2(100f, 100000f);
		IScroller scroller = Substitute.For<IScroller>();
		scroller.ScrollerElementIsUndersizedTo(Arg.Any<Vector2>(), Arg.Any<int>()).Returns(true);
		ElementCursorOffsetInPixelCalculator calculator = new ElementCursorOffsetInPixelCalculator(
			scroller,
			anyCursorLength, 
			cursorLocalPos, 
			anyScrollerElementLength
		);
		for(int i = 0; i < 2; i ++){
			float actual = calculator.Calculate(scrollerElementLocalPos[i], i);

			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases2")]
	public void Calculate_ScrollerElementIsNOTUndersizedToCursorLength_NormalizedCursoredPositionOnAxisWithinRange_ReturnsZero(
		Vector2 normalizedCursoredPosition
	){
		IScroller scroller = Substitute.For<IScroller>();
		scroller.ScrollerElementIsUndersizedTo(Arg.Any<Vector2>(), Arg.Any<int>()).Returns(false);
		Vector2 anyCursorLength = Vector2.zero;
		Vector2 anyCursorPosition = Vector2.zero;
		Vector2 anyScrollerElementLength = Vector2.zero;
		ElementCursorOffsetInPixelCalculator calculator = new ElementCursorOffsetInPixelCalculator(
			scroller,
			anyCursorLength,
			anyCursorPosition,
			anyScrollerElementLength
		);
		
		float anyLocalPos = 0f;
		for(int i = 0; i < 2; i ++){
			scroller.GetNormalizedCursoredPositionOnAxis(Arg.Any<float>(), i).Returns(normalizedCursoredPosition[i]);

			float actual = calculator.Calculate(anyLocalPos, i);
			Assert.That(actual, Is.EqualTo(0f));
		}
	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases3")]
	public void Calculate_ScrollerElementIsNOTUndersizedToCursorLength_NormalizedCursoredPositionOnAxisNotWithinRange_ReturnsNonZeroVarious(
		Vector2 normalizedCursoredPosition,
		Vector2 scrollerElementLength,
		Vector2 cursorLength,
		Vector2 expected
	){
		IScroller scroller = Substitute.For<IScroller>();
		scroller.ScrollerElementIsUndersizedTo(Arg.Any<Vector2>(), Arg.Any<int>()).Returns(false);
		Vector2 anyCursorPosition = Vector2.zero;
		ElementCursorOffsetInPixelCalculator calculator = new ElementCursorOffsetInPixelCalculator(
			scroller,
			cursorLength,
			anyCursorPosition,
			scrollerElementLength
		);
		
		for(int i = 0; i < 2; i ++){
			scroller.GetNormalizedCursoredPositionOnAxis(Arg.Any<float>(), i).Returns(normalizedCursoredPosition[i]);

			float actual = calculator.Calculate(normalizedCursoredPosition[i], i);
			Assert.That(actual, Is.EqualTo(expected[i]));
		}
	}
	public class Calculate_TestCase{
		public static object[] cases1 = {
			new object[]{
				new Vector2(0f, 0f),
				new Vector2(10f, 10f),
				new Vector2( -10f, -10f)
			},
			new object[]{
				new Vector2(-10f, -10f),
				new Vector2(10f, 10f),
				new Vector2( -20f, -20f)
			},
			new object[]{
				new Vector2(-10f, -10f),
				new Vector2(-10f, -10f),
				new Vector2( 0f, 0f)
			},
		};
		public static object[] cases2 = {
			new object[]{
				new Vector2(0f, 0f)
			},
			new object[]{
				new Vector2(.001f, .001f)
			},
			new object[]{
				new Vector2(.99f, .99f)
			},
			new object[]{
				new Vector2(1f, 1f)
			},
		};
		public static object[] cases3 = {
			/*  normalizedCursoredPosition (not between 0f to 1f)
				scrollerElementLength (bigger than cursor length)
				cursorLength
				expected
			*/
			new object[]{
				new Vector2(-1f, -1),
				new Vector2(2000f, 2000f),
				new Vector2(1000f, 1000f),
				new Vector2(-1000f, -1000f)
			},
			new object[]{
				new Vector2(-.5f, -.5f),
				new Vector2(2000f, 2000f),
				new Vector2(1000f, 1000f),
				new Vector2(-500f, -500f)
			},
			new object[]{
				new Vector2(1.5f, 1.5f),
				new Vector2(2000f, 2000f),
				new Vector2(1000f, 1000f),
				new Vector2(500f, 500f)
			},
			new object[]{
				new Vector2(2f, 2f),
				new Vector2(2000f, 2000f),
				new Vector2(1000f, 1000f),
				new Vector2(1000f, 1000f)
			},
		};
	}
}
