using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface ITravelableUIE: IUIElement{
		void SetRunningTravelProcess(ITravelProcess process);
		ITravelProcess GetRunningTravelProcess();
		void AbortRunningTravelProcess();
		void HandOverTravel(ITravelableUIE other);
	}	
}
