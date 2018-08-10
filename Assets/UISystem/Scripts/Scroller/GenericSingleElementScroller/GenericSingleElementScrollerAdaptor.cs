using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
public interface IGenericSingleElementScrollerAdaptor: IScrollerAdaptor{
	}
	public class GenericSingleElementScrollerAdaptor: AbsScrollerAdaptor<IGenericSingleElementScroller>, IGenericSingleElementScrollerAdaptor{
		public Vector2 relativeCursorLength;
		protected override IUIElement CreateUIElement(IUIImage image){
			IGenericSingleElementScrollerConstArg arg = new GenericSingleElementScrollerConstArg(relativeCursorLength, scrollerAxis, rubberBandLimitMultiplier, relativeCursorPosition, isEnabledInertia, thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			return new GenericSingleElementScroller(arg);
		}
		public override void GetReadyForActivation(IUIAActivationData passedData){
			base.GetReadyForActivation(passedData);
			IScroller scroller = (IScroller)GetUIElement();
			scroller.SetUpScrollerElement();
		}
	}
}

