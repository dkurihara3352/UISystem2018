using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IImageSmoothFollowHandler{
		void StartImageSmoothFollowDragPosition();
		void StopImageSmoothFollowDragPosition();
	}
	public interface IDragImageImplementor: IImageSmoothFollowHandler{
		void SetPickableUIE(IPickableUIE pickableUIE);
	}
	public class DragImageImplementor: IDragImageImplementor{
		public DragImageImplementor(IDragImageImplementorConstArg arg){
			thisDragThreshold = arg.dragThreshold;
			thisSmoothCoefficient = arg.smoothCoefficient;
			thisProcessFactory = arg.pickUpSystemProcessFactory;
		}
		public void SetPickableUIE(IPickableUIE pickableUIE){
			thisPickableUIE = pickableUIE;
		}
		IPickableUIE thisPickableUIE;
		public void StartImageSmoothFollowDragPosition(){
			StopRunningTravelProcess();
			IImageSmoothFollowDragPositionProcess smoothFollowProcess = thisProcessFactory.CreateImageSmoothFollowDragPositionProcess(thisPickableUIE, thisPickUpManager, thisDragThreshold, thisSmoothCoefficient);
			smoothFollowProcess.Run();
		}
		public void StopImageSmoothFollowDragPosition(){
			StopRunningTravelProcess();
		}
		void StopRunningTravelProcess(){
			thisPickableUIE.AbortRunningTravelProcess();
		}
		readonly float thisDragThreshold;
		readonly float thisSmoothCoefficient;
		readonly IPickUpSystemProcessFactory thisProcessFactory;
		readonly IPickUpManager thisPickUpManager;
	}
	public interface IDragImageImplementorConstArg{
		float dragThreshold{get;}
		float smoothCoefficient{get;}
		IPickUpSystemProcessFactory pickUpSystemProcessFactory{get;}
		IPickUpManager pickUpManager{get;}
	}
	public class DragImageImplementorConstArg: IDragImageImplementorConstArg{
		public DragImageImplementorConstArg(float dragThreshold, float smoothCoefficient, IPickUpSystemProcessFactory pickUpSystemProcessFactory, IPickUpManager pickUpManager){
			thisDragThreshold = dragThreshold;
			thisSmoothCoefficient = smoothCoefficient;
			thisPickUpSystemProcessFactory = pickUpSystemProcessFactory;
			thisPickUpManager = pickUpManager;
		}
		readonly float thisDragThreshold;
		public float dragThreshold{get{return thisDragThreshold;}}
		readonly float thisSmoothCoefficient;
		public float smoothCoefficient{get{return thisSmoothCoefficient;}}
		readonly IPickUpSystemProcessFactory thisPickUpSystemProcessFactory;
		public IPickUpSystemProcessFactory pickUpSystemProcessFactory{get{return thisPickUpSystemProcessFactory;}}
		readonly IPickUpManager thisPickUpManager;
		public IPickUpManager pickUpManager{get{return thisPickUpManager;}}

	}
}
