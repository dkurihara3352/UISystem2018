using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UISystem;
public class GUIManager : MonoBehaviour {
	void Awake(){
		Rect topLeftRect = CreateGUIRect(new Vector2(0f, 0f), new Vector2(.17f, .5f));
			topLeftSubRect_0 = GetSubRect(topLeftRect, 0, 6);
			topLeftSubRect_1 = GetSubRect(topLeftRect, 1, 6);
			topLeftSubRect_2 = GetSubRect(topLeftRect, 2, 6);
			topLeftSubRect_3 = GetSubRect(topLeftRect, 3, 6);
			topLeftSubRect_4 = GetSubRect(topLeftRect, 4, 6);
			topLeftSubRect_5 = GetSubRect(topLeftRect, 5, 6);
	}
	Rect topLeftRect;
	Rect topLeftSubRect_0;
	Rect topLeftSubRect_1;
	Rect topLeftSubRect_2;
	Rect topLeftSubRect_3;
	Rect topLeftSubRect_4;
	Rect topLeftSubRect_5;
	public bool thisGUIIsEnabled;
	void OnGUI(){
		if(thisGUIIsEnabled){
			GUI.Label(topLeftSubRect_0, "Activation");
			if(GUI.Button(topLeftSubRect_1, "GetReadyForA"))
				testUIManagerAdaptor.GetRootUIAReadyForActivation();
			if(GUI.Button(topLeftSubRect_2, "Activate"))
				testUIManagerAdaptor.ActivateRootUIElement();
			if(GUI.Button(topLeftSubRect_3, "Deactivate"))
				testUIManagerAdaptor.DeactivateRootUIElement();
			if(GUI.Button(topLeftSubRect_4, "ActivateInst"))
				testUIManagerAdaptor.ActivateRootUIElementInstantly();
			if(GUI.Button(topLeftSubRect_5, "DeactivateInst"))
				testUIManagerAdaptor.DeactivateRootUIElementInstantly();
		}
	}
	public TestUIManagerAdaptor testUIManagerAdaptor;
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
}
