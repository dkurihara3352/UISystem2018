using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class TestUIManagerAdaptor: UIManagerAdaptor{

		public PopUpAdaptor popUpAdaptor;
		IUIElement thisRootUIElement;
	
		
		// public void GetRootUIAReadyForActivation(){
		// 	IUIElementBaseConstData activationData = new RootUIAActivationData(uiManager, processFactory, uieFactory);
		// 	rootUIAdaptor.GetReadyForActivation(activationData);
		// 	thisRootUIElement = rootUIAdaptor.GetUIElement();
		// 	thisRootUIElement.CallOnUIReferenceSetRecursively();
		// 	uiManager.SetRootUIElement(thisRootUIElement);
		// }
		public void ActivateRootUIElement(){
			thisRootUIElement.InitiateActivation(false);
		}
		public void DeactivateRootUIElement(){
			thisRootUIElement.DeactivateRecursively(false);
		}
		public void ActivateRootUIElementInstantly(){
			thisRootUIElement.InitiateActivation(true);
		}
		public void DeactivateRootUIElementInstantly(){
			thisRootUIElement.DeactivateRecursively(true);
		}
		public void TogglePopUp(){
			IPopUp popUp = (IPopUp)popUpAdaptor.GetUIElement();
			if(popUp.IsShown())
				popUp.Hide(false);
			else
				popUp.Show(true);
		}
	}
}

