using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITravelInterpolator: IInterpolator{
		void UpdateTravellingII(IItemIcon newII);
	}
}
