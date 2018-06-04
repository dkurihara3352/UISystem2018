using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPickableUIElement: IUIElement{
		void PickUp();
		void CheckForPickUp();
	}
}
