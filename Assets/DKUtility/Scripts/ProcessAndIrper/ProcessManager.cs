using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IProcessManager{
		void AddRunningProcess(IProcess process);
		void RemoveRunningProcess(IProcess process);
		void UpdateAllRegisteredProcesses(float deltaT);
		bool RunningProcessesContains(IProcess process);
		float GetQuantityAnimationProcessExpireTime();
		float GetImageEmptificationExpireTime();
		float GetVisualPickednessProcessExpireTime();
		float GetSpringT(float normalizedT);
	}
}
