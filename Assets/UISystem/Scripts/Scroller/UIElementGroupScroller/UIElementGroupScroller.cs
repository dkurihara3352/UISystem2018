using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{
		IUIElement[] GetCursoredElements();
		int GetGroupElementIndex(IUIElement groupElement);
		void UpdateGroupElementLengthAndPadding();
	}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller{
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisCursorSize = MakeCursorSizeAtLeastOne(arg.cursorSize);
			thisInitiallyCursoredGroupElementIndex = arg.initiallyCursoredGroupElementIndex;
			thisStartSearchSpeed = MakeSureStartSearchSpeedIsGreaterThanZero(arg.startSearchSpeed);
			thisSwipeToSnapNext = arg.swipeToSnapNext;
			thisActivatesCursoredElementsOnly = arg.activatesCursoredElementsOnly;
		}
		/* Activation */
			readonly bool thisActivatesCursoredElementsOnly;
			public override void ActivateRecursively(bool instantly){
				this.ActivateSelf(instantly);
				thisUIElementGroup.ActivateSelf(instantly);
				if(thisActivatesCursoredElementsOnly)
					ActivateCursoredElements(instantly);
				else
					ActivateAllGroupElements(instantly);
			}
			void ActivateCursoredElements(bool instantly){
				foreach(IUIElement uie in thisCursoredElements)
					if(uie != null)
						uie.ActivateRecursively(instantly);
			}
			void ActivateAllGroupElements(bool instantly){
				foreach(IUIElement uie in thisGroupElements)
					if(uie != null)
						uie.ActivateRecursively(instantly);
			}
		/* SetUp */
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
			protected readonly int[] thisCursorSize;
			protected Vector2 thisPadding;
			protected Vector2 thisGroupElementLength;
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
			float marginOfError = .01f;
			protected override bool[] thisShouldApplyRubberBand{get{return new bool[2]{true, true};}}
			protected override Vector2 CalcCursorLength(){
				float cursorWidth = thisCursorSize[0] * (thisGroupElementLength.x + thisPadding.x) + thisPadding.x;
				float cursorHeight = thisCursorSize[1] * (thisGroupElementLength.y + thisPadding.y) + thisPadding.y;
				Vector2 newCursorLength = new Vector2(cursorWidth, cursorHeight);
				if(newCursorLength[0] <= thisRectLength[0] + marginOfError && newCursorLength[1] <= thisRectLength[1] + marginOfError)
					return newCursorLength;
				else{
					Debug.Log(
						"cursorLengh: " +
						newCursorLength.ToString() +
						", rectLength: " +
						thisRectLength.ToString()
					);
					throw new System.InvalidOperationException("cursorLength cannot exceed this rect length. provide lesser cursor size");
				}
			}
			public override void SetScrollerElementLocalPosOnAxis(float localPosOnAxis, int dimension){
				base.SetScrollerElementLocalPosOnAxis(localPosOnAxis, dimension);
				EvaluateCursoredGroupElements();
			}
			protected override void OnRectsSetUpComplete(){
				base.OnRectsSetUpComplete();
				thisCorrectedCursoredElementIndexCalculator = new CorrectedCursoredElementIndexCalculator(
					thisUIElementGroup, 
					thisCursorSize
				);
				thisElementGroupOffsetCalculator = new ElementGroupOffsetCalculator(
					thisUIElementGroup, 
					thisGroupElementLength, 
					thisPadding, 
					thisCursorLocalPosition
				);
				if(thisSwipeToSnapNext)
					thisSwipeNextTargetGroupElementArrayIndexCalculator = new SwipeNextTargetGroupElementArrayIndexCalculator(
						thisUIElementGroup, 
						thisCursorSize, 
						thisScrollerAxis
					);
			}
			protected override void SetUpScrollerElementRect(){
				base.SetUpScrollerElementRect();
				UpdateGroupElementLengthAndPadding();
			}
			public void UpdateGroupElementLengthAndPadding(){
				thisGroupElementLength = thisUIElementGroup.GetGroupElementLength();
				thisPadding = thisUIElementGroup.GetPadding();
			}
		/* Initially Cursored Element */
			readonly int thisInitiallyCursoredGroupElementIndex;
			protected override Vector2 GetInitialNormalizedCursoredPosition(){
				int correctedInitiallyCursoredGroupElementIndex = GetCursoredGroupElementIndexCorrectedForBounds(thisInitiallyCursoredGroupElementIndex);
				IUIElement initiallyCursoredGroupElement = thisUIElementGroup.GetGroupElement(correctedInitiallyCursoredGroupElementIndex);
				return GetNormalizedCursoredPositionFromGroupElementToCursor(initiallyCursoredGroupElement);
			}
			ICorrectedCursoredElementIndexCalculator thisCorrectedCursoredElementIndexCalculator;
			int GetCursoredGroupElementIndexCorrectedForBounds(int source){
				return thisCorrectedCursoredElementIndexCalculator.Calculate(source);
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
		/* Cursored Elements Evaluation */
			protected override void OnStaticBoundaryCheckFail(int dimension){
				CounterOffsetElementGroup(0f, dimension);
			}
			protected void CounterOffsetElementGroup(float initialVelocity, int dimension){
				if(GetElementGroupOffset(dimension) != 0f){
					IUIElement cursoredElement = thisCursoredElements[0];
					SnapToGroupElement(cursoredElement, initialVelocity, dimension);
					return;
				}else{
					base.OnStaticBoundaryCheckFail(dimension);
				}
			}
			/* Element Offset */
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
			/*  */
			IUIElement[] thisCursoredElements;
			public IUIElement[] GetCursoredElements(){
				return thisCursoredElements;
			}
			protected void SnapToGroupElement(IUIElement groupElement, float initialVelocity, int dimension){
				IUIElement targetGroupElement = GetCorrectedGroupElementCorrectedForBounds(groupElement);
				float targetNormalizedCursoredPosition = GetNormalizedCursoredPositionFromGroupElementToCursor(targetGroupElement, dimension);
				SnapTo(targetNormalizedCursoredPosition, initialVelocity, dimension);
			}
			IUIElement GetCorrectedGroupElementCorrectedForBounds(IUIElement source){
				int sourceIndex = thisUIElementGroup.GetGroupElementIndex(source);
				int correctedIndex = GetCursoredGroupElementIndexCorrectedForBounds(sourceIndex);
				return thisUIElementGroup.GetGroupElement(correctedIndex);
			}
			/*  */
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
							if(uie != null){
								uie.BecomeDefocusedInScrollerRecursively();
								if(thisActivatesCursoredElementsOnly )
									uie.DeactivateRecursively(false);
							}
						foreach(IUIElement uie in groupElementsToFocus)
							if(uie != null){
								uie.EvaluateScrollerFocusRecursively();
								if(thisActivatesCursoredElementsOnly )
									uie.ActivateRecursively(false);
							}
						thisCursoredElements = newCursoredElements;
						thisNoncursoredElements = CalcNoncurosredElements();
					}
				}
			}
			void EvaluateCursoredGroupElementsRaw(){
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

						thisCursoredElements = newCursoredElements;
						thisNoncursoredElements = CalcNoncurosredElements();
					}
				}
			}
			IUIElement[] thisNoncursoredElements;
			IUIElement[] thisGroupElements{
				get{
					return thisUIElementGroup.GetGroupElements().ToArray();
				}
			}
			IUIElement[] CalcNoncurosredElements(){
				List<IUIElement> result = new List<IUIElement>();
				foreach(IUIElement groupElement in thisGroupElements)
					if(groupElement != null){
						bool found = false;
						foreach(IUIElement cursoredElement in thisCursoredElements)
							if(cursoredElement == groupElement){
								found = true;
								break;
							}
						if(!found)
							result.Add(groupElement);
					}
				return result.ToArray();
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
			protected override void OnDynamicBoundaryCheckSuccess(float deltaPosOnAxis, float velocityOnAxis, int dimension){
				SnapToGroupElement(thisCursoredElements[0],　velocityOnAxis, dimension);
			}
			protected override void OnDynamicBoundaryCheckFail(float delatPosOnAxis, float velocityOnAxis, int dimension){
				if(Mathf.Abs(velocityOnAxis) <= thisStartSearchSpeed){
					IUIElement cursoredElement = thisCursoredElements[0];
					SnapToGroupElement(cursoredElement,　velocityOnAxis, dimension);
				}
			}
		/* Swipe override  */
			protected override void ProcessSwipe(ICustomEventData eventData){
				Vector2 swipeDeltaPos = CalcDragDeltaPos(eventData.deltaPos);
				if(thisSwipeToSnapNext){
					SnapNext(swipeDeltaPos, eventData.velocity);
				}else{
					if(thisIsEnabledInertia){
						StartInertialScroll(eventData.velocity);
						CheckAndPerformStaticBoundarySnapFrom(thisProximateParentScroller);
					}
					else
						CheckAndPerformStaticBoundarySnapFrom(this);
				}
			}
			readonly bool thisSwipeToSnapNext;
			protected void SnapNext(Vector2 swipeDeltaPos, Vector2 velocity){
				/*  Find the next groupElement in the direction of swipe delta
					make the element valid if not
				*/
				ResetDrag();

				IUIElement cursoredElement = thisCursoredElements[0];
				int[] cursoredElementArrayIndex = thisUIElementGroup.GetGroupElementArrayIndex(cursoredElement);
				int[] targetGroupElementArrayIndex = GetSwipeNextTargetGroupElementArrayIndex(velocity, cursoredElementArrayIndex);
				
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

				CheckAndPerformStaticBoundarySnapFrom(thisProximateParentScroller);
			}

			ISwipeNextTargetGroupElementArrayIndexCalculator thisSwipeNextTargetGroupElementArrayIndexCalculator;
			protected int[] GetSwipeNextTargetGroupElementArrayIndex(Vector2 velocity, int[] currentGroupElementAtCurRefPointIndex){
				return thisSwipeNextTargetGroupElementArrayIndexCalculator.Calculate(velocity, currentGroupElementAtCurRefPointIndex);
			}
		/* Scroller Focus Evaluation */
			public override void EvaluateScrollerFocusRecursively(){
				BecomeFocusedInScrollerSelf();
				thisUIElementGroup.BecomeFocusedInScrollerSelf();
				EvaluateCursoredGroupElementsRaw();
				foreach(IUIElement cursoredUIE in thisCursoredElements)
					if(cursoredUIE != null)
						cursoredUIE.EvaluateScrollerFocusRecursively();
				foreach(IUIElement noncursoredUIE in thisNoncursoredElements)
					if(noncursoredUIE != null)
						noncursoredUIE.BecomeDefocusedInScrollerRecursively();
			}
			string GetCursoredElementsIndexString(){
				string result = "";
				if(thisCursoredElements == null){
					return "cursored elements null";
				}else{
					foreach(IUIElement element in thisCursoredElements){
						int index = thisUIElementGroup.GetGroupElementIndex(element);
						result += index.ToString() + ", ";
					}
				}
				return result;
			}
			public override void BecomeFocusedInScrollerRecursively(){
				BecomeFocusedInScrollerSelf();
				thisUIElementGroup.BecomeFocusedInScrollerSelf();
				foreach(IUIElement cursoredElement in thisCursoredElements)
					if(cursoredElement != null)
						cursoredElement.BecomeFocusedInScrollerRecursively();
			}
	}


	
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int[] cursorSize{get;}
		int initiallyCursoredGroupElementIndex{get;}
		float startSearchSpeed{get;}
		bool swipeToSnapNext{get;}
		bool activatesCursoredElementsOnly{get;}
	}
	public class UIElementGroupScrollerConstArg: ScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(
			int initiallyCursoredElementIndex, 
			int[] cursorSize, 
			float startSearchSpeed, 
			bool activatesCursoredElementsOnly,

			Vector2 relativeCursorPosition, 
			ScrollerAxis scrollerAxis, 
			Vector2 rubberBandLimitMultiplier, 
			bool isEnabledInertia, 
			bool swipeToSnapNext, 
			float newScrollSpeedThreshold,

			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uieFactory, 
			IUIElementGroupScrollerAdaptor uia, 
			IUIImage image,
			ActivationMode activationMode
		): base(
			scrollerAxis, 
			relativeCursorPosition, 
			rubberBandLimitMultiplier, 
			isEnabledInertia, 
			newScrollSpeedThreshold,

			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			image,
			activationMode
		){
			thisCursorSize = cursorSize;
			thisInitiallyCursoredElementIndex = initiallyCursoredElementIndex;
			thisStartSearchSpeed = startSearchSpeed;
			thisSwipeToSnapNext = swipeToSnapNext;
			thisActivatesCursoredElementsOnly = activatesCursoredElementsOnly;
		}
		readonly int[] thisCursorSize;
		public int[] cursorSize{get{return thisCursorSize;}}
		readonly int thisInitiallyCursoredElementIndex;
		public int initiallyCursoredGroupElementIndex{get{return thisInitiallyCursoredElementIndex;}}
		readonly float thisStartSearchSpeed;
		public float startSearchSpeed{get{return thisStartSearchSpeed;}}
		readonly bool thisSwipeToSnapNext;
		public bool swipeToSnapNext{get{return thisSwipeToSnapNext;}}
		readonly bool thisActivatesCursoredElementsOnly;
		public bool activatesCursoredElementsOnly{get{return thisActivatesCursoredElementsOnly;}}
	}
}
