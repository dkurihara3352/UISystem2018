using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class TestUIManagerAdaptor: MonoBehaviour{
		public IUIManager uiManager;
		IUIElementFactory uieFactory;
		IUISystemProcessFactory processFactory;
		public RectTransform uieReserveTrans;
		public ProcessManager processManager;
		public UIAdaptor rootUIAdaptor;
		public PopUpAdaptor popUpAdaptor;
		IUIElement thisRootUIElement;
		public bool showsInputability;
		
		void Awake(){
			uiManager = new UIManager(
				uieReserveTrans, 
				showsInputability
			);
			processFactory = new UISystemProcessFactory(
				processManager, 
				uiManager
			);
			uieFactory = new UIElementFactory(
				uiManager
			);
		}
		public void GetRootUIAReadyForActivation(){
			IUIAActivationData activationData = new RootUIAActivationData(uiManager, processFactory, uieFactory);
			rootUIAdaptor.GetReadyForActivation(activationData);
			thisRootUIElement = rootUIAdaptor.GetUIElement();
			thisRootUIElement.CallOnUIReferenceSetRecursively();
			uiManager.SetRootUIElement(thisRootUIElement);
		}
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

