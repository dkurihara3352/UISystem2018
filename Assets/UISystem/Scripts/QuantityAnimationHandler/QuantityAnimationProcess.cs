using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IQuantityAnimationProcess: IProcess{
	}
	public abstract class AbsQuantityAnimationProcess<T>: AbsInterpolatorProcess<T>, IQuantityAnimationProcess where T: class, IQuantityAnimationInterpolator{
		public AbsQuantityAnimationProcess(
			IQuantityAnimationProcessConstArg arg
		): base(
			arg
		){
			thisTargetQuantity = arg.targetQuantity;
			thisQuantityRoller = arg.quantityRoller;
		}
		protected readonly int thisTargetQuantity;
		protected readonly IQuantityRoller thisQuantityRoller;
		protected override float GetLatestInitialValueDifference(){
			return thisTargetQuantity/1f - thisQuantityRoller.GetRollerValue();
		}
	}
	public class IncrementalQuantityAnimationProcess: AbsQuantityAnimationProcess<IIncrementalQuantityAnimationInterpolator>, IIncrementalQuantityAnimationProcess{
		public IncrementalQuantityAnimationProcess(
			IQuantityAnimationProcessConstArg arg
		): base(
			arg
		){}
		protected override IIncrementalQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return new IncrementalQuantityAnimationInterpolator(thisTargetQuantity, thisQuantityRoller);
		}
	}
	public interface IOneshotQuantityAnimationProcess: IQuantityAnimationProcess{
	}
	public class OneshotQuantityAnimationProcess: AbsQuantityAnimationProcess<IOneshotQuantityAnimationInterpolator>, IOneshotQuantityAnimationProcess{
		public OneshotQuantityAnimationProcess(
			IQuantityAnimationProcessConstArg arg
		): base(
			arg
		){}
		protected override IOneshotQuantityAnimationInterpolator InstantiateInterpolatorWithValues(){
			return null;
		}
	}
	public interface IQuantityAnimationProcessConstArg: IInterpolatorProcesssConstArg{
		int targetQuantity{get;}
		IQuantityRoller quantityRoller{get;}
	}
	public class QuantityAnimationProcessConstArg: InterpolatorProcessConstArg, IQuantityAnimationProcessConstArg{
		public QuantityAnimationProcessConstArg(
			IProcessManager processManager,
			ProcessConstraint processConstraint,
			float expireTime,
			bool useSpringT,
			
			int targetQuantity,
			IQuantityRoller quantityRoller
		): base(
			processManager,
			processConstraint,
			expireTime,
			true
		){
			thisTargetQuantity = targetQuantity;
			thisQuantityRoller = quantityRoller;
		}
		readonly int thisTargetQuantity;
		public int targetQuantity{get{return thisTargetQuantity;}}
		readonly IQuantityRoller thisQuantityRoller;
		public IQuantityRoller quantityRoller{get{return thisQuantityRoller;}}
	}
}
