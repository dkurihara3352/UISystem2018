using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class TestUIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		IUIElementFactory uieFactory;
		IUISystemProcessFactory processFactory;
		public RectTransform uieReserveTrans;
		public ProcessManager processManager;
		public UIAdaptor rootUIAdaptor;
		public UIAdaptor turnColorUIA;
		
		void Awake(){
			uiManager = new UIManager(uieReserveTrans);
			uieFactory = new UIElementFactory(uiManager);
			processFactory = new UISystemProcessFactory(processManager, uiManager);
		}
		public void GetRootUIAReadyForActivation(){
			IUIAActivationData activationData = new RootUIAActivationData(uiManager, processFactory, uieFactory);
			rootUIAdaptor.GetReadyForActivation(activationData);
		}
		public void ActivateRootUIElement(){
			rootUIAdaptor.ActivateUIElement();
		}
		public void DeactivateRootUIElement(){
			rootUIAdaptor.DeactivateUIElement();
		}
		public void ActivateRootUIElementInstantly(){
			rootUIAdaptor.ActivateUIElementInstantly();
		}
		public void DeactivateRootUIElementInstantly(){
			rootUIAdaptor.DeactivateUIElementInstantly();
		}
		public void TurnTargetRed(){
			IUIElement uie = turnColorUIA.GetUIElement();
			IUIImage image = uie.GetUIImage();
			image.FlashRed();
		}
		public void TurnTargetColorToOriginal(){
			IUIElement uie = turnColorUIA.GetUIElement();
			IUIImage image = uie.GetUIImage();
			image.TurnToOriginalColor();
		}
	}
}

