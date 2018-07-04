using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProcess{
		void UpdateProcess(float deltaT);
		void Run();
		void Stop();
		void Expire();
		void Reset();
		bool IsRunning();
		float GetSpringT(float normalizedT);
	}
	public abstract class AbsProcess: IProcess{
		public AbsProcess(IProcessManager procManager){
			if(procManager == null)
				throw new System.ArgumentException("process never works without processManager");
			thisProcessManager = procManager;
		}
		IProcessManager thisProcessManager;
		public abstract void UpdateProcess(float deltaT);
		public virtual void Run(){
			thisProcessManager.AddRunningProcess(this);
			UpdateProcess(0f);
		}
		public virtual void Stop(){
			thisProcessManager.RemoveRunningProcess(this);
		}
		public virtual void Expire(){
			this.Stop();
			this.Reset();
		}
		public abstract void Reset();
		public bool IsRunning(){
			return thisProcessManager.RunningProcessesContains(this);
		}
		public float GetSpringT(float normlizedT){
			return thisProcessManager.GetSpringT(normlizedT);
		}
	}
	public interface IWaitAndExpireProcess: IProcess{}
	public abstract class AbsWaitAndExpireProcess: AbsProcess, IWaitAndExpireProcess{
		public AbsWaitAndExpireProcess(IProcessManager procMan, IWaitAndExpireProcessState state, float expireT): base(procMan){
			thisExpireT = expireT;
			thisState = state;
			Reset();
		}
		readonly IWaitAndExpireProcessState thisState;
		float thisElapsedT;
		readonly float thisExpireT;
		protected float thisNormlizedT{
			get{return thisElapsedT/ thisExpireT;}
		}
		public sealed override void UpdateProcess(float deltaT){
			thisState.OnProcessUpdate(deltaT);
			thisElapsedT += deltaT;
			UpdateProcessImple(deltaT);
			if(this.ExpirationIsEnabled())
				if(thisElapsedT >= thisExpireT){
					this.Expire();
				}
		}
		protected abstract void UpdateProcessImple(float deltaT);
		bool ExpirationIsEnabled(){
			return thisExpireT > 0f;
		}
		public override void Expire(){
			base.Expire();
			thisState.OnProcessExpire();
		}
		public override void Stop(){
			base.Stop();
			Reset();
		}
		public override void Reset(){
			thisElapsedT = 0f;
		}
	}
	public class GenericWaitAndExpireProcess: AbsWaitAndExpireProcess{
		public GenericWaitAndExpireProcess(IProcessManager processManager, IWaitAndExpireProcessState state, float expireT): base(processManager, state, expireT){
		}
		protected override void UpdateProcessImple(float deltaT){return;}
	}
	public enum ProcessConstraint{
		none,
		rateOfChange,
		expireTime
	}
	public abstract class AbsConstrainedProcess: AbsProcess{
		public AbsConstrainedProcess(IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, IWaitAndExpireProcessState processState, float differenceThreshold): base(processManager){
			thisProcessConstraint = processConstraint;
			thisConstraintValue = constraintValue;
			thisProcessState = processState;
			if(differenceThreshold < 0f)
				throw new System.ArgumentException("diffThreshold must not be below zero");
			thisDifferenceThreshold = differenceThreshold;
		}
		readonly ProcessConstraint thisProcessConstraint;
		readonly float thisConstraintValue;
		IWaitAndExpireProcessState thisProcessState;
		sealed public override void Run(){
			Reset();
			if(ValueDifferenceIsBigEnough()){
				RunImple();
				base.Run();
			}else{
				Expire();
			}
		}
		protected bool ValueDifferenceIsBigEnough(){
			float diff = GetNormalizedValueDiff();
			if(diff == 0)
				return false;
			else{
				if(diff > 0f)
					return diff > thisDifferenceThreshold;
				else
					return diff < - thisDifferenceThreshold;
			}
		}
		readonly float thisDifferenceThreshold;
		protected abstract float GetNormalizedValueDiff();
		protected virtual void RunImple(){
			CalcAndSetConstraintValues();
		}
		void CalcAndSetConstraintValues(){
			/*	Comp detail
				Dx (max possible diff: constant)
				Tx (max possible total time: constant)
				r (rate of Change: constant)
				{
					Dx/Tx = r
					Dx = 1f
					Tx = 1f sec
					r = 1f
				}
				Da (actual diff)
				Ta(time it takes to irp actual diff)
				{
					Da/Ta = r
					Ta = Da/r
				}
				Ti (irperT)
				Te (elapsedT)
				{
					Ti = Te/Ta
				}
			*/
			if(thisProcessConstraint == ProcessConstraint.rateOfChange){
				thisRateOfChange = thisConstraintValue;
				float normalizedValueDiff = GetNormalizedValueDiff();
				thisExpireT = normalizedValueDiff / thisRateOfChange;
				if(thisExpireT < 0f)
					thisExpireT *= -1f;
			}else if(thisProcessConstraint == ProcessConstraint.expireTime){
				float constVal = thisConstraintValue;
				if(constVal < 0f)
					constVal *= -1f;
				thisExpireT = constVal;
			}else{
				return;
			}
		}
		protected float thisExpireT;
		protected float thisRateOfChange;
		sealed public override void UpdateProcess(float deltaT){
			thisElapsedT += deltaT;
			if(thisProcessState != null)
				thisProcessState.OnProcessUpdate(deltaT);
			UpdateProcessImple(deltaT);
			if(thisProcessConstraint != ProcessConstraint.none){
				if(thisElapsedT >= thisExpireT)
					Expire();
			}
		}
		protected abstract void UpdateProcessImple(float deltaT);
		protected float thisElapsedT;
		sealed public override void Expire(){
			if(thisProcessState != null)
				thisProcessState.OnProcessExpire();
			SetTerminalValue();
			base.Expire();
		}
		protected abstract void SetTerminalValue();
		public override void Reset(){
			thisExpireT = 0f;
			thisRateOfChange = 0f;
			thisElapsedT = 0f;
		}
		protected float ClampValueZeroToOne(float value){
			if(value < 0f)
				return 0f;
			if(value > 1f)
				return 1f;
			return value;
		}
	}
	public abstract class AbsInterpolatorProcess<T>: AbsConstrainedProcess, IProcess where T: class, IInterpolator{
		public AbsInterpolatorProcess(IProcessManager processManager, ProcessConstraint constraint, float constraintValue, IWaitAndExpireProcessState processState, float differenceThreshold): base(processManager, constraint, constraintValue, processState, differenceThreshold){
		}
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
		sealed protected override void RunImple(){
			thisInterpolator = InstantiateInterpolatorWithValues();
			base.RunImple();
		}
		protected override void UpdateProcessImple(float deltaT){
			thisInterpolator.Interpolate(thisNormalizedT);
		}
		public override void Reset(){
			base.Reset();
			thisInterpolator = null;
		}
	}
}

