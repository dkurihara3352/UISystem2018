using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericUIElementGroup: IUIElementGroup{}
	public class GenericUIElementGroup: AbsUIElementGroup<IUIElement>, IGenericUIElementGroup{
		public GenericUIElementGroup(IUIElementGroupConstArg arg): base(arg){}
	}
}
