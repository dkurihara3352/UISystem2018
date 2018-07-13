using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IEmptinessStateHandler{
		void DisemptifyInstantly(IUIItem item);
		void EmptifyInstantly();
		void Disemptify(IUIItem item);
		void Emptify(bool removesEmpty);
		void InitImage();
		void IncreaseBy(int quantity, bool doesIncrement);
		void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty);
	}
	public interface IEmptinessStateSwitcher{
		void SetToEmptifyingState(bool removesEmpty);
		void SetToDisemptifyingState();
		void SetToWaitingForDisemptifyState();
		void SetToWaitingForEmptifyState();
		void SetToWaitingForImageInitState();
	}
	public interface IItemIconEmptinessStateEngine: IEmptinessStateHandler, IEmptinessStateSwitcher{
		void SetItemIcon(IItemIcon itemIcon);
		bool IsEmpty();
		bool IsWaitingForImageInit();
	}
	public class ItemIconEmptinessStateEngine: AbsSwitchableStateEngine<IItemIconEmptinessState>, IItemIconEmptinessStateEngine{
		public ItemIconEmptinessStateEngine(IPickUpSystemProcessFactory processFactory){
			/* inst and set states here */
			thisWaitingForImageInitState = new WaitingForImageInitState(this);
			thisDisemptifyingState = new DisemptifyingState(this, processFactory);
			thisWaitingForEmptifyState = new WaitingForEmptifyState(this);
			thisEmptifyingState = new EmptifyingState(this, processFactory);
			thisWaitingForDisemptifyState = new WaitingForDisemptifyState(this);
		}
		public void SetItemIcon(IItemIcon itemIcon){
			thisItemIcon = itemIcon;
			SetItemIconOnAllStates(itemIcon);
		}
		void SetItemIconOnAllStates(IItemIcon itemIcon){
			thisWaitingForImageInitState.SetItemIcon(itemIcon);
		}
		IItemIcon thisItemIcon;
		/* states */
		readonly IWaitingForImageInitState thisWaitingForImageInitState;
		readonly IDisemptifyingState thisDisemptifyingState;
		readonly IWaitingForEmptifyState thisWaitingForEmptifyState;
		readonly IEmptifyingState thisEmptifyingState;
		readonly IWaitingForDisemptifyState thisWaitingForDisemptifyState;
		/* state delegate */
		public void DisemptifyInstantly(IUIItem item){
			thisCurState.DisemptifyInstantly(item);
		}
		public void EmptifyInstantly(){
			thisCurState.EmptifyInstantly();
		}
		public void Disemptify(IUIItem item){
			thisCurState.Disemptify(item);
		}
		public void Emptify(bool removesEmpty){
			thisCurState.Emptify(removesEmpty);
		}
		public void InitImage(){
			thisCurState.InitImage();
		}
		public void IncreaseBy(int quantity, bool doesIncrement){
			thisCurState.IncreaseBy(quantity, doesIncrement);
		}
		public void DecreaseBy(int quantity, bool doesIncrement, bool removesEmpty){
			thisCurState.DecreaseBy(quantity, doesIncrement, removesEmpty);
		}
		/* innate */
		public bool IsEmpty(){
			return thisCurState is IItemIconEmptyState;
		}
		public bool IsWaitingForImageInit(){
			return thisCurState is IWaitingForImageInitState;
		}
		/* switch */
		public void SetToWaitingForImageInitState(){
			TrySwitchState(thisWaitingForImageInitState);
		}
		public void SetToDisemptifyingState(){
			TrySwitchState(thisDisemptifyingState);
		}
		public void SetToWaitingForEmptifyState(){
			TrySwitchState(thisWaitingForEmptifyState);
		}
		public void SetToEmptifyingState(bool removesEmpty){
			thisEmptifyingState.ToggleRemoval(removesEmpty);
			TrySwitchState(thisEmptifyingState);
		}
		public void SetToWaitingForDisemptifyState(){
			TrySwitchState(thisWaitingForDisemptifyState);
		}
	}
}
