using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIAWaitForTapProcess: IUIAdaptorInputProcess{}
	public class UIAWaitForTapProcess : AbsUIAdaptorInputProcess, IUIAWaitForTapProcess {
		public UIAWaitForTapProcess(IUIAdaptorInputProcessConstArg arg): base(arg){

		}
		IWaitingForTapState typedState{
			get{
				return (IWaitingForTapState)thisState;
			}
		}
		protected override void UpdateProcessImple(float deltaT){
			thisEngine.HoldUIE(deltaT);
		}
		protected override void ExpireImple(){
			thisEngine.WaitForRelease();
			thisEngine.DelayTouchUIE();
		}
	}
	public interface IUIAWaitForReleaseProcess: IUIAdaptorInputProcess{}
	public class UIAWaitForReleaseProcess: AbsUIAdaptorInputProcess, IUIAWaitForReleaseProcess{
		public UIAWaitForReleaseProcess(IUIAdaptorInputProcessConstArg arg): base(arg){}
		protected override void UpdateProcessImple(float deltaT){
			thisEngine.HoldUIE(deltaT);
		}
	}
	public interface IUIAWaitForNextTouchProcess: IUIAdaptorInputProcess{}
	public class UIAWaitForNextTouchProcess: AbsUIAdaptorInputProcess, IUIAWaitForNextTouchProcess{
		public UIAWaitForNextTouchProcess(IUIAdaptorInputProcessConstArg arg): base(arg){}
		protected override void ExpireImple(){
			thisEngine.WaitForFirstTouch();
			thisEngine.DelayedReleaseUIE();
		}
	}
}
