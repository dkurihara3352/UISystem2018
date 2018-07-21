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
		void ExpireProcessOnCurrentProcessState();
		bool IsActivationComplete();
		void SetToActivatingState();
		void SetToActivationCompletedState();
		void SetToDeactivatingState();
		void SetToDeactivationCompletedState();
	}
	public abstract class AbsUIEActivationStateEngine: AbsSwitchableStateEngine<IUIEActivationState>, IUIEActivationStateEngine{
		public AbsUIEActivationStateEngine(IUISystemProcessFactory processFactory, IUIElement uiElement){
			thisActivatingState = CreateUIEActivatingState(processFactory, uiElement);
			thisActivationCompletedState = new UIEActivationCompletedState(this, uiElement);
			thisDeactivatingState = CreateUIEDeactivatingState(processFactory, uiElement);
			thisDeactivationCompletedState = new UIEDeactivationCompletedState(this, uiElement);
			SetToDeactivationCompletedState();
		}
		protected abstract IUIEActivatingState CreateUIEActivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement);
		protected abstract IUIEDeactivatingState CreateUIEDeactivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement);
		readonly protected IUIEActivatingState thisActivatingState;
		readonly protected IUIEActivationCompletedState thisActivationCompletedState;
		readonly protected IUIEDeactivatingState thisDeactivatingState;
		readonly protected IUIEDeactivationCompletedState thisDeactivationCompletedState;
		readonly protected IUIEActivationState[] thisStates;
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
	public class NonActivatorUIEActivationStateEngine: AbsUIEActivationStateEngine{
		public NonActivatorUIEActivationStateEngine(IUISystemProcessFactory processFactory, INonActivatorUIElement uiElement): base(processFactory, uiElement){}
		protected override IUIEActivatingState CreateUIEActivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			NonActivatorUIEActivatingState state = new NonActivatorUIEActivatingState(this, (INonActivatorUIElement)uiElement, processFactory);
			return state;
		}
		protected override IUIEDeactivatingState CreateUIEDeactivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			NonActivatorUIEDeactivatingState state = new NonActivatorUIEDeactivatingState(this, (INonActivatorUIElement)uiElement, processFactory);
			return state;
		}
	}
	public class AlphaActivatorUIEActivationStateEngine: AbsUIEActivationStateEngine{
		public AlphaActivatorUIEActivationStateEngine(IUISystemProcessFactory processFactory, IAlphaActivatorUIElement alphaActivatorUIE): base(processFactory, alphaActivatorUIE){
		}
		protected override IUIEActivatingState CreateUIEActivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			AlphaActivatorUIEActivatingState state = new AlphaActivatorUIEActivatingState(this, (IAlphaActivatorUIElement)uiElement, processFactory);
			return state;
		}
		protected override IUIEDeactivatingState CreateUIEDeactivatingState(IUISystemProcessFactory processFactory, IUIElement uiElement){
			AlphaActivatorUIEDeactivatingState state = new AlphaActivatorUIEDeactivatingState(this, (IAlphaActivatorUIElement)uiElement, processFactory);
			return state;
		}
	}
}
