using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickUpTransactionElement{
		void SetPickUpManager(IPickUpManager pum);
	}
	public interface IHoverabilityStateHandler{
		void WaitForPickUp();
		void BecomeHoverable();
		void BecomeUnhoverable();
		void BecomeHovered();
	}
	public interface IPickUpReceiver: IUIElement, IHoverabilityStateHandler, IPickUpTransactionElement{
		void CheckForHover();
	}
}

