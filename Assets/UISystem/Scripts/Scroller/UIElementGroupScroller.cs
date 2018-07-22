﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller, INonActivatorUIElement{
		/*  cyclability is static
			it is calculated and the result is cached everytime any factor of constitutent dimension changes
		*/
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisIsCyclicEnabled = arg.isCyclicEnabled;
			thisCursorSize = arg.cursorSize;
			thisElementDimension = arg.elementDimension;
			thisPadding = arg.padding;
		}
		readonly int[] thisCursorSize;
		readonly Vector2 thisPadding;
		readonly Vector2 thisElementDimension;
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
		protected override void InitializeSelectabilityState(){
			BecomeSelectable();
		}
		public override void ActivateImple(){
			base.ActivateImple();
			EvaluateCyclability();
		}
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
		protected override void DragImpleInner(Vector2 position, Vector2 deltaP){
			base.DragImpleInner(position, deltaP);
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
		void Cycle(){}
		protected override Vector2 CalcCursorDimension(Rect thisRect){
			float cursorWidth = thisCursorSize[0] * (thisElementDimension.x + thisPadding.x) + thisPadding.x;
			float cursorHeight = thisCursorSize[1] * (thisElementDimension.y + thisPadding.y) + thisPadding.y;
			IUIElementGroupScrollerAdaptor uia = (IUIElementGroupScrollerAdaptor)GetUIAdaptor();
			float newRectWidth = thisRect.width;
			float newRectHeight = thisRect.height;
			if(cursorWidth > thisRect.width)
				newRectWidth = cursorWidth;
			if(cursorHeight > thisRect.height)
				newRectHeight = cursorHeight;
			uia.SetRectDimension(newRectWidth, newRectHeight);

			return new Vector2(cursorWidth, cursorHeight);
		}
	}
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int[] cursorSize{get;}
		Vector2 elementDimension{get;}
		Vector2 padding{get;}
		bool[] isCyclicEnabled{get;}
	}
	public class UIElementGroupScrollerConstArg: AbsScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(int[] cursorSize, Vector2 elementDimension, Vector2 padding, bool[] isCyclicEnabled, Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, Vector2 rubberBandLimitMultiplier, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, uim, processFactory, uieFactory, uia, image){
			for(int i = 0; i < 2; i ++)
				cursorSize[i] = MakeCursorSizeInRange(cursorSize[i]);
			thisCursorSize = cursorSize;
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
		readonly int[] thisCursorSize;
		public int[] cursorSize{get{return thisCursorSize;}}
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
