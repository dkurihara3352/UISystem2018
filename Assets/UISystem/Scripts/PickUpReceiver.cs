using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IHoverabilityStateHandler{
		void WaitForPickUp();
		void BecomeHoverable();
		void BecomeUnhoverable();
	}
	public interface IPickUpReceiver: IUIElement, IHoverabilityStateHandler{
		void CheckForHover();
	}
}

