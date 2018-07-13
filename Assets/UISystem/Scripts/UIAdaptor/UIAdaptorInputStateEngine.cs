﻿using System.Collections;
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
		float GetSwipeThreshold();
	}
	public class UIAdaptorStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorStateEngine{
		public UIAdaptorStateEngine(IUIManager uim, IUIAdaptor uia, IUISystemProcessFactory procFac){
			thisUIE = uia.GetUIElement();
			thisWaitingForFirstTouchState = new WaitingForFirstTouchState(this);
			thisWaitingForTapState = new WaitingForTapState(this, procFac, uim);
			thisWaitingForReleaseState = new WaitingForReleaseState(this, procFac, uim);
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
