using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DKUtility;
public class TestEventHandler : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler{
	protected override void Awake(){
		base.Awake();
		Vector2 rectLength = new Vector2(300f, 20f);
		Vector2 rectPos1 = new Vector2(0f, Screen.height - rectLength[1]);
		Vector2 rectPos2 = new Vector2(0f, Screen.height - rectLength[1] - 20f);
		Vector2 rectPos3 = new Vector2(0f, Screen.height - rectLength[1] - 40f);
		Vector2 rectPos4 = new Vector2(0f, Screen.height - rectLength[1] - 60f);
		rect1 = new Rect(rectPos1, rectLength);
		rect2 = new Rect(rectPos2, rectLength);
		rect3 = new Rect(rectPos3, rectLength);
		rect4 = new Rect(rectPos4, rectLength);
	}
	public bool enableGUI;
	Rect rect1;
	Rect rect2;
	Rect rect3;
	Rect rect4;
	string pointerMessage = "No Input";
	string selectionMessage = "No Input";
	string pointerUpDownMessage = "No Input";
	string dragMessage = "No Input";
	void OnGUI(){
		if(enableGUI){
			GUI.color = new Color(1f, 1f, 1f);
			GUI.Label(rect1, "Pointer: "+ pointerMessage);
			GUI.Label(rect2,  "Selection: " + selectionMessage);
			GUI.Label(rect3, "PointerUpDown: " + pointerUpDownMessage);
			GUI.Label(rect4, "Drag: " + dragMessage);
		}
	}
	IEnumerator WaitAndResetPointerMessage(){
		yield return new WaitForSeconds(2f);
		pointerMessage = "No Input";
		StopCoroutine(WaitAndResetPointerMessage());
	}
	IEnumerator WaitAndResetSelectionMessage(){
		yield return new WaitForSeconds(2f);
		selectionMessage = "No Input";
		StopCoroutine(WaitAndResetSelectionMessage());
	}
	IEnumerator WaitAndResetPointerUpDownMessage(){
		yield return new WaitForSeconds(2f);
		pointerUpDownMessage = "No Input";
		StopCoroutine(WaitAndResetPointerUpDownMessage());
	}
	public void OnPointerEnter(PointerEventData eventData){
		pointerMessage = "OnPointerEnter";
		DebugHelper.PrintInRed(pointerMessage);
		// StartCoroutine(WaitAndResetPointerMessage());
	}
	public void OnPointerExit(PointerEventData eventData){
		pointerMessage = "OnPointerExit";
		DebugHelper.PrintInRed(pointerMessage);
		// StartCoroutine(WaitAndResetPointerMessage());
	}
	public void OnSelect(BaseEventData eventData){
		selectionMessage = "OnSelect";
		DebugHelper.PrintInBlue(selectionMessage);
		// StartCoroutine(WaitAndResetSelectionMessage());
	}
	public void OnDeselect(BaseEventData eventData){
		selectionMessage = "OnDeselect";
		DebugHelper.PrintInBlue(selectionMessage);
		// StartCoroutine(WaitAndResetSelectionMessage());
	}
	public void OnPointerDown(PointerEventData eventData){
		pointerUpDownMessage = "OnPointerDown";
		DebugHelper.PrintInGreen(pointerUpDownMessage);
		// StartCoroutine(WaitAndResetPointerUpDownMessage());
	}
	public void OnPointerUp(PointerEventData eventData){
		pointerUpDownMessage = "OnPointerUp";
		DebugHelper.PrintInGreen(pointerUpDownMessage);
		// StartCoroutine(WaitAndResetPointerUpDownMessage());
	}
	public void OnBeginDrag(PointerEventData eventData){
		dragMessage = "OnBeginDrag";
		DebugHelper.PrintInPink(dragMessage);
	}
	public void OnDrag(PointerEventData eventData){
		dragMessage = "OnDrag";
		DebugHelper.PrintInPink(dragMessage);
	}
	public void OnEndDrag(PointerEventData eventData){
		dragMessage = "OnEndDrag";
		DebugHelper.PrintInPink(dragMessage);
	}
}
