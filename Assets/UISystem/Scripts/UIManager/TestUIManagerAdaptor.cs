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
		public UIAdaptor roolUIAdaptor;
		
		void Awake(){
			uiManager = new UIManager(uieReserveTrans);
			uieFactory = new UIElementFactory(uiManager);
			processFactory = new UISystemProcessFactory(processManager, uiManager);

			Vector2 buttonPos = new Vector2(10f, 10f);
			Vector2 buttonLength = new Vector2(100f, 20f);
			readyForActivBurronRect = new Rect(buttonPos, buttonLength);
			activationButtonRect = new Rect(buttonPos + new Vector2(0f, 20f), buttonLength);
		}
		Rect readyForActivBurronRect;
		Rect activationButtonRect;
		void OnGUI(){
			if(GUI.Button(readyForActivBurronRect, "GetReadyForA")){
				IUIAActivationData activationData = new RootUIAActivationData(uiManager, processFactory, uieFactory);
				roolUIAdaptor.GetReadyForActivation(activationData);
			}
			if(GUI.Button(activationButtonRect, "Activate"))
				roolUIAdaptor.ActivateUIElement();
		}
	}
}

