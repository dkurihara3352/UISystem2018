using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemProcessFactory: IUISystemProcessFactory{
 		IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(
			 ITravelableUIE travelableUIE, 
			 IPickUpManager pum, 
			 float dragThreshold, 
			 float smoothCoefficient
		);
		IItemIconDisemptifyProcess CreateItemIconDisemptifyProcess(
			IItemIconImage uiImage,
			IItemIconEmptinessStateEngine engine
		);
		IItemIconEmptifyProcess CreateItemIconEmptifyProcess(
			IItemIconImage itemIconImage, 
			IItemIconEmptinessStateEngine engine,
			IItemIcon itemIcon,
			bool removesEmpty
		);
		IVisualPickednessProcess CreateVisualPickednessProcess(
			IPickableUIImage pickableUIImage, 
			float targetPickedness,
			IVisualPickednessStateEngine engine,
			bool becomePicked
		);
	}
	public class PickUpSystemProcessFactory: UISystemProcessFactory, IPickUpSystemProcessFactory{
		public PickUpSystemProcessFactory(IProcessManager processManager, IUIManager uim): base(processManager, uim){}
		public IImageSmoothFollowDragPositionProcess CreateImageSmoothFollowDragPositionProcess(ITravelableUIE travelableUIE, IPickUpManager pum, float dragThreshold, float smoothCoefficient){
			IImageSmoothFollowDragPositionProcessConstArg arg = new ImageSmoothFollowDragPositionProcessConstArg(
				thisProcessManager, 
				travelableUIE,
				pum,
				dragThreshold,
				smoothCoefficient
			);
			ImageSmoothFollowDragPositionProcess process = new ImageSmoothFollowDragPositionProcess(arg);
			return process;
		}
		public IItemIconDisemptifyProcess CreateItemIconDisemptifyProcess(
			IItemIconImage itemIconImage,
			IItemIconEmptinessStateEngine engine
		){
			float expireT = thisProcessManager.GetImageEmptificationExpireTime();
			IItemIconEmptificationProcessConstArg arg = new ItemIconEmptificationProcessConstArg(
				thisProcessManager,
				expireT,
				itemIconImage,
				engine
			);
			IItemIconDisemptifyProcess process = new ItemIconDisemptifyProcess(arg);
			return process;
		}
		public IItemIconEmptifyProcess CreateItemIconEmptifyProcess(
			IItemIconImage itemIconImage, 
			IItemIconEmptinessStateEngine engine,
			IItemIcon itemIcon,
			bool removesEmpty
		){
			float expireT = thisProcessManager.GetImageEmptificationExpireTime();
			IItemIconEmptifyProcessConstArg arg = new ItemIconEmptifyProcessConstArg(
				thisProcessManager,
				expireT,
				itemIconImage,
				engine,
				itemIcon,
				removesEmpty
			);
			IItemIconEmptifyProcess process = new ItemIconEmptifyProcess(arg);
			return process;
		}
		public IVisualPickednessProcess CreateVisualPickednessProcess(
			IPickableUIImage image, 
			float targetPickedness,
			IVisualPickednessStateEngine engine,
			bool becomePicked
		){
			float expireT = thisProcessManager.GetVisualPickednessProcessExpireTime();
			IVisualPickednessProcessConstArg arg = new VisualPickednessProcessConstArg(
				thisProcessManager,
				ProcessConstraint.ExpireTime,
				expireT,
				false,

				image,
				targetPickedness,
				engine
			);
			IVisualPickednessProcess process;
			if(becomePicked)
				process = new BecomeVisuallyPickedProcess(arg);
			else	
				process = new BecomeVisuallyUnpickedrocess(arg);
			return process;
		}
	}
}
