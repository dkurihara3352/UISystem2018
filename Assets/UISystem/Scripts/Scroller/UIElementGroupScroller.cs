using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScroller: IScroller{}
	public class UIElementGroupScroller : AbsScroller, IUIElementGroupScroller{
		public UIElementGroupScroller(IUIElementGroupScrollerConstArg arg): base(arg){
			thisIsCyclicEnabled = arg.isCyclicEnabled;
		}
		protected override void InitializeSelectabilityState(){
			BecomeSelectable();
		}
		protected override void Activate(){
			base.Activate();
			EvaluateElementsCountForCyclability();
		}
		Vector2 thisPadding{
			get{return thisUIElementGroup.GetPadding();}
		}
		Vector2 thisElementDimension{
			get{return thisUIElementGroup.GetElementDimension();}
		}
		void EvaluateElementsCountForCyclability(){

		}
		int[] GetMinimumRequiredElementsCountToCycle(){
			int[] result = new int[2];
			for(int i = 0; i < 2; i ++){
				Rect thisRect = GetUIAdaptor().GetRect();
				float rectLength = i == 0? thisRect.width: thisRect.height;
				float perfectlyFitLength = (rectLength - thisPadding[i])/ (thisElementDimension[i] + thisPadding[i]);
				int perfectlyContainableElementsCount = Mathf.FloorToInt(perfectlyFitLength);
				float remainingLength = rectLength - perfectlyFitLength;
				if(remainingLength <= 0f)
					perfectlyContainableElementsCount += 1;
				else
					perfectlyContainableElementsCount += 2;
				result[i] = perfectlyContainableElementsCount;
			}
			return result;
		}
		IUIElementGroup thisUIElementGroup{
			get{
				return (IUIElementGroup)thisScrollerElement;
			}
		}
		bool[] thisIsCyclic{
			get{
				bool[] result = new bool[2];
				for(int i = 0; i < 2; i ++){
					bool isCyclic = false;
					if(thisIsCyclicEnabled[i]){
						if(thisHasEnoughElementsToCycle[i])
							isCyclic = true;
					}
					result[i] = isCyclic;
				}
				return result;
			}
		}
		bool[] thisIsCyclicEnabled;
		bool[] thisHasEnoughElementsToCycle;
		int thisElementsCount{
			get{
				return thisUIElementGroup.GetSize();
			}
		}
		protected override bool thisShouldApplyRubberBand{
			get{
				return !thisIsCyclic;
			}
		}
		protected override void DragImple(Vector2 position, Vector2 deltaP){
			base.DragImple(position, deltaP);
			if(thisIsCyclic){
				if(thisShouldCycleThisFrame)
					Cycle();
			}
		}
		bool thisShouldCycleThisFrame{
			get{

			}
		}
		protected override Vector2 CalcCursorDimension(IScrollerConstArg arg){
			IUIElementGroupScrollerConstArg typedArg = (IUIElementGroupScrollerConstArg)arg;
			int horizontalCursorSize = typedArg.horizontalCursorSize;
			int verticalCursorSize = typedArg.verticalCursorSize;
			Vector2 elementDimension = typedArg.elementDimension;
			Vector2 padding = typedArg.padding;

			float cursorWidth = horizontalCursorSize * (elementDimension.x + padding.x) + padding.x;
			float cursorHeight = verticalCursorSize * (elementDimension.y + padding.y) + padding.y;
			IUIElementGroupScrollerAdaptor uia = (IUIElementGroupScrollerAdaptor)GetUIAdaptor();
			Rect thisRect = uia.GetRect();
			float newRectWidth = thisRect.width;
			float newRectHeight = thisRect.height;
			float newCursorRelativePosX = thisRelativeCursorPosition.x;
			float newCursorRelativePosY = thisRelativeCursorPosition.y;
			if(cursorWidth > thisRect.width){
				newRectWidth = cursorWidth;
				newCursorRelativePosX = -1f;
			}
			if(cursorHeight > thisRect.height){
				newRectHeight = cursorHeight;
				newCursorRelativePosY = -1f;
			}
			uia.SetRectDimension(new Vector2(newRectWidth, newRectHeight));
			thisRelativeCursorPosition = new Vector2(newCursorRelativePosX, newCursorRelativePosY);

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
