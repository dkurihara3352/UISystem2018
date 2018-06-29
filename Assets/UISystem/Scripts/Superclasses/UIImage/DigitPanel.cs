using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IDigitPanel: IUIElement{
		void SetNumber(int number);
		/*  if number = -1, substitute the panel image with Blank image
		*/
	}
}
