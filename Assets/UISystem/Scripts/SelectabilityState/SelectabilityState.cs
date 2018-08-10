using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityState: DKUtility.ISwitchableState{
	}
	public abstract class AbsSelectabilityState: ISelectabilityState{
		public AbsSelectabilityState(IUIImage uiImage){
			thisUIImage = uiImage;
		}
		protected readonly IUIImage thisUIImage;
		public abstract void OnEnter();
		public virtual void OnExit(){}
	}
	public class SelectableState: AbsSelectabilityState{
		public SelectableState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			thisUIImage.TurnToSelectableDarkness();
		}
	}
	public class UnselectableState: AbsSelectabilityState{
		public UnselectableState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			thisUIImage.TurnToUnselectableDarkenss();
		}
	}
	public class SelectedState: AbsSelectabilityState{
		public SelectedState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			
		}
	}
}

