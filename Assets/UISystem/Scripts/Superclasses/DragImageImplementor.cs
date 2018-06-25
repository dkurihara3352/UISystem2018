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

		public void SetPickableUIE(IPickableUIE pickableUIE){
			thisPickableUIE = pickableUIE;
		}
		IPickableUIE thisPickableUIE;
		public void StartImageSmoothFollowDragPosition(){
			StopRunningTravelIrper();
			/*  SetRunningTravelIrper on the ii
				SetDrivingProcess on the irper with newly created process
			*/
		}
		public void StopImageSmoothFollowDragPosition(){
			StopRunningTravelIrper();
		}
		void StopRunningTravelIrper(){
			thisPickableUIE.AbortRunningTravelInterpolator();
		}
		ITravelInterpolator thisRunningTravelIrper;
		IImageSmoothFollowDragPositionProcess thisRunningSmoothFollowProcess;
		readonly float thisDragThreshold;
		readonly float thisSmoothCoefficient;
		readonly IProcessFactory thisProcessFactory;
	}
}
