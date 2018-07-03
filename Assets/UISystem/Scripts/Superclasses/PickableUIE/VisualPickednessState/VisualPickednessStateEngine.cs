using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IVisualPickednessHandler{
		void BecomeVisuallyPickedUp();
		void BecomeVisuallyUnpicked();
	}
	public interface IVisualPickednessStateEngine: ISwitchableStateEngine<IVisualPickednessState>, IVisualPickednessHandler{
		void SetPickableUIImage(IPickableUIImage pickableUIImage);
		void SetToVisuallyUnpickedState();
		void SetToBecomingVisuallyPickedUpState();
		void SetToVisuallyPickedUpState();
		void SetToBecomingVisuallyUnpickedState();
	}
	public class VisualPickednessStateEngine: AbsSwitchableStateEngine<IVisualPickednessState>, IVisualPickednessStateEngine{
		public VisualPickednessStateEngine(IProcessFactory processFactory){
			thisVisuallyUnpickedState = new VisuallyUnpickedState(this);
			thisBecomingVisuallyPickedUpState = new BecomingVisuallyPickedUpState(this, processFactory);
			thisVisuallyPickedUpState = new VisuallyPickedUpState(this);
			thisBecomingVisuallyUnpickedState = new BecomingVisuallyUnpickedState(this, processFactory);
			thisStates = new List<IVisualPickednessState>(new IVisualPickednessState[]{
				thisVisuallyUnpickedState,
				thisBecomingVisuallyPickedUpState,
				thisVisuallyUnpickedState,
				thisBecomingVisuallyUnpickedState
			});
		}
		readonly List<IVisualPickednessState> thisStates;
		readonly IVisuallyUnpickedState thisVisuallyUnpickedState;
		readonly IBecomingVisuallyPickedUpState thisBecomingVisuallyPickedUpState;
		readonly IVisuallyPickedUpState thisVisuallyPickedUpState;
		readonly IBecomingVisuallyUnpickedState thisBecomingVisuallyUnpickedState;
		public void SetPickableUIImage(IPickableUIImage pickableUIImage){
			foreach(IVisualPickednessState state in thisStates){
				state.SetPickableUIImage(pickableUIImage);
			}
		}
		/* delegate */
		public void BecomeVisuallyPickedUp(){
			thisCurState.BecomeVisuallyPickedUp();
		}
		public void BecomeVisuallyUnpicked(){
			thisCurState.BecomeVisuallyUnpicked();
		}
		/* switch */
		public void SetToVisuallyUnpickedState(){
			TrySwitchState(thisVisuallyUnpickedState);
		}
		public void SetToBecomingVisuallyPickedUpState(){
			TrySwitchState(thisBecomingVisuallyPickedUpState);
		}
		public void SetToVisuallyPickedUpState(){
			TrySwitchState(thisVisuallyUnpickedState);
		}
		public void SetToBecomingVisuallyUnpickedState(){
			TrySwitchState(thisBecomingVisuallyUnpickedState);
		}
	}
}
