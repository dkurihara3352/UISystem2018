using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement: IUIInputHandler, ISelectabilityStateHandler{
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement uie, bool worldPositionStays);
		List<IUIElement> GetChildUIEs();
		Vector2 GetPositionInThisSpace(Vector2 worldPos);
		void SetLocalPosition(Vector2 localPos);
		IUIManager GetUIM();
		IUIAdaptor GetUIAdaptor();
		IUIImage GetUIImage();
		void Activate();
		void Deactivate();
	}
	public abstract class AbsUIElement: IUIElement{
		public AbsUIElement(IUIElementConstArg arg){
			this.uiManager = arg.uim;
			this.uiAdaptor = arg.uia;
			this.uiImage = arg.image;
			this.selectabilityEngine = new SelectabilityStateEngine(this, uiManager.GetProcessFactory());
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
		public virtual void Activate(){
			this.ActivateImple();
			foreach(IUIElement childUIE in this.GetChildUIEs()){
				if(childUIE != null)
					childUIE.Activate(); 
			}
		}
		protected virtual void ActivateImple(){
			InitializeSelectabilityState();
		}
		public virtual void Deactivate(){
			this.DeactivateImple();
			foreach(IUIElement childUIE in this.GetChildUIEs()){
				if(childUIE != null)
					childUIE.Deactivate();
			}
		}
		protected virtual void DeactivateImple(){}
		/* SelectabilityState */
			void InitializeSelectabilityState(){
				BecomeSelectable();
			}
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
		/* UIInput */
			public virtual void OnTouch( int touchCount){}
			public virtual void OnDelayedTouch(){}
			public virtual void OnRelease(){}
			public virtual void OnDelayedRelease(){}
			public virtual void OnTap( int tapCount){}
			public virtual void OnDrag( Vector2 pos, Vector2 deltaP){}
			public virtual void OnHold( float elapsedT){}
			public virtual void OnSwipe( Vector2 deltaP){}
		/*  */
		public Vector2 GetPositionInThisSpace(Vector2 worldPos){
			return this.uiAdaptor.GetPositionInThisSpace(worldPos);
		}
		public void SetLocalPosition(Vector2 localPos){
			this.uiAdaptor.SetLocalPosition(localPos);
		}
		public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
			this.uiAdaptor.SetParentUIE(newParentUIE, worldPositionStays);
		}
	}
	public interface IUIElementConstArg{
		IUIManager uim{get;}
		IUIAdaptor uia{get;}
		IUIImage image{get;}
	}
	public class UIElementConstArg: IUIElementConstArg{
		readonly IUIManager _uim;
		readonly IUIAdaptor _uia;
		readonly IUIImage _image;
		public UIElementConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image){
			this._uim = uim;
			this._uia = uia;
			this._image = image;
		}
		public IUIManager uim{get{return _uim;}}
		public IUIAdaptor uia{get{return _uia;}}
		public IUIImage image{get{return _image;}}
	}
	public interface IUIInputHandler{
		/* Releasing
			pointer up =>
				OnRelease
				if deltaP over thresh
					OnSwipe
				else
					if stays in-bound && within time frame
						OnTap

		*/
		void OnTouch( int touchCount);
		void OnDelayedTouch();
		void OnRelease();
		void OnDelayedRelease();
		/* called after both OnRelease and OnTap */
		void OnTap( int tapCount);
		void OnDrag( Vector2 pos, Vector2 deltaP);
		void OnHold( float deltaT);
		/* called every frame from pointer down to up */
		void OnSwipe( Vector2 deltaP);
	}
	public interface IVisibilitySwitcher{
		/* subclassed by same uies that are also IRootActivator?
			.Tools, Widgets
			** implementation idea **
			. CanvasGroup is assigned, and tweak its group alpha or like
		 */
		void Show();
		void Hide();
	}
}
