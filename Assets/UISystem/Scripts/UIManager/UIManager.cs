using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIManager{
		void SetDragWorldPosition(Vector2 dragPos);
		Vector2 GetDragWorldPosition();
		RectTransform GetUIElementReserveTrans();
		int registeredID{get;}
		bool TouchIDIsRegistered();
		void UnregisterTouchID();
		void RegisterTouchID(int touchID);
		bool ShowsInputability();
		bool ShowsNormal();
		void SetInputHandlingScroller(IScroller scroller, UIManager.InputName inputName);
		IScroller GetInputHandlingScroller();
		string GetEventName();
		IPopUpManager GetPopUpManager();
		void GetReadyForUISystemActivation();
		void ActivateUISystem(bool instantly);
		void DeactivateUISystem(bool instantly);
		float GetUIImageDarknedBrightness();
		float GetUIImageDefaultBrightness();
		float GetSwipeVelocityThreshold();
	}
	public class UIManager: IUIManager {
		public UIManager(
			IProcessManager processManager,
			IUIAdaptor rootUIAdaptor,
			RectTransform uieReserveTrans, 
			bool showsInputability,

			float imageDarkenedBrightness,
			float imageDefaultBrightness,

			float swipeVelocityThreshold
		){
			thisProcessManager = processManager;
			thisRootUIAdaptor = rootUIAdaptor;
			thisUIEReserveTrans = uieReserveTrans;
			thisShowsInputability = showsInputability;

			thisImageDarknedBrightness = imageDarkenedBrightness;
			thisImageDefaultBrightnes = imageDefaultBrightness;
			thisPopUpManager = new PopUpManager();

			thisSwipeVelocityThreshold = swipeVelocityThreshold;
		}
		readonly IUIAdaptor thisRootUIAdaptor;
		public void GetReadyForUISystemActivation(){

			IProcessFactory processFactory = CreateProcessFactory(
				thisProcessManager
			);
			IUIElementFactory uiElementFactory = new UIElementFactory(this);

			IUIAdaptorBaseInitializationData rootUIAdaptorBaseInitializationData = CreateRootUIEBaseConstData(
				processFactory,
				uiElementFactory
			);
			thisRootUIAdaptor.GetReadyForActivation(
				rootUIAdaptorBaseInitializationData,
				recursively: true
			);

			thisRootUIElement = thisRootUIAdaptor.GetUIElement();
			thisPopUpManager.SetRootUIElement(thisRootUIElement);
		}
		public void ActivateUISystem(bool instantly){
			thisRootUIElement.ActivateRecursively(instantly);
		}
		public void DeactivateUISystem(bool instantly){
			thisRootUIElement.DeactivateRecursively(instantly);
		}
		readonly IProcessManager thisProcessManager;
		protected virtual IProcessFactory CreateProcessFactory(IProcessManager processManager){
			return new UISystemProcessFactory(
				processManager, 
				this
			);
		}
		protected virtual IUIAdaptorBaseInitializationData CreateRootUIEBaseConstData(
			IProcessFactory processFactory,
			IUIElementFactory uiElementFactory
		){
			return new RootUIAActivationData(
				this,
				(IUISystemProcessFactory)processFactory,
				uiElementFactory
			);
		}
		IUIElement thisRootUIElement;
		Vector2 thisDragWorldPosition;
		IPopUpManager thisPopUpManager;
		public IPopUpManager GetPopUpManager(){return thisPopUpManager;}
		public void SetDragWorldPosition(Vector2 dragPos){
			thisDragWorldPosition = dragPos;
		}
		public Vector2 GetDragWorldPosition(){return thisDragWorldPosition;}
		readonly RectTransform thisUIEReserveTrans;
		public RectTransform GetUIElementReserveTrans(){
			return thisUIEReserveTrans;
		}


		/* Touch management */
		const int noFingerID = -10;
		int thisRegisteredID = -10;
		public int registeredID{get{return thisRegisteredID;}}
		public bool TouchIDIsRegistered(){
			return thisRegisteredID != noFingerID;
		}
		public void UnregisterTouchID(){
			thisRegisteredID = noFingerID;
		}
		public void RegisterTouchID(int touchID){
			thisRegisteredID = touchID;
		}
		/*  */
		readonly float thisImageDarknedBrightness;
		public float GetUIImageDarknedBrightness(){
			return thisImageDarknedBrightness;
		}
		readonly float thisImageDefaultBrightnes;
		public float GetUIImageDefaultBrightness(){
			return thisImageDefaultBrightnes;
		}


		/* Debug */
		readonly bool thisShowsInputability;
		public bool ShowsInputability(){
			return thisShowsInputability;
		}
		public bool ShowsNormal(){
			return !ShowsInputability();
		}
		public void SetInputHandlingScroller(IScroller scroller, InputName inputName){
			thisInputHandlingScroller = scroller;
			thisInputName = inputName;
		}
		InputName thisInputName = InputName.None;
		IScroller thisInputHandlingScroller;
		public IScroller GetInputHandlingScroller(){
			return thisInputHandlingScroller;
		}
		public string GetEventName(){
			return thisInputName.ToString();
		}
		public float GetSwipeVelocityThreshold(){
			return thisSwipeVelocityThreshold;
		}
		readonly float thisSwipeVelocityThreshold;
		public enum InputName{
			None,
			Release,
			Tap,
			Swipe,
			BeginDrag,
			Drag,
			Touch,
		}
	}
}
