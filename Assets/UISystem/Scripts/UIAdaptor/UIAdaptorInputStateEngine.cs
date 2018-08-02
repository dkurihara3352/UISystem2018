using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
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
		float GetSwipeVelocityThreshold();
	}
	public class UIAdaptorStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorStateEngine{
		public UIAdaptorStateEngine(IUIManager uim, IUIAdaptor uia, IUISystemProcessFactory procFac){
			thisUIE = uia.GetUIElement();
			thisWaitingForFirstTouchState = new WaitingForFirstTouchState(this);
			thisWaitingForTapState = new WaitingForTapState(procFac, this, uim);
			thisWaitingForReleaseState = new WaitingForReleaseState(procFac, this, uim);
			thisWaitingForNextTouchState = new WaitingForNextTouchState(procFac, this);
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
		bool DragVelocityIsOverThreshold(Vector2 dragVelocity){
			return dragVelocity.sqrMagnitude >= dragVelocityThreshold * dragVelocityThreshold;
		}
		protected float dragVelocityThreshold = 5f;
		public void DragUIE(ICustomEventData eventData){
			thisUIE.OnDrag(eventData);
		}
		public void HoldUIE(float deltaT){
			thisUIE.OnHold(deltaT);
		}
		public void SwipeUIE(ICustomEventData eventData){
			thisUIE.OnSwipe(eventData);
		}
		public float GetSwipeVelocityThreshold(){
			return thisSwipeVelocityThreshold;
		}
		float thisSwipeVelocityThreshold = 5f;
		/* IRawInputHandler */
			public void OnPointerDown(ICustomEventData eventData){
				thisCurState.OnPointerDown(eventData);
			}
			public void OnPointerUp(ICustomEventData eventData){
				thisCurState.OnPointerUp(eventData);
			}
			public void OnDrag(ICustomEventData eventData){
				if(DragVelocityIsOverThreshold(eventData.velocity))
					thisCurState.OnDrag(eventData);
			}
			public void OnPointerEnter(ICustomEventData eventData){
				thisCurState.OnPointerEnter(eventData);
			}
			public void OnPointerExit(ICustomEventData eventData){
				thisCurState.OnPointerExit(eventData);
			}
			public void OnCancel(){
				thisCurState.OnCancel();
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
