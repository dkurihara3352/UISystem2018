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
		void SetRunningProcess(IProcess process);
		void StartNewHideProcess();
		void StartNewShowProcess();

		/*  */
		void CallPopUpOnHide();
		void CAllPopUpOnShow();
	}
	public abstract class AbsPopUpStateEngine :AbsSwitchableStateEngine<IPopUpState>, IPopUpStateEngine, ISwitchableStateEngine<IPopUpState> {
		/*  start popUpManager's disablingOthers process when entered 			HidingState, if set so
		*/

		public AbsPopUpStateEngine(IPopUpStateEngineConstArg arg){
			thisPopUp = arg.popUp;
			thisPopUpManager = arg.popUpManager;

		}
		readonly IPopUp thisPopUp;
		readonly IPopUpManager thisPopUpManager;
		/* states */
		readonly IPopUpHiddenState thisHiddenState;
		readonly IPopUpHidingState thisHidingState;
		readonly IPopUpShownState thisShownState;
		readonly IPopUpShowingState thisShowingState;
		/*  */

	}
	public interface IPopUpStateEngineConstArg{
		IPopUp popUp{get;}
		IPopUpManager popUpManager{get;}
	}
}
