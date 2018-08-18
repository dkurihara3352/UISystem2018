using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUIEActivationProcess: IWaitAndExpireProcess{
	}
	public interface INonActivatorUIEActivationProcess: IUIEActivationProcess{}
	public class NonActivatorUIEActivationProcess: GenericWaitAndExpireProcess, INonActivatorUIEActivationProcess{
		public NonActivatorUIEActivationProcess(
			IProcessManager processManager, 
			float expireT,
			IUIEActivationStateEngine engine, 
			bool doesActivate
		): 
		base(
			processManager, 
			expireT, 
			null
		){
		}
		protected readonly IUIEActivationStateEngine thisEngine;
		protected readonly bool thisDoesActivate;
		public override void Expire(){
			base.Expire();
			if(thisDoesActivate)
				thisEngine.SetToActivationCompletedState();
			else
				thisEngine.SetToDeactivationCompletedState();
		}
	}
	public interface IAlphaActivatorUIEActivationProcess: IUIEActivationProcess{
	}
	public class AlphaActivatorUIEActivationProcess: AbsInterpolatorProcess<IGroupAlphaInterpolator>, IAlphaActivatorUIEActivationProcess{
		public AlphaActivatorUIEActivationProcess(
			IProcessManager processManager, 
			float expireT, 
			IUIEActivationStateEngine engine,
			IUIElement uiElement,
			bool doesActivate
		): base(
			processManager, 
			ProcessConstraint.expireTime, 
			expireT, 
			.05f, 
			false, 
			null
		){
			thisEngine = engine;
			thisUIElement = uiElement;
			thisDoesActivate = doesActivate;
			thisUIAdaptor = thisUIElement.GetUIAdaptor();
		}
		readonly IUIEActivationStateEngine thisEngine;
		readonly IUIElement thisUIElement;
		readonly bool thisDoesActivate;
		readonly IUIAdaptor thisUIAdaptor;
		protected override float GetLatestInitialValueDifference(){
			float currentGroupAlpha = thisUIAdaptor.GetGroupAlpha();
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
			IGroupAlphaInterpolator irper = new GroupAlphaInterpolator(thisUIAdaptor, targetGroupAlpha);
			return irper;
		}
		public override void Expire(){
			base.Expire();
			if(thisDoesActivate)
				thisEngine.SetToActivationCompletedState();
			else
				thisEngine.SetToDeactivationCompletedState();
		}
	}
	public interface IGroupAlphaInterpolator: IInterpolator{}
	public class GroupAlphaInterpolator: IGroupAlphaInterpolator{
		public GroupAlphaInterpolator(IUIAdaptor uiAdaptor, float targetGroupAlpha){
			thisUIAdaptor = uiAdaptor;
			thisTargetGroupAlpha = targetGroupAlpha;
			thisInitialGroupAlpha = uiAdaptor.GetGroupAlpha();
		}
		readonly IUIAdaptor thisUIAdaptor;
		readonly float thisTargetGroupAlpha;
		readonly float thisInitialGroupAlpha;
		public void Interpolate(float t){
			float newAlpha = Mathf.Lerp(thisInitialGroupAlpha, thisTargetGroupAlpha, t);
			thisUIAdaptor.SetGroupAlpha(newAlpha);
		}
		public void Terminate(){}
	}
}
