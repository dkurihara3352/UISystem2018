using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IPickUpSystemUIElementFactory: IUIElementFactory{
		IItemIcon CreateItemIcon(IUIItem item);
	}
}
