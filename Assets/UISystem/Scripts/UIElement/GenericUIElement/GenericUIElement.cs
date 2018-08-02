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
	}
}

