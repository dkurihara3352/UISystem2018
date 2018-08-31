using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpButtonAdaptor: IUIAdaptor{}
	public class PopUpButtonAdaptor : UIAdaptor, IPopUpButtonAdaptor {
		public PopUpAdaptor targetPopUpAdaptor;
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementConstArg arg = new UIElementConstArg(
				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				activationMode
			);
			return new PopUpButton(arg);
		}
		protected override void SetUpUIElementReferenceImple(){
			base.SetUpUIElementReferenceImple();
			IPopUp popUp = (IPopUp)targetPopUpAdaptor.GetUIElement();
			IPopUpButton popUpButton = (IPopUpButton)GetUIElement();
			popUpButton.SetTargetPopUp(popUp);
		}
	}
}
