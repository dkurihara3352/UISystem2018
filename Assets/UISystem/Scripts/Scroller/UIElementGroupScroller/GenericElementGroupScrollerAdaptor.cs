using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IGenericElementGroupScrollerAdaptor: IUIElementGroupScrollerAdaptor{}
	public class GenericElementGroupScrollerAdaptor : AbsScrollerAdaptor<IUIElementGroupScroller>, IGenericElementGroupScrollerAdaptor{
		public int initiallyCursoredElementIndex;
		public int[] cursorSize;
		public float startSearchSpeed;
		public bool swipeToSnapNext;
		public bool activatesCursoredElementsOnly;
		protected override IUIElement CreateUIElement(IUIImage image){
			IUIElementGroupAdaptor uieGroupAdaptor = GetChildUIElementGroupAdaptor();
			IUIElementGroupScrollerConstArg arg = new UIElementGroupScrollerConstArg(
				initiallyCursoredElementIndex, 
				cursorSize, 
				uieGroupAdaptor.GetGroupElementLength(), 
				uieGroupAdaptor.GetPadding(), 
				startSearchSpeed, 
				activatesCursoredElementsOnly, 

				relativeCursorPosition, 
				scrollerAxis, 
				rubberBandLimitMultiplier, 
				isEnabledInertia, 
				swipeToSnapNext, 
				newScrollSpeedThreshold,

				thisDomainActivationData.uim, 
				thisDomainActivationData.processFactory, 
				thisDomainActivationData.uiElementFactory, 
				this, 
				image
			);
			return new UIElementGroupScroller(arg);
		}
		IUIElementGroupAdaptor GetChildUIElementGroupAdaptor(){
			for(int i = 0; i < transform.childCount; i ++){
				Transform child = transform.GetChild(i);
				IUIElementGroupAdaptor adaptor = child.GetComponent<IUIElementGroupAdaptor>();
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
