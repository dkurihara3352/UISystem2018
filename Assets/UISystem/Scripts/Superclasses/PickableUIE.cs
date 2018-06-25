using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITravelableUIE: IUIElement{
		void SetRunningTravelInterpolator(ITravelInterpolator travelIrper);
		void AbortRunningTravelInterpolator();
	}
	public interface IPickableUIE: ITravelableUIE, IImageSmoothFollowHandler{
		void EvaluatePickability();

		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp(ICustomEventData eventData);
		void CheckForQuickDrop();
		void CheckForDelayedDrop();

		void BecomeVisuallyPickedUp();
		void BecomeVisuallyUnpicked();
	}
	public abstract class AbsPickableUIE: AbsUIElement, IPickableUIE{
		public AbsPickableUIE(IPickableUIEConstArg arg): base(arg){
			thisDragImageImplementor = arg.dragImageImplementor;
			thisDragImageImplementor.SetPickableUIE(this);
		}
		public abstract void EvaluatePickability();
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

		public abstract void BecomeVisuallyPickedUp();
		public abstract void BecomeVisuallyUnpicked();

		readonly IDragImageImplementor thisDragImageImplementor;
		public virtual void StartImageSmoothFollowDragPosition(){
			thisDragImageImplementor.StartImageSmoothFollowDragPosition();
		}
		public virtual void StopImageSmoothFollowDragPosition(){}
		ITravelInterpolator thisRunningTravelInterpolator;
		public void SetRunningTravelInterpolator(ITravelInterpolator travelIrper){
			thisRunningTravelInterpolator = travelIrper;
		}
		public void AbortRunningTravelInterpolator(){
			thisRunningTravelInterpolator.DisconnectFromDrivingInterpolatorProcess();
			SetRunningTravelInterpolator(null);
		}
	}
	public interface IPickableUIEConstArg: IUIElementConstArg{
		IDragImageImplementor dragImageImplementor{get;}
	}
	public class PickableUIEConstArg: UIElementConstArg, IPickableUIEConstArg{
		public PickableUIEConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, IDragImageImplementor dragImageImplementor): base(uim, uia, image){
			thisDragImageImplementor = dragImageImplementor;
		}
		readonly IDragImageImplementor thisDragImageImplementor;
		public IDragImageImplementor dragImageImplementor{get{return thisDragImageImplementor;}}
	}
}
