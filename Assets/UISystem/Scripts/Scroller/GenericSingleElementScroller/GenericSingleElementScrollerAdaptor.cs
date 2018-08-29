using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
public interface IGenericSingleElementScrollerAdaptor: IScrollerAdaptor{
	}
	public class GenericSingleElementScrollerAdaptor: AbsScrollerAdaptor<IGenericSingleElementScroller>, IGenericSingleElementScrollerAdaptor{
		public Vector2 relativeCursorLength;
		protected override IUIElement CreateUIElement(IUIImage image){
			IGenericSingleElementScrollerConstArg arg = new GenericSingleElementScrollerConstArg(
				relativeCursorLength, 
				scrollerAxis, 
				rubberBandLimitMultiplier, 
				relativeCursorPosition, 
				isEnabledInertia, 
				locksInputAboveThisVelocity,
				
				thisDomainInitializationData.uim, 
				thisDomainInitializationData.processFactory, 
				thisDomainInitializationData.uiElementFactory, 
				this, 
				image,
				activationMode
			);
			return new GenericSingleElementScroller(arg);
		}
	}
}

