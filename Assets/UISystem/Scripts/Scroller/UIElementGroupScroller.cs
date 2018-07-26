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
			thisCursorSize = arg.cursorSize;
			thisUIElementLength = arg.uiElementLength;
			thisPadding = arg.padding;
			thisInitiallyCursoredElementIndex = arg.initiallyCursoredElementIndex;
		}
		readonly int thisInitiallyCursoredElementIndex;
		protected override Vector2 GetInitialPositionNormalizedToCursor(){
			IUIElementGroup uieGroup = thisUIElementGroup;
			IUIElement initiallyCursoredElement = uieGroup.GetUIElement(thisInitiallyCursoredElementIndex);
			Vector2 result = Vector2.zero;
			Vector2 elementLocalPos = initiallyCursoredElement.GetLocalPosition();
			float resultX = GetNormalizedCursoredPositionFromPosInElementSpace(elementLocalPos.x - thisPadding[0], 0);
			float resultY = GetNormalizedCursoredPositionFromPosInElementSpace(elementLocalPos.y - thisPadding[1], 1);
			return new Vector2(resultX, resultY);
		}
		protected readonly int[] thisCursorSize;
		protected readonly Vector2 thisPadding;
		protected readonly Vector2 thisUIElementLength;
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
		int thisElementsCount{
			get{
				return thisUIElementGroup.GetSize();
			}
		}
		protected override bool thisShouldApplyRubberBand{get{return true;}}
		protected override Vector2 CalcCursorLength(){
			float cursorWidth = thisCursorSize[0] * (thisUIElementLength.x + thisPadding.x) + thisPadding.x;
			float cursorHeight = thisCursorSize[1] * (thisUIElementLength.y + thisPadding.y) + thisPadding.y;
			Vector2 newCursorLength = new Vector2(cursorWidth, cursorHeight);
			if(newCursorLength[0] <= thisRectLength[0] && newCursorLength[1] <= thisRectLength[1])
				return newCursorLength;
			else
				throw new System.InvalidOperationException("cursorLength cannot exceed this rect length. provide lesser cursor size");
		}
	}



	
	public interface IUIElementGroupScrollerConstArg: IScrollerConstArg{
		int[] cursorSize{get;}
		Vector2 uiElementLength{get;}
		Vector2 padding{get;}
		int initiallyCursoredElementIndex{get;}
	}
	public class UIElementGroupScrollerConstArg: ScrollerConstArg, IUIElementGroupScrollerConstArg{
		public UIElementGroupScrollerConstArg(int initiallyCursoredElementIndex, int[] cursorSize, Vector2 uiElementLength, Vector2 padding, Vector2 relativeCursorPosition, ScrollerAxis scrollerAxis, Vector2 rubberBandLimitMultiplier, bool isEnabledInertia, IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uieFactory, IUIElementGroupScrollerAdaptor uia, IUIImage image): base(scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, isEnabledInertia, uim, processFactory, uieFactory, uia, image){
			for(int i = 0; i < 2; i ++)
				cursorSize[i] = MakeCursorSizeInRange(cursorSize[i]);
			thisCursorSize = cursorSize;
			thisElementDimension = uiElementLength;
			thisPadding = padding;
			thisInitiallyCursoredElementIndex = initiallyCursoredElementIndex;
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
		public Vector2 uiElementLength{get{return thisElementDimension;}}
		readonly Vector2 thisPadding;
		public Vector2 padding{get{return thisPadding;}}
		readonly int thisInitiallyCursoredElementIndex;
		public int initiallyCursoredElementIndex{get{return thisInitiallyCursoredElementIndex;}}
	}
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{}
}
