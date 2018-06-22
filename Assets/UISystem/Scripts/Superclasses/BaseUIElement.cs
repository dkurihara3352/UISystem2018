using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IBaseUIElement{
		/*  Hiearchy stuff, and all those shared behaviour that once thought to belong UIA's responsibility
		*/
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement parentUIE);
		List<IUIElement> GetAllChildUIEs();
	}
	public class BaseUIElement: IBaseUIElement {
		IUIAdaptor thisUIA;
		public void GetReadyForActivation(IUIAActivationData passedData){
			/*  UIA 
					=> simply converts transforms and components
					=> and create UIE, providing consructor arg data through inspector
				this => handles hiearchy stuff and all
			*/
		}
	}
}
