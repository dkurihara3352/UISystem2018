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
        arg.uia.GetRect().Returns(new Rect(Vector2.zero, new Vector2(130f, 130f)));
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
    public class GetUIElementNormalizedCursoredPositionOnAxis_TestCase{
        public static object[] cases = {
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(10f, 10f), new Vector2(0f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(120f, 10f), new Vector2(1f, 0f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(10f, 70f), new Vector2(0f, 1f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(120f, 70f), new Vector2(1f, 1f)},
            new object[]{new Vector2(100f, 50f), new Vector2(10f, 10f), new int[]{2, 2}, new Vector2(65f, 40f), new Vector2(.5f, .5f)},
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
        Vector2 relativeCursorPosition,
        ScrollerAxis scrollerAxis,
        Vector2 rubberBandLimitMultiplier,
        Vector2 scrollerLength,
        Vector2 elementGroupLength
    ){
        IUIElementGroupScrollerConstArg arg = Substitute.For<IUIElementGroupScrollerConstArg>();
        arg.initiallyCursoredGroupElementIndex.Returns(initiallyCursoredElementIndex);
        arg.cursorSize.Returns(cursorSize);
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
        protected override IScroller FindProximateParentScroller(){
            IScroller parentScroller = Substitute.For<IScroller>();
            IScroller nullParentScroller = null;
            parentScroller.GetProximateParentScroller().Returns(nullParentScroller);
            return parentScroller;
        }
    }
}
