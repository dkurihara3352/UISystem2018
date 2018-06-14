using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPanelTransactionStateEngine<T>: ISwitchableStateEngine<T>, IHoverabilityStateHandler where T: IPanelTransactionState{}
	public class EquipToolPanelTransactionStateEngine: AbsSwitchableStateEngine<IEquipToolPanelTransactionState>, IPanelTransactionStateEngine<IEquipToolPanelTransactionState>{
		public EquipToolPanelTransactionStateEngine(IEquippableIITAManager eqpIITAM, IEquipToolPanel panel, IEquipTool tool){
			IEquipToolPanelTransactionStateConstArg arg = new EquipToolPanelTransactionStateConstArg(eqpIITAM, panel, tool);
			this.waitingForPickUpState = new EquipToolPanelWaitingForPickUpState(arg);
			this.hoverableState = new EquipToolPanelHoverableState(arg);
			this.unhoverableState = new EquipToolPanelUnhoverableState(arg);
			this.hoveredState = new EquipToolPanelHoveredState(arg);
			this.eqpIITAM = eqpIITAM;
		}
		readonly IEquipToolPanelWaitingForPickUpState waitingForPickUpState;
		readonly IEquipToolPanelHoverableState hoverableState;
		readonly IEquipToolPanelUnhoverableState unhoverableState;
		readonly IEquipToolPanelHoveredState hoveredState;
		readonly IEquippableIITAManager eqpIITAM;
		public void WaitForPickUp(){
			TrySwitchState(waitingForPickUpState);
		}
		public void BecomeHoverable(){
			TrySwitchState(hoverableState);
		}
		public void BecomeUnhoverable(){
			TrySwitchState(unhoverableState);
		}
		public void BecomeHovered(){
			hoveredState.SetPickedEquippableII(eqpIITAM.GetPickedEqpII());
			TrySwitchState(hoveredState);
		}
		public bool IsHoverable(){
			return curState is IPanelHoverableState;
		}
		public bool IsHovered(){
			return curState is IPanelHoveredState;
		}
	}
}
