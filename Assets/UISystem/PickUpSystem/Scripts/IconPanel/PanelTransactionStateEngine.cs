using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IPanelTransactionStateEngine: ISwitchableStateEngine<IPanelTransactionState>, IHoverabilityStateHandler{}
	public class EquipToolPanelTransactionStateEngine: AbsSwitchableStateEngine<IPanelTransactionState>, IPanelTransactionStateEngine{
		public EquipToolPanelTransactionStateEngine(IEquippableIITAManager eqpIITAM, IEquipToolPanel panel, IEquipTool tool){
			IEquipToolPanelTransactionStateConstArg arg = new EquipToolPanelTransactionStateConstArg(eqpIITAM, panel, tool);
			thisWaitingForPickUpState = new EquipToolPanelWaitingForPickUpState(arg);
			thisHhoverableState = new EquipToolPanelHoverableState(arg);
			thisUnhoverableState = new EquipToolPanelUnhoverableState(arg);
			thisHoveredState = new EquipToolPanelHoveredState(arg);
			thisEqpIITAM = eqpIITAM;
		}
		readonly IEquipToolPanelWaitingForPickUpState thisWaitingForPickUpState;
		readonly IEquipToolPanelHoverableState thisHhoverableState;
		readonly IEquipToolPanelUnhoverableState thisUnhoverableState;
		readonly IEquipToolPanelHoveredState thisHoveredState;
		readonly IEquippableIITAManager thisEqpIITAM;
		public void WaitForPickUp(){
			TrySwitchState(thisWaitingForPickUpState);
		}
		public void BecomeHoverable(){
			TrySwitchState(thisHhoverableState);
		}
		public void BecomeUnhoverable(){
			TrySwitchState(thisUnhoverableState);
		}
		public void BecomeHovered(){
			thisHoveredState.SetPickedEquippableII(thisEqpIITAM.GetPickedEqpII());
			TrySwitchState(thisHoveredState);
		}
		public bool IsHoverable(){
			return thisCurState is IPanelHoverableState;
		}
		public bool IsHovered(){
			return thisCurState is IPanelHoveredState;
		}
	}
}
