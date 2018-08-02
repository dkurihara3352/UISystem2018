using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UISystem{
	public interface ICustomEventData{
		Vector2 deltaPos{get;}
		Vector2 position{get;}
		Vector2 velocity{get;}
	}
	public struct CustomEventData: ICustomEventData{
		public CustomEventData(PointerEventData sourceData, float deltaTime){
			/* do some conversion here */
			thisDeltaP = sourceData.delta;
			thisPosition = sourceData.position;
			thisVelocity = thisDeltaP/ deltaTime;
		}
		public CustomEventData(Vector2 position, Vector2 deltaP, float deltaTime){
			thisPosition = position;
			thisDeltaP = deltaP;
			thisVelocity = deltaP/ deltaTime;
		}
		public Vector2 deltaPos{get{return thisDeltaP;}}
		Vector2 thisDeltaP;
		public Vector2 position{get{return thisPosition;}}
		Vector2 thisPosition;
		public Vector2 velocity{get{return thisVelocity;}}
		Vector2 thisVelocity;
	}
}
