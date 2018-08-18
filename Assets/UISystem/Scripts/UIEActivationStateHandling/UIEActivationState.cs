using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationState: ISwitchableState, IUIEActivationHandler{
	}
	public abstract class AbsUIEActivationState: IUIEActivationState{
		public AbsUIEActivationState(IUIEActivationStateEngine engine){
			thisEngine = engine;
		}
		readonly protected IUIEActivationStateEngine thisEngine;
		public virtual void OnEnter(){
			return;
		}
		public virtual void OnExit(){
			return;
		}
		public virtual void Activate(bool instantly){
			thisEngine.SetToActivatingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
		public virtual void Deactivate(bool instantly){
			thisEngine.SetToDeactivatingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}

	public interface IUIEActivatingState: IUIEActivationState{}
	public class UIEActivatingState: AbsUIEActivationState, IUIEActivatingState{
		public UIEActivatingState(IUIEActivationStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.StartNewActivateProcess();
		}
		public override void Activate(bool instantly){
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}
	public interface IUIEActivationCompletedState: IUIEActivationState{}
	public class UIEActivationCompletedState: AbsUIEActivationState, IUIEActivationCompletedState{
		public UIEActivationCompletedState(IUIEActivationStateEngine engine):base(engine){}
		public override void OnEnter(){
			thisEngine.CallUIElementOnActivationComplete();
		}
		public override void Activate(bool instantly){
			return;
		}
	}
	public interface IUIEDeactivatingState: IUIEActivationState{}
	public class UIEDeactivatingState: AbsUIEActivationState, IUIEDeactivatingState{
		public UIEDeactivatingState(IUIEActivationStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.StartNewDeactivateProcess();
		}
		public override void Deactivate(bool instantly){
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}
	public interface IUIEDeactivationCompletedState: IUIEActivationState{}
	public class UIEDeactivationCompletedState: AbsUIEActivationState, IUIEDeactivationCompletedState{
		public UIEDeactivationCompletedState(IUIEActivationStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.CallUIElementOnDeactivationComplete();
		}
		public override void Deactivate(bool instantly){
			return;
		}
	}

}
