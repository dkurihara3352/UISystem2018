﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationState: ISwitchableState, IUIEActivationHandler{
	}
	public abstract class AbsUIEActivationState: IUIEActivationState{
		public AbsUIEActivationState(IUIEActivationStateEngine engine, IUIElement uiElement){
			thisEngine = engine;
			thisUIElement = uiElement;
		}
		readonly protected IUIEActivationStateEngine thisEngine;
		readonly protected IUIElement thisUIElement;
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
	public interface IUIEActivationProcessState: IUIEActivationState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEActivationProcessState: AbsUIEActivationState, IUIEActivationProcessState{
		public AbsUIEActivationProcessState(IUIEActivationStateEngine engine, IUIElement uiElement, IUISystemProcessFactory processFactory): base(engine, uiElement){
			thisProcessFactory = processFactory;
		}
		protected IUIEActivationProcess thisProcess;
		readonly protected IUISystemProcessFactory thisProcessFactory;
		protected abstract IUIEActivationProcess CreateUIEActivationProcess();
		public override void OnEnter(){
			thisProcess = CreateUIEActivationProcess();
			thisProcess.Run();
		}
		public override void OnExit(){
			StopAndClearProcess(); 
		}
		public void OnProcessUpdate(float deltaT){
			return;
		}
		public abstract void OnProcessExpire();
		public void ExpireProcess(){
			ExpireAndClearProcess();
		}
		void StopAndClearProcess(){
			if(thisProcess != null)
				if(thisProcess.IsRunning())
					thisProcess.Stop();
			thisProcess = null;
		}
		void ExpireAndClearProcess(){
			if(thisProcess != null)
				if(thisProcess.IsRunning())
					thisProcess.Expire();
			thisProcess = null;
		}
	}
	public interface IUIEActivatingState: IUIEActivationState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEActivatingState: AbsUIEActivationProcessState, IUIEActivatingState{
		public AbsUIEActivatingState(IUIEActivationStateEngine engine, IUIElement uiElement, IUISystemProcessFactory processFactory):base(engine, uiElement, processFactory){}
		public override void OnEnter(){
			base.OnEnter();
			thisUIElement.ActivateImple();
		}
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
		public UIEActivationCompletedState(IUIEActivationStateEngine engine, IUIElement uiElement):base(engine, uiElement){}
		public override void OnEnter(){
			base.OnEnter();
			thisUIElement.OnActivationComplete();
		}
		public override void Activate(){return;}
		public override void ActivateInstantly(){return;}
	}
	public interface IUIEDeactivatingState: IUIEActivationState, IWaitAndExpireProcessState{}
	public abstract class AbsUIEDeactivatingState: AbsUIEActivationProcessState, IUIEDeactivatingState{
		public AbsUIEDeactivatingState(IUIEActivationStateEngine engine, IUIElement uiElement, IUISystemProcessFactory processFactory): base(engine, uiElement, processFactory){}
		public override void OnEnter(){
			base.OnEnter();
			thisUIElement.DeactivateImple();
		}
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
		public UIEDeactivationCompletedState(IUIEActivationStateEngine engine, IUIElement uiElement): base(engine, uiElement){}
		public override void OnEnter(){
			base.OnEnter();
			thisUIElement.OnDeactivationComplete();
		}
		public override void Deactivate(){return;}
		public override void DeactivateInstantly(){return;}
	}

}
