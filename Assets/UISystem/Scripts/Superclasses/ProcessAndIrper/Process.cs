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
	}
	public abstract class AbsProcess: IProcess{
		public AbsProcess(IProcessManager procManager){
			this.processManager = procManager;
		}
		IProcessManager processManager;
		public abstract void UpdateProcess(float deltaT);
		public virtual void Run(){
			processManager.AddRunningProcess(this);
		}
		public virtual void Stop(){
			processManager.RemoveRunningProcess(this);
		}
		public virtual void Expire(){
			this.Stop();
			this.Reset();
		}
		public abstract void Reset();
		public bool IsRunning(){
			return processManager.RunningProcessesContains(this);
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

