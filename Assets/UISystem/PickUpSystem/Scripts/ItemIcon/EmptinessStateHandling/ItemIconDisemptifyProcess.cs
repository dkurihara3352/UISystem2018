using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconDisemptifyProcess: IWaitAndExpireProcess{
	}
	public class ItemIconDisemptifyProcess: AbsWaitAndExpireProcess, IItemIconDisemptifyProcess{
		public ItemIconDisemptifyProcess(IProcessManager processManager, IDisemptifyingState disemptifyingState, float expireT, IItemIconImage itemIconImage): base(processManager, disemptifyingState, expireT){
			float currentEmptiness = itemIconImage.GetEmptiness();
			thisImageEmptinessInterpolator = new ItemIconImageEmptinessInterpolator(itemIconImage, currentEmptiness, 1f);
		}
		IItemIconImageEmptinessInterpolator thisImageEmptinessInterpolator;
		protected override void UpdateProcessImple(float deltaT){
			thisImageEmptinessInterpolator.Interpolate(thisNormlizedT);
		}
		public override void Expire(){
			base.Expire();
			thisImageEmptinessInterpolator.Terminate();
		}
	}
}
