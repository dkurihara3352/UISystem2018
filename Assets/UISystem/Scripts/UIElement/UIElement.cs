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
		string GetName();
		void UpdateRect();
		/* Activation */
		void ActivateSelf(bool instantly);
		void ActivateRecursively(bool instantly);
		void DeactivateRecursively(bool instantly);
		void ActivateImple();
		void DeactivateImple();
		void OnActivationComplete();
		void OnDeactivationComplete();
		bool IsActivated();
		/*  */
		void EnableInput();
		void EnableInputRecursively();
		void DisableInputRecursively();
		void DisableScrollInputRecursively(IScroller disablingScroller);
		void EnableScrollInputRecursively();
		void EnableScrollInputSelf();
		void CheckForScrollInputEnable();
		IScroller GetTopmostScrollerInMotion();

		/* Scroller */
		T FindProximateParentTypedUIElement<T>() where T: class, IUIElement;
		void CheckAndPerformStaticBoundarySnapFrom(IUIElement uieToStartCheck);
		IScroller GetProximateParentScroller();
		void EvaluateScrollerFocusRecursively();
		void BecomeFocusedInScrollerSelf();
		void BecomeDefocusedInScrollerSelf();
		void BecomeFocusedInScrollerRecursively();
		void BecomeDefocusedInScrollerRecursively();
		/* PopUp */
		void PopUpDisableRecursivelyDownTo(IPopUp disablingPopUp);
		void ReversePopUpDisableRecursively();
		/* Debug */
		void TurnTo(Color color);
		void Flash(Color color);
	}
	public class UIElement: IUIElement{
		public UIElement(IUIElementConstArg arg){
			thisUIM = arg.uim;
			thisProcessFactory = arg.processFactory;
			thisUIElementFactory = arg.uiElementFactory;
			thisUIA = arg.uia;
			thisImage = arg.image;
			thisSelectabilityEngine = new SelectabilityStateEngine(
				thisImage, 
				thisUIM
			);
			if(arg.activationMode == ActivationMode.Alpha)
				thisUIA.SetUpCanvasGroupComponent();
			thisUIEActivationStateEngine = new UIEActivationStateEngine(
				thisProcessFactory, 
				this,
				arg.activationMode
			);
			/* move this to SetUpUIEReference */
			thisProximateParentScroller = FindProximateParentScroller();
		}
		protected readonly IUIManager thisUIM;
		public IUIManager GetUIM(){
			return thisUIM;
		}
		IPopUpManager thisPopUpManager{
			get{
				return thisUIM.GetPopUpManager();
			}
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
		protected string thisName{
			get{return thisUIA.GetName();}
		}
		public string GetName(){return thisName;}
		public void SetLocalPosition(Vector2 localPos){
			thisUIA.SetLocalPosition(localPos);
		}
		public Vector2 GetLocalPosition(){
			return thisUIA.GetLocalPosition();
		}
		public void SetParentUIE(IUIElement newParentUIE, bool worldPositionStays){
			thisUIA.SetParentUIE(newParentUIE, worldPositionStays);
		}
		public virtual void UpdateRect(){
			
		}

		/* Activation */
			protected IUIEActivationStateEngine thisUIEActivationStateEngine;
			public void ActivateSelf(bool instantly){
				thisUIEActivationStateEngine.Activate(instantly);
			}
			public virtual void ActivateRecursively(bool instantly){
				ActivateSelf(instantly);
				ActivateAllChildren(instantly);
			}
			protected void ActivateAllChildren(bool instantly){
				foreach(IUIElement childUIE in this.GetChildUIEs()){
					if(childUIE != null)
						childUIE.ActivateRecursively(instantly); 
				}
			}
			public void ActivateImple(){
				InitializeSelectabilityState();
				OnUIActivate();
			}
			protected virtual void OnUIActivate(){}
			public virtual void DeactivateRecursively(bool instantly){
				thisUIEActivationStateEngine.Deactivate(instantly);
				foreach(IUIElement childUIE in this.GetChildUIEs()){
					if(childUIE != null)
						childUIE.DeactivateRecursively(instantly);
				}
			}
			bool IsActivationComplete(){
				return thisUIEActivationStateEngine.IsActivationComplete();
			}
			public bool IsActivated(){
				return thisUIEActivationStateEngine.IsActivated();
			}
			public void DeactivateImple(){
				this.BecomeDefocusedInScrollerSelf();
				OnUIDeactivate();
			}
			protected virtual void OnUIDeactivate(){}
			public virtual void OnActivationComplete(){
			}
			public virtual void OnDeactivationComplete(){
			}
		/* SelectabilityState */
			protected virtual void InitializeSelectabilityState(){
				if(thisIsFocusedInScroller)
					BecomeSelectable();
				else
					BecomeUnselectable();
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
			protected bool thisIsEnabledInput = true;
			/* Touch */
			public void OnTouch(int touchCount){
				if(this.IsActivated() && thisIsEnabledInput){
					IScroller scrollerToStartPauseMotorProcess = GetTargetUIEOrItsProximateParentAsScroller(this);
					if(scrollerToStartPauseMotorProcess != null)
						scrollerToStartPauseMotorProcess.PauseRunningMotorProcessRecursivelyUp();
					OnTouchImple(touchCount);
				}
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
			/* delayed touch */
			public void OnDelayedTouch(){
				if(this.IsActivated() && thisIsEnabledInput)
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
			/* Release */
			public void OnRelease(){
				if(this.IsActivated() && thisIsEnabledInput){
					CheckAndPerformStaticBoundarySnapFrom(this);
					OnReleaseImple();
				}
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
			/* tap */
			public void OnTap(int tapCount){
				if(this.IsActivated()){
					if(thisIsDisabledForPopUp)
						thisPopUpManager.CheckAndHideActivePopUp();
					else{
						if(thisIsEnabledInput){
							CheckAndPerformStaticBoundarySnapFrom(this);
							OnTapImple(tapCount);
						}
						else
							PassOnTapUpward(tapCount);
					}
				}
			}
			protected virtual void OnTapImple(int tapCount){
				PassOnTapUpward(tapCount);
			}
			void PassOnTapUpward(int tapCount){
				if(thisParentUIE != null)
					thisParentUIE.OnTap(tapCount);
			}
			public void CheckAndPerformStaticBoundarySnapFrom(IUIElement uieToStartCheck){
				ClearTopMostScroller();
				IScroller scrollerToStartCheck = GetTargetUIEOrItsProximateParentAsScroller(uieToStartCheck);
				IScroller scrollerToExamine = scrollerToStartCheck;
				while(true){
					if(scrollerToExamine == null)
						break;
					scrollerToExamine.ResetDrag();
					scrollerToExamine.CheckAndPerformStaticBoundarySnap();
					scrollerToExamine = scrollerToExamine.GetProximateParentScroller();
				}
			}
			IScroller GetTargetUIEOrItsProximateParentAsScroller(IUIElement targetUIElement){
				if(targetUIElement != null){
					if(targetUIElement is IScroller)
						return (IScroller)targetUIElement;
					else
						return targetUIElement.GetProximateParentScroller();
				}else
					return null;
			}
			readonly protected IScroller thisProximateParentScroller;
			public IScroller GetProximateParentScroller(){
				return thisProximateParentScroller;
			}
			protected virtual IScroller FindProximateParentScroller(){
				return FindProximateParentTypedUIElement<IScroller>();
			}
			public virtual T FindProximateParentTypedUIElement<T>() where T: class, IUIElement{
				IProximateParentTypedUIECalculator<T> calculator = new ProximateParentTypedUIECalculator<T>(this);
				return calculator.Calculate();
			}
			void ClearTopMostScroller(){
				ClearAllParentScrollerVelocity();
				if(thisTopmostScrollerInMotion != null)
					thisTopmostScrollerInMotion.EnableScrollInputRecursively();
			}
			void ClearAllParentScrollerVelocity(){
				IScroller scrollerToExamine = GetTargetUIEOrItsProximateParentAsScroller(this);
				while(true){
					if(scrollerToExamine == null)
						break;
					for(int i = 0; i < 2; i ++){
						scrollerToExamine.UpdateVelocity(0f, i);
					}
					scrollerToExamine = scrollerToExamine.GetProximateParentScroller();
				}
			}
			/*  */
			public void OnDelayedRelease(){
				if(this.IsActivated() && thisIsEnabledInput)
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
			public void OnBeginDrag(ICustomEventData eventData){
				if(this.IsActivated() && thisIsEnabledInput)
					OnBeginDragImple(eventData);
				else
					PassOnBeginDragUpward(eventData);
			}
			protected virtual void OnBeginDragImple(ICustomEventData eventData){
				PassOnBeginDragUpward(eventData);
			}
			void PassOnBeginDragUpward(ICustomEventData eventData){
				if(thisParentUIE != null)
					thisParentUIE.OnBeginDrag(eventData);
			}

			public void OnDrag( ICustomEventData eventData){
				if(this.IsActivated() && thisIsEnabledInput)
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
				if(this.IsActivated() && thisIsEnabledInput)
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
				if(this.IsActivated() && thisIsEnabledInput)
					OnSwipeImple(eventData);
				else
					PassOnSwipeUpward(eventData);
			}
			protected virtual void OnSwipeImple(ICustomEventData eventData){
				PassOnSwipeUpward(eventData);
			}
			void PassOnSwipeUpward(ICustomEventData eventData){
				if(thisParentUIE != null)
					thisParentUIE.OnSwipe(eventData);
			}



		
		/*  */
		public void EnableInput(){
			thisIsEnabledInput = true;
			if(thisUIM.ShowsInputability())
				TurnTo(GetUIImage().GetDefaultColor());
		}
		public void EnableInputRecursively(){
			this.EnableInput();
			foreach(IUIElement child in thisChildUIEs)
				child.EnableInputRecursively();
		}
		protected void DisableInput(){
			thisIsEnabledInput = false;
			if(thisUIM.ShowsInputability())
				TurnTo(Color.red);
		}
		public void DisableInputRecursively(){
			this.DisableInput();
			foreach(IUIElement child in thisChildUIEs)
				child.DisableInputRecursively();
		}
		protected IScroller thisTopmostScrollerInMotion;
		public IScroller GetTopmostScrollerInMotion(){
			return thisTopmostScrollerInMotion;
		}
		public virtual void DisableScrollInputRecursively(IScroller disablingScroller){
			this.DisableInput();
			thisTopmostScrollerInMotion = disablingScroller;
			foreach(IUIElement child in thisChildUIEs){
				child.DisableScrollInputRecursively(disablingScroller);
			}
		}
		public virtual void EnableScrollInputRecursively(){
			EnableScrollInputSelf();
			foreach(IUIElement child in thisChildUIEs){
				child.EnableScrollInputRecursively();
			}
		}
		public virtual void EnableScrollInputSelf(){
			this.EnableInput();
			thisTopmostScrollerInMotion = null;
		}
		public virtual void CheckForScrollInputEnable(){
			this.EnableScrollInputSelf();
			foreach(IUIElement child in thisChildUIEs)
				child.CheckForScrollInputEnable();
		}



		/* Scrolller */
			public virtual void EvaluateScrollerFocusRecursively(){
				this.BecomeFocusedInScrollerSelf();
				foreach(IUIElement childUIE in GetChildUIEs())
					if(childUIE != null)
						childUIE.EvaluateScrollerFocusRecursively();
			}
			protected bool thisIsFocusedInScroller = false;
			public virtual void BecomeFocusedInScrollerSelf(){
				thisIsFocusedInScroller = true;
				BecomeSelectable();
			}
			public virtual void BecomeDefocusedInScrollerSelf(){
				thisIsFocusedInScroller = false;
				BecomeUnselectable();
			}
			public virtual void BecomeFocusedInScrollerRecursively(){
				BecomeDefocusedInScrollerSelf();
				foreach(IUIElement child in GetChildUIEs())
					child.BecomeFocusedInScrollerRecursively();
			}
			public virtual void BecomeDefocusedInScrollerRecursively(){
				BecomeDefocusedInScrollerSelf();
				foreach(IUIElement child in GetChildUIEs())
					child.BecomeDefocusedInScrollerRecursively();
			}
		/* PopUp */
		protected bool thisIsDisabledForPopUp = false;
		bool thisWasSelectableAtPopUpDisable;
		bool thisInputWasEnabledAtPopUpDisable;
		void DisableForPopUp(){
			thisIsDisabledForPopUp = true;
			thisWasSelectableAtPopUpDisable = this.IsSelectable();
			thisInputWasEnabledAtPopUpDisable = thisIsEnabledInput;
			BecomeUnselectable();
			DisableInput();
		}
		public void PopUpDisableRecursivelyDownTo(IPopUp disablingPopUp){
			if(this.IsActivated()){
				if(this == disablingPopUp)
					return;
				else{
					DisableForPopUp();
					foreach(IUIElement child in thisChildUIEs)
						if(child != null)
							child.PopUpDisableRecursivelyDownTo(disablingPopUp);
				}
			}
		}
		void ReverseDisableForPopUp(){
			thisIsDisabledForPopUp = false;
			if(thisWasSelectableAtPopUpDisable)
				BecomeSelectable();
			if(thisInputWasEnabledAtPopUpDisable)
				EnableInput();
		}
		public void ReversePopUpDisableRecursively(){
			ReverseDisableForPopUp();
			if(this.IsActivated()){
				foreach(IUIElement child in thisChildUIEs)
					if(child != null)
						child.ReversePopUpDisableRecursively();
			}
		}
		/*  */
		public void TurnTo(Color color){
			GetUIImage().TurnTo(color);
		}
		public void Flash(Color color){
			GetUIImage().Flash(color);
		}
	}







	public interface IUIElementConstArg{
		IUIManager uim{get;}
		IUISystemProcessFactory processFactory{get;}
		IUIElementFactory uiElementFactory{get;}
		IUIAdaptor uia{get;}
		IUIImage image{get;}
		ActivationMode activationMode{get;}
	}
	public class UIElementConstArg: IUIElementConstArg{
		public UIElementConstArg(
			IUIManager uim, 
			IUISystemProcessFactory processFactory, 
			IUIElementFactory uiElementFactory, 
			IUIAdaptor uia, 
			IUIImage image,
			ActivationMode activationMode

		){
			thisUIM = uim;
			thisProcessFactory = processFactory;
			thisUIElementFactory = uiElementFactory;
			thisUIA = uia;
			thisImage = image;
			thisActivationMode = activationMode;
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
		protected Vector2 ClampVector2ZeroToOne(Vector2 source){
			return new Vector2(Mathf.Clamp01(source.x), Mathf.Clamp01(source.y));			
		}
		readonly ActivationMode thisActivationMode;
		public ActivationMode activationMode{get{return thisActivationMode;}}
	}
}
