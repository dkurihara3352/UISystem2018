using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IProcess{
		void UpdateProcess(float deltaT);
		void Run();
		void Stop();
		void Expire();
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
		}
		public bool IsRunning(){
			return thisProcessManager.RunningProcessesContains(this);
		}
		public float GetSpringT(float normlizedT){
			return thisProcessManager.GetSpringT(normlizedT);
		}
	}
	public interface IWaitAndExpireProcess: IProcess{
	}
	public abstract class AbsWaitAndExpireProcess: AbsProcess, IWaitAndExpireProcess{
		public AbsWaitAndExpireProcess(IProcessManager procMan, float expireT, IWaitAndExpireProcessState state): base(procMan){
			thisExpireT = expireT;
			thisState = state;
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
		}
	}
	public class GenericWaitAndExpireProcess: AbsWaitAndExpireProcess{
		public GenericWaitAndExpireProcess(IProcessManager processManager, float expireT, IWaitAndExpireProcessState state): base(processManager, expireT, state){
		}
		protected override void UpdateProcessImple(float deltaT){return;}
	}
	public enum ProcessConstraint{
		none,
		rateOfChange,
		expireTime
	}
	public abstract class AbsConstrainedProcess: AbsProcess, IWaitAndExpireProcess{
		public AbsConstrainedProcess(IProcessManager processManager, ProcessConstraint processConstraint, float constraintValue, float differenceThreshold, IWaitAndExpireProcessState state): base(processManager){
			thisProcessConstraint = processConstraint;
			thisConstraintValue = constraintValue;
			if(differenceThreshold < 0f)
				throw new System.ArgumentException("diffThreshold must not be below zero");
			thisDifferenceThreshold = differenceThreshold;
			thisProcessState = state;
		}
		readonly ProcessConstraint thisProcessConstraint;
		readonly float thisConstraintValue;
		protected readonly IWaitAndExpireProcessState thisProcessState;
		public override void Run(){
			if(ValueDifferenceIsBigEnough()){
				RunImple();
				base.Run();
			}else{
				Expire();
			}
		}
		protected bool ValueDifferenceIsBigEnough(){
			float diff = GetLatestInitialValueDifference();
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
		protected abstract float GetLatestInitialValueDifference();
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
				float valueDiff = GetLatestInitialValueDifference();
				thisExpireT = valueDiff / thisRateOfChange;
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
			if(thisProcessConstraint != ProcessConstraint.none){
				if(thisElapsedT >= thisExpireT){
					Expire();
					return;
				}
			}
			if(thisProcessState != null)
				thisProcessState.OnProcessUpdate(deltaT);
			UpdateProcessImple(deltaT);
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
		protected float ClampValueZeroToOne(float value){
			if(value < 0f)
				return 0f;
			if(value > 1f)
				return 1f;
			return value;
		}
	}
	public abstract class AbsInterpolatorProcess<T>: AbsConstrainedProcess, IProcess where T: class, IInterpolator{
		public AbsInterpolatorProcess(IProcessManager processManager, ProcessConstraint constraint, float constraintValue, float differenceThreshold, bool useSpringT, IWaitAndExpireProcessState state): base(processManager, constraint, constraintValue, differenceThreshold, state){
			thisUseSpringT = useSpringT;
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
		sealed protected override void RunImple(){
			// thisInterpolator = InstantiateInterpolatorWithValues();
			base.RunImple();
		}
		protected override void UpdateProcessImple(float deltaT){
			float t = thisUseSpringT? GetSpringT(thisNormalizedT): thisNormalizedT;
			thisInterpolator.Interpolate(t);
		}
		sealed protected override void SetTerminalValue(){
			thisInterpolator.Interpolate(1f);
		}
	}
}

