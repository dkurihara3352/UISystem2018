﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{
		IUIElement[] GetCursoredElements();
		int GetGroupElementIndex(IUIElement groupElement);
	}
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
		int GetCorrectedInitiallyCursoredGroupElementIndex(int source){
			IUIElement sourceElement = thisUIElementGroup.GetGroupElement(source);

			int columnCount = thisUIElementGroup.GetGroupElementsArraySize(0);
			int rowCount = thisUIElementGroup.GetGroupElementsArraySize(1);
			int[] arraySize = new int[]{columnCount, rowCount};
			int[] arrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(sourceElement);

			int[] targetArrayIndex = new int[2];
			for(int i = 0; i < 2; i ++){
				targetArrayIndex[i] = arrayIndex[i];
				if(arrayIndex[i] > arraySize[i] - thisCursorSize[i])
					targetArrayIndex[i] = arraySize[i] - thisCursorSize[i];
			}

			IUIElement targetInitElement = thisUIElementGroup.GetGroupElement(targetArrayIndex[0], targetArrayIndex[1]);
			int targetIndex = thisUIElementGroup.GetGroupElements().IndexOf(targetInitElement);
			Debug.Log(
					"source index: " + source.ToString() +
					", columnCount: " + columnCount.ToString() +
					", rowCount: " + rowCount.ToString() +
					", sourceColumnIndex: " + arrayIndex[0].ToString() + 
					", sourceRowIndex: " + arrayIndex[1].ToString() + 
					", targetColumnIndex: " + targetArrayIndex[0].ToString() + 
					", targetRowIndex: " + targetArrayIndex[1].ToString() +
					", targetIndex: " + targetIndex.ToString()
			);
			return targetIndex;
		}
		readonly int thisInitiallyCursoredGroupElementIndex;
		protected override Vector2 GetInitialNormalizedCursoredPosition(){
			IUIElementGroup uieGroup = thisUIElementGroup;
			int correctedInitiallyCursoredGroupElementIndex = GetCorrectedInitiallyCursoredGroupElementIndex(thisInitiallyCursoredGroupElementIndex);
			IUIElement initiallyCursoredGroupElement = uieGroup.GetGroupElement(correctedInitiallyCursoredGroupElementIndex);
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
				IUIElement bottomLeft = thisCursoredElements[0];
				// IUIElement bottomLeft = GetUIElementUnderCursorReferencePoint();
				// IUIElement bottomLeft = GetBottomLeftGroupElement(topLeft);
				SnapToGroupElement(bottomLeft, initialDeltaPosOnAxis, dimension);
			}
		}
		// IUIElement GetBottomLeftGroupElement(IUIElement topLeft){
		// 	int[] arrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(topLeft);
		// 	int targetColumnIndex = arrayIndex[0];
		// 	int targetRowIndex = arrayIndex[1] - thisCursorSize[1] + 1;

		// 	return thisUIElementGroup.GetGroupElement(targetColumnIndex, targetRowIndex);
		// }
		IUIElement[] thisCursoredElements;
		public IUIElement[] GetCursoredElements(){
			return thisCursoredElements;
		}
		public int GetGroupElementIndex(IUIElement groupElement){
			List<IUIElement> groupElements = thisUIElementGroup.GetGroupElements();
			return groupElements.IndexOf(groupElement);
		}
		protected void SnapToGroupElement(IUIElement groupElement, float initialDeltaPosOnAxis, int dimension){
			/*  need to correct for OB?
			*/
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
		protected override void DisplaceScrollerElement(Vector2 dragDeltaSinceTouch){
			base.DisplaceScrollerElement(dragDeltaSinceTouch);
			EvaluateCursoredGroupElements();
		}
		protected void EvaluateCursoredGroupElements(){
			IUIElement groupElementUnderCursorRefPoint = GetUIElementUnderCursorReferencePoint();
			if(groupElementUnderCursorRefPoint != null){
				if(
					thisCursoredElements == null || 
					// thisPrevElementUnderCursorRefPoint == null ||
					groupElementUnderCursorRefPoint != thisCursoredElements[0]/* thisPrevElementUnderCursorRefPoint */
				){
					int[,] indexRange = CalcCursoredGroupElementArrayIndexRange(groupElementUnderCursorRefPoint);
					int minColumnIndex = indexRange[0, 0];
					int minRowIndex = indexRange[0, 1];
					int maxColumnIndex = indexRange[1, 0];
					int maxRowIndex = indexRange[1, 1];
					
					// int rowCount = thisUIElementGroup.GetGroupElementsArraySize(1);
					// minRowIndex = rowCount - 1 - minRowIndex;
					// maxRowIndex = minRowIndex + thisCursorSize[1] - 1;

					IUIElement[] newCursoredElements = thisUIElementGroup.GetGroupElementsWithinIndexRange(minColumnIndex, minRowIndex, maxColumnIndex, maxRowIndex);
					IUIElement[] currentCursoredElements = thisCursoredElements;
					IUIElement[] groupElementsToDefocus;
					IUIElement[] groupElementsToFocus;
					SortOutCursoredGroupElements(currentCursoredElements, newCursoredElements, out groupElementsToDefocus, out groupElementsToFocus);

					foreach(IUIElement uie in groupElementsToDefocus)
						uie.OnScrollerDefocus();
					foreach(IUIElement uie in groupElementsToFocus)
						uie.OnScrollerFocus();
					thisCursoredElements = newCursoredElements;
					// thisPrevElementUnderCursorRefPoint = groupElementUnderCursorRefPoint;
				}
			}
		}
		// IUIElement thisPrevElementUnderCursorRefPoint;
		IUIElement GetUIElementUnderCursorReferencePoint(){
			/*  returns the top left => NG
				returns, instread, bottomLeft
			*/
			// float cursorRefPointX = thisCursorLocalPosition.x + thisPadding.x + (thisGroupElementLength.x * .5f);
			// float halfElementHeightPlusPaddingY = (thisGroupElementLength.y * .5f) + thisPadding.y;
			// float cursorRefPointY = thisCursorLocalPosition.y + thisCursorLength.y - halfElementHeightPlusPaddingY;
			// Vector2 cursorReferencePoint = new Vector2(cursorRefPointX, cursorRefPointY);
			Vector2 cursorReferencePoint = thisCursorLocalPosition + thisPadding + (thisGroupElementLength * .5f);

			Vector2 cursorRefPInElementGroupSpace = cursorReferencePoint - thisUIElementGroup.GetLocalPosition();
			IUIElement leastCursoredElement = thisUIElementGroup.GetGroupElementAtPositionInGroupSpace(cursorRefPInElementGroupSpace);

			return leastCursoredElement;
		}
		int[,] CalcCursoredGroupElementArrayIndexRange(IUIElement leastCursoredElement){
			/*  converted for use in elementGroup
			*/
			// int[] arrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(leastCursoredElement);
			// int[] minIndex = new int[2];
			// int[] maxIndex = new int[2];
			// minIndex[0] = arrayIndex[0];
			// minIndex[1] = arrayIndex[1] - thisCursorSize[1] + 1;
			// maxIndex[0] = minIndex[0] + thisCursorSize[0] -1;
			// maxIndex[1] = arrayIndex[1];
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
			if(array != null)
				foreach(IUIElement uieInAray in array)
					if(uieInAray == uie)
						return true;
			return false;
		}
		readonly float thisStartSearchSpeed;
		public override bool CheckForDynamicBoundarySnapOnAxis(float deltaPosOnAxis, float velocity, int dimension){
			if(!base.CheckForDynamicBoundarySnapOnAxis(deltaPosOnAxis, velocity, dimension)){
				if(Mathf.Abs(velocity) <= thisStartSearchSpeed){
					IUIElement cursoredElement = /* GetUIElementUnderCursorReferencePoint() */thisCursoredElements[0];
					// IUIElement bottomLeft = GetBottomLeftGroupElement(topLeft);
					SnapToGroupElement(cursoredElement, deltaPosOnAxis, dimension);
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
					SnapNext(swipeDeltaPos, eventData.velocity);
				}else{
					if(thisIsEnabledInertia)
						StartInertialScroll(eventData.velocity);
				}				
			}else
				base.OnSwipeImple(eventData);
			
			ResetDrag();
		}
		readonly bool thisSwipeToSnapNext;
		protected void SnapNext(Vector2 swipeDeltaPos, Vector2 velocity){
			/*  Find the next groupElement in the direction of swipe delta
				if not found, start inertial scroll instead
			*/
			IUIElement bottomLeft = thisCursoredElements[0]/* GetUIElementUnderCursorReferencePoint() */;
			// IUIElement bottomLeft = GetBottomLeftGroupElement(groupElementUnderCursorRefPoint);
			// int[] groupElementIndex = thisUIElementGroup.GetGroupElementArrayIndex(groupElementUnderCursorRefPoint);
			int[] bottomLeftArrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(bottomLeft);
			// int[] targetGroupElementIndex = GetSwipeNextTargetGroupElementArrayIndex(swipeDeltaPos, groupElementIndex);
			int[] targetGroupElementIndex = GetSwipeNextTargetGroupElementArrayIndex(swipeDeltaPos, bottomLeftArrayIndex);
			if(SwipeTargetGroupElementArrayIndexAreValid(targetGroupElementIndex)){
				IUIElement targetGroupElement = thisUIElementGroup.GetGroupElement(targetGroupElementIndex[0], targetGroupElementIndex[1]);
				SnapToGroupElement(targetGroupElement, swipeDeltaPos[0], 0);
				SnapToGroupElement(targetGroupElement, swipeDeltaPos[1], 1);
			}else{
				if(thisIsEnabledInertia)
					StartInertialScroll(velocity);
			}
		}
		protected int[] GetSwipeNextTargetGroupElementArrayIndex(Vector2 swipeDeltaPos, int[] currentGroupElementAtCurRefPointIndex){
			int[] result = new int[2];
			for(int i = 0; i < 2; i ++){
				if(swipeDeltaPos[i] != 0f){
					if(swipeDeltaPos[i] < 0f)
						result[i] = currentGroupElementAtCurRefPointIndex[i] + 1;
					else
						result[i] = currentGroupElementAtCurRefPointIndex[i] - 1;
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
					if(index[i] > thisUIElementGroup.GetGroupElementsArraySize(i) - thisCursorSize[i])
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
