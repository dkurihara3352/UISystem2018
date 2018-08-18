using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUpManager{
		void RegisterPopUp(IPopUp popUpToRegister);
		void UnregisterPopUp(IPopUp popUpToUnregister);
	}
	public class PopUpManager : IPopUpManager {

	}
}
