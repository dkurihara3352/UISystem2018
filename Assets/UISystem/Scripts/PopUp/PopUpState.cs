using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IPopUpState: ISwitchableState, IPopUpEventTrigger{}
	public abstract class AbsPopUpState : IPopUpState {
		public AbsPopUpState(IPopUpStateConstArg arg){
			thisEngine = arg.engine;
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
		public PopUpHiddenState(IPopUpStateConstArg arg): base(arg){}
		public override void OnEnter(){
			return;
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
		public PopUpHidingState(IPopUpStateConstArg arg): base(arg){}
		public override void OnEnter(){
			thisEngine.StartNewHideProcess();
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
		public PopUpShownState(IPopUpStateConstArg arg): base(arg){}
		public override void OnEnter(){
			return;
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
		public PopUpShowingState(IPopUpStateConstArg arg): base(arg){}
		public override void OnEnter(){
			thisEngine.StartNewShowProcess();
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
	/* ConstArg */
	public interface IPopUpStateConstArg{
		IPopUpStateEngine engine{get;}
	}
}

