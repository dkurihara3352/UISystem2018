using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public class TestUIManagerAdaptor: UIManagerAdaptor{

		public PopUpAdaptor popUpAdaptor;
		public void TogglePopUp(){
			IPopUp popUp = (IPopUp)popUpAdaptor.GetUIElement();
			if(popUp.IsShown())
				popUp.Hide(false);
			else
				popUp.Show(true);
		}
	}
}

