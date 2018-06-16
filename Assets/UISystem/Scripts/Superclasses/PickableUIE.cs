using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickabilityStateHandler{
		void PickUp();
		void BecomePickable();
		void BecomeUnpickable();
		void Drop();
		bool IsPickable();
		bool IsPicked();
	}
	public interface IPickableUIE: IUIElement, IPickabilityStateHandler, IPickUpTransactionElement{
		bool IsEligibleForQuickDrop();
		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp(Vector2 pos, Vector2 deltaP);
	}
	public abstract class AbsPickableUIE: AbsUIElement, IPickableUIE{
		public AbsPickableUIE(IUIElementConstArg arg): base(arg){}
		public override void OnTouch(int touchCount){
			CheckAndCallTouchPickUp(touchCount);
		}
		void CheckAndCallTouchPickUp(int touchCount){
			if(!this.IsPicked())
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
		public override void OnDrag(Vector2 pos, Vector2 deltaP){
			this.CheckForDragPickUp(pos, deltaP);
		}
		public override void OnRelease(){
			if(this.IsPicked() && this.IsEligibleForQuickDrop())
				this.Drop();
		}
		public override void OnDelayedRelease(){
			if(this.IsPicked())
				this.Drop();
		}
		public abstract void CheckForImmediatePickUp();
		public abstract void CheckForSecondTouchPickUp();
		public abstract void CheckForDelayedPickUp();
		public abstract void CheckForDragPickUp(Vector2 pos, Vector2 deltaP);

		public abstract void PickUp();
		public abstract void BecomePickable();
		public abstract void BecomeUnpickable();
		public abstract bool IsPicked();
		public abstract bool IsPickable();
		public abstract void Drop();
		public abstract bool IsEligibleForQuickDrop();
		public abstract void DeclinePickUp();
	}
}
