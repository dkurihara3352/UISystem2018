using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class TestUIManagerAdaptor: MonoBehaviour{
		IUIManager uiManager;
		ITestUIElementFactory testUIElementFactory;
		IUISystemProcessFactory processFactory;
		public RectTransform uieReserveTrans;
		public ProcessManager processManager;
		public Font font;
		public int fontSize;
		public Color imageColor;
		public GenericUIElementGroupAdaptor uiElementGroupAdaptor;
		IUIElementGroup uiElementGroup;
		List<IUIElement> thisUIEs;
		void Awake(){
			uiManager = new UIManager(uieReserveTrans);
			testUIElementFactory = new TestUIElementFactory(uiManager, font, fontSize, imageColor);
			processFactory = new UISystemProcessFactory(processManager, uiManager);
			sizeDelta = uiElementGroupAdaptor.elementLength;

			Vector2 buttonPos = new Vector2(10f, 10f);
			Vector2 buttonLength = new Vector2(100f, 20f);
			createButtonRect = new Rect(buttonPos, buttonLength);
			readyForActivBurronRect = new Rect(buttonPos + new Vector2(0f, 20f), buttonLength);
			setUpEleButtonRect = new Rect(buttonPos + new Vector2(0f, 40f), buttonLength);
		}
		public int elementCount;
		Vector2 sizeDelta;
		Rect createButtonRect;
		Rect readyForActivBurronRect;
		Rect setUpEleButtonRect;
		void OnGUI(){
			if(GUI.Button(createButtonRect, "Create")){
				thisUIEs = CreateUIEs();
			}
			if(GUI.Button(readyForActivBurronRect, "GetReadyForA")){
				IUIAActivationData activationData = new RootUIAActivationData(uiManager, processFactory, testUIElementFactory);
				uiElementGroupAdaptor.GetReadyForActivation(activationData);
				uiElementGroup = uiElementGroupAdaptor.GetUIElement() as IUIElementGroup;
			}
			if(GUI.Button(setUpEleButtonRect, "SetUpElements"))
				uiElementGroup.SetUpElements(thisUIEs);
		}
		List<IUIElement> CreateUIEs(){
			List<IUIElement> result = new List<IUIElement>();
			for(int i = 0; i < elementCount; i ++){
				result.Add(testUIElementFactory.CreateUIElementWithIndexText(i, sizeDelta, processFactory));
			}
			return result;
		}
	}
}

