using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconEmptinessState: ISwitchableState{
		void SetItemIcon(IItemIcon itemIcon);
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
		void SetItem(IUIItem item);
	}
	public abstract class AbsItemIconEmptinessState: IItemIconEmptinessState{
		protected IItemIcon thisItemIcon;
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		public abstract void IncreaseBy(int quantity, bool doesIncrement);
		public abstract void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
		public abstract void SetItem(IUIItem item);
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	/* NonemptyState */
	public interface IItemIconNonemptyState: IItemIconEmptinessState{}
	public abstract class AbsItemIconNonemptyState: AbsItemIconEmptinessState, IItemIconNonemptyState{
		public override void IncreaseBy(int quantity, bool doesIncrement){
			int sourceQuantity = thisItemIcon.GetItemQuantity();
			int targetQuantity = sourceQuantity + quantity;
			thisItemIcon.UpdateQuantity(sourceQuantity, targetQuantity, doesIncrement);
			if(thisItemIcon.IsGhostified())
				thisItemIcon.Deghostify();
		}
		public override void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			int sourceQuantity = thisItemIcon.GetItemQuantity();
			int targetQuantity = sourceQuantity - quantity;
			thisItemIcon.UpdateQuantity(sourceQuantity, targetQuantity, doesIncrement);
			this.CheckRemoval(removesEmpty);
		}
		protected abstract void CheckRemoval(bool removesEmpty);
	}
	public interface IWaitingForImageInitState: IItemIconNonemptyState{
	}
	public class WaitingForImageInitState: AbsItemIconNonemptyState, IWaitingForImageInitState{
		protected override void CheckRemoval(bool removesEmpty){
			if(thisItemIcon.GetItemQuantity() == 0){
				if(thisItemIcon.LeavesGhost())
					return;
				else{
					thisItemIcon.EmptifyInstantly();
					if(removesEmpty)
						thisItemIcon.RemoveAndMutate();
				}
			}
		}
		public override void OnEnter(){}
		public override void OnExit(){}
		public override void SetItem(IUIItem item){}
	}
	public interface IDisemptifyingState: IItemIconNonemptyState{}
	public class DisemptifyingState: AbsItemIconNonemptyState, IDisemptifyingState{
		protected override void CheckRemoval(bool removesEmpty){
			if(thisItemIcon.GetItemQuantity() == 0){
				if(thisItemIcon.LeavesGhost())
					thisItemIcon.Ghostify();
				else{
					if(removesEmpty)
						thisItemIcon.EmptifyAndRemove();
					else
						thisItemIcon.Emptify();
				}

			}
		}
		public override void OnEnter(){}
		public override void OnExit(){}
		public override void SetItem(IUIItem item){}
	}
	public interface IWaitingForEmptifyState: IItemIconNonemptyState{}
	public class WaitingForEmptifyingState: AbsItemIconNonemptyState, IWaitingForEmptifyState{
		protected override void CheckRemoval(bool removesEmpty){
			if(thisItemIcon.GetItemQuantity() == 0){
				if(thisItemIcon.LeavesGhost())
					thisItemIcon.Ghostify();
				else{
					if(removesEmpty)
						thisItemIcon.EmptifyAndRemove();
					else
						thisItemIcon.Emptify();
				}

			}
		}
		public override void OnEnter(){}
		public override void OnExit(){}
		public override void SetItem(IUIItem item){}
	}
	/* EmptyState */
	public interface IItemIconEmptyState: IItemIconEmptinessState{}
	public abstract class AbsItemIconEmptyState: AbsItemIconEmptinessState{
		public override void IncreaseBy(int quantity, bool doesIncrement){
			throw new System.InvalidOperationException("emptyItemIcon cannot be increased. Call SetItem instead");
		}
		public override void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			throw new System.InvalidOperationException("emptyItemIcon cannot be decreased.");
		}
	}
	public interface IEmptifyingState: IItemIconEmptyState{}
	public interface IWaitingForDisemptifyState: IItemIconEmptyState{}
}
