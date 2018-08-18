using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolUIE: IPickUpContextUIE{}
	public class EquipToolUIE: UIElement, IEquipToolUIE{
		public EquipToolUIE(IUIElementConstArg arg): base(arg){}
		public Vector2 GetPickUpReservePosition(){
			IEquipToolUIAdaptor eqpToolUIA = (IEquipToolUIAdaptor)this.GetUIAdaptor();
			return eqpToolUIA.GetPickUpReserveWorldPos();
		}
	}
}
