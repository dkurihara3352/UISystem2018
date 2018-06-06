using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickabilityStateHandler{
		void BecomePicked();
		void BecomePickable();
		void BecomeUnpickable();
		bool IsPickable();
		bool IsPicked();
	}
	public interface IPickableUIE: IUIElement, IPickabilityStateHandler, IPickUpTransactionElement{
		void PickUp();
		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp();
		void BecomeVisuallyPickedUp();
		void BecomeVisuallyUnpicked();
	}
}
