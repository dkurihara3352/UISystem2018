using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconEmptinessState: ISwitchableState, IEmptinessStateHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public abstract class AbsItemIconEmptinessState: IItemIconEmptinessState{
		public AbsItemIconEmptinessState(IItemIconEmptinessStateEngine stateEngine){
			thisStateEngine = stateEngine;
		}
		protected IItemIcon thisItemIcon;
		protected IItemIconEmptinessStateEngine thisStateEngine;
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
		}
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void DisemptifyInstantly(IUIItem item);
		public abstract void EmptifyInstantly();
		public abstract void Disemptify(IUIItem item);
		public abstract void Emptify();
		public abstract void InitImage();
		public abstract void IncreaseBy(int quantity, bool doesIncrement);
		public abstract void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
	}
	/* NonemptyState */
	public interface IItemIconNonEmptyState: IItemIconEmptinessState{}
	public abstract class AbsItemIconNonEmptyState: AbsItemIconEmptinessState, IItemIconNonEmptyState{
		public AbsItemIconNonEmptyState(IItemIconEmptinessStateEngine stateEngine): base(stateEngine){}
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
	public interface IWaitingForImageInitState: IItemIconNonEmptyState{
	}
	public class WaitingForImageInitState: AbsItemIconNonEmptyState, IWaitingForImageInitState{
		public WaitingForImageInitState(IItemIconEmptinessStateEngine stateEngine): base(stateEngine){}
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
		public override void InitImage(){
			thisStateEngine.SetToDisemptifyingState();
		}
		public override void Disemptify(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToDisemptifyingState();
		}
		public override void DisemptifyInstantly(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToWaitingForEmptifyState();
		}
		public override void Emptify(){
			this.EmptifyInstantly();
		}
		public override void EmptifyInstantly(){
			thisItemIcon.SetUIItem(null);
			thisStateEngine.SetToWaitingForDisemptifyState();
		}
	}
	public interface IDisemptifyingState: IItemIconNonEmptyState, IWaitAndExpireProcessState{
	}
	public class DisemptifyingState: AbsItemIconNonEmptyState, IDisemptifyingState{
		public DisemptifyingState(IItemIconEmptinessStateEngine stateEngine, IProcessFactory processFactory): base(stateEngine){
			thisItemIconDisemptifyProcess = processFactory.CreateDisemptifyingProcess();
		}
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
		IItemIconDisemptifyProcess thisItemIconDisemptifyProcess;
		public override void OnEnter(){
			thisItemIconDisemptifyProcess.Run();
		}
		public override void OnExit(){
			if(thisItemIconDisemptifyProcess.IsRunning())
				thisItemIconDisemptifyProcess.Stop();
		}
		public void OnProcessExpire(){
			thisStateEngine.SetToWaitingForEmptifyState();
		}
		public void OnProcessUpdate(float deltaT){
			return;
		}
		public void ExpireProcess(){
			thisItemIconDisemptifyProcess.Expire();
		}
		public override void InitImage(){
			throw new System.InvalidOperationException("this is disemptifying and already init'ed image");
		}
		public override void Disemptify(IUIItem item){
			throw new System.InvalidOperationException("this is already disemptifying, no new disemptification is allowed");
		}
		public override void DisemptifyInstantly(IUIItem item){
			throw new System.InvalidOperationException("this is already disemptifying, not new disemptification is allowed");
		}
		public override void Emptify(){
			thisStateEngine.SetToEmptifyingState();
		}
		public override void EmptifyInstantly(){
			thisStateEngine.SetToWaitingForDisemptifyState();
		}
	}
	public interface IWaitingForEmptifyState: IItemIconNonEmptyState{}
	public class WaitingForEmptifyingState: AbsItemIconNonEmptyState, IWaitingForEmptifyState{
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
	}
	/* EmptyState */
	public interface IItemIconEmptyState: IItemIconEmptinessState{}
	public abstract class AbsItemIconEmptyState: AbsItemIconEmptinessState{
		public AbsItemIconEmptyState(IItemIconEmptinessStateEngine stateEngine): base(stateEngine){}
		public override void IncreaseBy(int quantity, bool doesIncrement){
			throw new System.InvalidOperationException("emptyItemIcon cannot be increased. Call SetItem instead");
		}
		public override void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			throw new System.InvalidOperationException("emptyItemIcon cannot be decreased.");
		}
		public override void Emptify(){
			return;
		}
		public override void InitImage(){
			throw new System.InvalidOperationException("this is empty and cannot init image");
		}
	}
	public interface IEmptifyingState: IItemIconEmptyState{
		void ExpireProcess();
	}
	public interface IWaitingForDisemptifyState: IItemIconEmptyState{}
}
