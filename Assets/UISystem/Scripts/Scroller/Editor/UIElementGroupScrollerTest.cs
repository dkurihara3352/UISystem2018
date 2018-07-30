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

    [Test, TestCaseSource(typeof(Construction_CursorSizeNotGreaterThan0_TestCase), "cases")]
    public void Construction_CursorSizeLessThanOne_ReturnsOne(int[] cursorSize, int[] expected){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        arg.cursorSize.Returns(cursorSize);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        
        int[] actual = scroller.thisCursorSize_Test;

        for(int i = 0; i < 2; i ++)
            Assert.That(actual[i], Is.EqualTo(expected[i]));
    }
    public class Construction_CursorSizeNotGreaterThan0_TestCase{
        public static object[] cases = {
            new object[]{new int[]{0, 0}, new int[]{1 ,1}},
            new object[]{new int[]{-1, -1}, new int[]{1 ,1}},
            new object[]{new int[]{1, 1}, new int[]{1 ,1}},
            new object[]{new int[]{2, 2}, new int[]{2, 2}},
        };
    }
    [Test, TestCaseSource(typeof(Construction_StartSearchSpeedNotGreaterThanZero_TestCase), "cases")]
    public void Construction_StartSearchSpeedNotGreaterThanZero_ThrowException(float startSearchSpeed){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        arg.startSearchSpeed.Returns(startSearchSpeed);

        Assert.Throws(
            Is.TypeOf(typeof(System.InvalidOperationException)).And.Message.EqualTo("startSearchSpeed must be greater than zero"),
            () => {
                new TestUIElementGroupScroller(arg);
            }
        );
    }
    public class Construction_StartSearchSpeedNotGreaterThanZero_TestCase{
        public static object[] cases = {
            new object[]{0f},
            new object[]{-.1f},
            new object[]{-10f},
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
    [Test, TestCaseSource(typeof(GetUIElementNormalizedCursoredPositionOnAxis_TestCase), "cases")]
    public void GetUIElementNormalizedCursoredPositionOnAxis_Various(Vector2 elementLength, Vector2 padding, int[] arraySize, Vector2 elementLocalPos, Vector2 expected){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        arg.groupElementLength.Returns(elementLength);
        arg.padding.Returns(padding);
        IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
            IUIElementGroupScrollerAdaptor uieGroupAdaptor = Substitute.For<IUIElementGroupScrollerAdaptor>();
                float uieGroupRectLengthX = (elementLength.x + padding.x) * arraySize[0] + padding.x;
                float uieGroupRectLengthY = (elementLength.y + padding.y) * arraySize[1] + padding.y;
                Vector2 uieGroupRectLength = new Vector2(uieGroupRectLengthX, uieGroupRectLengthY);
                Rect uieGroupRect = new Rect(Vector2.zero, uieGroupRectLength);
            uieGroupAdaptor.GetRect().Returns(uieGroupRect);
        uieGroup.GetUIAdaptor().Returns(uieGroupAdaptor);
        List<IUIElement> children = new List<IUIElement>(new IUIElement[]{uieGroup});
        arg.uia.GetChildUIEs().Returns(children);
        IUIElement element = Substitute.For<IUIElement>();
        arg.cursorSize.Returns(new int[]{1, 1});
        arg.relativeCursorPosition.Returns(Vector2.zero);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        element.GetLocalPosition().Returns(elementLocalPos);
        scroller.ActivateImple();

        for(int i = 0; i < 2; i ++){
            float actual = scroller.GetUIElementNormalizedCursoredPositionOnAxis_Test(element, i);
            
            Assert.That(actual, Is.EqualTo(expected[i]));
        }
    }
    public class GetUIElementNormalizedCursoredPositionOnAxis_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(10f, 10f), new Vector2(0f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(120f, 10f), new Vector2(1f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(10f, 70f), new Vector2(0f, 1f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(120f, 70f), new Vector2(1f, 1f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(65f, 40f), new Vector2(.5f, .5f)},
        };
    }
    [Test, TestCaseSource(typeof(GetGroupElementOffset_TestCase), "cases")]
    public void GetGroupElementOffset_Various(Vector2 elementLength, Vector2 padding, int[] arraySize, Vector2 uieGroupLocalPos, Vector2 expected){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        arg.groupElementLength.Returns(elementLength);
        arg.padding.Returns(padding);
        IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
            IUIElementGroupScrollerAdaptor uieGroupAdaptor = Substitute.For<IUIElementGroupScrollerAdaptor>();
                float uieGroupRectLengthX = (elementLength.x + padding.x) * arraySize[0] + padding.x;
                float uieGroupRectLengthY = (elementLength.y + padding.y) * arraySize[1] + padding.y;
                Vector2 uieGroupRectLength = new Vector2(uieGroupRectLengthX, uieGroupRectLengthY);
                Rect uieGroupRect = new Rect(Vector2.zero, uieGroupRectLength);
            uieGroupAdaptor.GetRect().Returns(uieGroupRect);
        uieGroup.GetUIAdaptor().Returns(uieGroupAdaptor);
        uieGroup.GetLocalPosition().Returns(uieGroupLocalPos);
        List<IUIElement> children = new List<IUIElement>(new IUIElement[]{uieGroup});
        arg.uia.GetChildUIEs().Returns(children);
        arg.cursorSize.Returns(new int[]{1, 1});
        arg.relativeCursorPosition.Returns(Vector2.zero);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        scroller.ActivateImple();

        for(int i = 0; i < 2; i ++){
            float actual = scroller.GetElementGroupOffset_Test(i);
            
            Assert.That(actual, Is.EqualTo(expected[i]));
        }
    }
    public class GetGroupElementOffset_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, 0f), new Vector2(0f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(-27.5f, 0f), new Vector2(.25f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(-55f, 0f), new Vector2(.5f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(-82.5f, 0f), new Vector2(.75f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(-110f, 0f), new Vector2(0f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(27.5f, 0f), new Vector2(.75f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(55f, 0f), new Vector2(.5f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(82.5f, 0f), new Vector2(.25f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(110f, 0f), new Vector2(0f, 0f)},
            
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, -15f), new Vector2(0f, .25f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, -30f), new Vector2(0f, .5f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, -45f), new Vector2(0f, .75f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, -60f), new Vector2(0f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, 15f), new Vector2(0f, .75f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, 30f), new Vector2(0f, .5f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, 45f), new Vector2(0f, .25f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(0f, 60f), new Vector2(0f, 0f)},
        };
    }
    [Test, TestCaseSource(typeof(SortOutCursoredElements_TestCase), "cases")]
    public void SortOutCursoredElements_Various(int[] currentCursoredElementsIndex, int[] newCursoredElementsIndex, int[] expectedElementsToDefocusIndex, int[] expectedElementsToFocusIndex){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        List<IUIElement> elements = CreateUIElements(10);
        IUIElement[] currentCursoredElements = GetUIElementsFromIndex(currentCursoredElementsIndex, elements);
        IUIElement[] newCursoredElements = GetUIElementsFromIndex(newCursoredElementsIndex, elements);
        IUIElement[] expectedElementsToDefocus = GetUIElementsFromIndex(expectedElementsToDefocusIndex, elements);
        IUIElement[] expectedElementsToFocus = GetUIElementsFromIndex(expectedElementsToFocusIndex, elements);
        
        IUIElement[] actualElementsToDefocus;
        IUIElement[] actualElementsToFocus;

        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        
        scroller.SortOutCursoredElements_Test(currentCursoredElements, newCursoredElements, out actualElementsToDefocus, out actualElementsToFocus);

        Assert.That(actualElementsToDefocus, Is.EqualTo(expectedElementsToDefocus));
        Assert.That(actualElementsToFocus, Is.EqualTo(expectedElementsToFocus));
    }
    List<IUIElement> CreateUIElements(int count){
        List<IUIElement> result = new List<IUIElement>();
        for(int i = 0; i < count; i ++){
            result.Add(Substitute.For<IUIElement>());
        }
        return result;
    }
    IUIElement[] GetUIElementsFromIndex(int[] index, List<IUIElement> list){
        List<IUIElement> result = new List<IUIElement>();
        foreach(int i in index){
            result.Add(list[i]);
        }
        return result.ToArray();
    }
    public class SortOutCursoredElements_TestCase{
        public static object[] cases = {
            new object[]{
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{3, 4, 5, 6, 7, 8},
                new int[]{0, 1, 2},
                new int[]{6, 7, 8},
            },
            new object[]{
                new int[]{},
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{},
                new int[]{0, 1, 2, 3, 4, 5}
            },
            new object[]{
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{},
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{},
            },
            new object[]{
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{6, 7, 8, 9},
                new int[]{0, 1, 2, 3, 4, 5},
                new int[]{6, 7, 8, 9},
            },
        };
    }



    public IUIElementGroupScrollerConstArg CreateMockConstArg(){
        IUIElementGroupScrollerConstArg arg = Substitute.For<IUIElementGroupScrollerConstArg>();
        arg.initiallyCursoredGroupElementIndex.Returns(0);
        arg.cursorSize.Returns(new int[]{1, 1});
        arg.groupElementLength.Returns(new Vector2(50f, 50f));
        arg.padding.Returns(new Vector2(10f, 10f));
        arg.startSearchSpeed.Returns(1f);
        arg.relativeCursorPosition.Returns(Vector2.zero);
        arg.scrollerAxis.Returns(ScrollerAxis.Both);
        arg.rubberBandLimitMultiplier.Returns(new Vector2(.1f, .1f));
        arg.isEnabledInertia.Returns(true);
        arg.uim.Returns(Substitute.For<IUIManager>());
        arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
        arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
        IUIAdaptor uia = Substitute.For<IUIAdaptor>();
            uia.GetRect().Returns(new Rect(Vector2.zero, new Vector2(200f, 100f)));
            IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
            IUIElementGroupAdaptor uieGroupAdaptor = Substitute.For<IUIElementGroupAdaptor>();
            uieGroup.GetUIAdaptor().Returns(uieGroupAdaptor);
            uieGroupAdaptor.GetRect().Returns(new Rect(Vector2.zero, new Vector2(130f, 130f)));
        arg.uia.Returns(uia);
        arg.image.Returns(Substitute.For<IUIImage>());
        return arg;
    }
    public IUIElementGroupScrollerConstArg CreateUIElementGroupScrollerConstArgWithCursorSize(int[] cursorSize){
        Vector2 uiElementLength = new Vector2(100f, 50f);
        Vector2 padding = new Vector2(10f, 10f);
        float startSearchSpeed = 1f;
        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        ScrollerAxis scrollerAxis = ScrollerAxis.Both;
        Vector2 rubberBandLimitMultiplier = new Vector2(.1f, .1f);
        IUIManager uim = Substitute.For<IUIManager>();
        IUISystemProcessFactory processFactory = Substitute.For<IUISystemProcessFactory>();
        IUIElementFactory uieFactory = Substitute.For<IUIElementFactory>();
        IUIElementGroupScrollerAdaptor uia = Substitute.For<IUIElementGroupScrollerAdaptor>();
        IUIImage uiImage = Substitute.For<IUIImage>();
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(0, cursorSize, uiElementLength, padding, startSearchSpeed, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        return arg;
    }
    public class TestUIElementGroupScroller: UIElementGroupScroller{
        public TestUIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){}
        public Vector2 GetInitialPositionNormalizedToCursor_Test(){
            return GetInitialNormalizedCursoredPosition();
        }
        public int[] thisCursorSize_Test{get{return thisCursorSize;}}
        public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
        public Vector2 thisRectLength_Test{get{return thisRectLength;}}
        public Vector2 thisScrollerElementLength_Test{get{return thisScrollerElementLength;}}
        public float GetUIElementNormalizedCursoredPositionOnAxis_Test(IUIElement element, int dimension){
            return this.GetGroupElementNormalizedCursoredPositionOnAxis(element, dimension);
        }
        public float GetElementGroupOffset_Test(int dimension){
            return this.GetElementGroupOffset(dimension);
        }
        public void SortOutCursoredElements_Test(IUIElement[] currentCursoredElements, IUIElement[] newCursoredElements, out IUIElement[] elementsToDefocus, out IUIElement[] elementsToFocus){
            IUIElement[] thisElementsToDefocus;
            IUIElement[] thisElementsToFocus;
            this.SortOutCursoredGroupElements(currentCursoredElements, newCursoredElements, out thisElementsToDefocus, out thisElementsToFocus);
            elementsToDefocus = thisElementsToDefocus;
            elementsToFocus = thisElementsToFocus;
        }
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
        float startSearchSpeed = 1f;
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
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(0, cursorSize, uiElementLength, padding, startSearchSpeed, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        return scroller;
    }
    public TestUIElementGroupScroller CreateTestUIElementGroupScrollerFull(int initiallyCursoredElementIndex, Vector2 uiElementLocalPos, Vector2 uiElementLength, Vector2 padding, int[] cursorSize, int[] uieGroupMultiplierToCursor){
        ScrollerAxis scrollerAxis = ScrollerAxis.Both;
        float startSearchSpeed = 1f;
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
        IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(initiallyCursoredElementIndex, cursorSize, uiElementLength, padding, startSearchSpeed, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, true, uim, processFactory, uieFactory, uia, uiImage);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        return scroller;
    }
}
