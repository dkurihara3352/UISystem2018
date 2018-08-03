using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIManager{
		void SetDragWorldPosition(Vector2 dragPos);
		Vector2 GetDragWorldPosition();
		Transform GetUIElementReserveTrans();
		int registeredID{get;}
		bool TouchIDIsRegistered();
		void UnregisterTouchID();
		void RegisterTouchID(int touchID);
	}
	public class UIManager: IUIManager {
		public UIManager(Transform uieReserveTrans){
			thisUIEReserveTrans = uieReserveTrans;
		}
		Vector2 thisDragWorldPosition;
		public void SetDragWorldPosition(Vector2 dragPos){
			thisDragWorldPosition = dragPos;
		}
		public Vector2 GetDragWorldPosition(){return thisDragWorldPosition;}
		readonly Transform thisUIEReserveTrans;
		public Transform GetUIElementReserveTrans(){
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
	}
}
