using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver{
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIManager uim, IUIAdaptor uia) :base(uim, uia){}
		public void CheckForHover(){}
		public void WaitForPickUp(){}
	}
}
