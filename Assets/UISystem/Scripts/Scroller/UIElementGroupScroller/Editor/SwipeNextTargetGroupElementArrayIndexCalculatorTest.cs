using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
[TestFixture, Category("UISystem")]
public class SwipeNextTargetGroupElementArrayIndexCalculatorTest{

	[Test, TestCaseSource(typeof(Calculate_TestCase), "cases")]
    public void Calculate_Various(
        int[] arraySize, 
        int[] cursorSize, 
        ScrollerAxis scrollerAxis, 
        Vector2 velocity, 
        int[] currentElementUnderCursorArrayIndex, 
        int[] expectedArrayIndex
    ){
        IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
        for(int i = 0; i < 2; i ++)
            uieGroup.GetArraySize(i).Returns(arraySize[i]);
        ISwipeNextTargetGroupElementArrayIndexCalculator calculator = new SwipeNextTargetGroupElementArrayIndexCalculator(uieGroup, cursorSize, scrollerAxis);

        int[] actual = calculator.Calculate(velocity, currentElementUnderCursorArrayIndex);

        Assert.That(actual, Is.EqualTo(expectedArrayIndex));
    }
    public class Calculate_TestCase{
        public static object[] cases = {
            //Horizontal
            //valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Horizontal,
                new Vector2(-1f, 1f),
                new int[]{0, 0},
                new int[]{1, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Horizontal,
                new Vector2(1f, 1f),
                new int[]{0, 0},
                new int[]{0, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Horizontal,
                new Vector2(-1f, 1f),
                new int[]{1, 0},
                new int[]{1, 0}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{1, 2},//
                ScrollerAxis.Horizontal,
                new Vector2(-1f, 1f),
                new int[]{1, 0},
                new int[]{2, 0}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Horizontal,
                new Vector2(1f, 1f),
                new int[]{1, 0},//
                new int[]{0, 0}
            },

            //Vertical
            //valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Vertical,
                new Vector2(1f, -1f),
                new int[]{0, 0},
                new int[]{0, 1}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Vertical,
                new Vector2(1f, 1f),
                new int[]{0, 0},
                new int[]{0, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Vertical,
                new Vector2(1f, -1f),
                new int[]{0, 1},
                new int[]{0, 1}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 1},//
                ScrollerAxis.Vertical,
                new Vector2(1f, -1f),
                new int[]{0, 1},
                new int[]{0, 2}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Vertical,
                new Vector2(1f, 1f),
                new int[]{0, 1},//
                new int[]{0, 0}
            },


            //Both
            //valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(-1f, -1f),//horizontal
                new int[]{0, 0},
                new int[]{1, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(1f, 1f),//horiz
                new int[]{0, 0},
                new int[]{0, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(-1f, 1f),//hor
                new int[]{1, 0},
                new int[]{1, 0}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{1, 1},//
                ScrollerAxis.Both,
                new Vector2(-1f, -1f),//hor
                new int[]{1, 0},
                new int[]{2, 0}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(1f, 1f),//hor
                new int[]{1, 0},//
                new int[]{0, 0}
            },


            //valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(.99f, -1f),//Ver
                new int[]{0, 0},
                new int[]{0, 1}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(.99f, 1f),//Ver
                new int[]{0, 0},
                new int[]{0, 0}
            },
            // no change
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(.99f, -1f),//Ver
                new int[]{0, 1},
                new int[]{0, 1}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 1},//
                ScrollerAxis.Both,
                new Vector2(.99f, -1f),//Ver
                new int[]{0, 1},
                new int[]{0, 2}
            },
            // valid
            new object[]{
                new int[]{3, 3},
                new int[]{2, 2},
                ScrollerAxis.Both,
                new Vector2(.99f, 1f),//Ver
                new int[]{0, 1},//
                new int[]{0, 0}
            },
        };
    }
}
