using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericUIElementGroup: IUIElementGroup, INonActivatorUIElement{}
	public class GenericUIElementGroup: AbsUIElementGroup<IUIElement>, IGenericUIElementGroup{
		public GenericUIElementGroup(IUIElementGroupConstArg arg): base(arg){}
		protected override IUIEActivationStateEngine CreateUIEActivationStateEngine(){
			IUIEActivationStateEngine engine = new NonActivatorUIEActivationStateEngine(thisProcessFactory, this);
			return engine;
		}
	}
}
