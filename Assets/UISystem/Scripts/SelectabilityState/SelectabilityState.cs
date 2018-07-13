using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityState: DKUtility.ISwitchableState{
	}
	public abstract class TurnImageDarknessState: ISelectabilityState{
		protected ITurnImageDarknessProcess process;
		public TurnImageDarknessState(ITurnImageDarknessProcess process){
			this.process = process;
		}
		public void OnEnter(){
			StartTurningImageDarkness();
		}
		public void OnExit(){
			if(process.IsRunning())
				process.Stop();
		}
		void StartTurningImageDarkness(){
			process.Run();
		}
	}
	public class SelectableState: TurnImageDarknessState{
		public SelectableState(ITurnImageDarknessProcess process): base(process){}
	}
	public class UnselectableState: TurnImageDarknessState{
		public UnselectableState(ITurnImageDarknessProcess process): base(process){}
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

