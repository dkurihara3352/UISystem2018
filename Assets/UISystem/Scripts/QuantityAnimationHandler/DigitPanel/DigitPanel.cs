using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IDigitPanel: IUIElement{
		void SetNumber(int number);
		/*  if number = -1, substitute the panel image with Blank image
		*/
	}
	public class DigitPanel: UIElement, IDigitPanel{
		public DigitPanel(IDigitPanelConstArg arg): base(arg){
			CalcAndSetRectDimension(arg.panelDim, arg.localPosY);
		}
		public void SetNumber(int number){
			((IDigitPanelAdaptor)GetUIAdaptor()).SetImageNumber(number);
		}
		void CalcAndSetRectDimension(Vector2 panelDim, float localPosY){
			IResizableRectUIAdaptor thisAdaptor = (IResizableRectUIAdaptor)this.GetUIAdaptor();
			thisAdaptor.SetRectDimension(panelDim.y, panelDim.x, 0f, localPosY);
		}
	}
	public interface IDigitPanelConstArg: IUIElementConstArg{
		Vector2 panelDim{get;}
		float localPosY{get;}
	}
	public class DigitPanelConstArg: UIElementConstArg, IDigitPanelConstArg{
		public DigitPanelConstArg(
			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uiElementFactory, 
			IDigitPanelAdaptor digitPanelAdaptor, 
			IUIImage image, 

			Vector2 panelDim, 
			float localPosY
		): base(
			uim, 
			processFactory, 
			uiElementFactory, 
			digitPanelAdaptor, 
			image,
			ActivationMode.None
		){
			thisPanelDim = panelDim;
			thisLocalPosY = localPosY;
		}
		Vector2 thisPanelDim;
		public Vector2 panelDim{get{return thisPanelDim;}}
		float thisLocalPosY;
		public float localPosY{get{return thisLocalPosY;}}
	}
}
