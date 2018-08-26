using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementGroupScrollerAdaptor: IScrollerAdaptor{}
	public class UIElementGroupScrollerAdaptor: AbsScrollerAdaptor<IUIElementGroupScroller>, IUIElementGroupScrollerAdaptor{
		public int initiallyCursoredElementIndex;
		public int[] cursorSize;
		public Vector2 groupElementLength;
		public Vector2 padding;
		public float startSearchSpeed;
		public bool swipeToSnapNext;
		public bool activatesCursoredElementsOnly;
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(
				initiallyCursoredElementIndex, 
				cursorSize, 
				groupElementLength, 
				padding, 
				startSearchSpeed, 
				activatesCursoredElementsOnly,

				relativeCursorPosition, 
				scrollerAxis, 
				rubberBandLimitMultiplier, 
				isEnabledInertia, 
				swipeToSnapNext, 
				locksInputAboveThisVelocity,
				
				thisDomainActivationData.uim, 
				thisDomainActivationData.processFactory, 
				thisDomainActivationData.uiElementFactory, 
				this, 
				image,
				activationMode
			);
			return new UIElementGroupScroller(arg);
		}
	}
}
