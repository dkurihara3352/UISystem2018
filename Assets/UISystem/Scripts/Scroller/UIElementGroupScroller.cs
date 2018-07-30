using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller, INonActivatorUIElement{
		/*  cyclability is static
			it is calculated and the result is cached everytime any factor of constitutent dimension changes
		*/
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisCursorSize = MakeCursorSizeAtLeastOne(arg.cursorSize);
			thisGroupElementLength = arg.groupElementLength;
			thisPadding = arg.padding;
			thisInitiallyCursoredGroupElementIndex = arg.initiallyCursoredGroupElementIndex;
			thisStartSearchSpeed = MakeSureStartSearchSpeedIsGreaterThanZero(arg.startSearchSpeed);
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
			IUIElement initiallyCursoredGroupElement = uieGroup.GetUIElement(thisInitiallyCursoredGroupElementIndex);
			Vector2 result = Vector2.zero;
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
		/*  */
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
					int minColumnIndex;
					int minRowIndex;
					int maxColumnIndex;
					int maxRowIndex;
					CalcCursoredColumnRowIndex(groupElementUnderCursorRefPoint, out minColumnIndex, out minRowIndex, out maxColumnIndex, out maxRowIndex);

					IUIElement[] newCursoredElements = thisUIElementGroup.GetUIElementsWithinIndexRange(minColumnIndex, minRowIndex, maxColumnIndex, maxRowIndex);
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
			IUIElement leastCursoredElement = thisUIElementGroup.GetUIElementAtPositionInGroupSpace(cursorRefPInElementGroupSpace);

			return leastCursoredElement;
		}
		void CalcCursoredColumnRowIndex(IUIElement leastCursoredElement, out int minColumnIndex, out int minRowIndex, out int maxColumnIndex, out int maxRowIndex){
			int thisMinColumnIndex;
			int thisMinRowIndex;
			thisUIElementGroup.GetElementArrayIndex(leastCursoredElement, out thisMinColumnIndex, out thisMinRowIndex);
			int thisMaxColumnIndex = thisMinColumnIndex + thisCursorSize[0] - 1;
			int thisMaxRowIndex = thisMinRowIndex + thisCursorSize[1] - 1;
			
			minColumnIndex = thisMinColumnIndex;
			minRowIndex = thisMinRowIndex;
			maxColumnIndex = thisMaxColumnIndex;
			maxRowIndex = thisMaxRowIndex;
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
	}


	
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int[] cursorSize{get;}
		Vector2 groupElementLength{get;}
		Vector2 padding{get;}
		int initiallyCursoredGroupElementIndex{get;}
		float startSearchSpeed{get;}
	}
	public class UIElementGroupScrollerConstArg: ScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(int initiallyCursoredElementIndex, int[] cursorSize, Vector2 uiElementLength, Vector2 padding, float startSearchSpeed, Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, isEnabledInertia, uim, processFactory, uieFactory, uia, image){
			thisCursorSize = cursorSize;
			thisElementDimension = uiElementLength;
			thisPadding = padding;
			thisInitiallyCursoredElementIndex = initiallyCursoredElementIndex;
			thisStartSearchSpeed = startSearchSpeed;
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
	}
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{}
}
