using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IHoverabilityStateHandler{
		void WaitForPickUp();
		void BecomeHoverable();
		void BecomeUnhoverable();
		void BecomeHovered();
		bool IsHoverable();
		bool IsHovered();
	}
	public interface IPickUpReceiver: IUIElement, IHoverabilityStateHandler{
		void EvaluateHoverability(IPickableUIE pickedUIE);
		void CheckForHover();
	}
}

