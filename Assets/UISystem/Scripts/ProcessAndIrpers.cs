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
		public void Stop(){
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
	public interface IProcessManager{
		void AddRunningProcess(IProcess process);
		void RemoveRunningProcess(IProcess process);
		void UpdateAllRegisteredProcesses(float deltaT);
		bool RunningProcessesContains(IProcess process);

	}
	public interface IProcessFactory{
		ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
	}
	public class ProcessFactory: IProcessFactory{
		public ProcessFactory(IProcessManager procManager, IUIManager uim){
			if(procManager != null)
				this.processManager = procManager;
			else
				throw new System.ArgumentNullException("procManager", "ProcessFactory does not operate without a procManager");
			if(uim != null)
				this.uiManager = uim;
			else
				throw new System.ArgumentNullException("uim", "ProcessFactory does not operate without a uim");

		}
		readonly IProcessManager processManager;
		readonly IUIManager uiManager;
		public ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness){
			ITurnImageDarknessProcess process = new TurnImageDarknessProcess(processManager, image, targetDarkness);
			return process;
		}
		public IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime){
			IWaitAndExpireProcess process = new WaitAndExpireProcess(processManager, state, waitTime);
			return process;
		}
	}
	public interface IInterpolator{
		void Interpolate(float zeroToOne);
		void InterpolateImple(float zeroToOne);
		void Terminate();
	}
	public abstract class AbsInterpolater: IInterpolator{
		public abstract void InterpolateImple(float zeroToOne);
		public void Interpolate(float zeroToOne){
			this.InterpolateImple(zeroToOne);
			if(zeroToOne >= 1f)
				this.Terminate();
		}
		public abstract void Terminate();
	}
	public interface IWaitAndExpireProcessState{
		void OnProcessExpire();
		void OnProcessUpdate(float deltaT);
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
		public override void Reset(){
			elapsedT = 0f;
		}
	}
}

