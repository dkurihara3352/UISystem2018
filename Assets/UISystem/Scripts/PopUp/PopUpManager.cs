﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpManager{
		void RegisterPopUp(IPopUp popUpToRegister);
		void UnregisterPopUp(IPopUp popUpToUnregister);
		void HideActivePopUp();
		void SetRootUIElement(IUIElement uiElement);
	}
	public class PopUpManager : IPopUpManager {
		IUIElement thisRootUIElement;
		public void SetRootUIElement(IUIElement rootUIElement){
			thisRootUIElement = rootUIElement;
		}

		public void RegisterPopUp(IPopUp popUpToRegister){
			popUpToRegister.ShowHiddenProximateParentPopUpRecursively();
			DisableOthers(popUpToRegister);
			SetActivePopUp(popUpToRegister);
		}
		protected IPopUp thisActivePopUp;
		protected void SetActivePopUp(IPopUp popUp){
			thisActivePopUp = popUp;
		}
		public void HideActivePopUp(){
			if(thisActivePopUp != null)
				thisActivePopUp.Hide(false);
		}
		void DisableOthers(IPopUp disablingPopUp){
			if(thisActivePopUp == null){
				thisRootUIElement.PopUpDisableRecursivelyDownTo(disablingPopUp);
			}else{
				if(disablingPopUp.IsAncestorOf(thisActivePopUp))
					return;
				thisActivePopUp.PopUpDisableRecursivelyDownTo(disablingPopUp);
			}
		}
		public void UnregisterPopUp(IPopUp popUpToUnregister){
			popUpToUnregister.HideShownChildPopUpsRecursively();
			ReverseDisableOthers(popUpToUnregister);
		}
		void ReverseDisableOthers(IPopUp enablingPopUp){
			IPopUp parentPopUp = enablingPopUp.GetProximateParentPopUp();

			if(parentPopUp != null){
				parentPopUp.ReversePopUpDisableRecursively();
				RegisterPopUp(parentPopUp);
			}else{
				thisRootUIElement.ReversePopUpDisableRecursively();
				ClearActivePopUp();
			}
		}
		void ClearActivePopUp(){
			thisActivePopUp = null;
		}
	}
}
