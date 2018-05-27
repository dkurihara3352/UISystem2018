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
	}
	public abstract class AbsProcess: IProcess{
		public AbsProcess(IProcessManager procManager){
			_processManager = procManager;
		}
		IProcessManager _processManager;
		IProcessManager ThisProcManager(){return _processManager;}
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
	}
	public interface IProcessManager{
		void AddRunningProcess(IProcess process);
		void RemoveRunningProcess(IProcess process);
		void UpdateAllRegisteredProcesses(float deltaT);

	}
	public interface IProcessFactory{
	}
	public interface IProcessHandler{
		void InitWithProcesses(IProcessFactory procFactory);
	}
}

