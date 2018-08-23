using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
namespace UISystem{
	public interface IUIEActivationHandler{
		void Activate(bool instantly);
		void Deactivate(bool instantly);
	}
	public interface IUIEActivationStateEngine: ISwitchableStateEngine<IUIEActivationState>, IUIEActivationHandler{
		void ExpireCurrentProcess();
		bool IsActivated();
		bool IsActivationComplete();

		void SetToActivatingState();
		void SetToActivationCompletedState();
		void SetToDeactivatingState();
		void SetToDeactivationCompletedState();

		void StartNewActivateProcess();
		void StartNewDeactivateProcess();

		void CallUIElementOnActivationComplete();
		void CallUIElementOnDeactivationComplete();
	}
	public enum ActivationMode{
		None,
		Alpha
	}
	public class UIEActivationStateEngine: AbsSwitchableStateEngine<IUIEActivationState>, IUIEActivationStateEngine{
		public UIEActivationStateEngine(
			IUISystemProcessFactory processFactory, 
			IUIElement uiElement, 
			ActivationMode activationMode
		){
			thisProcessFactory = processFactory;
			thisUIElement = uiElement;
			thisActivationMode = activationMode;
			thisActivatingState = new UIEActivatingState(this);
			thisActivationCompletedState = new UIEActivationCompletedState(this);
			thisDeactivatingState = new UIEDeactivatingState(this);
			thisDeactivationCompletedState = new UIEDeactivationCompletedState(this);
			
			SetToDeactivationCompletedState();
		}
		readonly protected IUIEActivatingState thisActivatingState;
		readonly protected IUIEActivationCompletedState thisActivationCompletedState;
		readonly protected IUIEDeactivatingState thisDeactivatingState;
		readonly protected IUIEDeactivationCompletedState thisDeactivationCompletedState;
		readonly IUIEActivationState[] thisStates;
		readonly IUISystemProcessFactory thisProcessFactory;
		readonly IUIElement thisUIElement;
		readonly ActivationMode thisActivationMode;

		public void Activate(bool instantly){
			thisCurState.Activate(instantly);
		}
		public void Deactivate(bool instantly){
			thisCurState.Deactivate(instantly);
		}
		public bool IsActivated(){
			return thisCurState is IUIEActivatingState || IsActivationComplete();
		}
		public bool IsActivationComplete(){
			return thisCurState is IUIEActivationCompletedState;
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

		public void StartNewActivateProcess(){
			StartNewActivationProcess(true);
		}
		public void StartNewDeactivateProcess(){
			StartNewActivationProcess(false);
		}
		void StartNewActivationProcess(bool activates){
			IUIEActivationProcess newProcess = CreateNewProcess(activates);
			if(thisRunningProcess != null && thisRunningProcess.IsRunning())
				thisRunningProcess.Stop();
			thisRunningProcess = newProcess;
			newProcess.Run();
		}
		public void ExpireCurrentProcess(){
			if(thisRunningProcess != null)
				thisRunningProcess.Expire();
		}
		protected IUIEActivationProcess thisRunningProcess;
		IUIEActivationProcess CreateNewProcess(bool activates){
			IUIEActivationProcess newProcess;
			switch(thisActivationMode){
				case ActivationMode.None: 
					newProcess = thisProcessFactory.CreateNonActivatorUIEActivationProcess(this, activates);
					break;
				case ActivationMode.Alpha:
					newProcess = thisProcessFactory.CreateAlphaActivatorUIEActivationProcess(thisUIElement, this, activates);
					break;
				default: 
					newProcess = null;
					break;
			}
			return newProcess;
		}

		public void CallUIElementOnActivationComplete(){
			thisUIElement.OnActivationComplete();
		}
		public void CallUIElementOnDeactivationComplete(){
			thisUIElement.OnDeactivationComplete();
		}
	}
}
