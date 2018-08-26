using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public abstract class AbsInterpolatorProcess<T>: AbsConstrainedProcess where T: class, IInterpolator{
		public AbsInterpolatorProcess(
			IInterpolatorProcesssConstArg arg
		): base(
			arg
		){
			thisUseSpringT = arg.useSpringT;
		}
		readonly bool thisUseSpringT;
		protected float thisNormalizedT{
			get{
				if(thisElapsedT == 0f)
					return 0f;
				else{
					float result = thisElapsedT/ thisExpireT;
					if(result > 1f)
						result = 1f;
					return result;
				}
			}
		}
		protected T thisInterpolator;
		protected abstract T InstantiateInterpolatorWithValues();
		public override void Run(){
			thisInterpolator = InstantiateInterpolatorWithValues();
			base.Run();
		}
		protected override void UpdateProcessImple(float deltaT){
			float t = thisUseSpringT? GetSpringT(thisNormalizedT): thisNormalizedT;
			thisInterpolator.Interpolate(t);
		}
		public override void Expire(){
			this.Stop();
			thisInterpolator.Interpolate(1f);
			ExpireImple();
		}
	}
	public interface IInterpolatorProcesssConstArg: IConstrainedProcessConstArg{
		bool useSpringT{get;}
	}
	public class InterpolatorProcessConstArg: ConstrainedProcessConstArg, IInterpolatorProcesssConstArg{
		public InterpolatorProcessConstArg(
			IProcessManager processManager,
			ProcessConstraint processConstraint,
			float constraintValue,
			bool useSpringT
		): base(
			processManager,
			processConstraint,
			constraintValue
		){
			thisUseSpringT = useSpringT;
		}
		readonly bool thisUseSpringT;
		public bool useSpringT{get{return thisUseSpringT;}}
	}
}

