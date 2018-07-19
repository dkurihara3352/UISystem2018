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
		bool IsActivationComplete();
		void SetToActivatingState();
		void SetToActivationCompleteState();
		void SetToDeactivatingState();
		void SetToDeactivationCompleteState();
	}
	public class UIEActivationStateEngine: IUIEActivationStateEngine{
		public UIEActivationStateEngine(IUIEActivationStateEngineConstArg arg){

		}

	}
}
