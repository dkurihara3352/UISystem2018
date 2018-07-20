using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationProcess: IWaitAndExpireProcess{
	}
	public interface INonActivatorUIEActivationProcess: IUIEActivationProcess{}
	public class NonActivatorUIEActivationProcess: GenericWaitAndExpireProcess, INonActivatorUIEActivationProcess{
		public NonActivatorUIEActivationProcess(IProcessManager processManager, float expireT): base(processManager, expireT){}
	}
	public interface IAlphaActivatorUIEActivationProcess: IUIEActivationProcess{
		void SetAlphaActivatorUIE(IAlphaActivatorUIElement uie);
	}
	public class AlphaActivatorUIEActivationProcess: AbsInterpolatorProcess<IGroupAlphaInterpolator>, IAlphaActivatorUIEActivationProcess{
		public AlphaActivatorUIEActivationProcess(IProcessManager processManager, float expireT, bool doesActivate): base(processManager, ProcessConstraint.expireTime, expireT, .05f, false){
			thisDoesActivate = doesActivate;
		}
		readonly bool thisDoesActivate;
		public void SetAlphaActivatorUIE(IAlphaActivatorUIElement uie){
			thisAlphaActivatorUIElement = uie;
		}
		IAlphaActivatorUIElement thisAlphaActivatorUIElement;
		protected override float GetLatestInitialValueDifference(){
			IUIEActivationState uieActivationState = (IUIEActivationState)thisProcessState;
			float currentGroupAlpha = thisAlphaActivatorUIElement.GetGroupAlphaForActivation();
			float targetGroupAlpha = thisDoesActivate? 1f: 0f;
			return targetGroupAlpha - currentGroupAlpha;
		}
		protected override IGroupAlphaInterpolator InstantiateInterpolatorWithValues(){
			float targetGroupAlpha = thisDoesActivate? 1f: 0f;
			IGroupAlphaInterpolator irper = new GroupAlphaInterpolator(thisAlphaActivatorUIElement, targetGroupAlpha);
			return irper;
		}
	}
	public interface IGroupAlphaInterpolator: IInterpolator{}
	public class GroupAlphaInterpolator: IGroupAlphaInterpolator{
		public GroupAlphaInterpolator(IAlphaActivatorUIElement alphaActivatorUIElement, float targetGroupAlpha){
			thisAlphaActivatorUIElement = alphaActivatorUIElement;
			thisTargetGroupAlpha = targetGroupAlpha;
			thisInitialGroupAlpha = alphaActivatorUIElement.GetGroupAlphaForActivation();
		}
		readonly IAlphaActivatorUIElement thisAlphaActivatorUIElement;
		readonly float thisTargetGroupAlpha;
		readonly float thisInitialGroupAlpha;
		public void Interpolate(float t){
			float newAlpha = Mathf.Lerp(thisInitialGroupAlpha, thisTargetGroupAlpha, t);
			thisAlphaActivatorUIElement.SetGroupAlphaForActivation(newAlpha);
		}
		public void Terminate(){}
	}
}
