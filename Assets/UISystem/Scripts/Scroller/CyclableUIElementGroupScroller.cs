using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	/*  Not going to use this. Haven't got enought time to implement and test cycling feature
	*/
	public interface ICyclableUIElementGroupScroller: IUIElementGroupScroller{}
	public class CyclableUIElementGroupScroller : UIElementGroupScroller, ICyclableUIElementGroupScroller {
		public CyclableUIElementGroupScroller(ICyclableUIElementGroupScrollerConstArg arg): base(arg){
			thisIsCycleEnabled = arg.isCycleEnabled;
		}
		readonly bool[] thisIsCycleEnabled;
		protected override void OnUIActivate(){
			EvaluateCyclability();
		}
		protected override bool[] thisShouldApplyRubberBand{
			get{
				return new bool[]{ !this.IsCyclable(0), !this.IsCyclable(1)};
			}
		}
		/* Cyclability */
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
			return thisUIElementGroup.GetArraySize(dimension);
		}
		int GetMinimumRequiredElementsCountToCycle(int dimension){
			float perfectlyFitLength = (thisRectLength[dimension] - thisPadding[dimension])/ (thisGroupElementLength[dimension] + thisPadding[dimension]);
			int perfectlyContainableElementsCount = Mathf.FloorToInt(perfectlyFitLength);
			float remainingLength = thisRectLength[dimension] - perfectlyFitLength;
			if(remainingLength <= 0f)
				perfectlyContainableElementsCount += 1;
			else
				perfectlyContainableElementsCount += 2;
			return perfectlyContainableElementsCount;
		}
		protected override void DisplaceScrollerElement(Vector2 dragDeltaSinceTouch){
			base.DisplaceScrollerElement(dragDeltaSinceTouch);
			for(int i = 0; i < 2; i ++)
				if(this.IsCyclable(i)){
					if(this.ShouldCycleThisFrame(i))
						Cycle();
			}
		}
		bool ShouldCycleThisFrame(int dimension){
			/* precond: element is never undersized */
			float elementLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[dimension];
			float elementPositionNormalizedToCursor = GetNormalizedCursoredPositionOnAxis(elementLocalPosOnAxis, dimension);
			return elementPositionNormalizedToCursor != 0;
		}
		void Cycle(){}
	}
	public interface ICyclableUIElementGroupScrollerConstArg: IUIElementGroupScrollerConstArg{
		bool[] isCycleEnabled{get;}
	}
	public class CyclableUIElementGroupScrollerConstArg: UIElementGroupScrollerConstArg, ICyclableUIElementGroupScrollerConstArg{
		public CyclableUIElementGroupScrollerConstArg(
			bool[] isCycleEnabled, 
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
			initiallyCursoredElementIndex, 
			cursorSize, 
			startSearchSpeed,
			activatesCursoredElementsOnly,

			relativeCursorPosition, 
			scrollerAxis, 
			rubberBandLimitMultiplier, 
			isEnabledInertia, 
			swipeToSnapNext, 
			newScrollSpeedThreshold,

			uim, 
			processFactory, 
			uieFactory, 
			uia, 
			image,
			activationMode
		){
			thisIsCycleEnabled = isCycleEnabled;
		}
		readonly bool[] thisIsCycleEnabled;
		public bool[] isCycleEnabled{get{return thisIsCycleEnabled;}}
	}
}
