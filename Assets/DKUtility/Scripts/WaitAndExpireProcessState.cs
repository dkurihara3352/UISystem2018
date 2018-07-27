using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IWaitAndExpireProcessState{
		void OnProcessExpire();
		void OnProcessUpdate(float deltaT);
		void ExpireProcess();
	}
	public abstract class AbsWaitAndExpireProcessState<T>: ISwitchableState, IWaitAndExpireProcessState where T: class, IWaitAndExpireProcess{
		public AbsWaitAndExpireProcessState(IProcessFactory processFactory){
			thisProcessFactory = processFactory;
		}
		protected readonly IProcessFactory thisProcessFactory;
		protected abstract T CreateProcess();
		protected T thisProcess;
		public virtual void OnEnter(){
			thisProcess = CreateProcess();
			thisProcess.Run();
		}
		public virtual void OnExit(){
			StopAndClearProcess();
		}
		void StopAndClearProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
		public virtual void OnProcessUpdate(float deltaT){}
		public virtual void OnProcessExpire(){}
		public virtual void ExpireProcess(){
			StopAndClearProcess();
		}
	}
}
