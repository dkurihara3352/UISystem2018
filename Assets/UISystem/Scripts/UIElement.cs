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
	public abstract class AbsUIElement: IUIElement, ISelectabilityStateHandler{
		public AbsUIElement(IUIManager uim, IUIAdaptor uia){
			this.uiManager = uim;
			this.uiAdaptor = uia;
			IUIImage uiImage = this.CreateUIImage();
			this.uiImage = uiImage;
			this.selectabilityEngine = new SelectabilityStateEngine(this, uim.GetProcessFactory());
		}
		IUIManager uiManager;
		public IUIManager GetUIM(){
			return uiManager;
		}
		IUIAdaptor uiAdaptor;
		public IUIAdaptor GetUIAdaptor(){
			return uiAdaptor;
		}
		public IUIElement GetParentUIE(){
			return GetUIAdaptor().GetParentUIE();
		}
		public List<IUIElement> GetChildUIEs(){
			return GetUIAdaptor().GetChildUIEs();
		}
		public IUIImage GetUIImage(){
			return this.uiImage;
		}
		protected IUIImage uiImage;
		public abstract IUIImage CreateUIImage();
		ISelectabilityStateEngine selectabilityEngine;
		public void BecomeSelectable(){
			selectabilityEngine.BecomeSelectable();
		}
		public void BecomeUnselectable(){
			selectabilityEngine.BecomeUnselectable();
		}
		public void BecomeSelected(){
			selectabilityEngine.BecomeSelected();
		}
		public bool IsSelectable(){
			return selectabilityEngine.IsSelectable();
		}
		public bool IsSelected(){
			return selectabilityEngine.IsSelected();
		}
	}
	public interface IUIImage{
		float GetCurrentDarkness();
		float GetDefaultDarkness();
		float GetDarkenedDarkness();
		void SetDarkness(float darkness);
	}
}
