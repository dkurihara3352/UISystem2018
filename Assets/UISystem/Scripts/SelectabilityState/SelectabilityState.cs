using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityState: DKUtility.ISwitchableState{
	}
	public abstract class TurnImageDarknessState: ISelectabilityState{
		public TurnImageDarknessState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness){
			thisProcessFactory = processFactory;
			thisImage = image;
			thisTargetDarkness = targetDarkness;
		}
		protected ITurnImageDarknessProcess thisProcess;
		readonly IUISystemProcessFactory thisProcessFactory;
		readonly IUIImage thisImage;
		float thisTargetDarkness;
		public void OnEnter(){
			StartTurningImageDarkness();
		}
		public void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
		void StartTurningImageDarkness(){
			thisProcess = thisProcessFactory.CreateTurnImageDarknessProcess(thisImage, thisTargetDarkness);
			thisProcess.Run();
		}
	}
	public class SelectableState: TurnImageDarknessState{
		public SelectableState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness): base(processFactory, image, targetDarkness){}
	}
	public class UnselectableState: TurnImageDarknessState{
		public UnselectableState(IUISystemProcessFactory processFactory, IUIImage image, float targetDarkness): base(processFactory, image, targetDarkness){}
	}
	public class SelectedState: ISelectabilityState{
		public SelectedState(IUIElement uie){
			// no process required.
		}
		public void OnEnter(){
			// CursorManager.MoveCursor(this.image);
		}
		public void OnExit(){}
	}
}

