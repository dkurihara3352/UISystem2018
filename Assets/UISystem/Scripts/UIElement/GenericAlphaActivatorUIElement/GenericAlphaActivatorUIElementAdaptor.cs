using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IGenericAlphaActivatorUIElementAdaptor: IAlphaActivatorUIElementAdaptor{}
	[RequireComponent(typeof(CanvasGroup))]
	public class GenericAlphaActivatorUIElementAdaptor : AbsAlphaActivatorUIElementAdaptor, IGenericAlphaActivatorUIElementAdaptor {
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			return new GenericAlphaActivatorUIElement(arg);
		}
	}
}
