using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IProcessFactory{
	}
	public abstract class AbsProcessFactory: IProcessFactory{
		public AbsProcessFactory(IProcessManager procManager){
			if(procManager != null)
				thisProcessManager = procManager;
			else
				throw new System.ArgumentNullException("procManager", "ProcessFactory does not operate without a procManager");
		}
		protected readonly IProcessManager thisProcessManager;
	}
}
