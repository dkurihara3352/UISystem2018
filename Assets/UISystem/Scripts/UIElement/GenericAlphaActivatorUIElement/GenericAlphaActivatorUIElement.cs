using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericAlphaActivatorUIElement: IUIElement, IAlphaActivatorUIElement{}
	public class GenericAlphaActivatorUIElement : AbsAlphaActivatorUIElement, IGenericAlphaActivatorUIElement {
		public GenericAlphaActivatorUIElement(IUIElementConstArg arg): base(arg){}
	}
}
