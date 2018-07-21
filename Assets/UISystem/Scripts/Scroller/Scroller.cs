using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;
namespace UISystem{
	public interface IScroller: IUIElement{}
	public enum ScrollerAxis{
		Horizontal, Vertical, Both
	}
	public abstract class AbsScroller : AbsUIElement, IScroller{
		public AbsScroller(IScrollerConstArg arg): base(arg){
			Rect thisRect = GetUIAdaptor().GetRect();
			thisCursorDimension = CalcCursorDimension(arg, thisRect);
			FitCursorDimensionToThisRect(thisRect);
			thisCursorLocalPosition = CalcCursorLocalPos(arg.relativeCursorPosition, thisRect);
			thisScrollerAxis = arg.scrollerAxis;
			if(thisShouldApplyRubberBand)
				thisRubberBandCalculator = CreateRubberBandCalculator(arg, thisRect);
		}
		public override void ActivateImple(){
			List<IUIElement> childUIEs = GetChildUIEs();
			if(childUIEs == null)
				throw new System.NullReferenceException("childUIEs must not be null");
			if(childUIEs.Count != 1)
				throw new System.InvalidOperationException("Scroller must have only one UIE child as Scroller Element");
			if(childUIEs[0] == null)
				throw new System.InvalidOperationException("Scroller's only child must not be null");
			thisScrollerElement = childUIEs[0];
			EvaluateElementSizeRelativeToCursor();
			base.ActivateImple();
		}
		readonly  ScrollerAxis thisScrollerAxis;
		protected IUIElement thisScrollerElement;
		float GetScrollerElementLength(int dimension){
			IUIAdaptor scrollerAdaptor = thisScrollerElement.GetUIAdaptor();
			return GetRectLengthOnAxis(scrollerAdaptor.GetRect(), dimension);
		}
		float GetThisRectLength(int dimension){
			return GetRectLengthOnAxis(GetUIAdaptor().GetRect(), dimension);
		}
		protected float GetRectLengthOnAxis(Rect rect, int dimension){
			return dimension == 0? rect.width: rect.height;
		}
		protected int thisDragAxis;
		bool thisIsNotDragged{get{return thisDragAxis == -1;}}
		bool thisIsDraggedHorizontally{get{return thisDragAxis == 0;}}
		bool thisIsDraggedVertically{get{return thisDragAxis == 1;}}
		public override void OnRelease(){
			thisDragAxis = -1;
			base.OnRelease();
		}
		public override void OnDrag(ICustomEventData eventData){
			if(thisIsNotDragged){
				int dragAxis = CalcDragAxis(eventData.deltaP);
				thisDragAxis = dragAxis;
			}
			if(thisShouldProcessDrag){
				Vector2 deltaV2AlongAxis = GetDeltaV2AlongDragAxis(eventData.deltaP);
				DragImple(eventData.position, deltaV2AlongAxis);
			}else{
				PassDragUpwardInHierarchy(eventData);
			}
		}
		void PassDragUpwardInHierarchy(ICustomEventData eventData){
			base.OnDrag(eventData);
		}
		int CalcDragAxis(Vector2 deltaP){
			if(deltaP.x >= deltaP.y)
				return 0;
			else
				return 1;
		}
		bool thisShouldProcessDrag{
			get{
				if(thisScrollerAxis == ScrollerAxis.Both)
					return true;
				else{
					if(thisScrollerAxis == ScrollerAxis.Horizontal)
						return thisIsDraggedHorizontally;
					else if(thisScrollerAxis == ScrollerAxis.Vertical)
						return thisIsDraggedVertically;
					else
						throw new System.InvalidOperationException("dragAxis should not be None, should be already evaluated");
				}
			}
		}
		Vector2 GetDeltaV2AlongDragAxis(Vector2 deltaP){
			if(thisIsDraggedHorizontally)
				return new Vector2(deltaP.x, 0f);
			else
				return new Vector2(0f, deltaP.y);
		}
		protected abstract bool thisShouldApplyRubberBand{get;}// simply return true if wanna apply
		readonly RubberBandCalculator[] thisRubberBandCalculator;
		RubberBandCalculator[] CreateRubberBandCalculator(IScrollerConstArg arg, Rect thisRect){
			RubberBandCalculator[] result = new RubberBandCalculator[2];
			for(int i = 0; i < 2; i++){
				float rectLength = GetRectLengthOnAxis(thisRect, i); 
				result[i] = new RubberBandCalculator(1f, arg.rubberBandLimitMultiplier[i] * rectLength);
			}
			return result;
		}
		protected virtual void DragImple(Vector2 position, Vector2 deltaP){
			Vector2 newLocalPosition = GetLocalPosition() + deltaP;
			if(thisShouldApplyRubberBand){
				if(thisRequiresToCheckForHorizontalAxis){
					newLocalPosition[0] = CheckAndApplyRubberBand(deltaP[0], newLocalPosition[0], 0);
				}
				if(thisRequiresToCheckForVerticalAxis){
					newLocalPosition[1] = CheckAndApplyRubberBand(deltaP[1], newLocalPosition[1], 1);
				}
			}
			SetLocalPosition(newLocalPosition);
		}
		float CheckAndApplyRubberBand(float deltaPOnAxis, float elementLocalPosOnAxis, int dimension){
			float result = elementLocalPosOnAxis;
			if(ElementIsDisplacedInDragDeltaDirection(deltaPOnAxis, elementLocalPosOnAxis, dimension)){
				result = CalcRubberBandedPosOnAxis(elementLocalPosOnAxis, dimension);
			}
			return result;
		}
		bool ElementIsDisplacedInDragDeltaDirection(float deltaPOnAxis, float elementLocalPosOnAxis, int dimension){
			float displacement = GetElementCursorDisplacement(elementLocalPosOnAxis, dimension);
			if(displacement == 0f)
				return false;
			else{
				if(displacement < 0f)
					return deltaPOnAxis < 0f;
				else//displacement > 0f
					return deltaPOnAxis > 0f;
			}
		}
		float GetElementCursorDisplacement(float elementLocalPosOnAxis, int dimension){
			return GetElementDisplacement(elementLocalPosOnAxis, dimension, thisCursorDimension[dimension], thisCursorLocalPosition[dimension], thisCursorLocalPosition[dimension]);
			/*  when element is undersized, its is bound to cursor's origin
			*/
		}
		protected float GetElementScrollerDisplacement(float elementLocalPosOnAxis, int dimension){
			return GetElementDisplacement(elementLocalPosOnAxis, dimension, GetThisRectLength(dimension), 0f, thisCursorLocalPosition[dimension]);
			/*  undersized element is bound to cursor's origin
			*/
		}
		protected float GetElementDisplacement(float elementLocalPosOnAxis, int dimension, float referenceLength, float referenceMin, float referenceMinForUndersizeElement){
			float referenceMax;
			float elementLength = GetScrollerElementLength(dimension);
			float actualReferenceMin = referenceMin;
			if(ElementIsUndersizedToCursor(dimension)){
				actualReferenceMin = referenceMinForUndersizeElement;
				referenceMax = actualReferenceMin + elementLength;
			}else{
				referenceMax = actualReferenceMin + referenceLength;
			}
			float elementMin = elementLocalPosOnAxis;
			float elementMax = elementMin + elementLength;
			
			float lesserDisp = actualReferenceMin - elementMin;
			if(lesserDisp < 0f)
				return lesserDisp;
			float greaterDisp = referenceMax - elementMax;
			if(greaterDisp > 0f)
				return greaterDisp;
			return 0f;
		}
		protected bool ElementIsUndersizedToCursor(int dimension){
			/*  return true if smaller or equal to cursor
			 */
			return thisElementIsUndersizedToCursor[dimension];
		}
		bool[] thisElementIsUndersizedToCursor;
		void EvaluateElementSizeRelativeToCursor(){
			Rect thisRect = GetUIAdaptor().GetRect();
			bool[] result = new bool[2];
			for(int i = 0; i < 2; i++){
				result[i] = GetScrollerElementLength(i) <= thisCursorDimension[i];
			}
			thisElementIsUndersizedToCursor = result;
		}
		bool IsDraggedTowardBoundary(float deltaPAlongAxis, float elementCursorValue, int dimension){
			/*  Invert y axis
			*/
			if(elementCursorValue < 0f)
				if(dimension == 0)
					return deltaPAlongAxis > 0f;
				else
					return deltaPAlongAxis < 0f;
			else if(elementCursorValue > 1f)
				if(dimension == 0)
					return deltaPAlongAxis > 0f;
				else
					return deltaPAlongAxis < 0f;
			else
				throw new System.InvalidOperationException("element is not off cursor boundary");
		}
		protected bool thisRequiresToCheckForHorizontalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Horizontal || thisScrollerAxis == ScrollerAxis.Both;}
		}
		protected bool thisRequiresToCheckForVerticalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Vertical || thisScrollerAxis == ScrollerAxis.Both;}
		}
		float CalcRubberBandedPosOnAxis(float localPosOnAxis, int dimension){
			float elementCursorDisplacement = GetElementCursorDisplacement(localPosOnAxis, dimension);
			float cursorMin = thisCursorLocalPosition[dimension];
			float cursorMax;
			if(ElementIsUndersizedToCursor(dimension))
				cursorMax = cursorMin + GetScrollerElementLength(dimension);
			else
				cursorMax = cursorMin + thisCursorDimension[dimension];
			float basePoint;
			float displacementFromBasePoint;
			if(elementCursorDisplacement < 0f){
				basePoint = cursorMin;
				displacementFromBasePoint = localPosOnAxis - basePoint;
				return basePoint + thisRubberBandCalculator[dimension].CalcRubberBandValue(displacementFromBasePoint, invert: false);
			}else{
				basePoint = cursorMax;
				float elementLength = GetScrollerElementLength(dimension);
				displacementFromBasePoint = (localPosOnAxis + elementLength) - basePoint;
				float rubberValue = thisRubberBandCalculator[dimension].CalcRubberBandValue(displacementFromBasePoint, invert: true);
				return basePoint + rubberValue - elementLength;
			}
		}
		bool ScrollerElementIsOutOfCursorBounds(float newLocalPosAlongAxis, int dimension){
			return GetElementCursorDisplacement(newLocalPosAlongAxis, dimension) != 0f;
		}
		protected float GetElementCursorValue(float elementLocalPosOnAxis, int dimension){
			/*  (0f, 0f) if cursor rests on top left corner of the element
				(1f, 1f) if cursor rests on bottom right corner of the element
				value below 0f and over 1f indicates the element's displacement beyond cursor bounds
			*/
			if(ElementIsUndersizedToCursor(dimension)){
				return 0f;
			}else{
				IUIAdaptor scrollerElementAdaptor = thisScrollerElement.GetUIAdaptor();
				Rect scrollerElementRect = scrollerElementAdaptor.GetRect();
				float elementLength = GetRectLengthOnAxis(scrollerElementRect, dimension);
				float cursorLength = thisCursorDimension[dimension];
				float cursorMin = thisCursorLocalPosition[dimension];
				return (cursorMin - elementLocalPosOnAxis)/ (elementLength - cursorLength);
			}
		}
		Vector2 thisCursorDimension;
		protected abstract Vector2 CalcCursorDimension(IScrollerConstArg arg, Rect thisRect);

		protected Vector2 thisCursorLocalPosition;
		protected virtual Vector2 CalcCursorLocalPos(Vector2 relativeCursorPosition, Rect thisRect){
			Vector2 result = new Vector2();
			for(int i = 0; i < 2; i ++){
				float rectLength = GetRectLengthOnAxis(thisRect, i);
				float cursorLength = thisCursorDimension[i];
				float diffL = rectLength - cursorLength;

				float localPos;
				if(relativeCursorPosition[i] == 0f) 
					localPos = 0f;
				else
					localPos = relativeCursorPosition[i] * diffL;
				result[i] = localPos;
			}
			return result;
		}
		void FitCursorDimensionToThisRect(Rect thisRect){
			for(int i = 0; i < 2; i ++){
				float rectLength = GetRectLengthOnAxis(thisRect, i);
				if(thisCursorDimension[i] > rectLength)
					thisCursorDimension[i] = rectLength;
			}
		}
	}
	public interface IScrollerConstArg: IUIElementConstArg{
		Vector2 relativeCursorPosition{get;}
		ScrollerAxis scrollerAxis{get;}
		float[] rubberBandLimitMultiplier{get;}
		/* 	RelativeCursorPosition
			(0f, 0f)	corresponds upper left corner,
			(.5f, .5f)	center
			(1f, 1f)	bottom right corner
		*/
	}
	public abstract class AbsScrollerConstArg: UIElementConstArg, IScrollerConstArg{
		public AbsScrollerConstArg(Vector2 relativeCursorPos, ScrollerAxis scrollerAxis, float[] rubberBandLimitMultiplier, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IScrollerAdaptor uia, IUIImage uiImage): base(uim, processFactory, uieFactory, uia, uiImage){
			thisRelativeCursorPos = MakeRelativeCursorPosInRange(relativeCursorPos);
			thisScrollerAxis = scrollerAxis;
		}
		Vector2 MakeRelativeCursorPosInRange(Vector2 source){
			Vector2 newCusorPosition = new Vector2();
			for(int i = 0; i < 2; i ++){
				float newPos = Mathf.Clamp(source[i], -1f, 1f);
				newCusorPosition[i] = newPos;
			}
			return newCusorPosition;
		}
		readonly Vector2 thisRelativeCursorPos;
		public Vector2 relativeCursorPosition{
			get{return thisRelativeCursorPos;}
		}
		readonly ScrollerAxis thisScrollerAxis;
		public ScrollerAxis scrollerAxis{
			get{return thisScrollerAxis;}
		}
		readonly float[] thisRubberBandLimitMultiplier;
		public float[] rubberBandLimitMultiplier{
			get{return thisRubberBandLimitMultiplier;}
		}
	}
	public interface IScrollerAdaptor: IUIAdaptor{}
}
