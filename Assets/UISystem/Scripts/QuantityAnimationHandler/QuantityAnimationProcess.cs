using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityAnimationProcess: IProcess{
	}
	public abstract class AbsQuantityAnimationProcess<T>: AbsInterpolatorProcess<T>, IQuantityAnimationProcess where T: class, IQuantityAnimationInterpolator{
		public AbsQuantityAnimationProcess(int targetQuantity, IQuantityRoller quantityRoller, IProcessManager processManager, ProcessConstraint expireTimeConstraint, float expireTime, IWaitAndExpireProcessState processState, float differenceThreshold, bool useSpringT): base(processManager, expireTimeConstraint, expireTime, processState, differenceThreshold, useSpringT){
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
		public IncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity, IProcessManager processManager, ProcessConstraint expireTimeConstraint, float expireTime, IWaitAndExpireProcessState processState, float differenceThreshold, bool useSpringT): base(targetQuantity, quantityRoller, processManager, expireTimeConstraint, expireTime, processState, differenceThreshold, useSpringT){
		}
		protected override IIncrementalQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return new IncrementalQuantityAnimationInterpolator(thisTargetQuantity, thisQuantityRoller);
		}
	}
	public interface IOneshotQuantityAnimationProcess: IQuantityAnimationProcess{
	}
	public class OneshotQuantityAnimationProcess: AbsQuantityAnimationProcess<IOneshotQuantityAnimationInterpolator>, IOneshotQuantityAnimationProcess{
		public OneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity, IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, IWaitAndExpireProcessState processState, float diffThreshold, bool useSpringT): base(targetQuantity, quantityRoller, processManager, processConstraint, constraintValue, processState, diffThreshold, useSpringT){
		}
		protected override IOneshotQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return null;
		}
	}
}
