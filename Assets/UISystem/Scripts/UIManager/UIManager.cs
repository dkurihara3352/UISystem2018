using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIManager{
		void SetDragWorldPosition(Vector2 dragPos);
		Vector2 GetDragWorldPosition();
		Transform GetUIElementReserveTrans();
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
	}
}
