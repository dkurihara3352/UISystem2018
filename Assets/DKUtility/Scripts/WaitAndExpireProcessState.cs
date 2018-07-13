using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface IWaitAndExpireProcessState{
		void OnProcessExpire();
		void OnProcessUpdate(float deltaT);
		void ExpireProcess();
	}
}
