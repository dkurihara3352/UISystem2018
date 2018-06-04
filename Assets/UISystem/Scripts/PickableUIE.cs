using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickabilityStateHandler{
		void BecomePickable();
		void BecomeUnpickable();
		bool IsPickable();
		bool IsPicked();
	}
	public interface IPickable{
		void PickUp();
		void DeclinePickUp();
		void CheckForImmediatePickUp();
		void CheckForDelayedPickUp();
		void CheckForSecondTouchPickUp();
		void CheckForDragPickUp();
	}
	public interface IPickableUIE: IUIElement, IPickabilityStateHandler, IPickable{
	}
}
