using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationState: ISwitchableState, IUIEActivationHandler{
		void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement);
	}
	public abstract class AbsUIEActivationState<T>: IUIEActivationState where T: class, IUIElement{
		public AbsUIEActivationState(){}
		public void SetInitializationFields(IUIEActivationStateEngine engine, IUIElement uiElement){
			thisEngine = engine;
			thisUIElement = (T)uiElement;
		}
		protected IUIEActivationStateEngine thisEngine;
		protected T thisUIElement;
		public abstract void OnEnter();
		public abstract void OnExit();
		public virtual void Activate(){
			thisEngine.SetToActivatingState();
		}
		public virtual void ActivateInstantly(){
			thisEngine.SetToActivationCompleteState();
		}
		public virtual void Deactivate(){
			thisEngine.SetToDeactivatingState();
		}
		public virtual void DeactivateInstantly(){
			thisEngine.SetToDeactivationCompleteState();
		}

	}
	public interface IUIEActivatingState: IUIEActivationState, IWaitAndExpireProcessState{}
	public class NonActivatorUIEActivatingState: AbsUIEActivationState<INonActivatorUIElement>, IUIEActivatingState{
		public NonActivatorUIEActivatingState(INonActivatorUIEActivatingProcess process){
			thisProcess = process;
			thisProcess.SetWaitAndExpireProcessState(this);
		}
		readonly INonActivatorUIEActivatingProcess thisProcess;
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
		public void OnProcessExpire(){
			thisEngine.SetToActivationCompleteState();
		}
		public void ExpireProcess(){
			thisProcess.Expire();
		}
		public override void Activate(){
			return;
		}
	}
	public interface INonActivatorUIEActivatingProcess: IWaitAndExpireProcess{}
	public class NonActivatorUIEActivatingProcess: GenericWaitAndExpireProcess, INonActivatorUIEActivatingProcess{
		public NonActivatorUIEActivatingProcess(IProcessManager processManager, float expireT): base(processManager, expireT){
		}
	}
}
