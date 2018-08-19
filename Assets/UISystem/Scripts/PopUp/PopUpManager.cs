using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpManager{
		void RegisterPopUp(IPopUp popUpToRegister);
		void UnregisterPopUp(IPopUp popUpToUnregister);
	}
	public class PopUpManager : IPopUpManager {
		IUIElement thisRoolUIElement;

		public void RegisterPopUp(IPopUp popUpToRegister){
			popUpToRegister.ShowHiddenProximateParentPopUpRecursively();
			DisableOthers(popUpToRegister);
			SetActivePopUp(popUpToRegister);
		}
		IPopUp thisActivePopUp;
		void SetActivePopUp(IPopUp popUp){
			thisActivePopUp = popUp;
		}
		void DisableOthers(IPopUp disablingPopUp){
			if(thisActivePopUp == null){
				thisRoolUIElement.PopUpDisableRecursivelyDownTo(disablingPopUp);
			}else{
				thisActivePopUp.PopUpDisableRecursivelyDownTo(disablingPopUp);
			}
		}
		public void UnregisterPopUp(IPopUp popUpToUnregister){
			popUpToUnregister.HideShownChildPopUpsRecursively();
			ReverseDisableOthers(popUpToUnregister);
		}
		void ReverseDisableOthers(IPopUp enablingPopUp){
			/*  if got some other parent above, 
					popupEnable recursively down all the way from it
				if not
					popUpEnable recursively all the way down from root
						enabling/ disabling should also be performed upon hidden/ hiding popUps
				all parents popups if any should already be open by now
			*/
			IPopUp parentPopUp = enablingPopUp.GetProximateParentPopUp();
			if(parentPopUp != null){
				parentPopUp.ReversePopUpDisableRecursively();
				RegisterPopUp(parentPopUp);
			}else{
				thisRoolUIElement.ReversePopUpDisableRecursively();
				ClearActivePopUp();
			}
		}
		void ClearActivePopUp(){
			thisActivePopUp = null;
		}
	}
}
