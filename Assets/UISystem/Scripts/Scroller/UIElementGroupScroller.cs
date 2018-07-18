using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller{
		/*  cyclability is static
			it is calculated and the result is cached everytime any factor of constitutent dimension changes
		*/
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisIsCyclicEnabled = arg.isCyclicEnabled;
			thisPadding = arg.padding;
			thisElementDimension = arg.elementDimension;
		}
		protected override void InitializeSelectabilityState(){
			BecomeSelectable();
		}
		protected override void Activate(){
			base.Activate();
			EvaluateCyclability();
		}
		Vector2 thisPadding;
		Vector2 thisElementDimension;
		void EvaluateCyclability(){
			bool thisIsHorizontallyCyclable = false;
			bool thisIsVerticallyCyclable = false;
			if(thisRequiresToCheckForHorizontalAxis){
				thisIsHorizontallyCyclable = CalcCyclability(0);
			}
			if(thisRequiresToCheckForVerticalAxis)
				thisIsVerticallyCyclable = CalcCyclability(1);

			thisIsCyclable = new bool[2]{thisIsHorizontallyCyclable, thisIsVerticallyCyclable};
		}
		bool CalcCyclability(int dimension){
			bool isCyclic = false;
			if(thisIsCyclicEnabled[dimension]){
				if(this.HasEnoughElementsToCycle(dimension))
					isCyclic = true;
			}
			return isCyclic;
		}
		bool[] thisIsCyclable;
		bool IsCyclable(int dimension){
			return thisIsCyclable[dimension];
		}
		bool[] thisIsCyclicEnabled;
		bool HasEnoughElementsToCycle(int dimension){
			int elementsCountOnAxis = GetElementsCountOnAxis(dimension);
			int minimumRequiredElementsCountToCycleOnAxis = GetMinimumRequiredElementsCountToCycle(dimension);
			return elementsCountOnAxis >= minimumRequiredElementsCountToCycleOnAxis;
		}
		int GetElementsCountOnAxis(int dimension){
			return thisUIElementGroup.GetElementsArraySize(dimension);
		}

		int GetMinimumRequiredElementsCountToCycle(int dimension){
			Rect thisRect = GetUIAdaptor().GetRect();
			float rectLength = GetRectLengthOnAxis(thisRect, dimension);
			float perfectlyFitLength = (rectLength - thisPadding[dimension])/ (thisElementDimension[dimension] + thisPadding[dimension]);
			int perfectlyContainableElementsCount = Mathf.FloorToInt(perfectlyFitLength);
			float remainingLength = rectLength - perfectlyFitLength;
			if(remainingLength <= 0f)
				perfectlyContainableElementsCount += 1;
			else
				perfectlyContainableElementsCount += 2;
			return perfectlyContainableElementsCount;
		}
		IUIElementGroup thisUIElementGroup{
			get{
				return (IUIElementGroup)thisScrollerElement;
			}
		}
		int thisElementsCount{
			get{
				return thisUIElementGroup.GetSize();
			}
		}
		protected override bool thisShouldApplyRubberBand{
			get{
				return !this.IsCyclable(thisDragAxis);
			}
		}
		protected override void DragImple(Vector2 position, Vector2 deltaP){
			base.DragImple(position, deltaP);
			/* already translated to new local pos */
			if(this.IsCyclable(thisDragAxis)){
				if(this.ShouldCycleThisFrame(thisDragAxis))
					Cycle();
			}
		}
		bool ShouldCycleThisFrame(int dimension){
			float elementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			float elementScrollerDisplacement = GetElementScrollerDisplacement(elementLocalPosOnAxis, dimension);
			return elementScrollerDisplacement != 0;
		}
		protected override Vector2 CalcCursorDimension(IScrollerConstArg arg, Rect thisRect){
			IUIElementGroupScrollerConstArg typedArg = (IUIElementGroupScrollerConstArg)arg;
			int horizontalCursorSize = typedArg.horizontalCursorSize;
			int verticalCursorSize = typedArg.verticalCursorSize;
			Vector2 elementDimension = typedArg.elementDimension;
			Vector2 padding = typedArg.padding;

			float cursorWidth = horizontalCursorSize * (elementDimension.x + padding.x) + padding.x;
			float cursorHeight = verticalCursorSize * (elementDimension.y + padding.y) + padding.y;
			IUIElementGroupScrollerAdaptor uia = (IUIElementGroupScrollerAdaptor)GetUIAdaptor();
			float newRectWidth = thisRect.width;
			float newRectHeight = thisRect.height;
			if(cursorWidth > thisRect.width)
				newRectWidth = cursorWidth;
			if(cursorHeight > thisRect.height)
				newRectHeight = cursorHeight;
			uia.SetRectDimension(new Vector2(newRectWidth, newRectHeight));

			return new Vector2(cursorWidth, cursorHeight);
		}
	}
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int horizontalCursorSize{get;}
		int verticalCursorSize{get;}
		Vector2 elementDimension{get;}
		Vector2 padding{get;}
		bool[] isCyclicEnabled{get;}
	}
	public class UIElementGroupScrollerConstArg: AbsScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(int horizontalCursorSize, int verticalCursorSize, Vector2 elementDimension, Vector2 padding, bool[] isCyclicEnabled, Vector2 relativeCursorPosition, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(relativeCursorPosition, uim, processFactory, uieFactory, uia, image){
			thisHorizontalCursorSize = MakeCursorSizeInRange(horizontalCursorSize);
			thisVerticalCursorSize = MakeCursorSizeInRange(verticalCursorSize);
			thisElementDimension = elementDimension;
			thisPadding = padding;
			thisIsCyclicEnabled = isCyclicEnabled;
		}
		int MakeCursorSizeInRange(int source){
			if(source <= 0)
				return 1;
			else
				return source;
		}
		readonly int thisHorizontalCursorSize;
		public int horizontalCursorSize{
			get{return thisHorizontalCursorSize;}
		}
		readonly int thisVerticalCursorSize;
		public int verticalCursorSize{
			get{return thisVerticalCursorSize;}
		}
		readonly Vector2 thisElementDimension;
		public Vector2 elementDimension{
			get{return thisElementDimension;}
		}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
		readonly bool[] thisIsCyclicEnabled;
		public bool[] isCyclicEnabled{
			get{return thisIsCyclicEnabled;}
		}
	}
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{}
}
