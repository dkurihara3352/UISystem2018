using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class UIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		public ProcessManager processManager;
		public UIAdaptor rootUIAdaptor;/* assigned in inspector*/
		public RectTransform uieReserveTrans;
		public bool showsInputability;

		public void Awake(){
			uiManager = new UIManager(uieReserveTrans, showsInputability);
			IUISystemProcessFactory processFactory = new UISystemProcessFactory(processManager, uiManager);
			IUIElementFactory uiElementFactory = new UIElementFactory(uiManager);
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, processFactory, uiElementFactory);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
			IUIElement rootUIElement = rootUIAdaptor.GetUIElement();
			uiManager.SetRootUIElement(rootUIElement);
		}
	}	
}
