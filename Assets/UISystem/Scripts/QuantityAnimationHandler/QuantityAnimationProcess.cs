using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IQuantityAnimationProcess: IProcess{
	}
	public abstract class AbsQuantityAnimationProcess<T>: AbsInterpolatorProcess<T>, IQuantityAnimationProcess where T: class, IQuantityAnimationInterpolator{
		public AbsQuantityAnimationProcess(int targetQuantity, IQuantityRoller quantityRoller, IProcessManager processManager, ProcessConstraint expireTimeConstraint, float expireTime, float differenceThreshold, bool useSpringT): base(processManager, expireTimeConstraint, expireTime, differenceThreshold, useSpringT){
			thisTargetQuantity = targetQuantity;
			thisQuantityRoller = quantityRoller;
		}
		protected readonly int thisTargetQuantity;
		protected readonly IQuantityRoller thisQuantityRoller;
		protected override float GetLatestInitialValueDifference(){
			return thisTargetQuantity/1f - thisQuantityRoller.GetRollerValue();
		}
	}
	public class IncrementalQuantityAnimationProcess: AbsQuantityAnimationProcess<IIncrementalQuantityAnimationInterpolator>, IIncrementalQuantityAnimationProcess{
		public IncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity, IProcessManager processManager, ProcessConstraint expireTimeConstraint, float expireTime, float differenceThreshold, bool useSpringT): base(targetQuantity, quantityRoller, processManager, expireTimeConstraint, expireTime, differenceThreshold, useSpringT){
		}
		protected override IIncrementalQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return new IncrementalQuantityAnimationInterpolator(thisTargetQuantity, thisQuantityRoller);
		}
	}
	public interface IOneshotQuantityAnimationProcess: IQuantityAnimationProcess{
	}
	public class OneshotQuantityAnimationProcess: AbsQuantityAnimationProcess<IOneshotQuantityAnimationInterpolator>, IOneshotQuantityAnimationProcess{
		public OneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity, IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, float diffThreshold, bool useSpringT): base(targetQuantity, quantityRoller, processManager, processConstraint, constraintValue, diffThreshold, useSpringT){
		}
		protected override IOneshotQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return null;
		}
	}
}
