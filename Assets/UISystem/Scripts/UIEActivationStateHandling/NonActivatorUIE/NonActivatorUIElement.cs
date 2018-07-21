using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface INonActivatorUIElement: IUIElement{}
	public abstract class AbsNonActivatorUIElement: AbsUIElement, INonActivatorUIElement{
		public AbsNonActivatorUIElement(IUIElementConstArg arg): base(arg){}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			return new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
		}
	}
}
