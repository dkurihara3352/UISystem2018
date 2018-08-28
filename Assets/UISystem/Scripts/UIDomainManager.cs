using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIDomainManager{
		IUIAdaptorBaseInitializationData CreateDomainActivationData(IUIAdaptorBaseInitializationData passedData);
	}
}
