using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IProximateParentScrollerCalculator{
		IScroller Calculate();
	}
	public class ProximateParentScrollerCalculator: IProximateParentScrollerCalculator {

		public ProximateParentScrollerCalculator(IUIElement uieToExamine){
			thisUIElementToExamine = uieToExamine;
		}
		readonly IUIElement thisUIElementToExamine;
		public IScroller Calculate(){
			IUIElement uieToExamine = thisUIElementToExamine;
				while(true){
					IUIElement parentUIE = uieToExamine.GetParentUIE();
					if(parentUIE != null){
						if(parentUIE is IScroller){
							return ((IScroller)parentUIE);
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
