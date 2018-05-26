using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAdaptor{
		IUIElement GetParentUIE();
		List<IUIElement> GetChildUIEs();
	}
	public class UIAdaptor : IUIAdaptor {
		public IUIElement GetParentUIE(){
			return null;
		}
		public List<IUIElement> GetChildUIEs(){
			return null;
		}
	}
}
