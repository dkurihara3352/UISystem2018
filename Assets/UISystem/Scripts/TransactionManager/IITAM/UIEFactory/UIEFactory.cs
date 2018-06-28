using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElementFactory{
		IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller);
		IDigitPanel CreateDigitPanel();
	}
	public class UIElementFactory: IUIElementFactory{
		public UIElementFactory(IUIManager uim){
			thisUIM = uim;
			thisReserveTransformUIE = uim.GetReserveTransformUIE();
		}
		readonly IUIManager thisUIM;
		IReserveTransformUIE thisReserveTransformUIE;
		T CreateUIA<T>() where T: MonoBehaviour, IUIAdaptor{
			GameObject go = new GameObject();
			IUIAdaptor reserveUIA = thisReserveTransformUIE.GetUIAdaptor();
			go.transform.SetParent(reserveUIA.GetTransform());
			go.transform.position = thisReserveTransformUIE.GetReservePosition();
			go.transform.SetAsLastSibling();
			T uia = go.AddComponent<T>();
			return uia;
		}
		public IDigitPanelSet CreateDigitPanelSet(int digitPlace, IQuantityRoller quantityRoller){
			DigitPanelSetAdaptor digitPanelSetAdaptor = CreateUIA<DigitPanelSetAdaptor>();
			digitPanelSetAdaptor.SetDigitPlace(digitPlace);
			IUIAdaptor quantityRollerAdaptor = quantityRoller.GetUIAdaptor();
			digitPanelSetAdaptor.SetParentUIA(quantityRollerAdaptor, true);
			IUIAActivationData activationData = quantityRollerAdaptor.GetDomainActivationData();
			digitPanelSetAdaptor.GetReadyForActivation(activationData);
			DigitPanelSet digitPanelSet = (DigitPanelSet)digitPanelSetAdaptor.GetUIElement();
			return digitPanelSet;
		}
	}
	public interface IEquipToolUIEFactory: IUIElementFactory{
		IEquipToolUIE CreateEquipToolUIE(IEquipToolUIAdaptor uia);
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA uia ,IEquippableUIItem eqpItem);
	}
	public class EquipToolUIEFactory: IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM){
			thisUIM = uim;
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
			thisProcessFactory = uim.GetProcessFactory();
		}
		readonly IUIManager thisUIM;
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		readonly IProcessFactory thisProcessFactory;
		public IEquipToolUIE CreateEquipToolUIE(IEquipToolUIAdaptor uia){
			IUIImage image = CreateEquipToolUIImage();
			IUIElementConstArg arg = new UIElementConstArg(thisUIM, uia, image);
			EquipToolUIE uie = new EquipToolUIE(arg);
			return uie;
		}
		IUIImage CreateEquipToolUIImage(){
			return null;
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableItemIconUIA eqpIIUIA, IEquippableUIItem item){
			UIImage image = CreateEquippableItemIconUIImage(item);
			ItemIconPickUpImplementor iiPickUpImplementor = new ItemIconPickUpImplementor(thisEqpIITAM);
			EqpIITransactionStateEngine eqpIITAStateEngine = new EqpIITransactionStateEngine(thisEqpIITAM, thisEqpTool);
			ItemIconEmptinessStateEngine emptinessStateEngine = new ItemIconEmptinessStateEngine();
			DragImageImplementorConstArg dragImageImplementorConstArg = new DragImageImplementorConstArg(thisEqpIITAM.GetDragThreshold(), thisEqpIITAM.GetSmoothCoefficient(), thisProcessFactory, thisEqpIITAM);
			IDragImageImplementor dragImageImplementor = new DragImageImplementor(dragImageImplementorConstArg);
			IEquippableItemIconConstArg arg = new EquippableItemIconConstArg(thisUIM, eqpIIUIA, image, dragImageImplementor, thisEqpIITAM, item, eqpIITAStateEngine, iiPickUpImplementor, emptinessStateEngine, thisEqpTool);
			EquippableItemIcon eqpII = new EquippableItemIcon(arg);
			return eqpII;
		}
		UIImage CreateEquippableItemIconUIImage(IEquippableUIItem item){
			return null;
		}
	}
}

