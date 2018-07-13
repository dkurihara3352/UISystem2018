using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IInstatiableUIAdaptor: IUIAdaptor{
		void SetInitializationFields(IUIAInitializationData data);
	}
	public interface IUIAInitializationData{}
}
