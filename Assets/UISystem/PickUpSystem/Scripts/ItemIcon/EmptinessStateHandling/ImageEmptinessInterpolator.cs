using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
namespace UISystem.PickUpUISystem{
	public interface IItemIconImageEmptinessInterpolator: IInterpolator{}
	public class ItemIconImageEmptinessInterpolator: AbsInterpolator, IItemIconImageEmptinessInterpolator{
		public ItemIconImageEmptinessInterpolator(IItemIconImage itemIconImage, float sourceEmptiness, float targetEmptiness){
			thisItemIconImage = itemIconImage;
			thisSourceEmptiness = sourceEmptiness;
			thisTargetEmptiness = targetEmptiness;
		}
		readonly IItemIconImage thisItemIconImage;
		readonly float thisSourceEmptiness;
		readonly float thisTargetEmptiness;
		protected override void InterpolateImple(float normalizedT){
			thisItemIconImage.SetEmptiness(Mathf.Lerp(thisSourceEmptiness, thisTargetEmptiness, normalizedT));
		}
		public override void Terminate(){
			thisItemIconImage.SetEmptiness(thisTargetEmptiness);
		}
	}
}
