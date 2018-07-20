using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DKUtility{
	public interface ISwitchableStateEngine<T> where T: ISwitchableState{
		void TrySwitchState(T state);
	}
	public interface ISwitchableState{
		void OnEnter();
		void OnExit();
	}

	public class AbsSwitchableStateEngine<T>: ISwitchableStateEngine<T> where T: ISwitchableState{
		public void TrySwitchState(T state){
			if(state != null){
				if( thisCurState != null){
					if(thisCurState.GetType().Equals(state.GetType())){
						return;/* no state change */
					}else{
						thisCurState.OnExit();
						thisCurState = state;
						state.OnEnter();
					}
				}else{
					thisCurState = state;
					state.OnEnter();
				}
			}else{
				throw new System.ArgumentNullException("state", "switching to null state is not allowed");
			}
		}
		protected T thisCurState;
	}
}

