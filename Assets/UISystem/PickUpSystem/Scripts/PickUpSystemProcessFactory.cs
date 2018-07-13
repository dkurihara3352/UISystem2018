using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemProcessFactory: IUISystemProcessFactory{
 		IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient);
		IItemIconDisemptifyProcess CreateItemIconDisemptifyProcess(IDisemptifyingState disemptifyingState, IItemIconImage uiImage);
		IItemIconEmptifyProcess CreateItemIconEmptifyProcess(IEmptifyingState emptifyingState, IItemIconImage uiImage, IItemIcon itemIcon);
		IVisualPickednessProcess CreateVisualPickednessProcess(IWaitAndExpireProcessState state, IPickableUIImage pickableUIImage, float sourcePickedness, float targetPickedness);
	}
	public class PickUpSystemProcessFactory: UISystemProcessFactory, IPickUpSystemProcessFactory{
		public PickUpSystemProcessFactory(IProcessManager processManager, IUIManager uim): base(processManager, uim){}
		public IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient){
			ImageSmoothFollowDragPositionProcess process = new ImageSmoothFollowDragPositionProcess(travelableUIE, pum, dragThreshold, smoothCoefficient, thisProcessManager);
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
		public IVisualPickednessProcess CreateVisualPickednessProcess(IWaitAndExpireProcessState state, IPickableUIImage image, float sourcePickedness, float targetPickedness){
			float expireT = thisProcessManager.GetVisualPickednessProcessExpireTime();
			IVisualPickednessProcess process = new VisualPickednessProcess(thisProcessManager, state, expireT, image, sourcePickedness, targetPickedness);
			return process;
		}
	}
}
