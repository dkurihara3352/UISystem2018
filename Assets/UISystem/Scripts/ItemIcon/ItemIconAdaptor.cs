using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconUIAdaptor: IUIAdaptor{
	}
	public abstract class AbsItemIconUIAdaptor<T>: AbsUIAdaptor<T>, IItemIconUIAdaptor where T: IItemIcon{
	}
}

