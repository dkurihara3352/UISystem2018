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
	public class SelectabilityStateEngine: AbsSwitchableStateEngine<ISelectabilityState>, ISelectabilityStateEngine{
		public SelectabilityStateEngine(IUIElement uie, IProcessFactory procFac){
			IUIImage image = uie.GetUIImage();
			ITurnImageDarknessProcess turnToDefaultProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDefaultDarkness());
			ITurnImageDarknessProcess turnToDarkenedProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDarkenedDarkness());

			selectableState = new SelectableState(turnToDefaultProcess);
			unselectableState = new UnselectableState(turnToDarkenedProcess);
			selectedState = new SelectedState(uie);

			MakeSureStatesAreSet();

			this.SetToInitialState();
		}
		protected readonly SelectableState selectableState;
		protected readonly UnselectableState unselectableState;
		protected readonly SelectedState selectedState;
		void MakeSureStatesAreSet(){
			if(selectableState != null && unselectableState != null && selectedState != null)
				return;
			else
				throw new System.InvalidOperationException("any of the states not correctly set");
		}
		void SetToInitialState(){
			BecomeSelectable();
		}
		/* SelStateHandler */
			public void BecomeSelectable(){
				TrySwitchState(selectableState);
			}
			public void BecomeUnselectable(){
				TrySwitchState(unselectableState);
			}
			public void BecomeSelected(){
				if(this.IsSelectable() || this.IsSelected())
					TrySwitchState(selectedState);
				else
					throw new System.InvalidOperationException("This method should not be called while this is not selectable");
			}
			public bool IsSelectable(){
				return thisCurState is SelectableState;
			}
			public bool IsSelected(){
				return thisCurState is SelectedState;
			}
	}
}
