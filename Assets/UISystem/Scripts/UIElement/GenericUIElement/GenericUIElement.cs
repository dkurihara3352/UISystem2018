using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericUIElement: IUIElement{}
	public class GenericUIElement: UIElement, IGenericUIElement{
		public GenericUIElement(IUIElementConstArg arg): base(arg){}
		public override void BecomeDefocusedInScrollerRecursively(){
			base.BecomeDefocusedInScrollerRecursively();
			BecomeUnselectable();
		}
		public override void BecomeFocusedInScrollerRecursively(){
			base.BecomeFocusedInScrollerRecursively();
			BecomeSelectable();
		}
	}
}

