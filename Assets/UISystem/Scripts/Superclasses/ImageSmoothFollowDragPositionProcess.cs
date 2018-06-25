using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IInterpolatorProcess{
		void DisconnectInterpolater(IInterpolator interpolator);
	}
	public interface IImageSmoothFollowDragPositionProcess: IProcess{
	}
	public class ImageSmoothFollowDragPositionProcess: AbsProcess,  IImageSmoothFollowDragPositionProcess {
		public ImageSmoothFollowDragPositionProcess(IUIImage image, IPickUpManager pum, float dragThreshold, float smoothCoefficient, IProcessManager procMan): base(procMan){
			thisImage = image;
			thisPUM = pum;
			thisDragThreshold = dragThreshold;
			thisSmoothCoefficient = smoothCoefficient;
		}
		readonly IUIImage thisImage;
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
		public override void Reset(){

		}
	}
}
