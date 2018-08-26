using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IPopUpEventTrigger{
		void Hide(bool instantly);
		void Show(bool instantly);
	}
	public interface IPopUpStateEngine: IPopUpEventTrigger{
		void SwitchToHiddenState();
		void SwitchToHidingState();
		void SwitchToShownState();
		void SwitchToShowingState();
		/*  */
		void ExpireCurrentProcess();
		void StartNewHideProcess();
		void StartNewShowProcess();

		/*  */
		void CallPopUpOnHideBegin();
		void CallPopUpOnShowBegin();
		void CallPopUpOnHideComplete();
		void CallPopUpOnShowComplete();
		/*  */
		void RegisterPopUp();
		void UnregisterPopUp();
		/*  */
		bool IsHidden();
		bool IsShown();
		void TogglePopUpInteractability(bool interactable);
	}
	public class PopUpStateEngine :AbsSwitchableStateEngine<IPopUpState>, IPopUpStateEngine, ISwitchableStateEngine<IPopUpState> {

		public PopUpStateEngine(IPopUpStateEngineConstArg arg){
			thisProcessFactory = arg.processFactory;
			thisPopUp = arg.popUp;
			thisPopUpManager = arg.popUpManager;

			thisHiddenState = new PopUpHiddenState(this);
			thisHidingState = new PopUpHidingState(this);
			thisShownState = new PopUpShownState(this);
			thisShowingState = new PopUpShowingState(this);
			
			// Hide(true);
			thisCurState = thisHiddenState;
		}
		readonly IUISystemProcessFactory thisProcessFactory;
		readonly IPopUp thisPopUp;
		readonly IPopUpManager thisPopUpManager;
		readonly PopUpMode thisPopUpMode;
		/* states */
		readonly IPopUpHiddenState thisHiddenState;
		readonly IPopUpHidingState thisHidingState;
		readonly IPopUpShownState thisShownState;
		readonly IPopUpShowingState thisShowingState;
		/* switch */
		public void SwitchToHiddenState(){
			TrySwitchState(thisHiddenState);
		}
		public void SwitchToHidingState(){
			TrySwitchState(thisHidingState);
		}
		public void SwitchToShownState(){
			TrySwitchState(thisShownState);
		}
		public void SwitchToShowingState(){
			TrySwitchState(thisShowingState);
		}
		/*  */

		/* Process */
		IPopUpProcess thisRunningProcess;
		public void ExpireCurrentProcess(){
			if(thisRunningProcess != null)
				if(thisRunningProcess.IsRunning())
					thisRunningProcess.Expire();
		}
		void SetRunningProcess(IPopUpProcess process){
			if(thisRunningProcess != null)
				if(thisRunningProcess.IsRunning())
					thisRunningProcess.Stop();
			thisRunningProcess = process;
		}
		IPopUpProcess CreatePopUpProcess(bool hides){
			IPopUpProcess newProcess;
			switch(thisPopUpMode){
				case PopUpMode.Alpha:
					newProcess = thisProcessFactory.CreateAlphaPopUpProcess(thisPopUp, this, hides);
					break;
				default: 
					newProcess = null;
					break;
			}
			return newProcess;
		}
		public void StartNewHideProcess(){
			IPopUpProcess newPorcess = CreatePopUpProcess(true);
			newPorcess.Run();
			SetRunningProcess(newPorcess);
		}
		public void StartNewShowProcess(){
			IPopUpProcess newProcess = CreatePopUpProcess(false);
			newProcess.Run();
			SetRunningProcess(newProcess);
		}
		/*  */
		public void CallPopUpOnHideBegin(){
			thisPopUp.OnHideBegin();
		}
		public void CallPopUpOnHideComplete(){
			thisPopUp.OnHideComplete();
		}
		public void CallPopUpOnShowBegin(){
			thisPopUp.OnShowBegin();
		}
		public void CallPopUpOnShowComplete(){
			thisPopUp.OnShowComplete();
		}
		/*  */
		public void Hide(bool instantly){
			thisCurState.Hide(instantly);
		}
		public void Show(bool instantly){
			thisCurState.Show(instantly);
		}
		public bool IsHidden(){
			return thisCurState == thisHiddenState ||
				thisCurState == thisHidingState;
		}
		public bool IsShown(){
			return thisCurState == thisShownState ||
				thisCurState == thisShowingState;
		}
		/*  */
		public void RegisterPopUp(){
			thisPopUpManager.RegisterPopUp(thisPopUp);
		}
		public void UnregisterPopUp(){
			thisPopUpManager.UnregisterPopUp(thisPopUp);
		}
		/*  */
		public void TogglePopUpInteractability(bool interactable){
			((IPopUpAdaptor)thisPopUp.GetUIAdaptor()).ToggleRaycastBlock(interactable);
		}
	}
	public interface IPopUpStateEngineConstArg{
		IUISystemProcessFactory processFactory{get;}
		IPopUp popUp{get;}
		IPopUpManager popUpManager{get;}
		PopUpMode popUpMode{get;}
	}
	public class PopUpStateEngineConstArg: IPopUpStateEngineConstArg{
		public PopUpStateEngineConstArg(
			IUISystemProcessFactory processFactory,
			IPopUp popUp,
			IPopUpManager popUpManager,
			PopUpMode popUpMode
		){
			thisProcessFactory = processFactory;
			thisPopUp = popUp;
			thisPopUpManager = popUpManager;
			thisPopUpMode = popUpMode;
		}
		readonly IUISystemProcessFactory thisProcessFactory;
		public IUISystemProcessFactory processFactory{get{return thisProcessFactory;}}
		readonly IPopUp thisPopUp;
		public IPopUp popUp{get{return thisPopUp;}}
		readonly IPopUpManager thisPopUpManager;
		public IPopUpManager popUpManager{get{return thisPopUpManager;}}
		readonly PopUpMode thisPopUpMode;
		public PopUpMode popUpMode{get{return thisPopUpMode;}}
	}
}
