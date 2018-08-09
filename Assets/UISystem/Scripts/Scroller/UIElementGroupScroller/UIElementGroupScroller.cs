﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller, INonActivatorUIElement{
		IUIElement[] GetCursoredElements();
		int GetGroupElementIndex(IUIElement groupElement);
	}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller{
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
		protected override void OnScrollerElementReferenceSetUp(){
			base.OnScrollerElementReferenceSetUp();
			thisCorrectedCursoredElementIndexCalculator = new CorrectedCursoredElementIndexCalculator(thisUIElementGroup, thisCursorSize);
			thisElementGroupOffsetCalculator = new ElementGroupOffsetCalculator(thisUIElementGroup, thisGroupElementLength, thisPadding, thisCursorLocalPosition);
			thisSwipeNextTargetGroupElementArrayIndexCalculator = new SwipeNextTargetGroupElementArrayIndexCalculator(thisUIElementGroup, thisCursorSize, thisScrollerAxis, thisSwipeToSnapNext);
		}


		ICorrectedCursoredElementIndexCalculator thisCorrectedCursoredElementIndexCalculator;
		int GetCursoredGroupElementIndexCorrectedForBounds(int source){
			return thisCorrectedCursoredElementIndexCalculator.Calculate(source);
		}
		IUIElement GetCorrectedGroupElementCorrectedForBounds(IUIElement source){
			int sourceIndex = thisUIElementGroup.GetGroupElementIndex(source);
			int correctedIndex = GetCursoredGroupElementIndexCorrectedForBounds(sourceIndex);
			return thisUIElementGroup.GetGroupElement(correctedIndex);
		}


		readonly int thisInitiallyCursoredGroupElementIndex;
		protected override Vector2 GetInitialNormalizedCursoredPosition(){
			int correctedInitiallyCursoredGroupElementIndex = GetCursoredGroupElementIndexCorrectedForBounds(thisInitiallyCursoredGroupElementIndex);
			IUIElement initiallyCursoredGroupElement = thisUIElementGroup.GetGroupElement(correctedInitiallyCursoredGroupElementIndex);
			return GetNormalizedCursoredPositionFromGroupElementToCursor(initiallyCursoredGroupElement);
		}
		protected Vector2 GetNormalizedCursoredPositionFromGroupElementToCursor(IUIElement groupElement){
			Vector2 result = new Vector2();
			for(int i = 0 ; i < 2 ; i ++){
				result[i] = GetNormalizedCursoredPositionFromGroupElementToCursor(groupElement, i);
			}
			return result;
		}
		protected float GetNormalizedCursoredPositionFromGroupElementToCursor(IUIElement groupElement, int dimension){
			float groupElementLocalPositionOnAxis = groupElement.GetLocalPosition()[dimension];
			return GetNormalizedCursoredPositionFromPosInElementSpace(groupElementLocalPositionOnAxis - thisPadding[dimension], dimension);
		}
		
		protected readonly int[] thisCursorSize;
		protected readonly Vector2 thisPadding;
		protected readonly Vector2 thisGroupElementLength;
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
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
		public int GetGroupElementIndex(IUIElement groupElement){
			return thisUIElementGroup.GetGroupElementIndex(groupElement);
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
			EvaluateCursoredGroupElements();
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
		protected void CounterOffsetElementGroup(float initialVelocity, int dimension){
			if(GetElementGroupOffset(dimension) != 0f){
				IUIElement cursoredElement = thisCursoredElements[0];
				SnapToGroupElement(cursoredElement, initialVelocity, dimension);
			}
		}
		IUIElement[] thisCursoredElements;
		public IUIElement[] GetCursoredElements(){
			return thisCursoredElements;
		}

		protected void SnapToGroupElement(IUIElement groupElement, float initialVelocity, int dimension){
			IUIElement targetGroupElement = GetCorrectedGroupElementCorrectedForBounds(groupElement);
			float targetNormalizedCursoredPosition = GetNormalizedCursoredPositionFromGroupElementToCursor(targetGroupElement, dimension);
			SnapTo(targetNormalizedCursoredPosition, initialVelocity, dimension);
		}

		IElementGroupOffsetCalculator thisElementGroupOffsetCalculator;
		protected float GetElementGroupOffset(int dimension){
			return thisElementGroupOffsetCalculator.Calculate(dimension);
		}
		protected Vector2 GetGroupElementsOffset(){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++)
				result[i] = GetElementGroupOffset(i);
			return result;
		}
		protected override void DisplaceScrollerElement(Vector2 dragDeltaSinceTouch){
			base.DisplaceScrollerElement(dragDeltaSinceTouch);
			EvaluateCursoredGroupElements();
		}
		protected void EvaluateCursoredGroupElements(){
			IUIElement groupElementUnderCursorRefPoint = GetUIElementUnderCursorReferencePoint();
			if(groupElementUnderCursorRefPoint != null){
				if(
					thisCursoredElements == null || 
					groupElementUnderCursorRefPoint != thisCursoredElements[0]
				){
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
						if(uie != null)
							uie.OnScrollerDefocus();
					foreach(IUIElement uie in groupElementsToFocus)
						if(uie != null)
							uie.OnScrollerFocus();
					thisCursoredElements = newCursoredElements;
				}
			}
		}
		IUIElement GetUIElementUnderCursorReferencePoint(){
			/*  returns the top left => NG
				returns, instread, bottomLeft
			*/
			Vector2 cursorReferencePoint = thisCursorLocalPosition + thisPadding + (thisGroupElementLength * .5f);

			Vector2 cursorRefPInElementGroupSpace = cursorReferencePoint - thisUIElementGroup.GetLocalPosition();
			IUIElement leastCursoredElement = thisUIElementGroup.GetGroupElementAtPositionInGroupSpace(cursorRefPInElementGroupSpace);

			return leastCursoredElement;
		}
		int[,] CalcCursoredGroupElementArrayIndexRange(IUIElement cursoredElement){

			int[] minIndex = thisUIElementGroup.GetGroupElementArrayIndex(cursoredElement);
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
			if(array != null)
				foreach(IUIElement uieInAray in array)
					if(uieInAray == uie)
						return true;
			return false;
		}
		readonly float thisStartSearchSpeed;
		public override bool CheckForDynamicBoundarySnapOnAxis(float deltaPosOnAxis, float velocity, int dimension){
			if(base.CheckForDynamicBoundarySnapOnAxis(deltaPosOnAxis, velocity, dimension)){
				SnapToGroupElement(thisCursoredElements[0],　velocity, dimension);
				return true;
			}else{
				if(Mathf.Abs(velocity) <= thisStartSearchSpeed){
					IUIElement cursoredElement = thisCursoredElements[0];
					SnapToGroupElement(cursoredElement,　velocity, dimension);
					return true;				
				}else{
					return false;
				}
			}
		}
		
		
		/* Swipe override  */
		protected override void OnSwipeImple(ICustomEventData eventData){
			if(thisShouldProcessDrag){
				Vector2 swipeDeltaPos = CalcDragDeltaPos(eventData.deltaPos);
				if(thisSwipeToSnapNext){
					SnapNext(swipeDeltaPos, eventData.velocity);
				}else{
					if(thisIsEnabledInertia)
						StartInertialScroll(eventData.velocity);
				}
				CheckAndPerformStaticBoundarySnapCheckOnParentScrollers();
			}else{
				base.OnSwipeImple(eventData);
				CheckForStaticBoundarySnap();
			}
			ResetDrag();
		}
		readonly bool thisSwipeToSnapNext;
		protected void SnapNext(Vector2 swipeDeltaPos, Vector2 velocity){
			/*  Find the next groupElement in the direction of swipe delta
				make the element valid if not
			*/
			IUIElement cursoredElement = thisCursoredElements[0];
			int[] cursoredElementArrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(cursoredElement);
			int[] targetGroupElementArrayIndex = GetSwipeNextTargetGroupElementArrayIndex(swipeDeltaPos, cursoredElementArrayIndex);
			
			IUIElement targetGroupElement = thisUIElementGroup.GetGroupElement(targetGroupElementArrayIndex[0], targetGroupElementArrayIndex[1]);

			if(targetGroupElement == null)
				targetGroupElement = cursoredElement;
			
			if(thisScrollerAxis == ScrollerAxis.Both){
				SnapToGroupElement(targetGroupElement, velocity[0], 0);
				SnapToGroupElement(targetGroupElement, velocity[1], 1);
			}else{
				if(thisScrollerAxis == ScrollerAxis.Horizontal)
					SnapToGroupElement(targetGroupElement, velocity[0], 0);
				else
					SnapToGroupElement(targetGroupElement, velocity[1], 1);
			}
		}
		int GetDominantAxis(Vector2 delta){
			if(Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
				return 0;
			else
				return 1;
		}
		ISwipeNextTargetGroupElementArrayIndexCalculator thisSwipeNextTargetGroupElementArrayIndexCalculator;
		protected int[] GetSwipeNextTargetGroupElementArrayIndex(Vector2 swipeDeltaPos, int[] currentGroupElementAtCurRefPointIndex){
			// return thisSwipeNextTargetGroupElementArrayIndexCalculator.Calculate(swipeDeltaPos, currentGroupElementAtCurRefPointIndex);
			int[] result = new int[2];

			int dominantAxis = -1;
			if(thisScrollerAxis == ScrollerAxis.Both && thisSwipeToSnapNext){
				dominantAxis = GetDominantAxis(swipeDeltaPos);
			}

			for(int i = 0; i < 2; i ++){
				if(dominantAxis == -1 || dominantAxis == i){
					if(swipeDeltaPos[i] != 0f){
						if(swipeDeltaPos[i] < 0f)
							result[i] = currentGroupElementAtCurRefPointIndex[i] + 1;
						else
							result[i] = currentGroupElementAtCurRefPointIndex[i] - 1;
					}else
						result[i] = currentGroupElementAtCurRefPointIndex[i];
				}else{
					result[i] = currentGroupElementAtCurRefPointIndex[i];
				}
				result[i] = MakeTargetGroupElementArrayIndexWithinRange(result[i], i);
			}
			return result;
		}

		int MakeTargetGroupElementArrayIndexWithinRange(int source, int dimension){
			if(source < 0)
				return 0;
			else{
				int allowedMaxArrayIndex = thisUIElementGroup.GetArraySize()[dimension] - thisCursorSize[dimension];
				if(source > allowedMaxArrayIndex)
					return allowedMaxArrayIndex;
			}
			return source;
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
		public UIElementGroupScrollerConstArg(int initiallyCursoredElementIndex, int[] cursorSize, Vector2 uiElementLength, Vector2 padding, float startSearchSpeed, Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, bool swipeToSnapNext, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(scrollerAxis, relativeCursorPosition, rubberBandLimitMultiplier, isEnabledInertia, uim, processFactory, uieFactory, uia, image){
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
