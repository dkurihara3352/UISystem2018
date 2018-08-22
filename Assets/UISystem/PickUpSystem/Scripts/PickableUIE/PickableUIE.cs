using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickableUIE: ITravelableUIE, IImageSmoothFollowHandler, IVisualPickednessHandler{
		void EvaluatePickability();

		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp(ICustomEventData eventData);
		void CheckForQuickDrop();
		void CheckForDelayedDrop();

		IPickableUIImage GetPickableUIImage();
	}
	public abstract class AbsPickableUIE: UIElement, IPickableUIE{
		public AbsPickableUIE(IPickableUIEConstArg arg): base(arg){
			thisDragImageImplementor = arg.dragImageImplementor;
			thisDragImageImplementor.SetPickableUIE(this);
			thisVisualPickednessStateEngine = arg.visualPickednessStateEngine;
			thisVisualPickednessStateEngine.SetPickableUIImage(thisPickableUIImage);
		}
		IPickableUIImage thisPickableUIImage{
			get{return (IPickableUIImage)thisImage;}
		}
		public IPickableUIImage GetPickableUIImage(){
			return thisPickableUIImage;
		}
		public abstract void EvaluatePickability();
		protected override void OnTouchImple(int touchCount){
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
		protected override void OnDelayedTouchImple(){
			this.CheckForDelayedPickUp();
		}
		protected override void OnDragImple(ICustomEventData eventData){
			this.CheckForDragPickUp(eventData);
		}
		protected override void OnReleaseImple(){
			this.CheckForQuickDrop();
		}
		protected override void OnDelayedReleaseImple(){
			this.CheckForDelayedDrop();
		}
		public abstract void CheckForImmediatePickUp();
		public abstract void CheckForSecondTouchPickUp();
		public abstract void CheckForDelayedPickUp();
		public abstract void CheckForDragPickUp(ICustomEventData eventData);
		public abstract void CheckForQuickDrop();
		public abstract void CheckForDelayedDrop();
		public abstract void DeclinePickUp();
		/* Visual pickedness */
		readonly IVisualPickednessStateEngine thisVisualPickednessStateEngine;
		public void BecomeVisuallyPickedUp(){
			thisVisualPickednessStateEngine.BecomeVisuallyPickedUp();
		}
		public void BecomeVisuallyUnpicked(){
			thisVisualPickednessStateEngine.BecomeVisuallyUnpicked();
		}
		/* ImageSmoothFollowHandler imple */
		readonly IDragImageImplementor thisDragImageImplementor;
		public virtual void StartImageSmoothFollowDragPosition(){
			thisDragImageImplementor.StartImageSmoothFollowDragPosition();
		}
		public virtual void StopImageSmoothFollowDragPosition(){
			thisDragImageImplementor.StopImageSmoothFollowDragPosition();
		}
		/* Travelable UIE implementation */
			/*  updating thisRunningTravelProcess field is taken care by travel process
			*/
		public virtual void HandOverTravel(ITravelableUIE other){
			if(thisRunningTravelProcess != null)
				thisRunningTravelProcess.UpdateTravellingUIEFromTo(this, other);
		}
		ITravelProcess thisRunningTravelProcess;
		public void SetRunningTravelProcess(ITravelProcess travelProcess){
			thisRunningTravelProcess = travelProcess;
		}
		public ITravelProcess GetRunningTravelProcess(){
			return thisRunningTravelProcess;
		}
		public void AbortRunningTravelProcess(){
			thisRunningTravelProcess.UnregisterTravellingUIE(this);
		}
		/* ProcessEventHandler */
		public void OnProcessUpdate(float deltaT){return;}
		public void OnProcessExpire(DKUtility.IProcess expiringProcess){
			if(expiringProcess is ITravelProcess)
				this.SetRunningTravelProcess(null);
		}
	}
	public interface IPickableUIEConstArg: IUIElementConstArg{
		IDragImageImplementor dragImageImplementor{get;}
		IVisualPickednessStateEngine visualPickednessStateEngine{get;}
	}
	public class PickableUIEConstArg: UIElementConstArg, IPickableUIEConstArg{
		public PickableUIEConstArg(
			IUIManager uim, 
			IPickUpSystemProcessFactory pickUpSystemProcessFactory, 
			IPickUpSystemUIElementFactory pickUpSystemUIElementFactory, 
			IUIAdaptor uia, 
			IPickableUIImage pickableUIImage, 
			IUITool tool, 
			IDragImageImplementor dragImageImplementor, 
			IVisualPickednessStateEngine visualPickednessStateEngine
		): base(
			uim, 
			pickUpSystemProcessFactory, 
			pickUpSystemUIElementFactory, 
			uia, 
			pickableUIImage,
			ActivationMode.None
		){
			thisDragImageImplementor = dragImageImplementor;
			thisVisualPickednessStateEngien = visualPickednessStateEngine;
		}
		readonly IDragImageImplementor thisDragImageImplementor;
		public IDragImageImplementor dragImageImplementor{get{return thisDragImageImplementor;}}
		readonly IVisualPickednessStateEngine thisVisualPickednessStateEngien;
		public IVisualPickednessStateEngine visualPickednessStateEngine{get{
			return thisVisualPickednessStateEngien;
		}}
	}
}
