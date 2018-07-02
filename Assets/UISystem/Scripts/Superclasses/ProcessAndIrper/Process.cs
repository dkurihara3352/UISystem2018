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
			thisProcessManager = procManager;
		}
		IProcessManager thisProcessManager;
		public abstract void UpdateProcess(float deltaT);
		public virtual void Run(){
			thisProcessManager.AddRunningProcess(this);
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
}

