using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IPopUpState: ISwitchableState, IPopUpEventTrigger{}
	public abstract class AbsPopUpState : IPopUpState {
		public AbsPopUpState(IPopUpStateEngine engine){
			thisEngine = engine;
		}
		protected readonly IPopUpStateEngine thisEngine;
		public abstract void OnEnter();
		public virtual void OnExit(){
			return;
		}
		public abstract void Hide(bool instantly);
		public abstract void Show(bool instantly);
	}
	/* HiddenState */
	public interface IPopUpHiddenState: IPopUpState{}
	public class PopUpHiddenState: AbsPopUpState, IPopUpHiddenState{
		public PopUpHiddenState(IPopUpStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.CallPopUpOnHideComplete();
			thisEngine.TogglePopUpInteractability(false);
		}
		public override void Hide(bool instantly){
			return;
		}
		public override void Show(bool instantly){
			thisEngine.SwitchToShowingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}
	/* Hiding State */
	public interface IPopUpHidingState: IPopUpState{}
	public class PopUpHidingState: AbsPopUpState, IPopUpHidingState{
		public PopUpHidingState(IPopUpStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.StartNewHideProcess();
			thisEngine.CallPopUpOnHideBegin();
			thisEngine.UnregisterPopUp();
			thisEngine.TogglePopUpInteractability(false);
		}
		public override void Hide(bool instantly){
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
		public override void Show(bool instantly){
			thisEngine.SwitchToShowingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}
	/* ShownState */
	public interface IPopUpShownState: IPopUpState{}
	public class PopUpShownState: AbsPopUpState, IPopUpShownState{
		public PopUpShownState(IPopUpStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.CallPopUpOnShowComplete();
			thisEngine.TogglePopUpInteractability(true);
		}
		public override void Hide(bool instantly){
			thisEngine.SwitchToHidingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
		public override void Show(bool instantly){
			return;
		}
	}
	/* Showing State */
	public interface IPopUpShowingState: IPopUpState{}
	public class PopUpShowingState: AbsPopUpState, IPopUpShowingState{
		public PopUpShowingState(IPopUpStateEngine engine): base(engine){}
		public override void OnEnter(){
			thisEngine.StartNewShowProcess();
			thisEngine.CallPopUpOnShowBegin();
			thisEngine.RegisterPopUp();
			thisEngine.TogglePopUpInteractability(true);
		}
		public override void Hide(bool instantly){
			thisEngine.SwitchToHidingState();
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
		public override void Show(bool instantly){
			if(instantly)
				thisEngine.ExpireCurrentProcess();
		}
	}
	
}

