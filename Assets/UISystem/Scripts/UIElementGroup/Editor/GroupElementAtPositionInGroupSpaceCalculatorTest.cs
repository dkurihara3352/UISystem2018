using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;


[TestFixture, Category("UISystem")]
public class GroupElementAtPositionInGroupSpaceCalculatorTest {
	[Test, TestCaseSource(typeof(Calculate_TestCase), "outOfBounds")]
	public void Calculate_PositionOutOfBounds_ReturnsNull(Vector2 position){
		IUIElement[,] array;
		IGroupElementAtPositionInGroupSpaceCalculator calculator = CreateCalculator(out array);

		IUIElement actual = calculator.Calculate(position);

		Assert.That(actual, Is.Null);
	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "moduloZeroInvalid")]
	public void Calculate_ModuloZero_PositionZero_PaddingNonZero_ReturnsNull(Vector2 position){
		IUIElement[,] array;
		IGroupElementAtPositionInGroupSpaceCalculator calculator = CreateCalculator(out array);

		IUIElement actual = calculator.Calculate(position);

		Assert.That(actual, Is.Null);

	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "moduloZeroValid")]
	public void Calculate_ModuloZero_ReturnsElementAtPos(Vector2 position, int[] expectedArrayIndex){
		/*  pointer at the greater edge
		*/
		IUIElement[,] array;
		IGroupElementAtPositionInGroupSpaceCalculator calculator = CreateCalculator(out array);

		IUIElement actual = calculator.Calculate(position);

		IUIElement expected = array[expectedArrayIndex[0], expectedArrayIndex[1]];

		Assert.That(actual, Is.SameAs(expected));
	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "moduloNonZeroInvalid")]
	public void Calculate_ModuloNonZero_SmallerThanPaddingPlusMargin_ReturnsNull(Vector2 position){
		IUIElement[,] array;
		IGroupElementAtPositionInGroupSpaceCalculator calculator = CreateCalculator(out array);

		IUIElement actual = calculator.Calculate(position);

		Assert.That(actual, Is.Null);
	}
	[Test, TestCaseSource(typeof(Calculate_TestCase), "moduloNonZeroValid")]
	public void Calculate_ModuloNonZero_NotSmallerThanPaddingPlusMargin_ReturnsElementAtPos(Vector2 position, int[] expectedArrayIndex){
		IUIElement[,] array;
		IGroupElementAtPositionInGroupSpaceCalculator calculator = CreateCalculator(out array);

		IUIElement actual = calculator.Calculate(position);

		IUIElement expected = array[expectedArrayIndex[0], expectedArrayIndex[1]];
		Assert.That(actual, Is.SameAs(expected));
	}



	IGroupElementAtPositionInGroupSpaceCalculator CreateCalculator(out IUIElement[, ] array){
		IUIElement[,] elementsArray = CreateElementsArray();
		IGroupElementAtPositionInGroupSpaceCalculator calculator = new GroupElementAtPositionInGroupSpaceCalculator(
			elementsArray, 
			thisElementLength, 
			thisPadding, 
			thisGroupRectLength,
			"some name"
		);
		array = elementsArray;
		return calculator;
	}
	static Vector2 thisElementLength = new Vector2(100f, 50f);
	static Vector2 thisPadding = new Vector2(10f, 10f);
	static Vector2 thisGroupRectLength = new Vector2(340f, 190f);
	static Vector2 slightOffset = new Vector2(.001f, .001f);
	static float marginOfError = .01f;
	
	static Vector2 boundBottomLeft = new Vector2(
		thisPadding.x,
		thisPadding.y
	);
	static Vector2 boundTopLeft = new Vector2(
		thisPadding.x,
		thisGroupRectLength.y - thisPadding.y
	);
	static Vector2 boundTopRight = new Vector2(
		thisGroupRectLength.x - thisPadding.x,
		thisGroupRectLength.y - thisPadding.y

	);
	static Vector2 boundBottomRight = new Vector2(
		thisGroupRectLength.x - thisPadding.x,
		thisPadding.y
	);
	static Vector2 elementPlusPadding = thisPadding + thisElementLength;
	static Vector2 paddingMinusMargin = new Vector2(
		thisPadding.x - marginOfError,
		thisPadding.y - marginOfError
	);
	IUIElement[,] CreateElementsArray(){
		IUIElement[,] result = new IUIElement[3,3];
		for(int i = 0; i < 3; i ++){
			for(int j = 0; j < 3; j ++){
				result[i, j] = Substitute.For<IUIElement>();
			}
		}
		return result;
	}
	public class Calculate_TestCase{
		public static object[] outOfBounds = {
			new object[]{
				new Vector2(
					boundBottomLeft.x - slightOffset.x,
					boundBottomLeft.y - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundBottomLeft.x,
					boundBottomLeft.y - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundBottomLeft.x - slightOffset.x,
					boundBottomLeft.y
				)
			},

			new object[]{
				new Vector2(
					boundTopLeft.x - slightOffset.x,
					boundTopLeft.y + slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundTopLeft.x,
					boundTopLeft.y + slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundTopLeft.x - slightOffset.x,
					boundTopLeft.y
				)
			},
			
			new object[]{
				new Vector2(
					boundTopRight.x + slightOffset.x,
					boundTopRight.y + slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundTopRight.x,
					boundTopRight.y + slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundTopRight.x + slightOffset.x,
					boundTopRight.y
				)
			},
			
			new object[]{
				new Vector2(
					boundBottomRight.x + slightOffset.x,
					boundBottomRight.y - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundBottomRight.x,
					boundBottomRight.y - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					boundBottomRight.x + slightOffset.x,
					boundBottomRight.y
				)
			},
		};
		public static object[] moduloZeroInvalid = {
			new object[]{
				new Vector2(
					0f,
					1f
				)
			},
			new object[]{
				new Vector2(
					1f,
					0f
				)
			},
			new object[]{
				new Vector2(
					0f,
					0f
				)
			},
		};
		public static object[] moduloZeroValid = {
			new object[]{
				new Vector2(
					elementPlusPadding.x,
					elementPlusPadding.y
				),
				new int[]{0, 0}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 2f,
					elementPlusPadding.y
				),
				new int[]{1, 0}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 3f,
					elementPlusPadding.y
				),
				new int[]{2, 0}
			},

			new object[]{
				new Vector2(
					elementPlusPadding.x,
					elementPlusPadding.y * 2f
				),
				new int[]{0, 1}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 2f,
					elementPlusPadding.y * 2f
				),
				new int[]{1, 1}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 3f,
					elementPlusPadding.y * 2f
				),
				new int[]{2, 1}
			},

			new object[]{
				new Vector2(
					elementPlusPadding.x,
					elementPlusPadding.y * 3f
				),
				new int[]{0, 2}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 2f,
					elementPlusPadding.y * 3f
				),
				new int[]{1, 2}
			},
			new object[]{
				new Vector2(
					elementPlusPadding.x * 3f,
					elementPlusPadding.y * 3f
				),
				new int[]{2, 2}
			},
		};
		public static object[] moduloNonZeroInvalid = {
			new object[]{
				new Vector2(
					paddingMinusMargin.x - slightOffset.x,
					paddingMinusMargin.y
				)
			},
			new object[]{
				new Vector2(
					paddingMinusMargin.x,
					paddingMinusMargin.y - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					paddingMinusMargin.x + elementPlusPadding.x * 1f - slightOffset.x,
					paddingMinusMargin.y + elementPlusPadding.y * 1f
				)
			},
			new object[]{
				new Vector2(
					paddingMinusMargin.x + elementPlusPadding.x * 1f,
					paddingMinusMargin.y + elementPlusPadding.y * 1f - slightOffset.y
				)
			},
			new object[]{
				new Vector2(
					paddingMinusMargin.x + elementPlusPadding.x * 2f - slightOffset.x,
					paddingMinusMargin.y + elementPlusPadding.y * 2f
				)
			},
			new object[]{
				new Vector2(
					paddingMinusMargin.x + elementPlusPadding.x * 2f,
					paddingMinusMargin.y + elementPlusPadding.y * 2f - slightOffset.y
				)
			},
		};
		public static object[] moduloNonZeroValid = {
			new object[]{
				new Vector2(
					thisPadding.x,
					thisPadding.y
				),
				new int[]{
					0,
					0
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 1f,
					thisPadding.y
				),
				new int[]{
					1,
					0
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 2f,
					thisPadding.y
				),
				new int[]{
					2,
					0
				}
			},

			new object[]{
				new Vector2(
					thisPadding.x,
					thisPadding.y + elementPlusPadding.y * 1f
				),
				new int[]{
					0,
					1
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 1f,
					thisPadding.y + elementPlusPadding.y * 1f
				),
				new int[]{
					1,
					1
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 2f,
					thisPadding.y + elementPlusPadding.y * 1f
				),
				new int[]{
					2,
					1
				}
			},

			new object[]{
				new Vector2(
					thisPadding.x,
					thisPadding.y + elementPlusPadding.y * 2f
				),
				new int[]{
					0,
					2
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 1f,
					thisPadding.y + elementPlusPadding.y * 2f
				),
				new int[]{
					1,
					2
				}
			},
			new object[]{
				new Vector2(
					thisPadding.x + elementPlusPadding.x * 2f,
					thisPadding.y + elementPlusPadding.y * 2f
				),
				new int[]{
					2,
					2
				}
			},
		};
	}
}
