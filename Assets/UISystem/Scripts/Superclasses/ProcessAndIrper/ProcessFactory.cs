using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProcessFactory{
		ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
		IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient);
		IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IUIImage image, int sourceQuantity, int targetQuantity);
		IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IUIImage image, int sourceQuantity, int targetQuantity);
		IItemIconDisemptifyProcess CreateItemIconDisemptifyProcess(IDisemptifyingState disemptifyingState, IItemIconImage uiImage);
		IItemIconEmptifyProcess CreateItemIconEmptifyProcess(IEmptifyingState emptifyingState, IItemIconImage uiImage, IItemIcon itemIcon);
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
			IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(thisProcessManager, state, waitTime);
			return process;
		}
		public IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient){
			ImageSmoothFollowDragPositionProcess process = new ImageSmoothFollowDragPositionProcess(travelableUIE, pum, dragThreshold, smoothCoefficient, thisProcessManager);
			return process;
		}
		public IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IUIImage image, int sourceQuantity, int targetQuantity){
			IncrementalQuantityAnimationProcess process = new IncrementalQuantityAnimationProcess(thisProcessManager, image, sourceQuantity, targetQuantity);
			return process;
		}
		public IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IUIImage image, int sourceQuantity, int targetQuantity){
			OneshotQuantityAnimationProcess process = new OneshotQuantityAnimationProcess(thisProcessManager, image, sourceQuantity, targetQuantity);
			return process;
		}
		public IItemIconDisemptifyProcess CreateItemIconDisemptifyProcess(IDisemptifyingState disemptifyingState, IItemIconImage itemIconImage){
			float expireT = thisProcessManager.GetImageEmptificationExpireTime();
			IItemIconDisemptifyProcess process = new ItemIconDisemptifyProcess(thisProcessManager, disemptifyingState, expireT, itemIconImage);
			return process;
		}
		public IItemIconEmptifyProcess CreateItemIconEmptifyProcess(IEmptifyingState emptifyingState, IItemIconImage itemIconImage, IItemIcon itemIcon){
			float expireT = thisProcessManager.GetImageEmptificationExpireTime();
			IItemIconEmptifyProcess process = new ItemIconEmptifyProcess(thisProcessManager, emptifyingState, expireT, itemIconImage, itemIcon);
			return process;
		}
	}
}
