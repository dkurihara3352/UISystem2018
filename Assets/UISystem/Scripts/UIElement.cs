using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement{
		string SayHi();
	}
	public class UIElement: IUIElement{
		public string SayHi(){
			return "Hi";
		}
	}
}
