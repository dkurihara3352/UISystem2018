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
		public ImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient, IProcessManager procMan): base(travelableUIE, procMan){
			thisPUM = pum;
			thisDragThreshold = dragThreshold;
			thisSmoothCoefficient = smoothCoefficient;
		}
		IUIImage thisImage{
			get{return thisTravellingUIE.GetUIImage();}
		}
		readonly IPickUpManager thisPUM;
		readonly float thisDragThreshold;
		readonly float thisSmoothCoefficient;
		public override void UpdateProcess(float deltaT){
			Vector2 imageWorldPosition = thisImage.GetWorldPosition();
			Vector2 dragWorldPosition = thisPUM.GetDragWorldPosition();
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
}
