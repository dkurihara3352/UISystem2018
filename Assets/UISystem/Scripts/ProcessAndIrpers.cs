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
			processManager = procManager;
		}
		IProcessManager processManager;
		IProcessManager ThisProcManager(){return processManager;}
		public abstract void UpdateProcess(float deltaT);
		public virtual void Run(){
			ThisProcManager().AddRunningProcess(this);
		}
		public void Stop(){
			ThisProcManager().RemoveRunningProcess(this);
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
		TurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
	}
	public class ProcessFactory: IProcessFactory{
		public ProcessFactory(IProcessManager procManager){
			this.processManager = procManager;
		}
		IProcessManager processManager;
		IProcessManager GetProcessManager(){
			return processManager;
		}
		public TurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness){
			TurnImageDarknessProcess process = new TurnImageDarknessProcess(GetProcessManager(), image, targetDarkness);
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

