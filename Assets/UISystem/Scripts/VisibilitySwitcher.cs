using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IVisibilitySwitcher{
		/* subclassed by same uies that are also IRootActivator?
			.Tools, Widgets
			** implementation idea **
			. CanvasGroup is assigned, and tweak its group alpha or like
		 */
		void Show();
		void Hide();
	}	
}
