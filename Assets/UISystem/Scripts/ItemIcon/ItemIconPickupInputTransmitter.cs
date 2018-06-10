using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconPickUpInputTransmitter{
		void OnTouch(int touchCount);
		void OnDelayedTouch();
		void OnDrag(Vector2 dragPos, Vector2 deltaP);
	}
	public class ItemIconPickUpInputTransmitter: IItemIconPickUpInputTransmitter{
		readonly IItemIcon itemIcon;
		public void OnTouch(int touchCount){
			if(touchCount == 1){
				itemIcon.CheckForImmediatePickUp();
			}else{
				if(touchCount == 2){
					itemIcon.CheckForSecondTouchPickUp();
				}
			}
			return;
		}
		public void OnDelayedTouch(){
			itemIcon.CheckForDelayedPickUp();
		}
		public void OnDrag(Vector2 dragPos, Vector2 deltaP){
			return;
		}
	}
}
