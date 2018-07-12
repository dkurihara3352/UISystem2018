using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolUIEFactory: IUIElementFactory, IPickUpSystemUIElementFactory{
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem eqpItem);
	}
	public class EquipToolUIEFactory:UIElementFactory, IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM): base(uim){
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
			thisPickUpSystemProcessFactory = pickUpSystemProcessFactory;
		}
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		readonly IPickUpSystemProcessFactory thisPickUpSystemProcessFactory;
		public IItemIcon CreateItemIcon(IUIItem item){
			if(item is IEquippableUIItem)
				return CreateEquippableItemIcon((IEquippableUIItem)item);
			else
				throw new System.InvalidOperationException("item must be of type IEquippableUIItem");
		}
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem item){
			IEquippableItemIconAdaptor eqpIIAdaptor = this.CreateUIA<EquippableItemIconAdaptor>();
			eqpIIAdaptor.SetInitializationFields(item);
			IEquipToolActivationData activationData = new EquipToolUIAActivationData(thisUIM, thisPickUpSystemProcessFactory, this, thisEqpIITAM, thisEqpTool);
			eqpIIAdaptor.GetReadyForActivation(activationData);
			IEquippableItemIcon eqpItemIcon = (IEquippableItemIcon)eqpIIAdaptor.GetUIElement();
			return eqpItemIcon;
		}
	}
}
