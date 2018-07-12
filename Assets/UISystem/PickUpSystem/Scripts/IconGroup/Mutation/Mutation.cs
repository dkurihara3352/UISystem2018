using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IMutation{
		void FindInProspectiveIIsAndSwap(IItemIcon soueceII, IItemIcon targetII);
	}
}
