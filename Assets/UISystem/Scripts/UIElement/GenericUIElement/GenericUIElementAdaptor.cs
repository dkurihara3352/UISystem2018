using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class GenericUIElementAdaptor: UIAdaptor{
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(
				thisDomainActivationData.uim, 
				thisDomainActivationData.processFactory, 
				thisDomainActivationData.uiElementFactory, 
				this, 
				image,
				ActivationMode.None
			);
			IGenericUIElement uie = new GenericUIElement(arg);
			return uie;
		}
	}
}
