using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public enum ProcessConstraint{
		none,
		RateOfChange,
		ExpireTime
	}
	public abstract class AbsConstrainedProcess: AbsProcess{
		public AbsConstrainedProcess(
			IConstrainedProcessConstArg arg
		): base(
			arg
		){
			thisProcessConstraint = arg.processConstraint;
			thisConstraintValue = arg.constraintValue;
		}
		readonly ProcessConstraint thisProcessConstraint;
		readonly protected float thisConstraintValue;
		protected virtual float GetLatestInitialValueDifference(){
			if(thisProcessConstraint == ProcessConstraint.RateOfChange)
				throw new System.InvalidOperationException("Override GetLatesetInitialValueDifference if the process constraint is rate of change");
			return 0f;
		}
		protected override void RunImple(){			
			CalcAndSetConstraintValues();
		}
		void CalcAndSetConstraintValues(){
			if(thisProcessConstraint == ProcessConstraint.RateOfChange){
				thisRateOfChange = thisConstraintValue;
				float valueDiff = GetLatestInitialValueDifference();
				if(valueDiff == 0f)
					thisExpireT = 0f;
				else
					thisExpireT = valueDiff / thisRateOfChange;
				if(thisExpireT < 0f)
					thisExpireT *= -1f;
			}else if(thisProcessConstraint == ProcessConstraint.ExpireTime){
				float constVal = thisConstraintValue;
				if(constVal < 0f)
					constVal *= -1f;
				thisExpireT = constVal;
			}else{
				return;
			}
		}
		protected float thisElapsedT;
		protected float thisExpireT;
		protected float thisRateOfChange;
		sealed public override void UpdateProcess(float deltaT){
			thisElapsedT += deltaT;
			if(thisProcessConstraint != ProcessConstraint.none){
				if(thisElapsedT >= thisExpireT){
					Expire();
					return;
				}
			}
			UpdateProcessImple(deltaT);
		}
	}
	public interface IConstrainedProcessConstArg: IProcessConstArg{
		ProcessConstraint processConstraint{get;}
		float constraintValue{get;}
	}
	public class ConstrainedProcessConstArg: ProcessConstArg, IConstrainedProcessConstArg{
		public ConstrainedProcessConstArg(
			IProcessManager processManager,
			ProcessConstraint processConstraint,
			float constraintValue
		): base(
			processManager
		){
			thisProcessConstraint = processConstraint;
			thisConstraintValue = constraintValue;
		}
		readonly ProcessConstraint thisProcessConstraint;
		public ProcessConstraint processConstraint{get{return thisProcessConstraint;}}
		readonly float thisConstraintValue;
		public float constraintValue{get{return thisConstraintValue;}}
	}

	public class GenericWaitAndExpireProcess: AbsConstrainedProcess{
		public GenericWaitAndExpireProcess(
			IGenericWaitAndExpireProcessConstArg arg
		): base(
			arg	
		){}
	}
	public interface IGenericWaitAndExpireProcessConstArg: IConstrainedProcessConstArg{}
	public class GenericWaitAndExpireProcessConstArg: ProcessConstArg, IGenericWaitAndExpireProcessConstArg{
		public GenericWaitAndExpireProcessConstArg(
			IProcessManager processManager,
			float expireTime
		): base(
			processManager
		){
			thisExpireTime = expireTime;
		}
		public ProcessConstraint processConstraint{
			get{
				return ProcessConstraint.ExpireTime;
			}
		}
		readonly float thisExpireTime;
		public float constraintValue{
			get{
				return thisExpireTime;
			}
		}
	} 
}

