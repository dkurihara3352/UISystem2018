using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{
		int[] GetCursorSize();
	}
	public class UIElementGroupScrollerAdaptor: AbsScrollerAdaptor<IUIElementGroupScroller>, IUIElementGroupScrollerAdaptor{
		public int initiallyCursoredElementIndex = 0;
		public int[] cursorSize = new int[2]{1, 1};
		public int[] GetCursorSize(){
			return cursorSize;
		}
		public float startSearchSpeed = 200f;
		public bool swipeToSnapNext = false;
		public bool activatesCursoredElementsOnly = false;
		protected override void CompleteUIElementReferenceSetUpImple(){
			IUIElementGroupScroller scroller = (IUIElementGroupScroller)GetUIElement();
			scroller.SetUpScrollerElement();
			base.CompleteUIElementReferenceSetUpImple();
		}
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(
				initiallyCursoredElementIndex, 
				cursorSize, 
				startSearchSpeed, 
				activatesCursoredElementsOnly,

				relativeCursorPosition, 
				scrollerAxis, 
				rubberBandLimitMultiplier, 
				isEnabledInertia, 
				swipeToSnapNext, 
				locksInputAboveThisVelocity,
				
				thisDomainInitializationData.uim, 
				thisDomainInitializationData.processFactory, 
				thisDomainInitializationData.uiElementFactory, 
				this, 
				image,
				activationMode
			);
			return new UIElementGroupScroller(arg);
		}
	}
}
