using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IItemIconUIAdaptor: IUIAdaptor{
	}
	public abstract class AbsItemIconUIAdaptor<T>: AbsUIAdaptor<T>, IItemIconUIAdaptor where T: IItemIcon{
		IPickUpSystemUIAActivationData thisPickUpSystemDomainActivationData{
			get{return (IPickUpSystemUIAActivationData)thisDomainActivationData;}
		}
	}
}

