using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem{
	public interface IGenericAlphaActivatorUIElementAdaptor: IUIAdaptor{}
	[RequireComponent(typeof(CanvasGroup))]
	public class GenericAlphaActivatorUIElementAdaptor : UIAdaptor, IGenericAlphaActivatorUIElementAdaptor {
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(
				thisDomainActivationData.uim, 
				thisDomainActivationData.processFactory, 
				thisDomainActivationData.uiElementFactory, 
				this, 
				image,
				ActivationMode.Alpha
			);
			return new GenericUIElement(arg);
		}
	}
}
