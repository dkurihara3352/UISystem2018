using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IWaitAndExpireProcessState{
		void OnProcessExpire();
		void OnProcessUpdate(float deltaT);
	}
}
