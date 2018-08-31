using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpManager{
		void SetRootUIElement(IUIElement rootUIElement);
		void RegisterPopUp(IPopUp popUpToRegister);
		void UnregisterPopUp(IPopUp popUpToUnregister);
		void CheckAndHideActivePopUp();
		bool ActivePopUpHidesOnTappingOthers();
	}
	public class PopUpManager : IPopUpManager {

		public void SetRootUIElement(IUIElement rootUIElement){
			thisRootUIElement = rootUIElement;
		}
		IUIElement thisRootUIElement;

		public void RegisterPopUp(IPopUp popUpToRegister){
			popUpToRegister.ShowHiddenProximateParentPopUpRecursively();
			DisableOthers(popUpToRegister);
			SetActivePopUp(popUpToRegister);
		}
		protected IPopUp thisActivePopUp;
		protected void SetActivePopUp(IPopUp popUp){
			thisActivePopUp = popUp;
		}
		public bool ActivePopUpHidesOnTappingOthers(){
			return thisActivePopUp.HidesOnTappingOthers();
		}
		public void CheckAndHideActivePopUp(){
			if(thisActivePopUp!= null){
				if(thisActivePopUp.HidesOnTappingOthers())
					thisActivePopUp.Hide(false);
			}
		}
		void HideActivePopUp(){
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
