using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementFactory{
		IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller, Vector2 panelDim, Vector2 padding);
		IDigitPanel CreateDigitPanel();
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
			DigitPanelSet digitPanelSet = (DigitPanelSet)digitPanelSetAdaptor.GetUIElement();
			return digitPanelSet;
		}
	}
	public interface IEquipToolUIEFactory: IUIElementFactory{
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconAdaptor uia ,IEquippableUIItem eqpItem);
	}
	public class EquipToolUIEFactory:UIElementFactory, IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM): base(uim){
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
		}
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconAdaptor eqpIIUIA, IEquippableUIItem item){
			// UIImage image = CreateEquippableItemIconUIImage(item);
			// ItemIconPickUpImplementor iiPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM);
			// EqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			// ItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine();
			// DragImageImplementorConstArg dragImageImplementorConstArg = new DragImageImplementorConstArg(thisEqpIITAM.GetDragThreshold(), thisEqpIITAM.GetSmoothCoefficient(), thisProcessFactory, thisEqpIITAM);
			// IDragImageImplementor dragImageImplementor = new DragImageImplementor(dragImageImplementorConstArg);
			// IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(thisUIM, eqpIIUIA, image, thisEqpTool, dragImageImplementor, thisEqpIITAM, item, eqpIITAStateEngine, iiPickUpImplementor, emptinessStateEngine);
			// EquippableItemIcon eqpII = new EquippableItemIcon(arg);
			// return eqpII;
			IEquippableItemIconAdaptor eqpIIAdaptor = this.CreateUIA<EquippableItemIconAdaptor>();
		}
		UIImage CreateEquippableItemIconUIImage(IEquippableUIItem item){
			return null;
		}
	}
}

