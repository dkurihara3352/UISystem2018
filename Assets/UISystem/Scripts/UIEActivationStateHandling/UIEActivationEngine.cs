using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationHandler{
		void Activate();
		void ActivateInstantly();
		void Deactivate();
		void DeactivateInstantly();
	}
	public interface IUIEActivationStateEngine: ISwitchableStateEngine<IUIEActivationState>, IUIEActivationHandler{
		void SetInitializationFields(IUIElement uiElement);
		void ExpireProcessOnCurrentProcessState();
		bool IsActivationComplete();
		void SetToActivatingState();
		void SetToActivationCompletedState();
		void SetToDeactivatingState();
		void SetToDeactivationCompletedState();
	}
	public class UIEActivationStateEngine: AbsSwitchableStateEngine<IUIEActivationState>, IUIEActivationStateEngine{
		public UIEActivationStateEngine(IUIEActivationStateEngineConstArg arg){
			thisActivatingState = arg.activatingState;
			thisActivationCompletedState = arg.activationCompletedState;
			thisDeactivatingState = arg.deactivatingState;
			thisDeactivationCompletedState = arg.deactivationCompleteState;
			thisStates = new IUIEActivationState[4]{
				thisActivatingState, 
				thisActivationCompletedState, 
				thisDeactivatingState,
				thisDeactivationCompletedState
			};
		}
		readonly IUIEActivatingState thisActivatingState;
		readonly IUIEActivationCompletedState thisActivationCompletedState;
		readonly IUIEDeactivatingState thisDeactivatingState;
		readonly IUIEDeactivationCompletedState thisDeactivationCompletedState;
		readonly IUIEActivationState[] thisStates;
		public void SetInitializationFields(IUIElement uiElement){
			foreach(IUIEActivationState state in thisStates)
				state.SetInitializationFields(this, uiElement);
		}
		public void Activate(){
			thisCurState.Activate();
		}
		public void ActivateInstantly(){
			thisCurState.ActivateInstantly();
		}
		public void Deactivate(){
			thisCurState.Deactivate();
		}
		public void DeactivateInstantly(){
			thisCurState.DeactivateInstantly();
		}
		public bool IsActivationComplete(){
			return thisCurState is IUIEActivationCompletedState;
		}
		public void ExpireProcessOnCurrentProcessState(){
			if(thisCurState is IWaitAndExpireProcessState)
				((IWaitAndExpireProcessState)thisCurState).ExpireProcess();
		}
		public void SetToActivatingState(){
			TrySwitchState(thisActivatingState);
		}
		public void SetToActivationCompletedState(){
			TrySwitchState(thisActivationCompletedState);
		}
		public void SetToDeactivatingState(){
			TrySwitchState(thisDeactivatingState);
		}
		public void SetToDeactivationCompletedState(){
			TrySwitchState(thisDeactivationCompletedState);
		}
	}
	public interface IUIEActivationStateEngineConstArg{
		IUIEActivatingState activatingState{get;}
		IUIEActivationCompletedState activationCompletedState{get;}
		IUIEDeactivatingState deactivatingState{get;}
		IUIEDeactivationCompletedState deactivationCompleteState{get;}
	}
}
