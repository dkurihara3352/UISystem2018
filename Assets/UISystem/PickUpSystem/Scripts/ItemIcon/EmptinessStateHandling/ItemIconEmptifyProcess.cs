using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconEmptifyProcess: IWaitAndExpireProcess{
		void ToggleRemoval(bool removesEmpty);
	}
	public class ItemIconEmptifyProcess: AbsWaitAndExpireProcess, IItemIconEmptifyProcess{
		public ItemIconEmptifyProcess(IProcessManager processManager, IEmptifyingState disemptifyingState, float expireT, IItemIconImage itemIconImage, IItemIcon itemIcon): base(processManager, disemptifyingState, expireT){
			float currentEmptiness = itemIconImage.GetEmptiness();
			thisImageEmptinessInterpolator = new ItemIconImageEmptinessInterpolator(itemIconImage, currentEmptiness, 0f);
		}
		readonly IItemIcon thisItemIcon;
		bool thisRemovesEmpty;
		public void ToggleRemoval(bool removesEmpty){
			thisRemovesEmpty = removesEmpty;
		}
		IItemIconImageEmptinessInterpolator thisImageEmptinessInterpolator;
		protected override void UpdateProcessImple(float deltaT){
			thisImageEmptinessInterpolator.Interpolate(thisNormlizedT);
		}
		public override void Expire(){
			if(thisRemovesEmpty){
				IIconGroup ig = thisItemIcon.GetIconGroup();
				ig.RemoveIIAndMutate(thisItemIcon);
			}
			base.Expire();
			thisImageEmptinessInterpolator.Terminate();
		}
		public override void Reset(){
			thisRemovesEmpty = false;
		}
	}
}
