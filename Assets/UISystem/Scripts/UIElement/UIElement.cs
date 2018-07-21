using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIElement: IUIInputHandler, ISelectabilityStateHandler{
		IUIElement GetParentUIE();
		void SetParentUIE(IUIElement uie, bool worldPositionStays);
		List<IUIElement> GetChildUIEs();
		Vector2 GetLocalPosition();
		void SetLocalPosition(Vector2 localPos);
		IUIAdaptor GetUIAdaptor();
		IUIImage GetUIImage();
		void ActivateRecursively();
		void ActivateInstantlyRecursively();
		void DeactivateRecursively();
		void DeactivateInstantlyRecursively();
		void ActivateImple();
		void DeactivateImple();
		bool IsActivationComplete();
		void OnScrollerFocus();
		void OnScrollerDefocus();
	}
	public abstract class AbsUIElement: IUIElement{
		public AbsUIElement(IUIElementConstArg arg){
			thisUIM = arg.uim;
			thisProcessFactory = arg.processFactory;
			thisUIElementFactory = arg.uiElementFactory;
			thisUIA = arg.uia;
			thisImage = arg.image;
			thisSelectabilityEngine = new SelectabilityStateEngine(this, thisProcessFactory);
			thisUIEActivationStateEngine = CreateUIEActivationStateEngine();
			thisUIEActivationStateEngine.DeactivateInstantly();
		}
		protected readonly IUIManager thisUIM;
		public IUIManager GetUIM(){
			return thisUIM;
		}
		protected readonly IUIAdaptor thisUIA;
		public IUIAdaptor GetUIAdaptor(){
			return thisUIA;
		}
		protected readonly IUISystemProcessFactory thisProcessFactory;
		protected readonly IUIElementFactory thisUIElementFactory;
		public IUIElement GetParentUIE(){
			return GetUIAdaptor().GetParentUIE();
		}
		public List<IUIElement> GetChildUIEs(){
			return GetUIAdaptor().GetChildUIEs();
		}
		public IUIImage GetUIImage(){
			return thisImage;
		}
		protected IUIImage thisImage;
		/* Activation */
			protected abstract IUIEActivationStateEngine CreateUIEActivationStateEngine();
			readonly IUIEActivationStateEngine thisUIEActivationStateEngine;
			public void ActivateRecursively(){
				thisUIEActivationStateEngine.Activate();
				foreach(IUIElement childUIE in this.GetChildUIEs()){
					if(childUIE != null)
						childUIE.ActivateRecursively(); 
				}
			}
			public void ActivateInstantlyRecursively(){
				thisUIEActivationStateEngine.ActivateInstantly();
				foreach(IUIElement childUIE in this.GetChildUIEs()){
					if(childUIE != null)
						childUIE.ActivateInstantlyRecursively(); 
				}
			}
			public virtual void ActivateImple(){
				InitializeSelectabilityState();
			}
			public virtual void DeactivateRecursively(){
				thisUIEActivationStateEngine.Deactivate();
				foreach(IUIElement childUIE in this.GetChildUIEs()){
					if(childUIE != null)
						childUIE.DeactivateRecursively();
				}
			}
			public void DeactivateInstantlyRecursively(){
				thisUIEActivationStateEngine.DeactivateInstantly();
				foreach(IUIElement child in GetChildUIEs())
					if(child != null)
						child.DeactivateInstantlyRecursively();
			}
			public bool IsActivationComplete(){
				return false;
			}
			public virtual void DeactivateImple(){

			}
		/* SelectabilityState */
			protected virtual void InitializeSelectabilityState(){
				BecomeSelectable();
			}
			ISelectabilityStateEngine thisSelectabilityEngine;
			public void BecomeSelectable(){
				thisSelectabilityEngine.BecomeSelectable();
			}
			public void BecomeUnselectable(){
				thisSelectabilityEngine.BecomeUnselectable();
			}
			public void BecomeSelected(){
				thisSelectabilityEngine.BecomeSelected();
			}
			public bool IsSelectable(){
				return thisSelectabilityEngine.IsSelectable();
			}
			public bool IsSelected(){
				return thisSelectabilityEngine.IsSelected();
			}
		/* UIInput */
			public virtual void OnTouch( int touchCount){
				if(GetParentUIE() != null)
					GetParentUIE().OnTouch(touchCount);
			}
			public virtual void OnDelayedTouch(){
				if(GetParentUIE() != null)
					GetParentUIE().OnDelayedTouch();
			}
			public virtual void OnRelease(){
				if(GetParentUIE() != null)
					GetParentUIE().OnRelease();
			}
			public virtual void OnDelayedRelease(){
				if(GetParentUIE() != null) 
					GetParentUIE().OnDelayedRelease();
			}
			public virtual void OnTap( int tapCount){
				if(GetParentUIE() != null)
					GetParentUIE().OnTap(tapCount);
			}
			public virtual void OnDrag( ICustomEventData eventData){
				if(GetParentUIE() != null)
					GetParentUIE().OnDrag(eventData);
			}
			public virtual void OnHold( float elapsedT){
				if(GetParentUIE() != null)
					GetParentUIE().OnHold(elapsedT);
			}
			public virtual void OnSwipe( ICustomEventData eventData){
				if(GetParentUIE() != null)
					GetParentUIE().OnSwipe(eventData);
			}
		/*  */
		public virtual void OnScrollerFocus(){
			foreach(IUIElement child in GetChildUIEs())
				child.OnScrollerFocus();
		}
		public virtual void OnScrollerDefocus(){
			foreach(IUIElement child in GetChildUIEs())
				child.OnScrollerDefocus();
		}
		public Vector2 GetPositionInThisSpace(Vector2 worldPos){
			return thisUIA.GetPositionInThisSpace(worldPos);
		}
		public void SetLocalPosition(Vector2 localPos){
			thisUIA.SetLocalPosition(localPos);
		}
		public Vector2 GetLocalPosition(){
			return thisUIA.GetLocalPosition();
		}
		public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
			thisUIA.SetParentUIE(newParentUIE, worldPositionStays);
		}
	}
	public interface IUIElementConstArg{
		IUIManager uim{get;}
		IUISystemProcessFactory processFactory{get;}
		IUIElementFactory uiElementFactory{get;}
		IUIAdaptor uia{get;}
		IUIImage image{get;}
	}
	public class UIElementConstArg: IUIElementConstArg{
		public UIElementConstArg(IUIManager uim, IUISystemProcessFactory processFactory, IUIElementFactory uiElementFactory, IUIAdaptor uia, IUIImage image){
			thisUIM = uim;
			thisProcessFactory = processFactory;
			thisUIElementFactory = uiElementFactory;
			thisUIA = uia;
			thisImage = image;
		}
		readonly IUIManager thisUIM;
		public IUIManager uim{get{return thisUIM;}}
		readonly IUISystemProcessFactory thisProcessFactory;
		public IUISystemProcessFactory processFactory{get{return thisProcessFactory;}}
		readonly IUIElementFactory thisUIElementFactory;
		public IUIElementFactory uiElementFactory{get{return thisUIElementFactory;}}
		readonly IUIAdaptor thisUIA;
		public IUIAdaptor uia{get{return thisUIA;}}
		readonly IUIImage thisImage;
		public IUIImage image{get{return thisImage;}}
	}
}
