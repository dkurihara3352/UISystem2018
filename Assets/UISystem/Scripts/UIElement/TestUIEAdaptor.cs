using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public class TestUIEAdaptor: GenericUIElementAdaptor{
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			TestUIE uie = new TestUIE(arg);
			return uie;
		}
	}
}
