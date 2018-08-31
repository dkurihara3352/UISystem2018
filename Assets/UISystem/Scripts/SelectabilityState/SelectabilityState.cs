using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityState: DKUtility.ISwitchableState{
	}
	public abstract class AbsSelectabilityState: ISelectabilityState{
		public AbsSelectabilityState(IUIImage uiImage, IUIManager uim){
			thisUIImage = uiImage;
			thisUIM = uim;
		}
		protected readonly IUIImage thisUIImage;
		protected readonly IUIManager thisUIM;
		public abstract void OnEnter();
		public virtual void OnExit(){}
	}
	public class SelectableState: AbsSelectabilityState{
		public SelectableState(IUIImage uiImage, IUIManager uim): base(uiImage, uim){}
		public override void OnEnter(){
			if(thisUIM.ShowsNormal())
			thisUIImage.TurnToSelectableBrightness();
		}
	}
	public class UnselectableState: AbsSelectabilityState{
		public UnselectableState(IUIImage uiImage, IUIManager uim): base(uiImage, uim){}
		public override void OnEnter(){
			if(thisUIM.ShowsNormal())
			thisUIImage.TurnToUnselectableBrightness();
		}
	}
	public class SelectedState: AbsSelectabilityState{
		public SelectedState(IUIImage uiImage, IUIManager uim): base(uiImage, uim){}
		public override void OnEnter(){
			
		}
	}
}

