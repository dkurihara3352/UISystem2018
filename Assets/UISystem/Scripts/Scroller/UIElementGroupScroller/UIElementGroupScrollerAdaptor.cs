﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{}
	public class UIElementGroupScrollerAdaptor: AbsScrollerAdaptor<IUIElementGroupScroller>, IUIElementGroupScrollerAdaptor{
		public int initiallyCursoredElementIndex;
		public int[] cursorSize;
		public float startSearchSpeed;
		public bool swipeToSnapNext;
		public bool activatesCursoredElementsOnly;
		protected override void CompleteUIElementReferenceSetUpImple(){
			IUIElementGroupScroller scroller = (IUIElementGroupScroller)GetUIElement();
			scroller.UpdateGroupElementLengthAndPadding();
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
