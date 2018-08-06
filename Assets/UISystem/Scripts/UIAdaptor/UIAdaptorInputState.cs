using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DKUtility;

namespace UISystem{
	public interface IRawInputHandler{
		void OnPointerDown(ICustomEventData eventData);
		void OnPointerUp(ICustomEventData eventData);
		void OnBeginDrag(ICustomEventData eventData);
		void OnDrag(ICustomEventData eventData);
		void OnPointerEnter(ICustomEventData eventData);
		void OnPointerExit(ICustomEventData eventData);
	}
	/* States */
	public interface IUIAdaptorInputState: IRawInputHandler, ISwitchableState{
	}
	public abstract class AbsUIAdaptorInputState: IUIAdaptorInputState{
		public AbsUIAdaptorInputState(IUIAdaptorInputStateEngine engine){
			this.engine = engine;
		}
		protected readonly IUIAdaptorInputStateEngine engine;
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void OnPointerDown(ICustomEventData eventData);
		public abstract void OnPointerUp(ICustomEventData eventData);
		public abstract void OnBeginDrag(ICustomEventData eventData);
		public abstract void OnDrag(ICustomEventData eventData);
		public abstract void OnPointerEnter(ICustomEventData eventData);
		public abstract void OnPointerExit(ICustomEventData eventData);
	}
	public abstract class AbsPointerUpInputState: AbsUIAdaptorInputState{
		public AbsPointerUpInputState(IUIAdaptorInputStateEngine engine): base(engine){}
		public override void OnPointerUp(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerUp should not be called while pointer is already held up");
		}
		public override void OnBeginDrag(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnBeginDrag should not be called while pointer is held up");
		}
		public override void OnDrag(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnDrag should be impossible when pointer is held up, something's wrong");
		}
		public override void OnPointerEnter(ICustomEventData eventData){return;}
		public override void OnPointerExit(ICustomEventData eventData){return;}
	}
	public abstract class AbsPointerUpInputProcessState<T>: AbsPointerUpInputState, IWaitAndExpireProcessState where T: class, IWaitAndExpireProcess{
		public AbsPointerUpInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorInputStateEngine engine): base(engine){
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
		public AbsPointerDownInputState(IUIAdaptorInputStateEngine engine, IUIManager uim, int velocityStackSize): base(engine){
			thisUIM = uim;
			thisVelocityStackSize = velocityStackSize;
			thisVelocityStack = new Vector2[velocityStackSize];
		}
		int thisVelocityStackSize;
		public override void OnEnter(){
			thisVelocityStack = new Vector2[thisVelocityStackSize];
		}
		public override void OnPointerDown(ICustomEventData eventData){
			throw new System.InvalidOperationException("OnPointerDown should not be called while pointer is already held down");
		}
		protected bool VelocityIsOverSwipeVelocityThreshold(Vector2 velocity){
			float velocityThreshold = engine.GetSwipeVelocityThreshold();
			if(velocity.sqrMagnitude >= velocityThreshold * velocityThreshold)
				return true;
			else
				return false;
		}
		readonly IUIManager thisUIM;
		void UpdateDragWorldPosition(Vector2 dragWorldPosition){
			thisUIM.SetDragWorldPosition(dragWorldPosition);
		}
		public override void OnBeginDrag(ICustomEventData eventData){
			engine.BeginDragUIE(eventData);
			PushVelocityStack(eventData.velocity);
		}
		public override void OnDrag(ICustomEventData eventData){
			engine.DragUIE(eventData);
			UpdateDragWorldPosition(eventData.position);
			PushVelocityStack(eventData.velocity);
		}
		protected void PushVelocityStack(Vector2 velocity){
			int stackSize = thisVelocityStack.Length;
			Vector2[] newStack = new Vector2[stackSize];
			for(int i = 0; i < stackSize; i ++){
				if(i < stackSize -1)
					newStack[i] = thisVelocityStack[i + 1];
				else
					newStack[i] = velocity;
			}
			thisVelocityStack = newStack;
		}
		Vector2[] thisVelocityStack;
		protected Vector2 GetAverageVelocity(){
			int stackSize = thisVelocityStack.Length;
			Vector2 sum = Vector2.zero;
			int nonZeroCount = 0;
			for(int i = 0; i < stackSize; i ++){
				if(thisVelocityStack[i] != Vector2.zero){
					nonZeroCount ++;
					sum += thisVelocityStack[i];
				}
			}
			return sum/ nonZeroCount;
		}
	}
	public abstract class AbsPointerDownInputProcessState<T>: AbsPointerDownInputState, IWaitAndExpireProcessState where T: class, IWaitAndExpireProcess{
		public AbsPointerDownInputProcessState(IUISystemProcessFactory processFactory, IUIAdaptorInputStateEngine engine ,IUIManager uim, int velocityStackSize): base(engine, uim, velocityStackSize){
			thisProcessFactory = processFactory;
		}
		readonly protected IUISystemProcessFactory thisProcessFactory;
		protected T thisProcess;
		public override void OnEnter(){
			base.OnEnter();
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
		public WaitingForFirstTouchState(IUIAdaptorInputStateEngine engine): base(engine){}
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
		/*  In the event of cancel (pointer leaves in bound)
			OnPointerUp is called first, and then OnPointerExit
		*/
		public WaitingForTapState(IUISystemProcessFactory procFac, IUIAdaptorInputStateEngine engine, IUIManager uim, int velocityStackSize): base(procFac, engine, uim, velocityStackSize){
		}
		public override void OnPointerUp(ICustomEventData eventData){
			engine.WaitForNextTouch();

			PushVelocityStack(eventData.velocity);
			Vector2 velocity = GetAverageVelocity();
			eventData.SetVelocity(velocity);

			if(VelocityIsOverSwipeVelocityThreshold(eventData.velocity))
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
		public WaitingForReleaseState(IUISystemProcessFactory procFac, IUIAdaptorInputStateEngine engine, IUIManager uim, int velocityStackSize) :base(procFac, engine, uim, velocityStackSize){
		}
		public override void OnEnter(){
			base.OnEnter();
			engine.ResetTouchCounter();
		}
		public override void OnPointerUp(ICustomEventData eventData){
			engine.WaitForNextTouch();

			PushVelocityStack(eventData.velocity);
			Vector2 velocity = GetAverageVelocity();
			eventData.SetVelocity(velocity);

			if(VelocityIsOverSwipeVelocityThreshold(eventData.velocity))
				engine.SwipeUIE(eventData);
			else
				engine.ReleaseUIE();
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
		public WaitingForNextTouchState(IUISystemProcessFactory procFac, IUIAdaptorInputStateEngine engine) :base(procFac, engine){
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


