﻿using System.Collections;
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
		/* Activation */
		void ActivateRecursively();
		void ActivateInstantlyRecursively();
		void DeactivateRecursively();
		void DeactivateInstantlyRecursively();
		void ActivateImple();
		void DeactivateImple();
		/*  */
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
		protected IUIElement thisParentUIE{
			get{return thisUIA.GetParentUIE();}
		}
		public IUIElement GetParentUIE(){
			return thisParentUIE;
		}
		protected List<IUIElement> thisChildUIEs{
			get{return thisUIA.GetChildUIEs();}
		}
		public List<IUIElement> GetChildUIEs(){
			return thisChildUIEs;
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
			bool IsActivationComplete(){
				return thisUIEActivationStateEngine.IsActivationComplete();
			}
			bool IsActivated(){
				return thisUIEActivationStateEngine.IsActivated();
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
			protected virtual bool IsEnabledInput(){
				return this.IsActivated();
			}

			public void OnTouch(int touchCount){
				if(this.IsEnabledInput())
					OnTouchImple(touchCount);
				else
					PassOnTouchUpward(touchCount);
			}
			protected virtual void OnTouchImple(int touchCount){
				PassOnTouchUpward(touchCount);
			}
			void PassOnTouchUpward(int touchCount){
				if(thisParentUIE != null)
					thisParentUIE.OnTouch(touchCount);
			}
			
			public void OnDelayedTouch(){
				if(this.IsEnabledInput())
					OnDelayedTouchImple();
				else
					PassOnDelayedTouchUpward();
			}
			protected virtual void OnDelayedTouchImple(){
				PassOnDelayedTouchUpward();
			}
			void PassOnDelayedTouchUpward(){
				if(thisParentUIE != null)
					thisParentUIE.OnDelayedTouch();
			}

			public void OnRelease(){
				if(this.IsEnabledInput())
					OnReleaseImple();
				else
					PassOnReleaseUpward();
			}
			protected virtual void OnReleaseImple(){
				PassOnReleaseUpward();
			}
			void PassOnReleaseUpward(){
				if(thisParentUIE != null)
					thisParentUIE.OnRelease();
			}

			public void OnDelayedRelease(){
				if(this.IsEnabledInput())
					OnDelayedReleaseImple();
				else
					PassOnDelayedReleaseUpward();
			}
			protected virtual void OnDelayedReleaseImple(){
				PassOnDelayedReleaseUpward();
			}
			void PassOnDelayedReleaseUpward(){
				if(thisParentUIE != null)
					thisParentUIE.OnDelayedRelease();
			}

			public void OnTap(int tapCount){
				if(this.IsEnabledInput())
					OnTapImple(tapCount);
				else
					PassOnTapUpward(tapCount);
			}
			protected virtual void OnTapImple(int tapCount){
				PassOnTapUpward(tapCount);
			}
			void PassOnTapUpward(int tapCount){
				if(thisParentUIE != null)
					thisParentUIE.OnTap(tapCount);
			}

			public void OnDrag( ICustomEventData eventData){
				if(this.IsEnabledInput())
					OnDragImple(eventData);
				else
					PassOnDragUpward(eventData);
			}
			protected virtual void OnDragImple(ICustomEventData eventData){
				PassOnDragUpward(eventData);
			}
			void PassOnDragUpward(ICustomEventData eventData){
				if(thisParentUIE != null)
					thisParentUIE.OnDrag(eventData);
			}

			public void OnHold( float elapsedT){
				if(this.IsEnabledInput())
					OnHoldImple(elapsedT);
				else
					PassOnHoldUpward(elapsedT);
			}
			protected virtual void OnHoldImple(float elapsedT){
				PassOnHoldUpward(elapsedT);
			}
			void PassOnHoldUpward(float elapsedT){
				if(thisParentUIE != null)
					thisParentUIE.OnHold(elapsedT);
			}

			public void OnSwipe( ICustomEventData eventData){
				if(GetParentUIE() != null)
					GetParentUIE().OnSwipe(eventData);
			}
			protected virtual void OnSwipeImple(ICustomEventData eventData){
				PassOnSwipeUpward(eventData);
			}
			void PassOnSwipeUpward(ICustomEventData eventData){
				if(thisParentUIE != null)
					thisParentUIE.OnSwipe(eventData);
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