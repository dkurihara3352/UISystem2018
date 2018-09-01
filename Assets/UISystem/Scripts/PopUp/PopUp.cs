using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUp: IUIElement, IPopUpEventTrigger{
		void OnHideBegin();
		void OnShowBegin();
		void OnHideComplete();
		void OnShowComplete();

		bool HidesOnTappingOthers();
		void ShowHiddenProximateParentPopUpRecursively();
		void HideShownChildPopUpsRecursively();
		IPopUp GetProximateParentPopUp();
		void RegisterProximateChildPopUp(IPopUp childPopUp);
		bool IsHidden();
		bool IsShown();
		bool IsAncestorOf(IPopUp other);
		void SetUpPopUpHierarchy();
	}
	public enum PopUpMode{
		Alpha,
	}
	public class PopUp : UIElement, IPopUp {
		public PopUp(IPopUpConstArg arg): base(arg){
			thisPopUpManager = arg.popUpManager;
			thisHidesOnTappingOthers = arg.hidesOnTappingOthers;

			IPopUpStateEngineConstArg popUpStateEngineConstArg = new PopUpStateEngineConstArg(
				thisProcessFactory,
				this,
				thisPopUpManager,
				arg.popUpMode
			);
			thisStateEngine = new PopUpStateEngine(popUpStateEngineConstArg);
			if(arg.popUpMode == PopUpMode.Alpha)
				this.GetUIAdaptor().SetUpCanvasGroupComponent();
			GetPopUpAdaptor().ToggleRaycastBlock(false);
		}
		protected virtual IPopUpAdaptor GetPopUpAdaptor(){
			return (IPopUpAdaptor)GetUIAdaptor();
		}
		readonly IPopUpManager thisPopUpManager;
		public bool HidesOnTappingOthers(){
			return thisHidesOnTappingOthers;
		}
		readonly bool thisHidesOnTappingOthers;
		protected readonly IPopUpStateEngine thisStateEngine;
		public void Hide(bool instantly){
			thisStateEngine.Hide(instantly);
		}
		public void Show(bool instantly){
			thisStateEngine.Show(instantly);
		}
		public bool IsHidden(){
			return thisStateEngine.IsHidden();
		}
		public bool IsShown(){
			return thisStateEngine.IsShown();
		}
		public virtual void OnShowBegin(){}
		public virtual void OnHideBegin(){}
		public virtual void OnShowComplete(){}
		public virtual void OnHideComplete(){}
		public void SetUpPopUpHierarchy(){
			thisProximateParentPopUp = FindProximateParentPopUp();

			if(thisProximateParentPopUp != null)
				thisProximateParentPopUp.RegisterProximateChildPopUp(this);
			thisProximateChildPopUps = new List<IPopUp>();
		}
		protected virtual IPopUp FindProximateParentPopUp(){
			return FindProximateParentTypedUIElement<IPopUp>();
		}
		IPopUp thisProximateParentPopUp;
		public IPopUp GetProximateParentPopUp(){
			return thisProximateParentPopUp;
		}
		public void ShowHiddenProximateParentPopUpRecursively(){
			if(thisProximateParentPopUp != null){
				if(thisProximateParentPopUp.IsHidden()){
					thisProximateParentPopUp.Show(false);
					thisProximateParentPopUp.ShowHiddenProximateParentPopUpRecursively();
				}
			}
		}
		List<IPopUp> thisProximateChildPopUps;
		public void RegisterProximateChildPopUp(IPopUp childPopUp){
			thisProximateChildPopUps.Add(childPopUp);
		}
		public void HideShownChildPopUpsRecursively(){
			foreach(IPopUp childPopUp in thisProximateChildPopUps){
				if(childPopUp.IsActivated() && childPopUp.IsShown()){
					childPopUp.Hide(false);
					childPopUp.HideShownChildPopUpsRecursively();
				}
			}
		}
		public bool IsAncestorOf(IPopUp other){
			IPopUp popUpToExamine = other.GetProximateParentPopUp();
			while(true){
				if(popUpToExamine == null)
					return false;
				if(popUpToExamine == this)
					return true;
				popUpToExamine = popUpToExamine.GetProximateParentPopUp();
			}
		}
		protected override void OnTapImple(int tapCount){
			CheckAndPerformStaticBoundarySnapFrom(this);
			return;
		}
	}
	public interface IPopUpConstArg: IUIElementConstArg{
		IPopUpManager popUpManager{get;}
		bool hidesOnTappingOthers{get;}
		PopUpMode popUpMode{get;}
	}
	public class PopUpConstArg: UIElementConstArg, IPopUpConstArg{
		public PopUpConstArg(
			IUIManager uim,
			IUISystemProcessFactory processFactory,
			IUIElementFactory uiElementFactory,
			IPopUpAdaptor popUpAdaptor,
			IUIImage image,
			ActivationMode activationMode,

			IPopUpManager popUpManager,
			bool hidesOnTappingOthers,
			PopUpMode popUpMode
			
		): base(
			uim,
			processFactory,
			uiElementFactory,
			popUpAdaptor,
			image,
			activationMode
		){
			thisPopUpManager = popUpManager;
			thisHidesOnTappingOthers = hidesOnTappingOthers;
			thisPopUpMode = popUpMode;
		}
		readonly IPopUpManager thisPopUpManager;
		public IPopUpManager popUpManager{get{return thisPopUpManager;}}
		readonly bool thisHidesOnTappingOthers;
		public bool hidesOnTappingOthers{get{return thisHidesOnTappingOthers;}}
		readonly PopUpMode thisPopUpMode;
		public PopUpMode popUpMode{get{return thisPopUpMode;}}

	}

}
