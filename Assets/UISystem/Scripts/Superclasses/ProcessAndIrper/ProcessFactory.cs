using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProcessFactory{
		ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
		IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient);
	}
	public class ProcessFactory: IProcessFactory{
		public ProcessFactory(IProcessManager procManager, IUIManager uim){
			if(procManager != null)
				thisProcessManager = procManager;
			else
				throw new System.ArgumentNullException("procManager", "ProcessFactory does not operate without a procManager");
			if(uim != null)
				thisUIManager = uim;
			else
				throw new System.ArgumentNullException("uim", "ProcessFactory does not operate without a uim");

		}
		readonly IProcessManager thisProcessManager;
		readonly IUIManager thisUIManager;
		public ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness){
			ITurnImageDarknessProcess process = new TurnImageDarknessProcess(thisProcessManager, image, targetDarkness);
			return process;
		}
		public IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime){
			IWaitAndExpireProcess process = new WaitAndExpireProcess(thisProcessManager, state, waitTime);
			return process;
		}
		public IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient){
			ImageSmoothFollowDragPositionProcess process = new ImageSmoothFollowDragPositionProcess(travelableUIE, pum, dragThreshold, smoothCoefficient, thisProcessManager);
			return process;
		}
	}
}
