using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IQuantityAnimationInterpolator: IInterpolator{}
	public interface IIncrementalQuantityAnimationInterpolator: IQuantityAnimationInterpolator{}
	public class IncrementalQuantityAnimationInterpolator: AbsInterpolator, IIncrementalQuantityAnimationInterpolator{
		public IncrementalQuantityAnimationInterpolator(int targetQuantity, IQuantityRoller quantityRoller){
			thisSourceRollerValue = quantityRoller.GetRollerValue();
			thisTargetRollerValue = targetQuantity/1f;
			thisQuantityRoller = quantityRoller;
		}
		readonly float thisSourceRollerValue;
		readonly float thisTargetRollerValue;
		readonly IQuantityRoller thisQuantityRoller;
		protected override void InterpolateImple(float t){
			float newRollerValue = Mathf.Lerp(thisSourceRollerValue, thisTargetRollerValue, t);
			thisQuantityRoller.SetRollerValue(newRollerValue);
		}
		public override void Terminate(){return;}
	}
	public interface IOneshotQuantityAnimationInterpolator: IQuantityAnimationInterpolator{}
	public interface IIncrementalQuantityAnimationProcess: IQuantityAnimationProcess{
	}
}
