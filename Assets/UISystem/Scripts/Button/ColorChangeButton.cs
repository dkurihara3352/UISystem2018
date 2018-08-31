using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using UnityEngine.UI;
namespace UISystem{
	public interface IColorChangeButton: IUIElement{
		void SetTargetUIElement(IUIElement targetUIElement);
	}
	public class ColorChangeButton : UIElement, IColorChangeButton {

		public ColorChangeButton(IColorChangeButtonConstArg arg): base(arg){
			thisTargetColor = arg.targetColor;
			thisTargetText = arg.targetText;
		}
		IUIElement thisTargetUIElement;
		public void SetTargetUIElement(IUIElement targetUIElement){
			thisTargetUIElement = targetUIElement;
		}
		readonly Text thisTargetText;
		
		Color thisTargetUIEDefaultColor{
			get{
				IUIImage uiImage = thisTargetUIElement.GetUIImage();
				return uiImage.GetDefaultColor();
			}
		}
		readonly Color thisTargetColor;
		bool thisIsChangingToTarget = true;
		protected override void OnTapImple(int tapCount){
			thisIsChangingToTarget = !thisIsChangingToTarget;
			this.Flash(thisTargetColor);
			if(thisIsChangingToTarget)
				thisTargetUIElement.TurnTo(thisTargetUIEDefaultColor);
			else
				thisTargetUIElement.TurnTo(thisTargetColor);
		}
		int count = 0;
		protected override void OnUIActivate(){
			UpdateText();
		}
		void UpdateText(){
			thisTargetText.text = count.ToString();
		}
		protected override void OnTouchImple(int touchCount){
			count ++;
			UpdateText();
		}
		protected override void OnDelayedReleaseImple(){
			count = 0;
			UpdateText();
		}
	}
	public interface IColorChangeButtonConstArg: IUIElementConstArg{
		IUIAdaptor targetUIAdaptor{get;}
		Color targetColor{get;}
		Text targetText{get;}
	}
	public class ColorChangeButtonConstArg: UIElementConstArg, IColorChangeButtonConstArg{
		public ColorChangeButtonConstArg(
			IUIManager uim,
			IUISystemProcessFactory processFactory,
			IUIElementFactory uieFactory,
			IColorChangeButtonAdaptor uia,
			IUIImage uiImage,

			IUIAdaptor targetUIAdaptor,
			Color targetColor,
			Text targetText
		): base(
			uim,
			processFactory,
			uieFactory,
			uia,
			uiImage,
			ActivationMode.None
		){
			thisTargetUIAdaptor = targetUIAdaptor;
			thisTargetColor = targetColor;
			thisText = targetText;
		}
		readonly IUIAdaptor thisTargetUIAdaptor;
		public IUIAdaptor targetUIAdaptor{get{return thisTargetUIAdaptor;}}
		readonly Color thisTargetColor;
		public Color targetColor{get{return thisTargetColor;}}
		readonly Text thisText;
		public Text targetText{get{return thisText;}}
	}
}

