using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IItemIconDisemptifyProcess: IWaitAndExpireProcess{
	}
	public class ItemIconDisemptifyProcess: AbsInterpolatorProcess<IItemIconImageEmptinessInterpolator>, IItemIconDisemptifyProcess{
		public ItemIconDisemptifyProcess(IProcessManager processManager, float expireT, IItemIconImage itemIconImage, IDisemptifyingState state):base(processManager, ProcessConstraint.expireTime, expireT, 0.05f, false, state){
			thisItemIconImage = itemIconImage;
		}
		readonly IItemIconImage thisItemIconImage;
		protected override float GetLatestInitialValueDifference(){
			float curEmptiness = thisItemIconImage.GetEmptiness();
			float targetEmptiness = 1f;
			return targetEmptiness - curEmptiness;
		}
		protected override IItemIconImageEmptinessInterpolator InstantiateInterpolatorWithValues(){
			float curEmptiness = thisItemIconImage.GetEmptiness();
			IItemIconImageEmptinessInterpolator irper = new ItemIconImageEmptinessInterpolator(thisItemIconImage, curEmptiness, 1f);
			return irper;
		}
	}
}
