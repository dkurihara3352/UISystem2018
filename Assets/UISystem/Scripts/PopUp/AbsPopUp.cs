using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUp: IUIElement, IPopUpEventTrigger{
		void OnHideBegin();
		void OnShowBegin();
		void OnHideComplete();
		void OnShowComplete();

		bool DisablesOtherElements();
		void ShowHiddenProximateParentPopUpRecursively();
		void HideShownChildPopUpsRecursively();
		IPopUp GetProximateParentPopUp();
		void RegisterProximateChildPopUp(IPopUp childPopUp);
		bool IsHidden();
		bool IsShown();
	}
	public enum PopUpMode{
		Alpha,
	}
	public class PopUp : UIElement, IPopUp {
		public PopUp(IPopUpConstArg arg): base(arg){
			thisPopUpManager = arg.popUpManager;
			thisDisablesOthers = arg.disablesOthers;
			thisHidesOnTappingOthers = arg.hidesOnTappingOthers;

			IPopUpStateEngineConstArg popUpStateEngineConstArg = new PopUpStateEngineConstArg(
				thisProcessFactory,
				this,
				thisPopUpManager,
				arg.popUpMode
			);
			thisStateEngine = new PopUpStateEngine(popUpStateEngineConstArg);
			if(
				arg.popUpMode == PopUpMode.Alpha && 
				arg.activationMode != ActivationMode.Alpha
			)
				this.GetUIAdaptor().SetUpCanvasGroupComponent();

			thisProximateParentPopUp = FindProximateParentTypedUIElement<IPopUp>();

			if(thisProximateParentPopUp != null)
				thisProximateParentPopUp.RegisterProximateChildPopUp(this);
			thisProximateChildPopUps = new List<IPopUp>();
		}
		readonly IPopUpManager thisPopUpManager;
		readonly bool thisDisablesOthers;
		public bool DisablesOtherElements(){
			return thisDisablesOthers;
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
	}
	public interface IPopUpConstArg: IUIElementConstArg{
		IPopUpManager popUpManager{get;}
		bool disablesOthers{get;}
		bool hidesOnTappingOthers{get;}
		PopUpMode popUpMode{get;}
	}

}
