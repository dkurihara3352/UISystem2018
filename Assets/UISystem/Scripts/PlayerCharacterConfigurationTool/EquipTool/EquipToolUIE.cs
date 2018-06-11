using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolUIE: IPickUpContextUIE{}
	public class EquipToolUIE: AbsUIElement, IEquipToolUIE{
		public EquipToolUIE(IUIManager uim, IEquipToolUIAdaptor uia, IUIImage image): base(uim, uia, image){}
		public Vector2 GetPickUpReserveWorldPos(){
			IEquipToolUIAdaptor eqpToolUIA = this.GetUIAdaptor() as IEquipToolUIAdaptor;
			return eqpToolUIA.GetPickUpReserveWorldPos();
		}
	}
}
