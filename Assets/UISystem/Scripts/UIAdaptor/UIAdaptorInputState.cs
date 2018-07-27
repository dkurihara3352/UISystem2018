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
	public abstract class AbsPointerUpInputState: AbsUIAdaptorInputState{
		public AbsPointerUpInputState(IUIAdaptorStateEngine engine): base(engine){}
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
	public abstract class AbsPointerUpInputProcessState<T>: AbsPointerUpInputState, IWaitAndExpireProcessState where T: class, IWaitAndExpireProcess{
		public AbsPointerUpInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorStateEngine engine): base(engine){
			thisProcessFactory = processFactory;
		}
		protected readonly IUISystemProcessFactory thisProcessFactory;
		protected T thisProcess;
		public override void OnEnter(){
			thisProcess = CreateProcess();
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
		protected abstract T CreateProcess();
		public virtual void OnProcessUpdate(float deltaT){}
		public virtual void OnProcessExpire(){}
		public virtual void ExpireProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Expire();
		}
	}
	public abstract class AbsPointerDownInputState: AbsUIAdaptorInputState{
		public AbsPointerDownInputState(IUIAdaptorStateEngine engine, IUIManager uim): base(engine){
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
	public abstract class AbsPointerDownInputProcessState<T>: AbsPointerDownInputState, IWaitAndExpireProcessState where T: class, IWaitAndExpireProcess{
		public AbsPointerDownInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorStateEngine engine ,IUIManager uim): base(engine, uim){
			thisProcessFactory = processFactory;
		}
		readonly protected IUISystemProcessFactory thisProcessFactory;
		protected T thisProcess;
		public override void OnEnter(){
			thisProcess = CreateProcess();
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
		protected abstract T CreateProcess();
		public virtual void OnProcessUpdate(float deltaT){}
		public virtual void OnProcessExpire(){}
		public virtual void ExpireProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Expire();
		}
	}

	public class WaitingForFirstTouchState: AbsPointerUpInputState{
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
	public class WaitingForTapState: AbsPointerDownInputProcessState<IWaitAndExpireProcess>{
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
		public WaitingForTapState(IUISystemProcessFactory procFac, IUIAdaptorStateEngine engine, IUIManager uim): base(procFac, engine, uim){
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
		public override void OnProcessExpire(){
			engine.WaitForRelease();
			engine.DelayTouchUIE();
		}
		public override void OnProcessUpdate(float deltaT){
			engine.HoldUIE(deltaT);
		}
		public override void OnDrag(ICustomEventData eventData){
			engine.DragUIE(eventData);
			base.OnDrag(eventData);
		}
		protected override IWaitAndExpireProcess CreateProcess(){
			return thisProcessFactory.CreateWaitAndExpireProcess(this, engine.GetTapExpireT());
		}
	}
	public class WaitingForReleaseState: AbsPointerDownInputProcessState<IWaitAndExpireProcess>{
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
		public WaitingForReleaseState(IUISystemProcessFactory procFac, IUIAdaptorStateEngine engine, IUIManager uim) :base(procFac, engine, uim){
		}
		public override void OnEnter(){
			base.OnEnter();
			engine.ResetTouchCounter();
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
		public override void OnProcessUpdate(float deltaT){
			engine.HoldUIE(deltaT);
		}
		public override void OnDrag(ICustomEventData eventData){
			engine.DragUIE(eventData);
			base.OnDrag(eventData);
		}
		protected override IWaitAndExpireProcess CreateProcess(){
			return thisProcessFactory.CreateWaitAndExpireProcess(this, 0f);
		}
	}
	public class WaitingForNextTouchState: AbsPointerUpInputProcessState<IWaitAndExpireProcess>, IWaitAndExpireProcessState{
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
		public WaitingForNextTouchState(IUISystemProcessFactory procFac, IUIAdaptorStateEngine engine) :base(procFac, engine){
		}
		protected override IWaitAndExpireProcess CreateProcess(){
			return thisProcessFactory.CreateWaitAndExpireProcess(this, engine.GetNextTouchExpireT());
		}
		public override void OnPointerDown(ICustomEventData eventData){
			engine.IncrementTouchCounter();
			engine.TouchUIE();
			engine.WaitForTap();
		}
		public override void OnProcessExpire(){
			engine.WaitForFirstTouch();
			engine.DelayedReleaseUIE();
		}
	}
}


