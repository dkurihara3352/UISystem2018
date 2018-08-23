using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface IWaitingForFirstTouchState: IUIAdaptorInputState{}
	public class WaitingForFirstTouchState: AbsPointerUpInputState, IWaitingForFirstTouchState{
		/*  Up state
			enter =>
				touchCounter reset
			pointer down =>
				touch counter ++
				OnTouch( touchCounter)
				WFTapState
		*/
		public WaitingForFirstTouchState(
			IUIAdaptorInputStateConstArg engine
		): base(
			engine
		){}

		public override void OnEnter(){
			thisEngine.ResetTouchCounter();
		}
		public override void OnExit(){}
		public override void OnPointerDown(ICustomEventData eventData){
			thisEngine.IncrementTouchCounter();
			thisEngine.TouchUIE();
			thisEngine.WaitForTap();
		}
	}
	public interface IWaitingForTapState: IUIAdaptorInputState{}
	public class WaitingForTapState: AbsPointerDownInputProcessState<IUIAWaitForTapProcess>, IWaitingForTapState{
		/* 	Down state
			enter =>
				Runs WFTapProcess
			exit =>
				stop the process if its running
			pointer up => 
				WFNextTouchState
				ReleaseUIE
				if deltaP is over thresh
					SwipeUIE
				else
					TapUIE(touchCounter)
			process expire => 
				WFReleaseState
				DelayTouchUIE
			process update =>
				HoldUIE
			drag =>
				DragUIE
			pointer enter =>
				do nothing
			pointer exit =>
				WFRelease
		*/
		/*  In the event of cancel (pointer leaves in bound)
			OnPointerUp is called first, and then OnPointerExit
		*/
		public WaitingForTapState(
			IPointerDownInputProcessStateConstArg arg
		): base(
			arg
		){}

		public override void OnPointerUp(ICustomEventData eventData){
			thisEngine.WaitForNextTouch();

			PushVelocityStack(eventData.velocity);
			Vector2 velocity = GetAverageVelocity();
			eventData.SetVelocity(velocity);

			if(VelocityIsOverSwipeThreshold(eventData.velocity))
				thisEngine.SwipeUIE(eventData);
			else
				thisEngine.TapUIE();
		}
		public override void OnPointerEnter(ICustomEventData eventData){}
		public override void OnPointerExit(ICustomEventData eventData){
			thisEngine.WaitForRelease();
		}
		protected override IUIAWaitForTapProcess CreateProcess(){
			return thisProcessFactory.CreateUIAWaitForTapProcess(this, thisEngine);
		}
	}
	public interface IWaitingForReleaseState: IUIAdaptorInputState{}
	public class WaitingForReleaseState: AbsPointerDownInputProcessState<IUIAWaitForReleaseProcess>, IWaitingForReleaseState{
		/* 	DownState
			enter =>
				touch count reset
				Runs a process =>
					HoldUIE every frame during its running
			exit =>
				stop the process
			pointer up => 
				WFNextTouchState
				ReleaseUIE
				if deltaP is over thresh
					SwipeUIE
			drag =>
				DragUIE
			pointer enter => 
				do nothing
			pointer exit =>
				do nothing
		*/
		public WaitingForReleaseState(
			IPointerDownInputProcessStateConstArg arg
		) :base(
			arg
		){}

		public override void OnEnter(){
			base.OnEnter();
			thisEngine.ResetTouchCounter();
		}
		public override void OnPointerUp(ICustomEventData eventData){
			thisEngine.WaitForNextTouch();

			PushVelocityStack(eventData.velocity);
			Vector2 velocity = GetAverageVelocity();
			eventData.SetVelocity(velocity);

			if(VelocityIsOverSwipeThreshold(eventData.velocity))
				thisEngine.SwipeUIE(eventData);
			else
				thisEngine.ReleaseUIE();
		}
		public override void OnPointerEnter(ICustomEventData eventData){
			return;
		}
		public override void OnPointerExit(ICustomEventData eventData){
			return;
		}
		protected override IUIAWaitForReleaseProcess CreateProcess(){
			return thisProcessFactory.CreateUIAWaitForReleaseProcess(this, thisEngine);
		}
	}
	public interface IWaitingForNextTouchState: IUIAdaptorInputState{}
	public class WaitingForNextTouchState: AbsPointerUpInputProcessState<IUIAWaitForNextTouchProcess>, IWaitingForNextTouchState{
		/*  UpState
			enter =>
				Runs WFNextTouchProcess
					pointer down => 
						touch counter ++
						OnTouch( touchCounter)
						WFTapState
					expire =>
						WFFTouchState
						DelayedReleaseUIE
		*/
		public WaitingForNextTouchState(
			IPointerUpInputProcessStateConstArg arg
		) :base(
			arg
		){}

		protected override IUIAWaitForNextTouchProcess CreateProcess(){
			return thisProcessFactory.CreateUIAWaitForNextTouchProcess(this, thisEngine);
		}
		public override void OnPointerDown(ICustomEventData eventData){
			thisEngine.IncrementTouchCounter();
			thisEngine.TouchUIE();
			thisEngine.WaitForTap();
		}
	}
}

