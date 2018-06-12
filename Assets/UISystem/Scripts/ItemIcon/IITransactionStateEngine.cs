using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITransactionStateEngine: IPickabilityStateHandler, IHoverabilityStateHandler{}
	public class IITransactionStateEngine: AbsSwitchableStateEngine<IIITransactionState>, IIITransactionStateEngine{
		readonly IItemIcon itemIcon;
		readonly IIIPickedState pickedState;
		readonly IIIPickableState pickableState;
		readonly IIIUnpickableState unpickableState;
		readonly IIIHoverableState hoverableState;
		readonly IIIUnhoverableState unhoverableState;
		readonly IIIHoveredState hoveredState;
		public bool IsPickable(){
			return curState is IIIPickedState;
		}
		public bool IsPicked(){
			return curState is IIIPickedState;
		}
		public void BecomePicked(){
			if(this.IsPickable())
				TrySwitchState(pickedState);
			else
				throw new System.InvalidOperationException("should not be picked while not pickable");
		}
		public void BecomePickable(){
			TrySwitchState(pickableState);
		}
		public void BecomeUnpickable(){
			TrySwitchState(unpickableState);
		}
		public void WaitForPickUp(){
			return;
		}
		public void BecomeHoverable(){
			TrySwitchState(hoverableState);
		}
		public void BecomeUnhoverable(){
			TrySwitchState(unhoverableState);
		}
		public void BecomeHovered(){
			TrySwitchState(hoveredState);
		}
		public bool IsHoverable(){
			return curState is IIIHoverableState;
		}
		public bool IsHovered(){
			return curState is IIIHoveredState;
		}
	}
}
