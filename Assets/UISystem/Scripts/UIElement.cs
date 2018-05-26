using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement{
		IUIElement GetParentUIE();
		List<IUIElement> GetChildUIEs();
		IUIManager GetUIM();
		IUIAdaptor GetUIAdaptor();
		IUIImage GetUIImage();
	}
	public class UIElement: IUIElement{
		public UIElement(IUIManager uim, IUIAdaptor uia){
			this._uiManager = uim;
			this._uiAdaptor = uia;
		}
		IUIManager _uiManager;
		public IUIManager GetUIM(){
			return _uiManager;
		}
		IUIAdaptor _uiAdaptor;
		public IUIAdaptor GetUIAdaptor(){
			return _uiAdaptor;
		}
		public IUIElement GetParentUIE(){
			return GetUIAdaptor().GetParentUIE();
		}
		public List<IUIElement> GetChildUIEs(){
			return GetUIAdaptor().GetChildUIEs();
		}
		public virtual IUIImage GetUIImage(){
			return null;
		}
	}
	public interface IUIImage{
		void SetDarkness(float darkness);
		float GetDarkness();
		float GetDefaultDarkness();
		float GetDisabledDarkness();
		void SetAlpha(float a);
		float GetAlpha();
	}
}
