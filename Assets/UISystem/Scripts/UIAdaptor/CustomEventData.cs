using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface ICustomEventData{
		Vector2 deltaP{get;}
		Vector2 position{get;}
	}
	public struct CustomEventData: ICustomEventData{
		public CustomEventData(PointerEventData sourceData){
			/* do some conversion here */
			thisDeltaP = sourceData.delta;
			thisPosition = sourceData.position;
		}
		public Vector2 deltaP{get{return thisDeltaP;}}
		Vector2 thisDeltaP;
		public Vector2 position{get{return thisPosition;}}
		Vector2 thisPosition;
	}
}
