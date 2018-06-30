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
	public class WaitAndExpireProcess: AbsProcess, IWaitAndExpireProcess{
		public WaitAndExpireProcess(IProcessManager procMan, IWaitAndExpireProcessState state, float expireT): base(procMan){
			this.expireT = expireT;
			this.state = state;
			Reset();
		}
		readonly IWaitAndExpireProcessState state;
		float elapsedT;
		readonly float expireT;
		public override void UpdateProcess(float deltaT){
			state.OnProcessUpdate(deltaT);
			elapsedT += deltaT;
			if(this.ExpirationIsEnabled())
				if(elapsedT >= expireT){
					this.Expire();
				}
		}
		bool ExpirationIsEnabled(){
			return expireT > 0f;
		}
		public override void Expire(){
			base.Expire();
			state.OnProcessExpire();
		}
		public override void Stop(){
			base.Stop();
			Reset();
		}
		public override void Reset(){
			elapsedT = 0f;
		}
	}
}

