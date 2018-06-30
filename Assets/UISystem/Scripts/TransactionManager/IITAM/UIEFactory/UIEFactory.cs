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
		IReserveTransformUIE thisReserveTransformUIE;
		protected T CreateUIA<T>() where T: MonoBehaviour, IUIAdaptor{
			GameObject go = new GameObject();
			IUIAdaptor reserveUIA = thisReserveTransformUIE.GetUIAdaptor();
			go.transform.SetParent(reserveUIA.GetTransform());
			go.transform.position = thisReserveTransformUIE.GetReservePosition();
			go.transform.SetAsLastSibling();
			T uia = go.AddComponent<T>();
			return uia;
		}
		public IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller, Vector2 panelDim, Vector2 padding){
			DigitPanelSetAdaptor digitPanelSetAdaptor = CreateUIA<DigitPanelSetAdaptor>();
			digitPanelSetAdaptor.SetInitializationFields(digitPlace, panelDim, padding);
			IUIAdaptor quantityRollerAdaptor = quantityRoller.GetUIAdaptor();
			digitPanelSetAdaptor.SetParentUIA(quantityRollerAdaptor, true);
			IUIAActivationData activationData = quantityRollerAdaptor.GetDomainActivationData();
			digitPanelSetAdaptor.GetReadyForActivation(activationData);
			IDigitPanelSet digitPanelSet = (IDigitPanelSet)digitPanelSetAdaptor.GetUIElement();
			return digitPanelSet;
		}
		public IDigitPanel CreateDigitPanel(IDigitPanelSet parentDigitPanelSet, Vector2 panelDim, float localPosY){
			DigitPanelAdaptor digitPanelAdaptor = CreateUIA<DigitPanelAdaptor>();
			digitPanelAdaptor.SetInitializationFields(panelDim, localPosY);
			IUIAdaptor parentUIA = parentDigitPanelSet.GetUIAdaptor();
			digitPanelAdaptor.SetParentUIA(parentUIA, true);
			IUIAActivationData activationData = parentUIA.GetDomainActivationData();
			digitPanelAdaptor.GetReadyForActivation(activationData);
			IDigitPanel digitPanel = (IDigitPanel)digitPanelAdaptor.GetUIElement();
			return digitPanel;
		}
	}
}

