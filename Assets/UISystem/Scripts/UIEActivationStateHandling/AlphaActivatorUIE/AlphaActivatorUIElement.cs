using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IAlphaActivatorUIElement: IUIElement{
		float GetGroupAlphaForActivation();
		void SetGroupAlphaForActivation(float groupAlpha);
	}
}
