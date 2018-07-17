using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DKUtility;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnDrag(ICustomEventData eventData);
		void OnPointerEnter(ICustomEventData eventData);
		void OnPointerExit(ICustomEventData eventData);
		void OnCancel();
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
		public abstract void OnCancel();
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
		public override void OnCancel(){
			throw new System.InvalidOperationException("OnCancel should not be called while pointer is held up");
		}
	}
	public abstract class PointerDownInputState: AbsUIAdaptorInputState{
		public PointerDownInputState(IUIAdaptorStateEngine engine, IUIManager uim): base(engine){
			thisUIM = uim;
		}
		public override void OnPointerDown(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerDown should not be called while pointer is already held down");
		}
		protected bool DeltaPIsOverSwipeThreshold(Vector2 deltaP){
			float swipeThreshold = engine.GetSwipeThreshold();
			if(deltaP.sqrMagnitude >= swipeThreshold * swipeThreshold)
				return true;
			return false;
		}
		readonly IUIManager thisUIM;
		void UpdateDragWorldPosition(Vector2 dragWorldPosition){
			thisUIM.SetDragWorldPosition(dragWorldPosition);
		}
		public override void OnDrag(ICustomEventData eventData){
			UpdateDragWorldPosition(eventData.position);
		}
 		public override void OnCancel(){
			engine.WaitForFirstTouch();
			engine.ReleaseUIE();
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
		public WaitingForTapState(IUIAdaptorStateEngine engine, IUISystemProcessFactory procFac, IUIManager uim): base(engine, uim){
			thisWaitForTapProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetTapExpireT());
		}
		readonly IWaitAndExpireProcess thisWaitForTapProcess;
		public override void OnEnter(){
			thisWaitForTapProcess.Run();
		}
		public override void OnExit(){
			if(thisWaitForTapProcess.IsRunning())
				thisWaitForTapProcess.Stop();
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
			base.OnDrag(eventData);
		}
		public void ExpireProcess(){
			if(thisWaitForTapProcess.IsRunning())
				thisWaitForTapProcess.Expire();
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
		public WaitingForReleaseState(IUIAdaptorStateEngine engine, IUISystemProcessFactory procFac, IUIManager uim) :base(engine, uim){
			this.thisWaitForReleaseProcess = procFac.CreateWaitAndExpireProcess(this, 0f);
		}
		readonly IWaitAndExpireProcess thisWaitForReleaseProcess;
		public override void OnEnter(){
			engine.ResetTouchCounter();
			thisWaitForReleaseProcess.Run();
		}
		public override void OnExit(){
			if(thisWaitForReleaseProcess.IsRunning())
				thisWaitForReleaseProcess.Stop();
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
			base.OnDrag(eventData);
		}
		public void ExpireProcess(){
			if(thisWaitForReleaseProcess.IsRunning())
				thisWaitForReleaseProcess.Expire();
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
		public WaitingForNextTouchState(IUIAdaptorStateEngine engine, IUISystemProcessFactory procFac) :base(engine){
			thisWaitAndExpireProcess = procFac.CreateWaitAndExpireProcess(this, engine.GetNextTouchExpireT());
		}
		readonly IProcess thisWaitAndExpireProcess;
		public override void OnEnter(){
			thisWaitAndExpireProcess.Run();
		}
		public override void OnExit(){
			if(thisWaitAndExpireProcess.IsRunning())
				thisWaitAndExpireProcess.Stop();
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
		public void ExpireProcess(){
			if(thisWaitAndExpireProcess.IsRunning())
				thisWaitAndExpireProcess.Expire();
		}
	}
}


