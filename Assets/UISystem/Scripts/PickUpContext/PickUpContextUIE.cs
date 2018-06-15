using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpContextUIE: IUIElement{
		/*  the uie to which PickUpManager is attached to implement this, such as ToolUIE or WidgetUIE
		*/
		Vector2 GetPickUpReservePosition();
	}
}
