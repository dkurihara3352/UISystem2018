using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationState: ISwitchableState, IUIEActivationHandler{
		void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement);
	}
	public abstract class AbsUIEActivationState: IUIEActivationState{
		public AbsUIEActivationState(){}
		public virtual void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement){
			thisEngine = engine;
			thisUIElement = uiElement;
		}
		protected IUIEActivationStateEngine thisEngine;
		protected IUIElement thisUIElement;
		public virtual void OnEnter(){}
		public virtual void OnExit(){}
		public virtual void Activate(){
			thisEngine.SetToActivatingState();
		}
		public virtual void ActivateInstantly(){
			thisEngine.SetToActivatingState();
			thisEngine.ExpireProcessOnCurrentProcessState();
		}
		public virtual void Deactivate(){
			thisEngine.SetToDeactivatingState();
		}
		public virtual void DeactivateInstantly(){
			thisEngine.SetToDeactivatingState();
			thisEngine.ExpireProcessOnCurrentProcessState();
		}
	}
	public interface IUIEActivationProcessState: IUIEActivatingState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEActivationProcessState: AbsUIEActivationState, IUIEActivationProcessState{
		protected IUIEActivationProcess thisProcess;
		public override void OnEnter(){
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning()){
				thisProcess.Stop();
			}
		}
		public void OnProcessUpdate(float deltaT){
			return;
		}
		public abstract void OnProcessExpire();
		public void ExpireProcess(){
			thisProcess.Expire();
		}
	}
	public interface IUIEActivatingState: IUIEActivationState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEActivatingState: AbsUIEActivationProcessState, IUIEActivatingState{
		public override void OnProcessExpire(){
			thisEngine.SetToActivationCompletedState();
		}
		public override void Activate(){return;}
		public override void ActivateInstantly(){
			this.ExpireProcess();
		}
	}
	public interface IUIEActivationCompletedState: IUIEActivationState{}
	public class UIEActivationCompletedState: AbsUIEActivationState, IUIEActivationCompletedState{
		public override void Activate(){return;}
		public override void ActivateInstantly(){return;}
	}
	public interface IUIEDeactivatingState: IUIEActivationState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEDeactivatingState: AbsUIEActivationProcessState, IUIEDeactivatingState{
		public override void OnProcessExpire(){
			thisEngine.SetToDeactivationCompletedState();
		}
		public override void Deactivate(){return;}
		public override void DeactivateInstantly(){
			this.ExpireProcess();
		}
	}
	public interface IUIEDeactivationCompletedState: IUIEActivationState{}
	public class UIEDeactivationCompletedState: AbsUIEActivationState, IUIEDeactivationCompletedState{
		public override void Deactivate(){return;}
		public override void DeactivateInstantly(){return;}
	}

}
