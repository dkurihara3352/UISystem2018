using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProcessManager{
		void AddRunningProcess(IProcess process);
		void RemoveRunningProcess(IProcess process);
		void UpdateAllRegisteredProcesses(float deltaT);
		bool RunningProcessesContains(IProcess process);
	}
}
