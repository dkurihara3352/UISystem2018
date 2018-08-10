using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityState: DKUtility.ISwitchableState{
	}
	// public abstract class TurnImageDarknessState: ISelectabilityState{
	// 	public TurnImageDarknessState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness){
	// 		thisProcessFactory = processFactory;
	// 		thisImage = image;
	// 		thisTargetDarkness = targetDarkness;
	// 	}
	// 	protected ITurnImageDarknessProcess thisProcess;
	// 	readonly IUISystemProcessFactory thisProcessFactory;
	// 	readonly IUIImage thisImage;
	// 	float thisTargetDarkness;
	// 	public void OnEnter(){
	// 		StartTurningImageDarkness();
	// 	}
	// 	public void OnExit(){
	// 		if(thisProcess.IsRunning())
	// 			thisProcess.Stop();
	// 		thisProcess = null;
	// 	}
	// 	void StartTurningImageDarkness(){
	// 		thisProcess = thisProcessFactory.CreateTurnImageDarknessProcess(thisImage, thisTargetDarkness);
	// 		thisProcess.Run();
	// 	}
	// }
	public abstract class AbsSelectabilityState: ISelectabilityState{
		public AbsSelectabilityState(IUIImage uiImage){
			thisUIImage = uiImage;
		}
		protected readonly IUIImage thisUIImage;
		public abstract void OnEnter();
		public virtual void OnExit(){}
	}
	public class SelectableState: /* TurnImageDarknessState */AbsSelectabilityState{
		// public SelectableState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness): base(processFactory, image, targetDarkness){}
		public SelectableState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			thisUIImage.TurnToSelectableDarkness();
		}
	}
	public class UnselectableState: /* TurnImageDarknessState */AbsSelectabilityState{
		// public UnselectableState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness): base(processFactory, image, targetDarkness){}
		public UnselectableState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			thisUIImage.TurnToSelectableDarkness();
		}
	}
	public class SelectedState: /* ISelectabilityState */AbsSelectabilityState{
		// public SelectedState(IUIElement uie){
		// 	// no process required.
		// }
		public SelectedState(IUIImage uiImage): base(uiImage){}
		public override void OnEnter(){
			
		}
	}
}

