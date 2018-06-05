	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnDrag(Vector2 dragPos, Vector2 dragDeltaP);
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
		public abstract void OnDrag(Vector2 dragPos, Vector2 dragDeltaP);
		public abstract void OnPointerEnter(ICustomEventData eventData);
		public abstract void OnPointerExit(ICustomEventData eventData);
	}
	public abstract class PointerUpInputState: AbsUIAdaptorInputState{
		public PointerUpInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerUp(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already help up");
		}
		public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
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
				engine.SwipeUIE(eventData.deltaP);
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
		public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
			engine.DragUIE(dragPos, dragDeltaP);
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
				engine.SwipeUIE(eventData.deltaP);
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
		public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
			engine.DragUIE(dragPos, dragDeltaP);
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
		void DragUIE(Vector2 dragPois, Vector2 dragDeltaP);
		void HoldUIE(float deltaT);
		void SwipeUIE(Vector2 deltaP);
		float GetSwipeThreshold();
	}
	public class UIAdaptorStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorStateEngine{
		public UIAdaptorStateEngine(IUIAdaptor uia, IProcessFactory procFac){
			this.uie = uia.GetUIElement();
			this.waitingForFirstTouchState = new WaitingForFirstTouchState(this);
			this.waitingForTapState = new WaitingForTapState(this, procFac);
			this.waitingForReleaseState = new WaitingForReleaseState(this, procFac);
			this.waitingForNextTouchState = new WaitingForNextTouchState(this, procFac);
			SetWithInitState();
			ResetTouchCounter();
		}
		readonly IUIElement uie;
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
			uie.OnTouch(GetTouchCount());
		}
		public void TapUIE(){
			uie.OnTap(GetTouchCount());
		}
		public float GetTapExpireT(){
			return 0.5f;
		}
		public float GetNextTouchExpireT(){
			return 0.5f;
		}
		public void DelayTouchUIE(){
			this.uie.OnDelayedTouch();
		}
		public void ReleaseUIE(){
			this.uie.OnRelease();
		}
		public void DelayedReleaseUIE(){
			this.uie.OnDelayedRelease();
		}
		bool DragDeltaPIsOverThreshold(Vector2 dragDeltaP){
			float deltaPSqrMag = dragDeltaP.sqrMagnitude;
			return deltaPSqrMag >= dragDeltaPThreshold * dragDeltaPThreshold;
		}
		protected float dragDeltaPThreshold = 5f;/* tweak */
		public void DragUIE(Vector2 dragPos, Vector2 dragDeltaP){
			this.uie.OnDrag(dragPos, dragDeltaP);
		}
		public void HoldUIE(float deltaT){
			this.uie.OnHold(deltaT);
		}
		public void SwipeUIE(Vector2 deltaP){
			this.uie.OnSwipe(deltaP);
		}
		public float GetSwipeThreshold(){
			return this.swipeThreshold;
		}
		float swipeThreshold = 5f;
		/* IRawInputHandler */
			public void OnPointerDown(ICustomEventData eventData){
				curState.OnPointerDown(eventData);
			}
			public void OnPointerUp(ICustomEventData eventData){
				curState.OnPointerUp(eventData);
			}
			public void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
				if(DragDeltaPIsOverThreshold(dragDeltaP))
					curState.OnDrag(dragPos, dragDeltaP);
			}
			public void OnPointerEnter(ICustomEventData eventData){
				curState.OnPointerEnter(eventData);
			}
			public void OnPointerExit(ICustomEventData eventData){
				curState.OnPointerExit(eventData);
			}
		/* IUIAdaptorStateHandler imple and states switch */
			protected readonly WaitingForFirstTouchState waitingForFirstTouchState;
			protected readonly WaitingForTapState waitingForTapState;
			protected readonly WaitingForReleaseState waitingForReleaseState;
			protected readonly WaitingForNextTouchState waitingForNextTouchState;
			public void WaitForFirstTouch(){
				TrySwitchState(waitingForFirstTouchState);
			}
			public void WaitForTap(){
				TrySwitchState(waitingForTapState);
			}
			public void WaitForRelease(){
				TrySwitchState(waitingForReleaseState);
			}
			public void WaitForNextTouch(){
				TrySwitchState(waitingForNextTouchState);
			}
	}
}


