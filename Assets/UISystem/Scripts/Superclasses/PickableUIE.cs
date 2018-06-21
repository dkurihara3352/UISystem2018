using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickableUIE: IUIElement, IPickUpTransactionElement{
		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp(ICustomEventData eventData);
		void CheckForQuickDrop();
		void CheckForDelayedDrop();
	}
	public abstract class AbsPickableUIE: AbsUIElement, IPickableUIE{
		public AbsPickableUIE(IUIElementConstArg arg): base(arg){}
		public override void OnTouch(int touchCount){
			CheckAndCallTouchPickUp(touchCount);
		}
		void CheckAndCallTouchPickUp(int touchCount){
			if(touchCount == 1){
				this.CheckForImmediatePickUp();
			}else{
				if(touchCount == 2){
					this.CheckForSecondTouchPickUp();
				}
			}
			return;
		}
		public override void OnDelayedTouch(){
			this.CheckForDelayedPickUp();
		}
		public override void OnDrag(ICustomEventData eventData){
			this.CheckForDragPickUp(eventData);
		}
		public override void OnRelease(){
			this.CheckForQuickDrop();
		}
		public override void OnDelayedRelease(){
			this.CheckForDelayedDrop();
		}
		public abstract void CheckForImmediatePickUp();
		public abstract void CheckForSecondTouchPickUp();
		public abstract void CheckForDelayedPickUp();
		public abstract void CheckForDragPickUp(ICustomEventData eventData);
		public abstract void CheckForQuickDrop();
		public abstract void CheckForDelayedDrop();
		public abstract void DeclinePickUp();
	}
}
