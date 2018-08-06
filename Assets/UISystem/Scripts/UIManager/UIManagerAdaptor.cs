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

		public void Awake(){
			uiManager = new UIManager(uieReserveTrans);
			IUISystemProcessFactory processFactory = new UISystemProcessFactory(processManager, uiManager);
			IUIElementFactory uiElementFactory = new UIElementFactory(uiManager);
			IUIAActivationData rootUIAActivationArg = new RootUIAActivationData(uiManager, processFactory, uiElementFactory);
			rootUIAdaptor.GetReadyForActivation(rootUIAActivationArg);
		}
	}	
}
