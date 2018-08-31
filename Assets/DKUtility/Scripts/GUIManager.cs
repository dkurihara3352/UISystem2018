using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
public class GUIManager : MonoBehaviour {
	void Awake(){
		Rect topLeftRect = CreateGUIRect(new Vector2(0f, 0f), new Vector2(.17f, .5f));
			topLeftSubRect_0 = GetSubRect(topLeftRect, 0, 8);
			topLeftSubRect_1 = GetSubRect(topLeftRect, 1, 8);
			topLeftSubRect_2 = GetSubRect(topLeftRect, 2, 8);
			topLeftSubRect_3 = GetSubRect(topLeftRect, 3, 8);
			topLeftSubRect_4 = GetSubRect(topLeftRect, 4, 8);
			topLeftSubRect_5 = GetSubRect(topLeftRect, 5, 8);
			topLeftSubRect_6 = GetSubRect(topLeftRect, 6, 8);
			topLeftSubRect_7 = GetSubRect(topLeftRect, 7, 8);
		Rect topRight = CreateGUIRect(new Vector2(1f, 0f), new Vector2(.25f, .3f));
			topRightSub_0 = GetSubRect(topRight, 0, 6);
			topRightSub_1 = GetSubRect(topRight, 1, 6);
			topRightSub_2 = GetSubRect(topRight, 2, 6);
			topRightSub_3 = GetSubRect(topRight, 3, 6);
			topRightSub_4 = GetSubRect(topRight, 4, 6);
			topRightSub_5 = GetSubRect(topRight, 5, 6);
		
	}
	Rect topLeftRect;
	Rect topLeftSubRect_0;
	Rect topLeftSubRect_1;
	Rect topLeftSubRect_2;
	Rect topLeftSubRect_3;
	Rect topLeftSubRect_4;
	Rect topLeftSubRect_5;
	Rect topLeftSubRect_6;
	Rect topLeftSubRect_7;
	public bool thisGUIIsEnabled;
	public bool topLeftIsEnabled = true;
	public bool topRightIsEnabled = true;
	void OnGUI(){
		if(thisGUIIsEnabled){
			if(topLeftIsEnabled){
				DrawTopLeft();
			}
			if(topRightIsEnabled){
				DrawTopRight();
			}

		}
	}
	public TestUIManagerAdaptor testUIManagerAdaptor;
	IUIManager thisUIM{
		get{return testUIManagerAdaptor.GetUIManager();}
	}
	protected Rect CreateGUIRect(Vector2 normalizedPosition, Vector2 normalizedSize){
		MakeSureValuesAreInRange(normalizedPosition);
		MakeSureValuesAreInRange(normalizedSize);
		Vector2 rectLength = new Vector2(Screen.width * normalizedSize.x, Screen.height * normalizedSize.y);
		float rectX = (Screen.width - rectLength.x) * normalizedPosition.x;
		float rectY = (Screen.height - rectLength.y) * normalizedPosition.y;
		Vector2 rectPosition = new Vector2(rectX, rectY);
		return new Rect(rectPosition, rectLength);
	}
	protected Rect GetSubRect(Rect sourceRect, int index, int rowCount){
		if(index >= rowCount)
			throw new System.InvalidOperationException("index out of range");
		float rectHeight = sourceRect.height/ rowCount;
		float rectPosY = index * rectHeight + sourceRect.y;
		return new Rect(new Vector2(sourceRect.x, rectPosY), new Vector2(sourceRect.width, rectHeight));
	}
	void MakeSureValuesAreInRange(Vector2 value){
		for(int i = 0; i < 2; i ++)
			if(value[i] < 0f || value[i] > 1f)
				throw new System.InvalidOperationException("value is not in range");
	}
	public UIAdaptor uieGroupScrollerAdaptor;
	string GetCursoredElementsText(){
		IUIElementGroupScroller uieGroupScroller = (IUIElementGroupScroller)uieGroupScrollerAdaptor.GetUIElement();
		string result = "";
		if(uieGroupScroller != null){
			IUIElement[] curosredElements = uieGroupScroller.GetCursoredElements();
			if(curosredElements != null)
				foreach(IUIElement cursoredElement in curosredElements)
					if(cursoredElement == null)
						result += ", null";
					else
						result += ", " + uieGroupScroller.GetGroupElementIndex(cursoredElement).ToString();
			else
				result = "cursoredElements not evaluated";
		}else{
			result = "uie is not evaluated yet";
		}
		return result;
	}
	string GetCursorOffsetString(){
		IUIElementGroupScroller uieGroupScroller = (IUIElementGroupScroller)uieGroupScrollerAdaptor.GetUIElement();
		if(uieGroupScroller != null){
			IUIElement scrollerElement = uieGroupScroller.GetScrollerElement() ;
			Vector2 offset = new Vector2();
			for(int i = 0; i < 2; i++){
				float cursorOffset = uieGroupScroller.GetElementCursorOffsetInPixel(
					scrollerElement.GetLocalPosition()[i],
					i
				);
				offset[i] = cursorOffset;
			}
			string result = "Cursored Offset: ( " +
				offset.x +
				", " +
				offset.y +
				" )";
			return result;
		}
		return "";
	}
	string GetHandlingScrollerVelocityString(){
		if(thisUIM != null){
			IScroller scrollerInMotion = thisUIM.GetInputHandlingScroller();
			if(scrollerInMotion != null){
				Vector2 velocity = scrollerInMotion.GetVelocity();
				return velocity.magnitude.ToString();
			}else
				return "scroller: null";

		}else{
			return "uim is not set";
		}
	}
	string GetScrollerProximateParentScrollerString(){
		if(thisUIM != null){
			IScroller scrollerInMotion = thisUIM.GetInputHandlingScroller();
			if(scrollerInMotion != null){
				IScroller proximateParentScroller = scrollerInMotion.GetProximateParentScroller();
				string result = "par: ";
				if(proximateParentScroller != null)
					result += proximateParentScroller.GetUIAdaptor().GetName();
				else
					result += "null";
				return result;
			}else
				return "scroller: null";

		}else{
			return "uim is not set";
		}
	}
	Rect topRight;
	Rect topRightSub_0;
	Rect topRightSub_1;
	Rect topRightSub_2;
	Rect topRightSub_3;
	Rect topRightSub_4;
	Rect topRightSub_5;
	string GetInputHandlingScrollerString(){
		string result = "InputHandling: \n";
		IScroller scroller = thisUIM.GetInputHandlingScroller();
		if(scroller == null)
			return result += "null";
		else
			return result += scroller.GetUIAdaptor().GetName();
		
	}
	string GetEventHandledString(){
		string result = "Event: \n";
		return result += thisUIM.GetEventName();
		
	}
	string GetTopMostScrollerInMotionString(){
		string result = "TopMost: \n";
		IScroller handlingScroller = thisUIM.GetInputHandlingScroller();
		if(handlingScroller == null)
			result += "no handling scroller";
		else{
			IScroller topMost = handlingScroller.GetTopmostScrollerInMotion();
			if(topMost == null)
				result += "null";
			else
				result += topMost.GetUIAdaptor().GetName();
		}
		return result;
	}
	void DrawTopLeft(){
		GUI.Label(topLeftSubRect_0, "Activation");
		if(GUI.Button(topLeftSubRect_1, "GetReadyForA"))
			thisUIM.GetReadyForUISystemActivation();
		if(GUI.Button(topLeftSubRect_2, "Activate"))
			thisUIM.ActivateUISystem(false);
		if(GUI.Button(topLeftSubRect_3, "Deactivate"))
			thisUIM.DeactivateUISystem(false);
		if(GUI.Button(topLeftSubRect_4, "ActivateInst"))
			thisUIM.ActivateUISystem(true);
		if(GUI.Button(topLeftSubRect_5, "DeactivateInst"))
			thisUIM.DeactivateUISystem(true);
		if(GUI.Button(topLeftSubRect_6, "TogglePopUp"))
			testUIManagerAdaptor.TogglePopUp();
	}
	void DrawTopRight(){
		GUI.Label(
			topRightSub_0, 
			GetInputHandlingScrollerString()
		);
		GUI.Label(
			topRightSub_1, 
		 	GetEventHandledString()
		);
		GUI.Label(
			topRightSub_2, 
			GetCursoredElementsText()
		);
		GUI.Label(
			topRightSub_3, 
			// GetTopMostScrollerInMotionString()
			GetCursorOffsetString()
		);
		GUI.Label(
			topRightSub_4, 
			GetHandlingScrollerVelocityString()
		);
		GUI.Label(
			topRightSub_5, 
			GetScrollerProximateParentScrollerString()
		);
	}
}
