using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolActivationData: IPickUpSystemUIAActivationData{
		IEquipTool eqpTool{get;}
		IEquippableIITAManager eqpIITAM{get;}
		IEquipToolUIEFactory eqpToolUIEFactory{get;}
	}
	public class EquipToolUIAActivationData: AbsPickUpSystemUIAActivationData, IEquipToolActivationData{
		public EquipToolUIAActivationData(IUIManager uim, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IEquipToolUIEFactory equipToolUIEFactory, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(uim, pickUpSystemProcessFactory, equipToolUIEFactory, eqpIITAM){
			thisEqpTool = eqpTool;
		}
		readonly IEquipTool thisEqpTool;
		public IEquipTool eqpTool{get{return thisEqpTool;}}
		public IEquippableIITAManager eqpIITAM{get{return (IEquippableIITAManager)pickUpManager;}}
		public IEquipToolUIEFactory eqpToolUIEFactory{get{return (IEquipToolUIEFactory)uiElementFactory;}}
	}
}
