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
			thisReserveTransformUIE = uim.GetReserveTransformUIE();
		}
		protected readonly IUIManager thisUIM;
		readonly IUIElement thisReserveTransformUIE;
		protected T CreateInstatiableUIA<T>(IUIAInitializationData uiaInitializationData) where T: MonoBehaviour, IInstatiableUIAdaptor{
			GameObject go = new GameObject();
			IUIAdaptor reserveUIA = thisReserveTransformUIE.GetUIAdaptor();
			go.transform.SetParent(reserveUIA.GetTransform());
			go.transform.position = reserveUIA.GetLocalPosition();
			go.transform.SetAsLastSibling();
			T uia = go.AddComponent<T>();
			uia.SetInitializationFields(uiaInitializationData);
			return uia;
		}
		public IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller, Vector2 panelDim, Vector2 padding){
			IDigitPanelSetAdaptorInitializationData uiaInitData = new DigitPanelSetAdaptorInitializationData(digitPlace, panelDim, padding);
			DigitPanelSetAdaptor digitPanelSetAdaptor = CreateInstatiableUIA<DigitPanelSetAdaptor>(uiaInitData);
			IUIAdaptor quantityRollerAdaptor = quantityRoller.GetUIAdaptor();
			digitPanelSetAdaptor.SetParentUIA(quantityRollerAdaptor, true);
			IUIAActivationData activationData = quantityRollerAdaptor.GetDomainActivationData();
			digitPanelSetAdaptor.GetReadyForActivation(activationData);
			IDigitPanelSet digitPanelSet = (IDigitPanelSet)digitPanelSetAdaptor.GetUIElement();
			return digitPanelSet;
		}
		public IDigitPanel CreateDigitPanel(IDigitPanelSet parentDigitPanelSet, Vector2 panelDim, float localPosY){
			IDigitPanelAdaptorInitializationData uiaInitData = new DigitPanelAdaptorInitializationData(panelDim, localPosY);
			DigitPanelAdaptor digitPanelAdaptor = CreateInstatiableUIA<DigitPanelAdaptor>(uiaInitData);
			IUIAdaptor parentUIA = parentDigitPanelSet.GetUIAdaptor();
			digitPanelAdaptor.SetParentUIA(parentUIA, true);
			IUIAActivationData activationData = parentUIA.GetDomainActivationData();
			digitPanelAdaptor.GetReadyForActivation(activationData);
			IDigitPanel digitPanel = (IDigitPanel)digitPanelAdaptor.GetUIElement();
			return digitPanel;
		}
	}
}

