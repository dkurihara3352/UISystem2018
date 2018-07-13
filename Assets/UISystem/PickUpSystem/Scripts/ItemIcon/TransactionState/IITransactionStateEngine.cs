using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IPickabilityStateHandler{
		void PickUp();
		void BecomePickable();
		void BecomeUnpickable();
		void Drop();
		bool IsPickable();
		bool IsPicked();
	}
	public interface IItemIconTransactionStateEngine: IPickabilityStateHandler, IHoverabilityStateHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public abstract class AbsIITransactionStateEngine: AbsSwitchableStateEngine<IIITransactionState>, IItemIconTransactionStateEngine{
		public void SetItemIcon(IItemIcon itemIcon){
			foreach(IIITransactionState state in thisStates){
				state.SetItemIcon(itemIcon);
			}
		}
		protected List<IIITransactionState> thisStates;
		protected IIIPickableState pickableState;
		protected IIIUnpickableState unpickableState;
		protected IIIPickedState pickedState;
		protected IIIHoverableState hoverableState;
		protected IIIUnhoverableState unhoverableState;
		protected IIIHoveredState hoveredState;
		protected IIIDroppedState droppedState;
		public bool IsPickable(){
			return thisCurState is IIIPickedState;
		}
		public bool IsPicked(){
			return thisCurState is IIIPickedState;
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
			return thisCurState is IIIHoverableState;
		}
		public bool IsHovered(){
			return thisCurState is IIIHoveredState;
		}
	}
	public interface IEqpIITransactionStateEngine: IItemIconTransactionStateEngine{}
	public class EqpIITransactionStateEngine: AbsIITransactionStateEngine, IEqpIITransactionStateEngine{
		public EqpIITransactionStateEngine(IEquippableIITAManager eqpIITAM, IEquipTool eqpTool){
			IEqpIITAStateConstArg arg = new EqpIITAStateConstArg(eqpIITAM, eqpTool);
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
			thisStates.Add(pickableState);
			thisStates.Add(unpickableState);
			thisStates.Add(pickedState);
			thisStates.Add(hoverableState);
			thisStates.Add(unhoverableState);
			thisStates.Add(hoveredState);
			thisStates.Add(droppedState);
		}
	}
}
