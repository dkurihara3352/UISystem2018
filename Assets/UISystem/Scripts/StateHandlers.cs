using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityStateHandler{
		void BecomeSelectable();
		void BecomeUnselectable();
		void BecomeSelected();
		bool IsSelectable();
		bool IsSelected();
	}
	public interface ISelectabilityStateEngine: ISelectabilityStateHandler{
		
	}
	public class SelectabilityStateEngine: ISelectabilityStateEngine{
		public void BecomeSelectable(){
		}
		public void BecomeUnselectable(){
		}
		public void BecomeSelected(){
		}
		public bool IsSelectable(){return false;}
		public bool IsSelected(){return false;}
	}
	public interface ISelectabilityState{
		void OnEnter();
		void OnExit();
	}
	public class SelectableState: ISelectabilityState{
		public SelectableState(IUIElement uie){
			this._uiImage = uie.GetUIImage();
		}
		IUIImage _uiImage;
		public void OnEnter(){
			this.StartTuringUIImageToDefaultDarkness();
		}
		public void OnExit(){
			this.StopTurningUIImageToDefaultDarkness();
		}
	}
}

