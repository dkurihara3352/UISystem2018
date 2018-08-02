using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller, INonActivatorUIElement{
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisCursorSize = MakeCursorSizeAtLeastOne(arg.cursorSize);
			thisGroupElementLength = arg.groupElementLength;
			thisPadding = arg.padding;
			thisInitiallyCursoredGroupElementIndex = arg.initiallyCursoredGroupElementIndex;
			thisStartSearchSpeed = MakeSureStartSearchSpeedIsGreaterThanZero(arg.startSearchSpeed);
			thisSwipeToSnapNext = arg.swipeToSnapNext;
		}
		int[] MakeCursorSizeAtLeastOne(int[] source){
			int[] result = new int[2];
			for(int i = 0; i < 2; i ++){
				if(source[i] <= 0)
					result[i] = 1;
				else
					result[i] = source[i];
			}
			return result;
		}
		float MakeSureStartSearchSpeedIsGreaterThanZero(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("startSearchSpeed must be greater than zero");
			return source;
		}
		readonly int thisInitiallyCursoredGroupElementIndex;
		protected override Vector2 GetInitialNormalizedCursoredPosition(){
			IUIElementGroup uieGroup = thisUIElementGroup;
			IUIElement initiallyCursoredGroupElement = uieGroup.GetGroupElement(thisInitiallyCursoredGroupElementIndex);
			Vector2 groupElementLocalPos = initiallyCursoredGroupElement.GetLocalPosition();
			float resultX = GetNormalizedCursoredPositionFromPosInElementSpace(groupElementLocalPos.x - thisPadding[0], 0);
			float resultY = GetNormalizedCursoredPositionFromPosInElementSpace(groupElementLocalPos.y - thisPadding[1], 1);
			return new Vector2(resultX, resultY);
		}
		protected readonly int[] thisCursorSize;
		protected readonly Vector2 thisPadding;
		protected readonly Vector2 thisGroupElementLength;
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		protected override void InitializeSelectabilityState(){
			BecomeSelectable();
		}
		/*  */
		protected IUIElementGroup thisUIElementGroup{
			get{
				return (IUIElementGroup)thisScrollerElement;
			}
		}
		int thisGroupElementsCount{
			get{
				return thisUIElementGroup.GetSize();
			}
		}
		protected override bool[] thisShouldApplyRubberBand{get{return new bool[2]{true, true};}}
		protected override Vector2 CalcCursorLength(){
			float cursorWidth = thisCursorSize[0] * (thisGroupElementLength.x + thisPadding.x) + thisPadding.x;
			float cursorHeight = thisCursorSize[1] * (thisGroupElementLength.y + thisPadding.y) + thisPadding.y;
			Vector2 newCursorLength = new Vector2(cursorWidth, cursorHeight);
			if(newCursorLength[0] <= thisRectLength[0] && newCursorLength[1] <= thisRectLength[1])
				return newCursorLength;
			else
				throw new System.InvalidOperationException("cursorLength cannot exceed this rect length. provide lesser cursor size");
		}
		public override void SetScrollerElementLocalPosOnAxis(float localPosOnAxis, int dimension){
			base.SetScrollerElementLocalPosOnAxis(localPosOnAxis, dimension);
			if(thisScrollerAxis == ScrollerAxis.Horizontal){
				if(dimension == 0)
					EvaluateCursoredGroupElements();
			}else{// veritcal or both
				if(dimension == 1)
					EvaluateCursoredGroupElements();
			}				
		}



		/* Cursored Elements Evaluation */
		protected override void InitializeScrollerElementForActivation(){
			base.InitializeScrollerElementForActivation();
			EvaluateCursoredGroupElements();
		}
		protected override bool CheckForStaticBoundarySnapOnAxis(int dimension){
			if(!base.CheckForStaticBoundarySnapOnAxis(dimension))
				CounterOffsetElementGroup(0f, dimension);
			return true;
		}
		protected void CounterOffsetElementGroup(float initialDeltaPosOnAxis, int dimension){
			if(GetElementGroupOffset(dimension) != 0f){
				IUIElement leastCursoredElement = thisCursoredElements[0];
				SnapToGroupElement(leastCursoredElement, initialDeltaPosOnAxis, dimension);
			}
		}
		IUIElement[] thisCursoredElements;
		protected void SnapToGroupElement(IUIElement groupElement, float initialDeltaPosOnAxis, int dimension){
			float groupElementNormalizedCursoredPosition = GetGroupElementNormalizedCursoredPositionOnAxis(groupElement, dimension);
			SnapTo(groupElementNormalizedCursoredPosition, initialDeltaPosOnAxis, dimension);
		}
		protected float GetGroupElementNormalizedCursoredPositionOnAxis(IUIElement groupElement, int dimension){
			Vector2 groupElementLocalPosMinusPadding = groupElement.GetLocalPosition() - thisPadding;
			return GetNormalizedCursoredPositionFromPosInElementSpace(groupElementLocalPosMinusPadding[dimension], dimension);
		}
		protected float GetElementGroupOffset(int dimension){
			float sectionLength = thisGroupElementLength[dimension] + thisPadding[dimension];
			Vector2 uieGroupCursoredPosition = thisCursorLocalPosition - thisUIElementGroup.GetLocalPosition();
			float modulo = uieGroupCursoredPosition[dimension] % sectionLength;
			if(modulo == 0f)
				return 0f;
			else{
				float cursoredPosNormalziedToSectionLength = modulo/ sectionLength;
				if(cursoredPosNormalziedToSectionLength < 0f)
					cursoredPosNormalziedToSectionLength = 1f + cursoredPosNormalziedToSectionLength;
				return cursoredPosNormalziedToSectionLength;
			}
		}
		protected Vector2 GetGroupElementsOffset(){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++)
				result[i] = GetElementGroupOffset(i);
			return result;
		}
		protected override void DisplaceScrollerElement(Vector2 deltaP){
			base.DisplaceScrollerElement(deltaP);
			EvaluateCursoredGroupElements();
		}
		protected void EvaluateCursoredGroupElements(){
			IUIElement groupElementUnderCursorRefPoint = GetUIElementUnderCursorReferencePoint();
			if(groupElementUnderCursorRefPoint != null){
				if(thisCursoredElements == null || groupElementUnderCursorRefPoint != thisCursoredElements[0]){
					int[,] indexRange = CalcCursoredGroupElementArrayIndexRange(groupElementUnderCursorRefPoint);
					int minColumnIndex = indexRange[0, 0];
					int minRowIndex = indexRange[0, 1];
					int maxColumnIndex = indexRange[1, 0];
					int maxRowIndex = indexRange[1, 1];
					IUIElement[] newCursoredElements = thisUIElementGroup.GetGroupElementsWithinIndexRange(minColumnIndex, minRowIndex, maxColumnIndex, maxRowIndex);
					IUIElement[] currentCursoredElements = thisCursoredElements;
					IUIElement[] groupElementsToDefocus;
					IUIElement[] groupElementsToFocus;
					SortOutCursoredGroupElements(currentCursoredElements, newCursoredElements, out groupElementsToDefocus, out groupElementsToFocus);

					foreach(IUIElement uie in groupElementsToDefocus)
						uie.OnScrollerDefocus();
					foreach(IUIElement uie in groupElementsToFocus)
						uie.OnScrollerFocus();
					thisCursoredElements = currentCursoredElements;
				}
			}
		}
		IUIElement GetUIElementUnderCursorReferencePoint(){
			Vector2 cursorReferencePoint = thisCursorLocalPosition + thisPadding + (thisGroupElementLength * .5f);
			Vector2 cursorRefPInElementGroupSpace = cursorReferencePoint - thisUIElementGroup.GetLocalPosition();
			IUIElement leastCursoredElement = thisUIElementGroup.GetGroupElementAtPositionInGroupSpace(cursorRefPInElementGroupSpace);

			return leastCursoredElement;
		}
		int[,] CalcCursoredGroupElementArrayIndexRange(IUIElement leastCursoredElement){
			int[] minIndex = thisUIElementGroup.GetGroupElementArrayIndex(leastCursoredElement);
			int thisMaxColumnIndex = minIndex[0] + thisCursorSize[0] - 1;
			int thisMaxRowIndex = minIndex[1] + thisCursorSize[1] - 1;
			int[] maxIndex = new int[]{thisMaxColumnIndex, thisMaxRowIndex};
			
			return new int[,]{
				{minIndex[0], minIndex[1]}, {maxIndex[0], maxIndex[1]}
			};
		}
		protected void SortOutCursoredGroupElements(IUIElement[] currentCursoredElements, IUIElement[] newCursoredElements, out IUIElement[] groupElementsToDefocus, out IUIElement[] groupElementsToFocus){
			List<IUIElement> groupElementsToDefocusResult = new List<IUIElement>();
			List<IUIElement> groupElementsToFocusResult = new List<IUIElement>();
			if(currentCursoredElements != null){
				foreach(IUIElement curCursoredUIE in currentCursoredElements){
					bool foundInNewList = IsFoundIn(curCursoredUIE, newCursoredElements);
					if(!foundInNewList)
						groupElementsToDefocusResult.Add(curCursoredUIE);
				}
			}
			foreach(IUIElement newCursoredUIE in newCursoredElements){
				bool foundInCurrent = IsFoundIn(newCursoredUIE, currentCursoredElements);
				if(!foundInCurrent)
					groupElementsToFocusResult.Add(newCursoredUIE);
			}
			groupElementsToDefocus = groupElementsToDefocusResult.ToArray();
			groupElementsToFocus = groupElementsToFocusResult.ToArray();
		}
		bool IsFoundIn(IUIElement uie, IUIElement[] array){
			foreach(IUIElement uieInAray in array)
				if(uieInAray == uie)
					return true;
			return false;
		}
		readonly float thisStartSearchSpeed;
		public override bool CheckForDynamicBoundarySnap(float deltaPosOnAxis, int dimension){
			if(!base.CheckForDynamicBoundarySnap(deltaPosOnAxis, dimension)){
				if(Mathf.Abs(deltaPosOnAxis) <= thisStartSearchSpeed){
					IUIElement groupElementAtCusorRefPoint = GetUIElementUnderCursorReferencePoint();
					SnapToGroupElement(groupElementAtCusorRefPoint, deltaPosOnAxis, dimension);
					return true;				
				}else
					return false;
			}else
				return true;
		}
		
		
		/* Swipe override  */
		protected override void OnSwipeImple(ICustomEventData eventData){
			if(thisShouldProcessDrag){
				Vector2 swipeDeltaPos = CalcDragDeltaPos(eventData.deltaPos);
				if(thisSwipeToSnapNext){
					SnapNext(swipeDeltaPos);
				}else{
					if(thisIsEnabledInertia)
						StartInertialScroll(swipeDeltaPos);
				}				
			}else
				base.OnSwipeImple(eventData);
			
			ResetDrag();
		}
		readonly bool thisSwipeToSnapNext;
		protected void SnapNext(Vector2 swipeDeltaPos){
			/*  Find the next groupElement in the direction of swipe delta
				if not found, start inertial scroll instead
			*/
			IUIElement groupElementUnderCursorRefPoint = thisCursoredElements[0];
			int[] groupElementIndex = thisUIElementGroup.GetGroupElementArrayIndex(groupElementUnderCursorRefPoint);
			int[] targetGroupElementIndex = GetSwipeNextTargetGroupElementArrayIndex(swipeDeltaPos, groupElementIndex);
			if(SwipeTargetGroupElementArrayIndexAreValid(targetGroupElementIndex)){
				IUIElement targetGroupElement = thisUIElementGroup.GetGroupElement(targetGroupElementIndex[0], targetGroupElementIndex[1]);
				SnapToGroupElement(targetGroupElement, swipeDeltaPos[0], 0);
				SnapToGroupElement(targetGroupElement, swipeDeltaPos[1], 1);
			}else{
				if(thisIsEnabledInertia)
					StartInertialScroll(swipeDeltaPos);
			}
		}
		protected int[] GetSwipeNextTargetGroupElementArrayIndex(Vector2 swipeDeltaPos, int[] currentGroupElementAtCurRefPointIndex){
			int[] result = new int[2];
			for(int i = 0; i < 2; i ++){
				if(swipeDeltaPos[i] != 0f){
					if(swipeDeltaPos[i] < 0f)
						result[i] = currentGroupElementAtCurRefPointIndex[i] - 1;
					else
						result[i] = currentGroupElementAtCurRefPointIndex[i] + 1;
				}else
					result[i] = currentGroupElementAtCurRefPointIndex[i];
			}
			return result;
		}
		protected virtual bool SwipeTargetGroupElementArrayIndexAreValid(int[] index){
			for(int i = 0; i < 2; i ++){
				if(index[i] < 0)
					return false;
				else
					if(index[i] >= thisUIElementGroup.GetGroupElementsArraySize(i))
						return false;
			}
			return true;
		}
	}


	
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int[] cursorSize{get;}
		Vector2 groupElementLength{get;}
		Vector2 padding{get;}
		int initiallyCursoredGroupElementIndex{get;}
		float startSearchSpeed{get;}
		bool swipeToSnapNext{get;}
	}
	public class UIElementGroupScrollerConstArg: ScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(int initiallyCursoredElementIndex, int[] cursorSize, Vector2 uiElementLength, Vector2 padding, float startSearchSpeed, Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, bool swipeToSnapNext, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, isEnabledInertia, uim, processFactory, uieFactory, uia, image){
			thisCursorSize = cursorSize;
			thisElementDimension = uiElementLength;
			thisPadding = padding;
			thisInitiallyCursoredElementIndex = initiallyCursoredElementIndex;
			thisStartSearchSpeed = startSearchSpeed;
			thisSwipeToSnapNext = swipeToSnapNext;
		}
		readonly int[] thisCursorSize;
		public int[] cursorSize{get{return thisCursorSize;}}
		readonly Vector2 thisElementDimension;
		public Vector2 groupElementLength{get{return thisElementDimension;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
		readonly int thisInitiallyCursoredElementIndex;
		public int initiallyCursoredGroupElementIndex{get{return thisInitiallyCursoredElementIndex;}}
		readonly float thisStartSearchSpeed;
		public float startSearchSpeed{get{return thisStartSearchSpeed;}}
		readonly bool thisSwipeToSnapNext;
		public bool swipeToSnapNext{get{return thisSwipeToSnapNext;}}
	}
}
