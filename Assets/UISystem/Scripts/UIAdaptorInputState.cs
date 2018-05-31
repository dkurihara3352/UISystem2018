	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(PointerEventData eventData);
		void OnPointerUp(PointerEventData eventData);
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
		public abstract void OnPointerDown(PointerEventData eventData);
		public abstract void OnPointerUp(PointerEventData eventData);

	}
	public abstract class PointerUpInputState: AbsUIAdaptorInputState{
		public PointerUpInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerUp(PointerEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already help up");
		}
	}
	public abstract class PointerDownInputState: AbsUIAdaptorInputState{
		public PointerDownInputState(IUIAdaptorStateEngine engine): base(engine){}
		public override void OnPointerDown(PointerEventData eventData){
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
		public override void OnPointerDown(PointerEventData eventData){
			engine.IncrementTouchCounter();
			engine.TouchUIE();
			engine.WaitForTap();
		}
	}
	public class WaitingForTapState: PointerDownInputState, IWaitAndExpireProcessState{
		/* 	Down state
			enter =>
				Runs WFTapProcess
					pointer release => 
						WFNextTouchState
						Tap(touchCounter)
					expire => 
						WFReleaseState
		*/
		public WaitingForTapState(IUIAdaptorStateEngine engine, IProcessFactory procFac): base(engine){
			waitingForTapProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetTapExpireT());
		}
		readonly IWaitAndExpireProcess waitingForTapProcess;
		public override void OnEnter(){
			waitingForTapProcess.Run();
		}
		public override void OnExit(){}
		public override void OnPointerUp(PointerEventData eventData){
			engine.WaitForNextTouch();
			engine.TapUIE();
		}
		public void OnProcessExpire(){
			engine.WaitForRelease();
		}
	}
	public class WaitingForReleaseState: PointerDownInputState{
		/* 	DownState
			enter =>
				touch count reset
			pointer release => 
				WFNextTouchState
		*/
		public WaitingForReleaseState(IUIAdaptorStateEngine engine) :base(engine){
		}
		public override void OnEnter(){
			engine.ResetTouchCounter();
		}
		public override void OnExit(){
		}
		public override void OnPointerUp(PointerEventData eventData){
			engine.WaitForNextTouch();
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
		*/
		public WaitingForNextTouchState(IUIAdaptorStateEngine engine, IProcessFactory procFac) :base(engine){
			waitAndExpireProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetNextTouchExpireT());
		}
		readonly IProcess waitAndExpireProcess;
		public override void OnEnter(){
			waitAndExpireProcess.Run();
		}
		public override void OnExit(){}
		public override void OnPointerDown(PointerEventData eventData){
			engine.IncrementTouchCounter();
			engine.TouchUIE();
			engine.WaitForTap();
		}
		public void OnProcessExpire(){
			engine.WaitForFirstTouch();
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
		/* IRawInputHandler */
			public void OnPointerDown(PointerEventData eventData){
				curState.OnPointerDown(eventData);
			}
			public void OnPointerUp(PointerEventData eventData){
				curState.OnPointerUp(eventData);
			}
		/* IUIAdaptorStateHandler imple and states switch */
			readonly WaitingForFirstTouchState waitingForFirstTouchState;
			readonly WaitingForTapState waitingForTapState;
			readonly WaitingForReleaseState waitingForReleaseState;
			readonly WaitingForNextTouchState waitingForNextTouchState;
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


