using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IInterpolatorProcess{
		void DisconnectInterpolater(IInterpolator interpolator);
	}
	public interface IImageSmoothFollowDragPositionProcess: ITravelProcess{
	}
	public class ImageSmoothFollowDragPositionProcess: AbsSingleTravellerTravelProcess,  IImageSmoothFollowDragPositionProcess {
		public ImageSmoothFollowDragPositionProcess(IImageSmoothFollowDragPositionProcessConstArg arg): base(arg){
			thisPickUpManager = arg.pickUpManager;
			thisDragThreshold = arg.dragThreshold;
			thisSmoothCoefficient = arg.smoothCoefficient;
		}
		IUIImage thisImage{
			get{return thisTravellingUIE.GetUIImage();}
		}
		readonly IPickUpManager thisPickUpManager;
		readonly float thisDragThreshold;
		readonly float thisSmoothCoefficient;
		public override void UpdateProcess(float deltaT){
			Vector2 imageWorldPosition = thisImage.GetWorldPosition();
			Vector2 dragWorldPosition = thisPickUpManager.GetDragWorldPosition();
			Vector2 displacement = dragWorldPosition - imageWorldPosition;
			float displacementSqrMagnitude = displacement.sqrMagnitude;
			if(displacementSqrMagnitude > thisDragThreshold * thisDragThreshold){
				Vector2 deltaPosition = CalcDeltaPosition(deltaT, displacement);
				Vector2 newWorldPosition = imageWorldPosition + deltaPosition;
				thisImage.SetWorldPosition(newWorldPosition);
			}
		}
		Vector2 CalcDeltaPosition(float deltaT, Vector2 displacement){
			return displacement * thisSmoothCoefficient * deltaT; 
		}
	}
	public interface IImageSmoothFollowDragPositionProcessConstArg: ITravelProcessConstArg{
		IPickUpManager pickUpManager{get;}
		float dragThreshold{get;}
		float smoothCoefficient{get;}
	}
	public class ImageSmoothFollowDragPositionProcessConstArg: TravelProcessConstArg, IImageSmoothFollowDragPositionProcessConstArg{
		public ImageSmoothFollowDragPositionProcessConstArg(
			IProcessManager processManager,
			ITravelableUIE travelableUIE,
			IPickUpManager pickUpManager,
			float dragThreshold,
			float smoothCoefficient
		): base(
			processManager,
			travelableUIE
		){
			thisPickUpManager = pickUpManager;
			thisDragThreshold = dragThreshold;
			thisSmoothCoefficient = smoothCoefficient;
		}
		readonly IPickUpManager thisPickUpManager;
		public IPickUpManager pickUpManager{get{return thisPickUpManager;}}
		readonly float thisDragThreshold;
		public float dragThreshold{get{return thisDragThreshold;}}
		readonly float thisSmoothCoefficient;
		public float smoothCoefficient{get{return thisSmoothCoefficient;}}
	}
}
