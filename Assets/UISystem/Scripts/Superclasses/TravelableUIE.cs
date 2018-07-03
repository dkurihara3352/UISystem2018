﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ITravelableUIE: IUIElement{
		void SetRunningTravelProcess(ITravelProcess process);
		ITravelProcess GetRunningTravelProcess();
		void AbortRunningTravelProcess();
		void HandOverTravel(ITravelableUIE other);
	}	
}
