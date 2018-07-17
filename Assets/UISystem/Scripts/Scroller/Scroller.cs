using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility.CurveUtility;
namespace UISystem{
	public interface IScroller: IUIElement{}
	public enum ScrollerAxis{
		Horizontal, Vertical, Both
	}
	public enum DragAxis{
		Horizontal, Vertical, None
	}
	public abstract class AbsScroller : AbsUIElement, IScroller{
		public AbsScroller(IScrollerConstArg arg): base(arg){
			thisRelativeCursorPosition = arg.relativeCursorPosition;
			thisCursorDimension = CalcCursorDimension(arg);
			thisScrollerAxis = arg.scrollerAxis;
			thisRubberBandLimit = arg.rubberBandLimit;
		}
		protected override void Activate(){
			List<IUIElement> childUIEs = GetChildUIEs();
			if(childUIEs.Count != 1)
				throw new System.InvalidOperationException("Scroller must have only one UIE child (=> scrollerElement)");
			if(childUIEs[0] == null)
				throw new System.InvalidOperationException("Scroller's only child must not be null");
			thisScrollerElement = childUIEs[0];
			base.Activate();
		}
		readonly  ScrollerAxis thisScrollerAxis;
		protected IUIElement thisScrollerElement;
		DragAxis thisDragAxis;
		public override void OnRelease(){
			thisDragAxis = DragAxis.None;
			base.OnRelease();
		}
		public override void OnDrag(ICustomEventData eventData){
			if(thisDragAxis == DragAxis.None){
				DragAxis dragAxis = CalcDragAxis(eventData.deltaP);
				thisDragAxis = dragAxis;
			}
			if(thisShouldProcessDrag){
				Vector2 deltaV2AlongAxis = GetDeltaV2AlongDragAxis(eventData.deltaP);
				DragImple(eventData.position, deltaV2AlongAxis);
			}else{
				base.OnDrag(eventData);
			}
		}
		DragAxis CalcDragAxis(Vector2 deltaP){
			if(deltaP.x >= deltaP.y)
				return DragAxis.Horizontal;
			else
				return DragAxis.Vertical;
		}
		bool thisShouldProcessDrag{
			get{
				if(thisScrollerAxis == ScrollerAxis.Both)
					return true;
				else{
					if(thisScrollerAxis == ScrollerAxis.Horizontal)
						return thisDragAxis == DragAxis.Horizontal;
					else if(thisScrollerAxis == ScrollerAxis.Vertical)
						return thisDragAxis == DragAxis.Vertical;
					else
						throw new System.InvalidOperationException("dragAxis should not be None, should be already evaluated");
				}
			}
		}
		Vector2 GetDeltaV2AlongDragAxis(Vector2 deltaP){
			if(thisDragAxis == DragAxis.Horizontal)
				return new Vector2(deltaP.x, 0f);
			else
				return new Vector2(0f, deltaP.y);
		}
		protected abstract bool thisShouldApplyRubberBand{get;}// simply return true if wanna apply
		readonly Vector2 thisRubberBandLimit;
		protected virtual void DragImple(Vector2 position, Vector2 deltaP){
			Vector2 newLocalPosition = GetLocalPosition() + deltaP;
			if(thisShouldApplyRubberBand){
				if(thisRequiresToCheckForHorizontalAxis){
					if(ScrollerElementIsOutOfCursorBounds(newLocalPosition[0], 0))
						if(this.IsDraggedTowardBoundary(deltaP, 0)){
							newLocalPosition[0] = CalcRubberBandedPosAlongAxis(newLocalPosition[0], 0);
						}
				}
				if(thisRequiresToCheckForVerticalAxis){

				}
			}
			SetLocalPosition(newLocalPosition);
		}
		bool thisRequiresToCheckForHorizontalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Horizontal || thisScrollerAxis == ScrollerAxis.Both;}
		}
		bool thisRequiresToCheckForVerticalAxis{
			get{return thisScrollerAxis == ScrollerAxis.Vertical || thisScrollerAxis == ScrollerAxis.Both;}
		}
		float CalcRubberBandedPosAlongAxis(float localPosAlongAxis, int dimension){
			float displacement = GetScrollerElementDisplacement(localPosAlongAxis, dimension);
			if(displacement < 0f){

			}else{

			}
		}
		bool ScrollerElementIsOutOfCursorBounds(float newLocalPosAlongAxis, int dimension){
			float elementNormalizedPosAlongAxis = GetScrollerElementNormalizedPosAlongAxis(newLocalPosAlongAxis, dimension);
			return elementNormalizedPosAlongAxis < 0f || elementNormalizedPosAlongAxis > 1f;
		}
		bool[] thisScrollerElementOnOrOutOfCursorBounds{
			get{
				bool[] result = new bool[2];
				for(int i = 0; i < 2; i ++){
					Vector2 elementNormPos = GetScrollerElementNormalizedPos();
					result[i] = elementNormPos[i] <= 0f || elementNormPos[i] >= 1f;
				}
				return result;
			}
		}
		float GetScrollerElementDisplacement(float newLocalPosAlongAxis, int dimension){
			Rect thisRect = GetUIAdaptor().GetRect();
			float rectLength = dimension == 0? thisRect.width: thisRect.height;
			float cursorLength = thisCursorDimension[dimension];
			float cursorMin = (rectLength - cursorLength) * thisRelativeCursorPosition[dimension];
			return cursorMin - newLocalPosAlongAxis;
		}
		float GetScrollerElementNormalizedPosAlongAxis(float newLocalPosAlongAxis, int dimension){
			/*  (0f, 0f) if cursor rests on top left corner of the element
				(1f, 1f) if cursor rests on bottom right corner of the element
				value below 0f and over 1f indicates the element's displacement beyond cursor bounds
			*/
			float displacement = GetScrollerElementDisplacement(newLocalPosAlongAxis, dimension);
			return displacement / thisCursorDimension[dimension];
		}
		readonly Vector2 thisCursorDimension;
		protected abstract Vector2 CalcCursorDimension(IScrollerConstArg arg);
		protected Vector2 thisRelativeCursorPosition; 
		/* 	(-1f, -1f)	corresponds upper left corner,
			(0f, 0f)	center
			(1f, 1f)	bottom right corner
		*/
	}
	public interface IScrollerConstArg: IUIElementConstArg{
		Vector2 relativeCursorPosition{get;}
	}
	public abstract class AbsScrollerConstArg: UIElementConstArg, IScrollerConstArg{
		public AbsScrollerConstArg(Vector2 relativeCursorPos, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IScrollerAdaptor uia, IUIImage uiImage): base(uim, processFactory, uieFactory, uia, uiImage){
			thisRelativeCursorPos = MakeRelativeCursorPosInRange(relativeCursorPos);
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
			get{
				return thisRelativeCursorPos;
			}
		}
	}
	public interface IScrollerAdaptor: IUIAdaptor{}
}
