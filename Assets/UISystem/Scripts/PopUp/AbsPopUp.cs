﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPopUp: IUIElement, IPopUpEventTrigger{
		void OnHideBegin();
		void OnShowBegin();
		void OnHideComplete();
		void OnShowComplete();

		bool DisablesOtherElements();
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
			if(arg.popUpMode == PopUpMode.Alpha)
				this.GetUIAdaptor().SetUpCanvasGroupComponent();
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
		public virtual void OnShowBegin(){}
		public virtual void OnHideBegin(){}
		public virtual void OnShowComplete(){}
		public virtual void OnHideComplete(){}
	}
	public interface IPopUpConstArg: IUIElementConstArg{
		IPopUpManager popUpManager{get;}
		bool disablesOthers{get;}
		bool hidesOnTappingOthers{get;}
		PopUpMode popUpMode{get;}
	}

}
