using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IColorChangeButtonAdaptor: IUIAdaptor{}
	public class ColorChangeButtonAdaptor : UIAdaptor, IColorChangeButtonAdaptor {

		public UIAdaptor targetUIElementAdaptor;
		public Color targetColor;
		public UnityEngine.UI.Text targetText;
		protected override IUIElement CreateUIElement(IUIImage image){
			IColorChangeButtonConstArg arg = new ColorChangeButtonConstArg(
				thisDomainInitializationData.uim,
				thisDomainInitializationData.processFactory,
				thisDomainInitializationData.uiElementFactory,
				this,
				image,
				targetUIElementAdaptor,
				targetColor,
				targetText
			);
			ColorChangeButton button = new ColorChangeButton(arg);
			return button;
		}
		protected override void SetUpUIElementReferenceImple(){
			base.SetUpUIElementReferenceImple();
			IColorChangeButton button = (IColorChangeButton)GetUIElement();
			IUIElement targetUIElement = targetUIElementAdaptor.GetUIElement();
			button.SetTargetUIElement(targetUIElement);
		}
	}
}
