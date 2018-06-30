using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolUIEFactory: IUIElementFactory{
		IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem eqpItem);
	}
	public class EquipToolUIEFactory:UIElementFactory, IEquipToolUIEFactory{
		public EquipToolUIEFactory(IUIManager uim, IEquipTool eqpTool, IEquippableIITAManager eqpIITAM): base(uim){
			thisEqpTool = eqpTool;
			thisEqpIITAM = eqpIITAM;
		}
		readonly IEquipTool thisEqpTool;
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableItemIcon CreateEquippableItemIcon(IEquippableUIItem item){
			IEquippableItemIconAdaptor eqpIIAdaptor = this.CreateUIA<EquippableItemIconAdaptor>();
			eqpIIAdaptor.SetInitializationFields(item);
			IEquipToolActivationData activationData = new EquipToolUIAActivationData(thisUIM, thisEqpIITAM, thisEqpTool);
			eqpIIAdaptor.GetReadyForActivation(activationData);
			IEquippableItemIcon eqpItemIcon = (IEquippableItemIcon)eqpIIAdaptor.GetUIElement();
			return eqpItemIcon;
		}
	}
}
