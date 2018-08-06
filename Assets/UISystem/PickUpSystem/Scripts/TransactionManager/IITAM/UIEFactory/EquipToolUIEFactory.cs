using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolUIEFactory: IUIElementFactory, IPickUpSystemUIElementFactory{
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem eqpItem);
		IQuantityRoller CreateItemIconQuantityRoller(IItemIconUIAdaptor itemIconAdaptor);
	}
	public class EquipToolUIEFactory: UIElementFactory, IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM): base(uim){
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
			thisPickUpSystemProcessFactory = pickUpSystemProcessFactory;
		}
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		readonly IPickUpSystemProcessFactory thisPickUpSystemProcessFactory;
		public IItemIcon CreateItemIcon(IUIItem item){
			// if(item is IEquippableUIItem)
			// 	return CreateEquippableItemIcon((IEquippableUIItem)item);
			// else
			// 	throw new System.InvalidOperationException("item must be of type IEquippableUIItem");
			return null;
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem item){
			// IEquippableItemIconAdaptorInitializationData uiaInitData = new EquippableItemIconAdaptorInitializationData(item);
			// IEquippableItemIconAdaptor eqpIIAdaptor = this.CreateInstatiableUIA<EquippableItemIconAdaptor>(uiaInitData);
			// IEquipToolActivationData activationData = new EquipToolUIAActivationData(thisUIM, thisPickUpSystemProcessFactory, this, thisEqpIITAM, thisEqpTool);
			// eqpIIAdaptor.GetReadyForActivation(activationData);
			// IEquippableItemIcon eqpItemIcon = (IEquippableItemIcon)eqpIIAdaptor.GetUIElement();
			// return eqpItemIcon;
			return null;
		}
		public IQuantityRoller CreateItemIconQuantityRoller(IItemIconUIAdaptor itemIconUIAdaptor){
			// Rect itemIconRect = itemIconUIAdaptor.GetRect();
			// int maxQuantity = 99;
			// Vector2 panelDimension = new Vector2(itemIconRect.width * .15f, itemIconRect.height * .2f);
			// Vector2 padding = new Vector2(itemIconRect.width * .05f, itemIconRect.height * .05f);
			// Vector2 normalizedPos = new Vector2(1f, 1f);
			// IQuantityRollerAdaptorInitializationData uiaInitData = new QuantityRollerAdaptorInitializationData(maxQuantity, panelDimension, padding, normalizedPos);
			// IQuantityRollerAdaptor quantityRollerAdaptor = this.CreateInstatiableUIA<QuantityRollerAdaptor>(uiaInitData);
			// quantityRollerAdaptor.GetTransform().SetParent(itemIconUIAdaptor.GetTransform());
			// IUIAActivationData activationData = itemIconUIAdaptor.GetDomainActivationData();

			// quantityRollerAdaptor.GetReadyForActivation(activationData);
			// IQuantityRoller quantityRoller = (IQuantityRoller)quantityRollerAdaptor.GetUIElement();
			// return quantityRoller;
			return null;
		}
	}
}
