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
		IUIImage CreateUIImage();
	}
	public abstract class AbsUIElement: IUIElement{
		public AbsUIElement(IUIManager uim, IUIAdaptor uia){
			this._uiManager = uim;
			this._uiAdaptor = uia;
			IUIImage uiImage = this.CreateUIImage();
			this._uiImage = uiImage;
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
		public IUIImage GetUIImage(){
			return this._uiImage;
		}
		IUIImage _uiImage;
		public abstract IUIImage CreateUIImage();
	}
	public interface IUIImage{
	}
}
