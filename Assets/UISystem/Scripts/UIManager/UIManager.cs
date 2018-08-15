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
		IScroller GetLatestScrollerInMotion();
		void SetLatestScrollerInMotion(IScroller scroller);
		void SetInputHandlingScroller(IScroller scroller, UIManager.InputName inputName);
		IScroller GetInputHandlingScroller();
		string GetEventName();
	}
	public class UIManager: IUIManager {
		public UIManager(RectTransform uieReserveTrans, bool showsInputability){
			thisUIEReserveTrans = uieReserveTrans;
			thisShowsInputability = showsInputability;
		}
		Vector2 thisDragWorldPosition;
		public void SetDragWorldPosition(Vector2 dragPos){
			thisDragWorldPosition = dragPos;
		}
		public Vector2 GetDragWorldPosition(){return thisDragWorldPosition;}
		readonly RectTransform thisUIEReserveTrans;
		public RectTransform GetUIElementReserveTrans(){
			return thisUIEReserveTrans;
		}
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
		readonly bool thisShowsInputability;
		public bool ShowsInputability(){
			return thisShowsInputability;
		}
		public bool ShowsNormal(){
			return !ShowsInputability();
		}

		IScroller thisLatestScrollerInMotion;
		public void SetLatestScrollerInMotion(IScroller scroller){
			thisLatestScrollerInMotion = scroller;
		}
		public IScroller GetLatestScrollerInMotion(){
			return thisLatestScrollerInMotion;
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
