using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericUIElement: IUIElement, INonActivatorUIElement{}
	public class GenericUIElement: AbsUIElement, IGenericUIElement{
		public GenericUIElement(IUIElementConstArg arg): base(arg){}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			NonActivatorUIEActivationStateEngine engine = new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
			return engine;
		}
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

