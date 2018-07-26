using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using NSubstitute;
using UISystem;
using DKUtility;

[TestFixture, Category("UISystem")]
public class UIElementGroupScrollerTest {

    [Test, TestCaseSource(typeof(ConstructorArg_MakeCursorSizeInRange_TestCase), "cases")]
    public void ConstructorArg_CursorSizeLessThanOne_ClampItUpToOne(int[] cursorSize, int[] expected){
        IUIElementGroupScrollerConstArg arg = CreateUIElementGroupScrollerConstArgWithCursorSize(cursorSize);

        int[] actual = arg.cursorSize;

        for(int i = 0; i < 2; i ++){
            Assert.That(actual[i], Is.EqualTo(expected[i]));
        }
    }
    public class ConstructorArg_MakeCursorSizeInRange_TestCase{
        public static object[] cases = {
            new object[]{new int[]{0,0}, new int[]{1, 1}},
            new object[]{new int[]{-1,-1}, new int[]{1, 1}},
            new object[]{new int[]{1,1}, new int[]{1, 1}},
            new object[]{new int[]{10,10}, new int[]{10, 10}},
        };
    }
    [Test, TestCaseSource(typeof(ThisCursorLength_ReturnsCalculatedValue_TestCase), "cases")]
    public void ThisCursorLength_CursorLengthUndersizedToScrollerLength_ReturnsCalculatedValue(int[] cursorSize, Vector2 uiElementLength, Vector2 padding, Vector2 expected){
        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        float uiElementGroupLengthX = (cursorSize[0] * 2) * (uiElementLength[0] + padding[0]) + padding[0];
        float uiElementGroupLengthY = (cursorSize[1] * 2) * (uiElementLength[1] + padding[1]) + padding[1];
        Rect uiElementGroupRect = new Rect(Vector2.zero, new Vector2(uiElementGroupLengthX, uiElementGroupLengthY));
        Rect scrollerRect = new Rect(Vector2.zero, uiElementGroupRect.size * 1.2f);
        TestUIElementGroupScroller scroller = CreateTestUIElementGroupScrollerWithRects(scrollerRect, cursorSize, relativeCursorPosition, uiElementLength, padding, uiElementGroupRect);
        scroller.ActivateImple();

        Vector2 actual = scroller.thisCursorLength_Test;

        Assert.That(actual, Is.EqualTo(expected));
    }
    [Test, TestCaseSource(typeof(ThisCursorLength_ReturnsCalculatedValue_TestCase), "cases")]
    public void ThisCursorLength_CursorLengthOversizedToScrollerLength_ThrowsException(int[] cursorSize, Vector2 uiElementLength, Vector2 padding, Vector2 expected){
        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        float uiElementGroupLengthX = (cursorSize[0] * 2) * (uiElementLength[0] + padding[0]) + padding[0];
        float uiElementGroupLengthY = (cursorSize[1] * 2) * (uiElementLength[1] + padding[1]) + padding[1];
        Rect uiElementGroupRect = new Rect(Vector2.zero, new Vector2(uiElementGroupLengthX, uiElementGroupLengthY));
        Rect scrollerRect = new Rect(Vector2.zero, new Vector2(119f, 69f));
        TestUIElementGroupScroller scroller = CreateTestUIElementGroupScrollerWithRects(scrollerRect, cursorSize, relativeCursorPosition, uiElementLength, padding, uiElementGroupRect);
        
        Assert.Throws(Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("cursorLength cannot exceed this rect length. provide lesser cursor size"), () => {scroller.ActivateImple();});

    }
    public class ThisCursorLength_ReturnsCalculatedValue_TestCase{
        public static object[] cases = {
            new object[]{new int[]{1, 1}, new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f, 70f)},
            new object[]{new int[]{2, 2}, new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(230f, 130f)},
            new object[]{new int[]{1, 3}, new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(120f, 190f)},
            new object[]{new int[]{5, 5}, new Vector2(100f, 50f), new Vector2(10f, 10f), new Vector2(560f, 310f)},
        };
    }
    [Test, TestCaseSource(typeof(GetInitialPositionNormalizedToCursor_TestCase), "cases")]
    public void GetInitialPositionNormalizedToCursor_Various(int initiallyCursoredElementIndex, Vector2 uiElementLocalPos, Vector2 expected, int[] cursorSize, int[] uieGroupMultiplierToCursor){
        Vector2 uiElementLength = new Vector2(280f, 80f);
        Vector2 padding = new Vector2(10f, 10f);

        TestUIElementGroupScroller scroller = CreateTestUIElementGroupScrollerFull(initiallyCursoredElementIndex, uiElementLocalPos, uiElementLength, padding, cursorSize, uieGroupMultiplierToCursor);
        scroller.ActivateImple();

        Vector2 actual = scroller.GetInitialPositionNormalizedToCursor_Test();

        Assert.That(actual, Is.EqualTo(expected));
    }
    public class GetInitialPositionNormalizedToCursor_TestCase{
        public static object[] cases = {
            new object[]{0, new Vector2(10f, 10f), new Vector2(0f, 0f), new int[]{1, 3}, new int[]{1,2}},
            new object[]{1, new Vector2(10f, 100f), new Vector2(0f, 1f/3f), new int[]{1, 3}, new int[]{1,2}},
            new object[]{2, new Vector2(10f, 190f), new Vector2(0f, 2f/3f), new int[]{1, 3}, new int[]{1,2}},
            new object[]{0, new Vector2(10f, 10f), new Vector2(0f, 0f), new int[]{2, 2}, new int[]{2,2}},
            new object[]{0, new Vector2(10f, 100f), new Vector2(0f, .5f), new int[]{2, 2}, new int[]{2,2}},
            new object[]{0, new Vector2(10f, 190f), new Vector2(0f, 1f), new int[]{2, 2}, new int[]{2,2}},
            new object[]{0, new Vector2(10f, 280f), new Vector2(0f, 1.5f), new int[]{2, 2}, new int[]{2,2}},
            new object[]{0, new Vector2(300f, 100f), new Vector2(.5f, .5f), new int[]{2, 2}, new int[]{2,2}},
            new object[]{0, new Vector2(590f, 190f), new Vector2(1f, 1f), new int[]{2, 2}, new int[]{2,2}},
        };
    }




    public IUIElementGroupScrollerConstArg CreateUIElementGroupScrollerConstArgWithCursorSize(int[] cursorSize){
        Vector2 uiElementLength = new Vector2(100f, 50f);
        Vector2 padding = new Vector2(10f, 10f);
        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        ScrollerAxis scrollerAxis = ScrollerAxis.Both;
        Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
        IUIManager uim = Substitute.For<IUIManager>();
        IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
        IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
        IUIElementGroupScrollerAdaptor uia = Substitute.For<IUIElementGroupScrollerAdaptor>();
        IUIImage uiImage = Substitute.For<IUIImage>();
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(0, cursorSize, uiElementLength, padding, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        return arg;
    }
    public class TestUIElementGroupScroller: UIElementGroupScroller{
        public TestUIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){}
        public Vector2 GetInitialPositionNormalizedToCursor_Test(){
            return GetInitialPositionNormalizedToCursor();
        }
        public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
        public Vector2 thisRectLength_Test{get{return thisRectLength;}}
        public Vector2 thisScrollerElementLength_Test{get{return thisScrollerElementLength;}}
    }
    [Test, TestCaseSource(typeof(TestUIElementGroupScroller_CreateFull_TestCase), "cases")]
    public void TestUIElementGroupScroller_CreateFull_WorksFine(Vector2 uiElementLength, Vector2 padding, int[] cursorSize, int[] uieGroupMultiplierToCursor, Vector2 expScrollerRectLength, Vector2 expCursorLength, Vector2 expScrEleLength){
        TestUIElementGroupScroller scroller = CreateTestUIElementGroupScrollerFull(0, Vector2.zero, uiElementLength, padding, cursorSize, uieGroupMultiplierToCursor);
        scroller.ActivateImple();

        Assert.That(scroller.thisRectLength_Test, Is.EqualTo(expScrollerRectLength));
        Assert.That(scroller.thisCursorLength_Test, Is.EqualTo(expCursorLength));
        Assert.That(scroller.thisScrollerElementLength_Test, Is.EqualTo(expScrEleLength));
    }
    public class TestUIElementGroupScroller_CreateFull_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(280f, 80f), new Vector2(10f, 10f), new int[]{1, 3}, new int[]{1, 2}, new Vector2(300f, 280f), new Vector2(300f, 280f), new Vector2(300f, 550f)},
            new object[]{new Vector2(280f, 80f), new Vector2(10f, 10f), new int[]{2, 2}, new int[]{2, 2}, new Vector2(590f, 190f), new Vector2(590f, 190f), new Vector2(1170f, 370f)},
        };
    }
    public TestUIElementGroupScroller CreateTestUIElementGroupScrollerWithRects(Rect scrollerRect, int[] cursorSize, Vector2 relativeCursorPosition, Vector2 uiElementLength, Vector2 padding, Rect uiElementGroupRect){
        ScrollerAxis scrollerAxis = ScrollerAxis.Both;
        Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
        IUIManager uim = Substitute.For<IUIManager>();
        IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
        IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
        IUIElementGroupScrollerAdaptor uia = Substitute.For<IUIElementGroupScrollerAdaptor>();
            uia.GetRect().Returns(scrollerRect);
            IUIElementGroup uiElementGroup = Substitute.For<IUIElementGroup>();
            IUIAdaptor childAdaptor = Substitute.For<IUIAdaptor>();
            uiElementGroup.GetUIAdaptor().Returns(childAdaptor);
            childAdaptor.GetRect().Returns(uiElementGroupRect);
            List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{uiElementGroup});
            uia.GetChildUIEs().Returns(returnedList);
        IUIImage uiImage = Substitute.For<IUIImage>();
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(0, cursorSize, uiElementLength, padding, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        return scroller;
    }
    public TestUIElementGroupScroller CreateTestUIElementGroupScrollerFull(int initiallyCursoredElementIndex, Vector2 uiElementLocalPos, Vector2 uiElementLength, Vector2 padding, int[] cursorSize, int[] uieGroupMultiplierToCursor){
   ScrollerAxis scrollerAxis = ScrollerAxis.Both;
        Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
        IUIManager uim = Substitute.For<IUIManager>();
        IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
        IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
        IUIElementGroupScrollerAdaptor uia = Substitute.For<IUIElementGroupScrollerAdaptor>();
            IUIElementGroup uiElementGroup = Substitute.For<IUIElementGroup>();
                IUIElement initiallyCursoredElement = Substitute.For<IUIElement>();
                uiElementGroup.GetUIElement(initiallyCursoredElementIndex).Returns(initiallyCursoredElement);
                initiallyCursoredElement.GetLocalPosition().Returns(uiElementLocalPos);
            IUIAdaptor childAdaptor = Substitute.For<IUIAdaptor>();
            uiElementGroup.GetUIAdaptor().Returns(childAdaptor);
            float uiElementGroupRectLengthX = cursorSize[0] * uieGroupMultiplierToCursor[0] * (uiElementLength[0] + padding[0]) + padding[0];
            float uiElementGroupRectLengthY = cursorSize[1] * uieGroupMultiplierToCursor[1] * (uiElementLength[1] + padding[1]) + padding[1];
            Rect uiElementGroupRect = new Rect(Vector2.zero, new Vector2(uiElementGroupRectLengthX, uiElementGroupRectLengthY));
            childAdaptor.GetRect().Returns(uiElementGroupRect);
            List<IUIElement> returnedList = new List<IUIElement>(new IUIElement[]{uiElementGroup});
            uia.GetChildUIEs().Returns(returnedList);
            float scrollerRectLengthX = cursorSize[0] * (uiElementLength[0] + padding[0]) + padding[0];
            float scrollerRectLengthY = cursorSize[1] * (uiElementLength[1] + padding[1]) + padding[1];
            Rect scrollerRect = new Rect(Vector2.zero, new Vector2(scrollerRectLengthX, scrollerRectLengthY));
            uia.GetRect().Returns(scrollerRect);
        IUIImage uiImage = Substitute.For<IUIImage>();
        Vector2 relativeCursorPosition = new Vector2(0f, 0f);
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(initiallyCursoredElementIndex, cursorSize, uiElementLength, padding, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        return scroller;
    }
}
