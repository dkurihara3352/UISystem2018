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
	}
	public class ProcessFactory: IProcessFactory{
		public ProcessFactory(IProcessManager procManager){
			if(procManager != null)
				this.processManager = procManager;
			else
				throw new System.ArgumentNullException("procManager", "ProcessFactory does not operate without a procManager");
		}
		IProcessManager processManager;
		IProcessManager GetProcessManager(){
			return processManager;
		}
		public ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness){
			ITurnImageDarknessProcess process = new TurnImageDarknessProcess(GetProcessManager(), image, targetDarkness);
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
}

