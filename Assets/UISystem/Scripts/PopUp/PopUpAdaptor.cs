using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpAdaptor: IUIAdaptor{
		void ToggleRaycastBlock(bool interactable);
	}
	public class PopUpAdaptor : UIAdaptor, IPopUpAdaptor{
		public bool hidesOnTappingOthers;
		public PopUpMode popUpMode;
		protected override IUIElement CreateUIElement(IUIImage image){
			IPopUpConstArg arg = new PopUpConstArg(
				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				activationMode,

				thisDomainInitializationData.uim.GetPopUpManager(),
				hidesOnTappingOthers,
				popUpMode
			);
			return new PopUp(arg);
		}
		public void ToggleRaycastBlock(bool blocks){
			CanvasGroup canvasGroup = this.transform.GetComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = blocks;
		}
		protected override void SetUpUIElementReferenceImple(){
			base.SetUpUIElementReferenceImple();
			IPopUp popUp = (IPopUp)GetUIElement();
			popUp.SetUpPopUpHierarchy();
		}
	}
}
