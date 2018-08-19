using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProximateParentTypedUIECalculator<T> where T: class, IUIElement{
		T Calculate();
	}
	public class ProximateParentTypedUIECalculator<T>: IProximateParentTypedUIECalculator<T> where T: class, IUIElement{

		public ProximateParentTypedUIECalculator(IUIElement uieToExamine){
			thisUIElementToExamine = uieToExamine;
		}
		readonly IUIElement thisUIElementToExamine;
		public T Calculate(){
			IUIElement uieToExamine = thisUIElementToExamine;
				while(true){
					IUIElement parentUIE = uieToExamine.GetParentUIE();
					if(parentUIE != null){
						if(parentUIE is T){
							return ((T)parentUIE);
						}else
							uieToExamine = parentUIE;
					}else{
						break;
					}
				}	
				return null;
		}
	}
}
