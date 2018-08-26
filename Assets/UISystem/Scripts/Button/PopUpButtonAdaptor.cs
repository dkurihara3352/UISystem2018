using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpButtonAdaptor: IUIAdaptor{}
	public class PopUpButtonAdaptor : UIAdaptor, IPopUpButtonAdaptor {
		public PopUpAdaptor targetPopUpAdaptor;
		protected override IUIElement CreateUIElement(IUIImage image){
			IPopUpButtonConstArg arg = new PopUpButtonConstArg(
				thisDomainActivationData.uim,
				thisDomainActivationData.processFactory,
				thisDomainActivationData.uiElementFactory,
				this,
				image,
				activationMode,
				targetPopUpAdaptor
			);
			return new PopUpButton(arg);
		}
	}
}
