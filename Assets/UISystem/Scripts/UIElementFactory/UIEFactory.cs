using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementFactory{
		IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller, Vector2 panelDim, Vector2 padding);
		IDigitPanel CreateDigitPanel(IDigitPanelSet parentDigitPanelSet, Vector2 panelDim, float localPosY);
	}
	public class UIElementFactory: IUIElementFactory{
		public UIElementFactory(IUIManager uim){
			thisUIM = uim;
			reserveTrans = uim.GetUIElementReserveTrans();
		}
		protected readonly IUIManager thisUIM;
		protected readonly RectTransform reserveTrans;
		protected T CreateInstatiableUIA<T>(IInstantiableUIAdaptorInstantiationData instatiationData) where T: MonoBehaviour, IInstatiableUIAdaptor{
			GameObject go = new GameObject("uiaGO");
			RectTransform rectTrans = CreateRectTransform(
				go, 
				reserveTrans
			);
			go.AddComponent<CanvasRenderer>();
			go.transform.SetAsLastSibling();
			T uia = go.AddComponent<T>();
			uia.SetInitializationFields(instatiationData.initializationData);
			return uia;
		}
		RectTransform CreateRectTransform(
			GameObject gameObject, 
			RectTransform parentRT
		){
			RectTransform rectTrans = gameObject.AddComponent<RectTransform>();
			rectTrans.SetParent(parentRT);
			rectTrans.pivot = new Vector2(0f, 0f);
			rectTrans.anchorMin = Vector2.zero;
			rectTrans.anchorMax = Vector2.zero;
			rectTrans.anchoredPosition = new Vector2(0f, 0f);
			return rectTrans;
		}
		public IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller, Vector2 panelDim, Vector2 padding){
			IDigitPanelSetAdaptorInitializationData uiaInitData = new DigitPanelSetAdaptorInitializationData(
				digitPlace, 
				panelDim, 
				padding
			);
			float panelSetWidth = panelDim.x;
			float panelSetHeight = panelDim.y * 2 + padding.y;
			Vector2 panelSetLength = new Vector2(panelSetWidth, panelSetHeight);
			IDigitPanelSetInstantiationData instData = new DigitPanelSetInstantiationData(
				panelSetLength, 
				uiaInitData
			);
			DigitPanelSetAdaptor digitPanelSetAdaptor = CreateInstatiableUIA<DigitPanelSetAdaptor>(instData);
			IUIAdaptor quantityRollerAdaptor = quantityRoller.GetUIAdaptor();
			digitPanelSetAdaptor.SetParentUIA(quantityRollerAdaptor, true);
			IUIAdaptorBaseInitializationData baseInitData = quantityRollerAdaptor.GetDomainInitializationData();
			digitPanelSetAdaptor.GetReadyForActivation(
				baseInitData, 
				false
			);
			IDigitPanelSet digitPanelSet = (IDigitPanelSet)digitPanelSetAdaptor.GetUIElement();
			return digitPanelSet;
		}
		public IDigitPanel CreateDigitPanel(IDigitPanelSet parentDigitPanelSet, Vector2 panelDim, float localPosY){
			IDigitPanelAdaptorInitializationData uiaInitData = new DigitPanelAdaptorInitializationData(panelDim, localPosY);
			IDigitPanelInstantiationData instData = new DigitPanelInstantiationData(panelDim, uiaInitData);
			DigitPanelAdaptor digitPanelAdaptor = CreateInstatiableUIA<DigitPanelAdaptor>(instData);
			IUIAdaptor parentUIA = parentDigitPanelSet.GetUIAdaptor();
			digitPanelAdaptor.SetParentUIA(parentUIA, true);
			IUIAdaptorBaseInitializationData baseInitData = parentUIA.GetDomainInitializationData();
			digitPanelAdaptor.GetReadyForActivation(
				baseInitData, 
				false
			);
			IDigitPanel digitPanel = (IDigitPanel)digitPanelAdaptor.GetUIElement();
			return digitPanel;
		}
	}
}

