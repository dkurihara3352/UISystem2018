	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnDrag(Vector2 dragPos, Vector2 dragDeltaP);
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

	}
	public abstract class PointerUpInputState: AbsUIAdaptorInputState{
		public PointerUpInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerUp(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already help up");
		}
		public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
			throw new System.InvalidOperationException("OnDrag should be impossible when pointer is held up, something's wrong");
		}
	}
	public abstract class PointerDownInputState: AbsUIAdaptorInputState{
		public PointerDownInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerDown(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerDown should not be called while pointer is already held down");
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
					pointer up => 
						WFNextTouchState
						Tap(touchCounter)
					expire => 
						WFReleaseState
						DelayTouchUIE
					drag =>
							DragUIE
							WFReleaseState

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
			engine.TapUIE();
		}
		public void OnProcessExpire(){
			engine.WaitForRelease();
			engine.DelayTouchUIE();
		}
		public override void OnDrag(Vector2 dragPos, Vector2 dragDeltaP){
			engine.DragUIE(dragPos, dragDeltaP);
			engine.WaitForRelease();
		}
	}
	public class WaitingForReleaseState: PointerDownInputState{
		/* 	DownState
			enter =>
				touch count reset
			pointer up => 
				WFNextTouchState
				ReleaseUIE
			drag =>
				DragUIE
		*/
		public WaitingForReleaseState(IUIAdaptorStateEngine engine) :base(engine){
		}
		public override void OnEnter(){
			engine.ResetTouchCounter();
		}
		public override void OnExit(){
		}
		public override void OnPointerUp(ICustomEventData eventData){
			engine.WaitForNextTouch();
			engine.ReleaseUIE();
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
	}
	public class UIAdaptorStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorStateEngine{
		public UIAdaptorStateEngine(IUIAdaptor uia, IProcessFactory procFac){
			this.uie = uia.GetUIElement();
			this.waitingForFirstTouchState = new WaitingForFirstTouchState(this);
			this.waitingForTapState = new WaitingForTapState(this, procFac);
			this.waitingForReleaseState = new WaitingForReleaseState(this);
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
		float dragDeltaPThreshold = 5f;/* tweak */
		public void DragUIE(Vector2 dragPos, Vector2 dragDeltaP){
			this.uie.OnDrag(dragPos, dragDeltaP);
		}
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


