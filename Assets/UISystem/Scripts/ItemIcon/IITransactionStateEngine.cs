using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickabilityStateHandler{
		void PickUp();
		void BecomePickable();
		void BecomeUnpickable();
		void Drop();
		bool IsPickable();
		bool IsPicked();
	}
	public interface IIITransactionStateEngine: IPickabilityStateHandler, IHoverabilityStateHandler{}
	public abstract class AbsIITransactionStateEngine: AbsSwitchableStateEngine<IIITransactionState>, IIITransactionStateEngine{
		protected IIIPickableState pickableState;
		protected IIIUnpickableState unpickableState;
		protected IIIPickedState pickedState;
		protected IIIHoverableState hoverableState;
		protected IIIUnhoverableState unhoverableState;
		protected IIIHoveredState hoveredState;
		protected IIIDroppedState droppedState;
		public bool IsPickable(){
			return curState is IIIPickedState;
		}
		public bool IsPicked(){
			return curState is IIIPickedState;
		}
		public void PickUp(){
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
		public void Drop(){
			TrySwitchState(droppedState);
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
	public interface IEqpIITransactionStateEngine: IIITransactionStateEngine{}
	public class EqpIITransactionStateEngine: AbsIITransactionStateEngine, IEqpIITransactionStateEngine{
		public EqpIITransactionStateEngine(IEquippableItemIcon eqpII, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool){
			IEqpIITAStateConstArg arg = new EqpIITAStateConstArg(eqpII, eqpIITAM, eqpTool);
			InitializeStates(arg);
		}
		void InitializeStates(IEqpIITAStateConstArg arg){
			this.pickableState = new EqpIIPickableState(arg);
			this.unpickableState = new EqpIIUnpickableState(arg);
			this.pickedState = new EqpIIPickedState(arg);
			this.hoverableState = new EqpIIHoverableState(arg);
			this.unhoverableState = new EqpIIUnhoverableState(arg);
			this.hoveredState = new EqpIIHoveredState(arg);
			this.droppedState = new EqpIIDroppedState(arg);
		}
	}
}
