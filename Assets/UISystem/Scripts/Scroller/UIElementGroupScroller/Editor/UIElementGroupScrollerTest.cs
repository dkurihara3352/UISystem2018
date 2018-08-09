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

        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();

        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        float uiElementGroupLengthX = (cursorSize[0] * 2) * (uiElementLength[0] + padding[0]) + padding[0];
        float uiElementGroupLengthY = (cursorSize[1] * 2) * (uiElementLength[1] + padding[1]) + padding[1];
        Rect uiElementGroupRect = new Rect(Vector2.zero, new Vector2(uiElementGroupLengthX, uiElementGroupLengthY));
        Rect scrollerRect = new Rect(Vector2.zero, uiElementGroupRect.size * 1.2f);
        arg.relativeCursorPosition.Returns(relativeCursorPosition);
        arg.uia.GetRect().Returns(scrollerRect);
        arg.groupElementLength.Returns(uiElementLength);
        arg.padding.Returns(padding);
        arg.cursorSize.Returns(cursorSize);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        scroller.ActivateImple();

        Vector2 actual = scroller.thisCursorLength_Test;

        Assert.That(actual, Is.EqualTo(expected));
    }
    [Test, TestCaseSource(typeof(ThisCursorLength_ReturnsCalculatedValue_TestCase), "cases")]
    public void ThisCursorLength_CursorLengthOversizedToScrollerLength_ThrowsException(
        int[] cursorSize, 
        Vector2 uiElementLength, 
        Vector2 padding, 
        Vector2 expected
    ){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg();
        Vector2 relativeCursorPosition = new Vector2(.5f, .5f);
        float uiElementGroupLengthX = (cursorSize[0] * 2) * (uiElementLength[0] + padding[0]) + padding[0];
        float uiElementGroupLengthY = (cursorSize[1] * 2) * (uiElementLength[1] + padding[1]) + padding[1];
        Rect uiElementGroupRect = new Rect(Vector2.zero, new Vector2(uiElementGroupLengthX, uiElementGroupLengthY));
        Rect scrollerRect = new Rect(Vector2.zero, new Vector2(119f, 69f));
        arg.groupElementLength.Returns(uiElementLength);
        arg.padding.Returns(padding);
        arg.cursorSize.Returns(cursorSize);
        arg.relativeCursorPosition.Returns(relativeCursorPosition);
        arg.uia.GetRect().Returns(scrollerRect);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        
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
        uieGroup.GetGroupElementArrayIndex(Arg.Any<IUIElement>()).Returns(new int[]{0, 0});
        List<IUIElement> children = new List<IUIElement>(new IUIElement[]{uieGroup});
        arg.uia.GetChildUIEs().Returns(children);
        IUIElement element = Substitute.For<IUIElement>();
        arg.cursorSize.Returns(new int[]{1, 1});
        arg.relativeCursorPosition.Returns(Vector2.zero);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        element.GetLocalPosition().Returns(elementLocalPos);
        scroller.ActivateImple();

        for(int i = 0; i < 2; i ++){
            float actual = scroller.GetNormalizedCursoredPositionFromGroupElementToCursor_Test(element, i);
            
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
        uieGroup.GetGroupElementArrayIndex(Arg.Any<IUIElement>()).Returns(new int[]{0, 0});
        List<IUIElement> children = new List<IUIElement>(new IUIElement[]{uieGroup});
        arg.uia.GetChildUIEs().Returns(children);
        arg.cursorSize.Returns(new int[]{1, 1});
        arg.relativeCursorPosition.Returns(Vector2.zero);
        TestUIElementGroupScroller scroller = new TestUIElementGroupScroller(arg);
        IUIElement mock = Substitute.For<IUIElement>();
        uieGroup.GetGroupElement(Arg.Any<int>(), Arg.Any<int>()).Returns(mock);
        uieGroup.GetGroupElementIndex(mock).Returns(arg.initiallyCursoredGroupElementIndex);
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







    public IUIElementGroupScrollerConstArg CreateMockConstArg(
        int initiallyCursoredElementIndex,
        int[] cursorSize,
        Vector2 groupElementLength,
        Vector2 padding,
        Vector2 relativeCursorPosition,
        ScrollerAxis scrollerAxis,
        Vector2 rubberBandLimitMultiplier,
        Vector2 scrollerLength,
        Vector2 elementGroupLength
    ){
        IUIElementGroupScrollerConstArg arg = Substitute.For<IUIElementGroupScrollerConstArg>();
        arg.initiallyCursoredGroupElementIndex.Returns(initiallyCursoredElementIndex);
        arg.cursorSize.Returns(cursorSize);
        arg.groupElementLength.Returns(groupElementLength);
        arg.padding.Returns(padding);
        arg.startSearchSpeed.Returns(1f);
        arg.relativeCursorPosition.Returns(relativeCursorPosition);
        arg.scrollerAxis.Returns(scrollerAxis);
        arg.rubberBandLimitMultiplier.Returns(rubberBandLimitMultiplier);
        arg.isEnabledInertia.Returns(true);
        arg.swipeToSnapNext.Returns(false);
        arg.uim.Returns(Substitute.For<IUIManager>());
        arg.processFactory.Returns(Substitute.For<IUISystemProcessFactory>());
        arg.uiElementFactory.Returns(Substitute.For<IUIElementFactory>());
        IScrollerAdaptor scrollerAdaptor = Substitute.For<IScrollerAdaptor>();
            scrollerAdaptor.GetRect().Returns(new Rect(Vector2.zero, scrollerLength));
            IUIElementGroup uieGroup = Substitute.For<IUIElementGroup>();
                uieGroup.GetGroupElementArrayIndex(Arg.Any<IUIElement>()).Returns(new int[]{0, 0});
            IUIElementGroupAdaptor uieGroupAdaptor = Substitute.For<IUIElementGroupAdaptor>();
            uieGroup.GetUIAdaptor().Returns(uieGroupAdaptor);
            uieGroupAdaptor.GetRect().Returns(new Rect(Vector2.zero, elementGroupLength));
            scrollerAdaptor.GetChildUIEs().Returns(new List<IUIElement>(new IUIElement[]{uieGroup}));
        arg.uia.Returns(scrollerAdaptor);
        arg.image.Returns(Substitute.For<IUIImage>());
        return arg;
    }
    public IUIElementGroupScrollerConstArg CreateMockConstArg(){
        IUIElementGroupScrollerConstArg arg = CreateMockConstArg(
            initiallyCursoredElementIndex: 0,
            cursorSize: new int[]{1, 1},
            groupElementLength: new Vector2(50f, 50f),
            padding: new Vector2(10f, 10f),
            relativeCursorPosition: new Vector2(0f, 0f),
            scrollerAxis: ScrollerAxis.Both,
            rubberBandLimitMultiplier: new Vector2(.1f, .1f),
            scrollerLength: new Vector2(200f, 100f),
            elementGroupLength: new Vector2(130f, 130f)
        );
        return arg;
    }
    public class TestUIElementGroupScroller: UIElementGroupScroller{
        public TestUIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){}
        public Vector2 GetInitialNormalizedCursoredPosition_Test(){
            return GetInitialNormalizedCursoredPosition();
        }
        public int[] thisCursorSize_Test{get{return thisCursorSize;}}
        public Vector2 thisCursorLength_Test{get{return thisCursorLength;}}
        public Vector2 thisRectLength_Test{get{return thisRectLength;}}
        public Vector2 thisScrollerElementLength_Test{get{return thisScrollerElementLength;}}
        public float GetNormalizedCursoredPositionFromGroupElementToCursor_Test(IUIElement element, int dimension){
            return this.GetNormalizedCursoredPositionFromGroupElementToCursor(element, dimension);
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
        public int[] GetSwipeNextTargetGroupElementArrayIndex_Test(Vector2 swipeDeltaPos, int[] currentGroupElementAtCurRefPointIndex){
            return this.GetSwipeNextTargetGroupElementArrayIndex(swipeDeltaPos, currentGroupElementAtCurRefPointIndex);
        }
    }
}
