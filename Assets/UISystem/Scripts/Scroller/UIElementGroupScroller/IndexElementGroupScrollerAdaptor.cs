using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIndexElementGroupScrollerAdaptor: IUIElementGroupScrollerAdaptor{}
	public class IndexElementGroupScrollerAdaptor : AbsScrollerAdaptor<IUIElementGroupScroller>, IIndexElementGroupScrollerAdaptor{
		public int initiallyCursoredElementIndex;
		public int[] cursorSize;
		public float startSearchSpeed;
		public bool swipeToSnapNext;
		protected override IUIElement CreateUIElement(IUIImage image){
			GenericUIElementGroupAdaptor uieGroupAdaptor = GetChildUIElementGroupAdaptor();
			IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(initiallyCursoredElementIndex, cursorSize, uieGroupAdaptor.groupElementLength, uieGroupAdaptor.padding, startSearchSpeed, relativeCursorPosition, this.scrollerAxis, rubberBandLimitMultiplier, isEnabledInertia, swipeToSnapNext, thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
			return new UIElementGroupScroller(arg);
		}
		GenericUIElementGroupAdaptor GetChildUIElementGroupAdaptor(){
			for(int i = 0; i < transform.childCount; i ++){
				Transform child = transform.GetChild(i);
				GenericUIElementGroupAdaptor adaptor = child.GetComponent<GenericUIElementGroupAdaptor>();
				if(adaptor != null)
					return adaptor;
			}
			throw new System.InvalidOperationException("there's no child with GenericUIElementGroupAdaptor attached");
		}
		Rect cursoredElementsRect;
		protected override void Awake(){
			base.Awake();
			Vector2 guiRectLength = new Vector2(120f, 100f);
			cursoredElementsRect = new Rect(new Vector2(Screen.width - guiRectLength.x, 10f), new Vector2(120f, 20f));
		}
		public override void OnGUI(){
			base.OnGUI();
			GUI.Label(cursoredElementsRect, "CursoredElements: ");
			GUI.Label(new Rect(cursoredElementsRect.position + new Vector2(0f, 20f), cursoredElementsRect.size), GetCursoredElementsIndex());
		}
		string GetCursoredElementsIndex(){
			if(this.GetUIElement() != null){
				IUIElementGroupScroller scroller = this.GetUIElement() as IUIElementGroupScroller;
				IUIElement[] cursoredElements = scroller.GetCursoredElements();
				if(cursoredElements == null){
					return " null";
				}else{
					List<int> indexList = new List<int>();
					foreach(IUIElement uie in cursoredElements){
						int uieIndex = scroller.GetGroupElementIndex(uie);
						indexList.Add(uieIndex);
					}
					string result = "";
					foreach(int i in indexList){
						result += i.ToString() +", ";
					}
					return result;
				}
			}
			return "scroller not set";
		}
	}
}
