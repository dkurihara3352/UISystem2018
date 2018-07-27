using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementSnapProcess: IScrollerElementMotorProcess{}
	public class ScrollerElementSnapProcess: AbsScrollerElementMotorProcess, IScrollerElementSnapProcess{
		public ScrollerElementSnapProcess(float targetElementLocalPositionOnAxis, float initialVelOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension, float diffThreshold, IProcessManager processManager): base(scroller, scrollerElement, dimension, processManager){

			float initialElementLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisTargetElementLocalPositionOnAxis = targetElementLocalPositionOnAxis;

			ExpireIfValueDifferenceIsSmallEnough(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, diffThreshold);

			float springCoefficient = processManager.GetScrollerElementSnapSpringCoefficient();
			int resolution = processManager.GetScrollerElementSnapSpringResolution();
			thisSpringCalculator = new RealTimeCriticallyDampedSpringCalculator(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, initialVelOnAxis, springCoefficient);
			thisDiffThreshold = MakeSureThresholdIsGreaterThanZero(diffThreshold);
		}
		void ExpireIfValueDifferenceIsSmallEnough(float initValue, float termValue, float diffThreshold){
			if(Mathf.Abs(initValue - termValue) >= diffThreshold)
				Expire();
		}
		protected float MakeSureThresholdIsGreaterThanZero(float source){
			if(source < 0f)
				throw new System.InvalidOperationException("source threshold must be greater than 0");
			else return source;
		}
		readonly float thisTargetElementLocalPositionOnAxis;
		readonly IRealTimeCriticallyDampedSpringCalculator thisSpringCalculator;
		readonly float thisDiffThreshold;
		
		float thisElapsedTime = 0f;
		float thisPrevValue = 0f;
		float GetDeltaValue(float newValue){
			return Mathf.Abs(newValue - thisPrevValue);
		}

		public override void UpdateProcess(float deltaT){
			thisElapsedTime += deltaT;
			float newElementLocalPosOnAxis = thisSpringCalculator.GetSpringValue(thisElapsedTime);

			if(GetDeltaValue(newElementLocalPosOnAxis) <= thisDiffThreshold)
				Expire();

			SetScrollerElementLocalPosOnAxis(newElementLocalPosOnAxis);
			thisPrevValue = newElementLocalPosOnAxis;
		}
		public override void Expire(){
			SetScrollerElementLocalPosOnAxis(thisTargetElementLocalPositionOnAxis);
			base.Expire();
		}
	}	
}
