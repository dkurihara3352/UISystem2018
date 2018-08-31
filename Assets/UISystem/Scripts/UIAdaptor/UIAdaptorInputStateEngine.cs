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
	public interface IUIAdaptorInputStateEngine: ISwitchableStateEngine<IUIAdaptorInputState>, IRawInputHandler, IUIAdaptorStateHandler{
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
		void BeginDragUIE(ICustomEventData eventData);
		void DragUIE(ICustomEventData eventData);
		void HoldUIE(float deltaT);
		void SwipeUIE(ICustomEventData eventData);
		float GetSwipeVelocityThreshold();
		string GetName();
	}
	public class UIAdaptorInputStateEngine: AbsSwitchableStateEngine<IUIAdaptorInputState> ,IUIAdaptorInputStateEngine{
		public UIAdaptorInputStateEngine(
			IUIAdaptorInputStateEngineConstArg arg
		){
			IUIAdaptor uia = arg.uiAdaptor;
			thisUIE = arg.uiElement;
			IUISystemProcessFactory procFac = arg.processFactory;
			thisUIManager = arg.uiManager;

			IUIAdaptorInputStateConstArg pointerUpInputStateArg = new UIAdaptorInputStateConstArg(
				this
			);
			thisWaitingForFirstTouchState = new WaitingForFirstTouchState(
				pointerUpInputStateArg
			);

			IPointerDownInputProcessStateConstArg pointerDownProcessStateArg = new PointerDownInputProcessStateConstArg(
				this,
				thisUIManager,
				thisVelocityStackSize,
				procFac
			);
			thisWaitingForTapState = new WaitingForTapState(
				pointerDownProcessStateArg
			);
			thisWaitingForReleaseState = new WaitingForReleaseState(
				pointerDownProcessStateArg
			);

			IPointerUpInputProcessStateConstArg pointerUpInputProcessStateArg = new PointerUpInputProcessStateConstArg(
				this,
				procFac
			);
			thisWaitingForNextTouchState = new WaitingForNextTouchState(
				pointerUpInputProcessStateArg
			);
			SetWithInitState();
			ResetTouchCounter();
		}
		readonly IUIManager thisUIManager;
		public string GetName(){
			return thisUIE.GetName();
		}
		const int thisVelocityStackSize = 3;
		readonly IUIElement thisUIE;
		void SetWithInitState(){
			this.WaitForFirstTouch();
		}
		protected int thisTouchCount;
		public void ResetTouchCounter(){
			thisTouchCount = 0;
		}
		public void IncrementTouchCounter(){
			thisTouchCount ++;
		}
		public int GetTouchCount(){
			return thisTouchCount;
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
		public void BeginDragUIE(ICustomEventData eventData){
			thisUIE.OnBeginDrag(eventData);
		}
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
			return thisUIManager.GetSwipeVelocityThreshold();
		}
		// float thisSwipeVelocityThreshold = 200f;
		/* IRawInputHandler */
			public void OnPointerDown(ICustomEventData eventData){
				thisCurState.OnPointerDown(eventData);
			}
			public void OnPointerUp(ICustomEventData eventData){
				thisCurState.OnPointerUp(eventData);
			}
			public void OnBeginDrag(ICustomEventData eventData){
				thisCurState.OnBeginDrag(eventData);
			}
			public void OnDrag(ICustomEventData eventData){
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
	public interface IUIAdaptorInputStateEngineConstArg{
		IUIManager uiManager{get;}
		IUIAdaptor uiAdaptor{get;}
		IUIElement uiElement{get;}
		IUISystemProcessFactory processFactory{get;}
	}
	public class UIAdaptorInputStateEngineConstArg: IUIAdaptorInputStateEngineConstArg{
		public UIAdaptorInputStateEngineConstArg(
			IUIManager uiManager,
			IUIElement uiElement,
			IUIAdaptor uiAdaptor,
			IUISystemProcessFactory processFactory
		){
			thisUIManager = uiManager;
			thisUIAdaptor = uiAdaptor;
			thisUIElement = uiElement;
			thisProcessFactory = processFactory;
		}
		readonly IUIManager thisUIManager;
		public IUIManager uiManager{get{return thisUIManager;}}
		readonly IUIAdaptor thisUIAdaptor;
		public IUIAdaptor uiAdaptor{get{return thisUIAdaptor;}}
		readonly IUIElement thisUIElement;
		public IUIElement uiElement{get{return thisUIElement;}}
		readonly IUISystemProcessFactory thisProcessFactory;
		public IUISystemProcessFactory processFactory{get{return thisProcessFactory;}}
	}
}
