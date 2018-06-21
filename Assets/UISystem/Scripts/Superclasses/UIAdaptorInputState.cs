﻿	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnDrag(ICustomEventData eventData);
		void OnPointerEnter(ICustomEventData eventData);
		void OnPointerExit(ICustomEventData eventData);
	}
	/* States */
	public interface IUIAdaptorInputState: IRawInputHandler, ISwitchableState{
	}
	public abstract class AbsUIAdaptorInputState: IUIAdaptorInputState{
		public AbsUIAdaptorInputState(IUIAdaptorStateEngine engine){
			this.engine = engine;
		}
		protected readonly IUIAdaptorStateEngine engine;
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void OnPointerDown(ICustomEventData eventData);
		public abstract void OnPointerUp(ICustomEventData eventData);
		public abstract void OnDrag(ICustomEventData eventData);
		public abstract void OnPointerEnter(ICustomEventData eventData);
		public abstract void OnPointerExit(ICustomEventData eventData);
	}
	public abstract class PointerUpInputState: AbsUIAdaptorInputState{
		public PointerUpInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerUp(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already help up");
		}
		public override void OnDrag(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnDrag should be impossible when pointer is held up, something's wrong");
		}
		public override void OnPointerEnter(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerEnter should not be called while pointer is held up");
		}
		public override void OnPointerExit(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerExit should not be called while pointer is held up");
		}
	}
	public abstract class PointerDownInputState: AbsUIAdaptorInputState{
		public PointerDownInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerDown(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerDown should not be called while pointer is already held down");
		}
		protected bool DeltaPIsOverSwipeThreshold(Vector2 deltaP){
			float swipeThreshold = engine.GetSwipeThreshold();
			if(deltaP.sqrMagnitude >= swipeThreshold * swipeThreshold)
				return true;
			return false;
		}
		
	}
	public class WaitingForFirstTouchState: PointerUpInputState{
		/*  Up state
			enter =>
				touchCounter reset
			pointer down =>
				touch counter ++
				OnTouch( touchCounter)
				WFTapState
		*/
		public WaitingForFirstTouchState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnEnter(){
			engine.ResetTouchCounter();
		}
		public override void OnExit(){}
		public override void OnPointerDown(ICustomEventData eventData){
			engine.IncrementTouchCounter();
			engine.TouchUIE();
			engine.WaitForTap();
		}
	}
	public class WaitingForTapState: PointerDownInputState, IWaitAndExpireProcessState{
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
		public WaitingForTapState(IUIAdaptorStateEngine engine, IProcessFactory procFac): base(engine){
			waitingForTapProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetTapExpireT());
		}
		readonly IWaitAndExpireProcess waitingForTapProcess;
		public override void OnEnter(){
			waitingForTapProcess.Run();
		}
		public override void OnExit(){
			if(waitingForTapProcess.IsRunning())
				waitingForTapProcess.Stop();
		}
		public override void OnPointerUp(ICustomEventData eventData){
			engine.WaitForNextTouch();
			engine.ReleaseUIE();
			if(DeltaPIsOverSwipeThreshold(eventData.deltaP))
				engine.SwipeUIE(eventData);
			else
				engine.TapUIE();
		}
		public override void OnPointerEnter(ICustomEventData eventData){}
		public override void OnPointerExit(ICustomEventData eventData){
			engine.WaitForRelease();
		}
		public void OnProcessExpire(){
			engine.WaitForRelease();
			engine.DelayTouchUIE();
		}
		public void OnProcessUpdate(float deltaT){
			engine.HoldUIE(deltaT);
		}
		public override void OnDrag(ICustomEventData eventData){
			engine.DragUIE(eventData);
		}
	}
	public class WaitingForReleaseState: PointerDownInputState, IWaitAndExpireProcessState{
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
		public WaitingForReleaseState(IUIAdaptorStateEngine engine, IProcessFactory procFac) :base(engine){
			this.wfReleaseProcess = procFac.CreateWaitAndExpireProcess(this, 0f);
		}
		readonly IWaitAndExpireProcess wfReleaseProcess;
		public override void OnEnter(){
			engine.ResetTouchCounter();
			wfReleaseProcess.Run();
		}
		public override void OnExit(){
			if(wfReleaseProcess.IsRunning())
				wfReleaseProcess.Stop();
		}
		public override void OnPointerUp(ICustomEventData eventData){
			engine.WaitForNextTouch();
			engine.ReleaseUIE();
			if(DeltaPIsOverSwipeThreshold(eventData.deltaP))
				engine.SwipeUIE(eventData);
		}
		public override void OnPointerEnter(ICustomEventData eventData){
			return;
		}
		public override void OnPointerExit(ICustomEventData eventData){
			return;
		}
		public void OnProcessExpire(){
			return;
		}
		public void OnProcessUpdate(float deltaT){
			engine.HoldUIE(deltaT);
		}
		public override void OnDrag(ICustomEventData eventData){
			engine.DragUIE(eventData);
		}
	}
	public class WaitingForNextTouchState: PointerUpInputState, IWaitAndExpireProcessState{
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
		public WaitingForNextTouchState(IUIAdaptorStateEngine engine, IProcessFactory procFac) :base(engine){
			waitAndExpireProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetNextTouchExpireT());
		}
		readonly IProcess waitAndExpireProcess;
		public override void OnEnter(){
			waitAndExpireProcess.Run();
		}
		public override void OnExit(){
			if(waitAndExpireProcess.IsRunning())
				waitAndExpireProcess.Stop();
		}
		public override void OnPointerDown(ICustomEventData eventData){
			engine.IncrementTouchCounter();
			engine.TouchUIE();
			engine.WaitForTap();
		}
		public void OnProcessExpire(){
			engine.WaitForFirstTouch();
			engine.DelayedReleaseUIE();
		}
		public void OnProcessUpdate(float deltaT){
			return;
		}
	}
	/* Engine */
	public interface IUIAdaptorStateHandler{
		void WaitForFirstTouch();
		void WaitForTap();
		void WaitForRelease();
		void WaitForNextTouch();
	}
	public interface IUIAdaptorStateEngine: ISwitchableStateEngine<IUIAdaptorInputState>, IRawInputHandler, IUIAdaptorStateHandler{
		void ResetTouchCounter();
		void IncrementTouchCounter();
		int GetTouchCount();
		void TouchUIE();
		void TapUIE();
		float GetTapExpireT();
		float GetNextTouchExpireT();
		void DelayTouchUIE();
		void ReleaseUIE();
		void DelayedReleaseUIE();
		void DragUIE(ICustomEventData eventData);
		void HoldUIE(float deltaT);
		void SwipeUIE(ICustomEventData eventData);
		float GetSwipeThreshold();
	}
	public class UIAdaptorStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorStateEngine{
		public UIAdaptorStateEngine(IUIAdaptor uia, IProcessFactory procFac){
			thisUIE = uia.GetUIElement();
			thisWaitingForFirstTouchState = new WaitingForFirstTouchState(this);
			thisWaitingForTapState = new WaitingForTapState(this, procFac);
			thisWaitingForReleaseState = new WaitingForReleaseState(this, procFac);
			thisWaitingForNextTouchState = new WaitingForNextTouchState(this, procFac);
			SetWithInitState();
			ResetTouchCounter();
		}
		readonly IUIElement thisUIE;
		void SetWithInitState(){
			this.WaitForFirstTouch();
		}
		int touchCounter;
		public void ResetTouchCounter(){
			touchCounter = 0;
		}
		public void IncrementTouchCounter(){
			touchCounter ++;
		}
		public int GetTouchCount(){
			return touchCounter;
		}
		public void TouchUIE(){
			thisUIE.OnTouch(GetTouchCount());
		}
		public void TapUIE(){
			thisUIE.OnTap(GetTouchCount());
		}
		public float GetTapExpireT(){
			return 0.5f;
		}
		public float GetNextTouchExpireT(){
			return 0.5f;
		}
		public void DelayTouchUIE(){
			thisUIE.OnDelayedTouch();
		}
		public void ReleaseUIE(){
			thisUIE.OnRelease();
		}
		public void DelayedReleaseUIE(){
			thisUIE.OnDelayedRelease();
		}
		bool DragDeltaPIsOverThreshold(Vector2 dragDeltaP){
			float deltaPSqrMag = dragDeltaP.sqrMagnitude;
			return deltaPSqrMag >= dragDeltaPThreshold * dragDeltaPThreshold;
		}
		protected float dragDeltaPThreshold = 5f;/* tweak */
		public void DragUIE(ICustomEventData eventData){
			thisUIE.OnDrag(eventData);
		}
		public void HoldUIE(float deltaT){
			thisUIE.OnHold(deltaT);
		}
		public void SwipeUIE(ICustomEventData eventData){
			thisUIE.OnSwipe(eventData);
		}
		public float GetSwipeThreshold(){
			return thisSwipeThreshold;
		}
		float thisSwipeThreshold = 5f;
		/* IRawInputHandler */
			public void OnPointerDown(ICustomEventData eventData){
				thisCurState.OnPointerDown(eventData);
			}
			public void OnPointerUp(ICustomEventData eventData){
				thisCurState.OnPointerUp(eventData);
			}
			public void OnDrag(ICustomEventData eventData){
				if(DragDeltaPIsOverThreshold(eventData.deltaP))
					thisCurState.OnDrag(eventData);
			}
			public void OnPointerEnter(ICustomEventData eventData){
				thisCurState.OnPointerEnter(eventData);
			}
			public void OnPointerExit(ICustomEventData eventData){
				thisCurState.OnPointerExit(eventData);
			}
		/* IUIAdaptorStateHandler imple and states switch */
			protected readonly WaitingForFirstTouchState thisWaitingForFirstTouchState;
			protected readonly WaitingForTapState thisWaitingForTapState;
			protected readonly WaitingForReleaseState thisWaitingForReleaseState;
			protected readonly WaitingForNextTouchState thisWaitingForNextTouchState;
			public void WaitForFirstTouch(){
				TrySwitchState(thisWaitingForFirstTouchState);
			}
			public void WaitForTap(){
				TrySwitchState(thisWaitingForTapState);
			}
			public void WaitForRelease(){
				TrySwitchState(thisWaitingForReleaseState);
			}
			public void WaitForNextTouch(){
				TrySwitchState(thisWaitingForNextTouchState);
			}
	}
}


