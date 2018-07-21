using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationProcess: IWaitAndExpireProcess{
	}
	public interface INonActivatorUIEActivationProcess: IUIEActivationProcess{}
	public class NonActivatorUIEActivationProcess: GenericWaitAndExpireProcess, INonActivatorUIEActivationProcess{
		public NonActivatorUIEActivationProcess(IProcessManager processManager, float expireT, IUIEActivationProcessState state): base(processManager, expireT, state){}
	}
	public interface IAlphaActivatorUIEActivationProcess: IUIEActivationProcess{
	}
	public class AlphaActivatorUIEActivationProcess: AbsInterpolatorProcess<IGroupAlphaInterpolator>, IAlphaActivatorUIEActivationProcess{
		public AlphaActivatorUIEActivationProcess(IProcessManager processManager, float expireT, bool doesActivate, IUIEActivationProcessState state, IAlphaActivatorUIElement alphaActivatorUIElement): base(processManager, ProcessConstraint.expireTime, expireT, .05f, false, state){
			thisDoesActivate = doesActivate;
			thisAlphaActivatorUIElement = alphaActivatorUIElement;
		}
		readonly bool thisDoesActivate;
		readonly IAlphaActivatorUIElement thisAlphaActivatorUIElement;
		protected override float GetLatestInitialValueDifference(){
			float currentGroupAlpha = thisAlphaActivatorUIElement.GetNormalizedGroupAlphaForActivation();
			currentGroupAlpha = MakeGroupAlphaValueInRange(currentGroupAlpha);
			float targetGroupAlpha = thisDoesActivate? 1f: 0f;
			return targetGroupAlpha - currentGroupAlpha;
		}
		float MakeGroupAlphaValueInRange(float source){
			if(source < 0f)
				return 0f;
			else if(source > 1f)
				return 1f;
			else return source;
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
			thisInitialGroupAlpha = alphaActivatorUIElement.GetNormalizedGroupAlphaForActivation();
		}
		readonly IAlphaActivatorUIElement thisAlphaActivatorUIElement;
		readonly float thisTargetGroupAlpha;
		readonly float thisInitialGroupAlpha;
		public void Interpolate(float t){
			float newAlpha = Mathf.Lerp(thisInitialGroupAlpha, thisTargetGroupAlpha, t);
			thisAlphaActivatorUIElement.SetNormalizedGroupAlphaForActivation(newAlpha);
		}
		public void Terminate(){}
	}
}
