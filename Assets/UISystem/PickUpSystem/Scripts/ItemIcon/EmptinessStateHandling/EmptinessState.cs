using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IItemIconEmptinessState: ISwitchableState, IEmptinessStateHandler{
		void SetItemIcon(IItemIcon itemIcon);
	}
	public abstract class AbsItemIconEmptinessState: IItemIconEmptinessState{
		public AbsItemIconEmptinessState(IItemIconEmptinessStateEngine stateEngine){
			thisStateEngine = stateEngine;
		}
		protected IItemIcon thisItemIcon;
		protected IItemIconImage thisItemIconImage;
		readonly protected IItemIconEmptinessStateEngine thisStateEngine;
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
			thisItemIconImage = (IItemIconImage)thisItemIcon.GetUIImage();
		}
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void DisemptifyInstantly(IUIItem item);
		public abstract void EmptifyInstantly();
		public abstract void Disemptify(IUIItem item);
		public abstract void Emptify(bool removesEmpty);
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
			thisItemIcon.UpdateQuantity(targetQuantity, doesIncrement);
			if(thisItemIcon.IsGhostified())
				thisItemIcon.Deghostify();
		}
		public override void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			int sourceQuantity = thisItemIcon.GetItemQuantity();
			int targetQuantity = sourceQuantity - quantity;
			thisItemIcon.UpdateQuantity(targetQuantity, doesIncrement);
			this.CheckRemoval(removesEmpty);
		}
		protected abstract void CheckRemoval(bool removesEmpty);
		public override void InitImage(){
			throw new System.InvalidOperationException("this is disemptifying and already init'ed image");
		}
		public override void Disemptify(IUIItem item){
			throw new System.InvalidOperationException("this is already disemptifying, no new disemptification is allowed");
		}
		public override void DisemptifyInstantly(IUIItem item){
			throw new System.InvalidOperationException("this is already disemptifying, not new disemptification is allowed");
		}
		public override void Emptify(bool removesEmpty){
			thisStateEngine.SetToEmptifyingState(removesEmpty);
		}
		public override void EmptifyInstantly(){
			thisStateEngine.SetToWaitingForDisemptifyState();
		}
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
			if(thisItemIcon.GetItemQuantity() == 0 && thisItemIcon.LeavesGhost())
				thisItemIcon.Ghostify();
			IUIItem item = thisItemIcon.GetUIItem();
			if(item.IsStackable()){
				thisItemIcon.SetQuantityInstantly(0);
				thisItemIcon.UpdateQuantity(thisItemIcon.GetItemQuantity(), true);
			}
		}
		public override void Disemptify(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToDisemptifyingState();
		}
		public override void DisemptifyInstantly(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToWaitingForEmptifyState();
		}
		public override void Emptify(bool removesEmpty){
			this.EmptifyInstantly();
			if(removesEmpty){
				IIconGroup ig = thisItemIcon.GetIconGroup();
				ig.RemoveIIAndMutate(thisItemIcon);
			}
		}
	}
	public interface IDisemptifyingState: IItemIconNonEmptyState{
	}
	public class DisemptifyingState: AbsItemIconNonEmptyState, IDisemptifyingState{
		public DisemptifyingState(
			IItemIconEmptinessStateEngine stateEngine, IPickUpSystemProcessFactory pickUpSystemProcessFactory
		): base(
			stateEngine
		){
			thisProcessFactory = pickUpSystemProcessFactory;
		}
		protected override void CheckRemoval(bool removesEmpty){
			if(thisItemIcon.GetItemQuantity() == 0){
				if(thisItemIcon.LeavesGhost())
					thisItemIcon.Ghostify();
				else{
					thisItemIcon.Emptify(removesEmpty);
				}
			}
		}
		IItemIconDisemptifyProcess thisProcess;
		readonly IPickUpSystemProcessFactory thisProcessFactory;
		public override void OnEnter(){
			thisProcess = thisProcessFactory.CreateItemIconDisemptifyProcess(
				thisItemIconImage, 
				thisStateEngine
			);
			thisProcess.Run();
		}
		public override void OnExit(){
			StopAndClearProcess();
		}
		public void ExpireProcess(){
			StopAndClearProcess();
		}
		void StopAndClearProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
	}
	public interface IWaitingForEmptifyState: IItemIconNonEmptyState{}
	public class WaitingForEmptifyState: AbsItemIconNonEmptyState, IWaitingForEmptifyState{
		public WaitingForEmptifyState(IItemIconEmptinessStateEngine stateEngine): base(stateEngine){}
		protected override void CheckRemoval(bool removesEmpty){
			if(thisItemIcon.GetItemQuantity() == 0){
				if(thisItemIcon.LeavesGhost())
					thisItemIcon.Ghostify();
				else{
					thisItemIcon.Emptify(removesEmpty);
				}
			}
		}
		public override void OnEnter(){
			thisItemIconImage.SetEmptiness(1f);
		}
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
		public override void Disemptify(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToDisemptifyingState();
		}
		public override void DisemptifyInstantly(IUIItem item){
			thisItemIcon.SetUIItem(item);
			thisStateEngine.SetToWaitingForEmptifyState();
		}
		public override void Emptify(bool removesEmpty){
			return;
		}
		public override void InitImage(){
			throw new System.InvalidOperationException("this is empty and cannot init image");
		}
	}
	public interface IEmptifyingState: IItemIconEmptyState{
		void ToggleRemoval(bool removesEmpty);
	}
	public class EmptifyingState: AbsItemIconEmptyState, IEmptifyingState{
		public EmptifyingState(
			IItemIconEmptinessStateEngine stateEngine, IPickUpSystemProcessFactory pickUpSystemProcessFactory
		): base(
			stateEngine
		){
			thisProcessFactory = pickUpSystemProcessFactory;
		}
		IItemIconEmptifyProcess thisProcess;
		readonly IPickUpSystemProcessFactory thisProcessFactory;
		bool thisRemovesEmpty;
		public void ToggleRemoval(bool removesEmpty){
			thisRemovesEmpty = removesEmpty;
		}
		public override void OnEnter(){
			thisProcess = thisProcessFactory.CreateItemIconEmptifyProcess(
				thisItemIconImage, 
				thisStateEngine,
				thisItemIcon,
				thisRemovesEmpty
			);
			thisProcess.Run();
		}
		public override void OnExit(){
			StopAndClearProcess();
		}
		public void ExpireProcess(){
			StopAndClearProcess();
		}
		public override void EmptifyInstantly(){
			this.ExpireProcess();
		}
		void StopAndClearProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
	}
	public interface IWaitingForDisemptifyState: IItemIconEmptyState{}
	public class WaitingForDisemptifyState: AbsItemIconEmptyState, IWaitingForDisemptifyState{
		public WaitingForDisemptifyState(IItemIconEmptinessStateEngine stateEngine): base(stateEngine){}
		public override void OnEnter(){
			thisItemIconImage.SetEmptiness(0f);
			// thisItemIcon.RemoveFromContainingReformation();
			/*	uncomment this when mutation and reformation is done
			*/
		}
		public override void OnExit(){
			return;
		}
		public override void EmptifyInstantly(){
			return;
		}
	}
}
