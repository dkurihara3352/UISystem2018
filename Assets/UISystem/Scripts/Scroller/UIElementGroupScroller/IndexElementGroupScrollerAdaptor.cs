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
			IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(initiallyCursoredElementIndex, cursorSize, uieGroupAdaptor.groupElementLength, uieGroupAdaptor.padding, startSearchSpeed, relativeCursorPosition, scrollerAxis, rubberBandLimitMultiplier, isEnabledInertia, swipeToSnapNext, thisDomainActivationData.uim, thisDomainActivationData.processFactory, thisDomainActivationData.uiElementFactory, this, image);
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
		string GetCursoredElementsString(){
			if(this.GetUIElement() == null)
				return "uie is not set yet";
			else{
				IUIElementGroupScroller scroller = this.GetUIElement() as IUIElementGroupScroller;
				IUIElement[] cursoredElements = scroller.GetCursoredElements();
				string result = "null";
				if(cursoredElements != null){
					result = "";
					foreach(IUIElement uie in cursoredElements){
						if(uie == null)
							result += "null, ";
						else{
							int i = scroller.GetGroupElementIndex(uie);
							result += i.ToString() + ", ";
						}
					}
				}
				return result;
			}
		}
	}
}
