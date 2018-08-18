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
		public UIAdaptor turnColorUIA;
		IUIElement thisRootUIElement;
		public bool showsInputability;
		
		void Awake(){
			uiManager = new UIManager(uieReserveTrans, showsInputability);
			uieFactory = new UIElementFactory(uiManager);
			processFactory = new UISystemProcessFactory(processManager, uiManager);
		}
		public void GetRootUIAReadyForActivation(){
			IUIAActivationData activationData = new RootUIAActivationData(uiManager, processFactory, uieFactory);
			rootUIAdaptor.GetReadyForActivation(activationData);
			thisRootUIElement = rootUIAdaptor.GetUIElement();
			thisRootUIElement.CallOnUIReferenceSetRecursively();
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
	}
}

