using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement: IUIInputHandler{
		IUIElement GetParentUIE();
		List<IUIElement> GetChildUIEs();
		IUIManager GetUIM();
		IUIAdaptor GetUIAdaptor();
		IUIImage GetUIImage();
		IUIImage CreateUIImage();
		void Activate();
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
		public virtual void Activate(){
			foreach(IUIElement childUIE in this.GetChildUIEs()){
				if(childUIE != null)
					childUIE.Activate(); 
			}
		}
		/* SelectabilityState */
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
	public interface IUIImage{
		/* Color.Lerp(white, black, darknessValue)
		 */
		float GetCurrentDarkness();/* range is from 0f to 1f */
		float GetDefaultDarkness();/* usually, 1f */
		float GetDarkenedDarkness();/* somewhere around .5f */
		void SetDarkness(float darkness);
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
