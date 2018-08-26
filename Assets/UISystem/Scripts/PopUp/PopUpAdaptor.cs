using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpAdaptor: IUIAdaptor{
		void ToggleRaycastBlock(bool interactable);
	}
	public class PopUpAdaptor : UIAdaptor, IPopUpAdaptor{
		public bool disablesOthers;
		public bool hidesOnTappingOthers;
		public PopUpMode popUpMode;
		protected override IUIElement CreateUIElement(IUIImage image){
			IPopUpConstArg arg = new PopUpConstArg(
				thisDomainActivationData.uim,
				thisDomainActivationData.processFactory,
				thisDomainActivationData.uiElementFactory,
				this,
				image,
				activationMode,

				thisDomainActivationData.uim.GetPopUpManager(),
				disablesOthers,
				hidesOnTappingOthers,
				popUpMode
			);
			return new PopUp(arg);
		}
		public void ToggleRaycastBlock(bool blocks){
			CanvasGroup canvasGroup = this.transform.GetComponent<CanvasGroup>();
			canvasGroup.blocksRaycasts = blocks;
		}
	}
}
