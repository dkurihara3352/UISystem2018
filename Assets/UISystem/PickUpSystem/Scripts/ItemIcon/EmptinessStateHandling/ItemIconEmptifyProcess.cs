using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IItemIconEmptifyProcess: IWaitAndExpireProcess{
	}
	public class ItemIconEmptifyProcess: AbsInterpolatorProcess<IItemIconImageEmptinessInterpolator>, IItemIconEmptifyProcess{
		public ItemIconEmptifyProcess(IProcessManager processManager, float expireT, IItemIconImage itemIconImage, IEmptifyingState state): base(processManager, ProcessConstraint.expireTime, expireT, 0.05f, false, state){
			thisItemIconImage = itemIconImage;
		}
		readonly IItemIconImage thisItemIconImage;
		protected override float GetLatestInitialValueDifference(){
			return 0f - thisItemIconImage.GetEmptiness();
		}
		protected override IItemIconImageEmptinessInterpolator InstantiateInterpolatorWithValues(){
			return new ItemIconImageEmptinessInterpolator(thisItemIconImage, thisItemIconImage.GetEmptiness(), 0f);
		}
	}
}
