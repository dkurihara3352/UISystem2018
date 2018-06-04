using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpReceiverStateHandler{
		void WaitForPickUp();
	}
	public interface IPickUpReceiver: IUIElement, IPickUpReceiverStateHandler{
		void CheckForHover();
	}
}

