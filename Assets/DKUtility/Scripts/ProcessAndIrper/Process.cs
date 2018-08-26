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
		public AbsProcess(IProcessConstArg arg){
			if(arg.processManager == null)
				throw new System.ArgumentException("process never works without processManager");
			thisProcessManager = arg.processManager;
		}
		readonly protected IProcessManager thisProcessManager;
		public virtual void UpdateProcess(float deltaT){
			UpdateProcessImple(deltaT);
		}
		protected virtual void UpdateProcessImple(float deltaT){}
		public virtual void Run(){
			thisProcessManager.AddRunningProcess(this);
			RunImple();
			UpdateProcess(0f);
		}
		protected virtual void RunImple(){}
		public void Stop(){
			thisProcessManager.RemoveRunningProcess(this);
			StopImple();
		}
		protected virtual void StopImple(){}
		public virtual void Expire(){
			this.Stop();
			ExpireImple();
		}
		protected virtual void ExpireImple(){}
		public bool IsRunning(){
			return thisProcessManager.RunningProcessesContains(this);
		}
		public float GetSpringT(float normlizedT){
			return thisProcessManager.GetSpringT(normlizedT);
		}
	}


	
	public interface IProcessConstArg{
		IProcessManager processManager{get;}
	}
	public class ProcessConstArg{
		public ProcessConstArg(
			IProcessManager processManager
		){
			thisProcessManager = processManager;
		}
		readonly IProcessManager thisProcessManager;
		public IProcessManager processManager{get{return thisProcessManager;}}
	}
}

