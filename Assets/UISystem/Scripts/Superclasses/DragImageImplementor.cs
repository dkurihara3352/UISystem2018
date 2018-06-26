using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
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
			thisProcessFactory = arg.processFactory;
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
		readonly IProcessFactory thisProcessFactory;
		readonly IPickUpManager thisPickUpManager;
	}
	public interface IDragImageImplementorConstArg{
		float dragThreshold{get;}
		float smoothCoefficient{get;}
		IProcessFactory processFactory{get;}
		IPickUpManager pickUpManager{get;}
	}
	public class DragImageImplementorConstArg: IDragImageImplementorConstArg{
		public DragImageImplementorConstArg(float dragThreshold, float smoothCoefficient, IProcessFactory processFactory, IPickUpManager pickUpManager){
			thisDragThreshold = dragThreshold;
			thisSmoothCoefficient = smoothCoefficient;
			thisProcessFactory = processFactory;
			thisPickUpManager = pickUpManager;
		}
		readonly float thisDragThreshold;
		public float dragThreshold{get{return thisDragThreshold;}}
		readonly float thisSmoothCoefficient;
		public float smoothCoefficient{get{return thisSmoothCoefficient;}}
		readonly IProcessFactory thisProcessFactory;
		public IProcessFactory processFactory{get{return thisProcessFactory;}}
		readonly IPickUpManager thisPickUpManager;
		public IPickUpManager pickUpManager{get{return thisPickUpManager;}}

	}
}
