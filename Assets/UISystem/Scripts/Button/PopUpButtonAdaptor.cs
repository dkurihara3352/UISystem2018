using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpButtonAdaptor: IUIAdaptor{}
	public class PopUpButtonAdaptor : UIAdaptor, IPopUpButtonAdaptor {
		public PopUpAdaptor targetPopUpAdaptor;
		protected override IUIElement CreateUIElement(IUIImage image){
			IPopUpButtonConstArg arg = new PopUpButtonConstArg(
				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				activationMode,
				targetPopUpAdaptor
			);
			return new PopUpButton(arg);
		}
	}
}
