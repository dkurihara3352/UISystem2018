using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementSnapProcess: IScrollerElementMotorProcess{}
	public class ScrollerElementSnapProcess: AbsScrollerElementMotorProcess, IScrollerElementSnapProcess{
		public ScrollerElementSnapProcess(float targetElementLocalPositionOnAxis, float initialVelOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension, float diffThreshold, float stopVelocity, IProcessManager processManager): base(scroller, scrollerElement, dimension, processManager){

			float initialElementLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisTargetElementLocalPositionOnAxis = targetElementLocalPositionOnAxis;

			thisDiffThreshold = MakeSureThresholdIsGreaterThanZero(diffThreshold);
			thisStopVelocity = MakeSureThresholdIsGreaterThanZero(stopVelocity);
			ExpireIfValueDifferenceIsSmallEnough(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, diffThreshold);

			float springCoefficient = processManager.GetScrollerElementSnapSpringCoefficient();
			thisSpringCalculator = new RealTimeCriticallyDampedSpringCalculator(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, initialVelOnAxis, springCoefficient);
			
			prevLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
		}
		void ExpireIfValueDifferenceIsSmallEnough(float initValue, float termValue, float diffThreshold){
			if(Mathf.Abs(initValue - termValue) >= diffThreshold)
				Expire();
		}
		protected float MakeSureThresholdIsGreaterThanZero(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("source threshold must be greater than 0");
			else return source;
		}
		readonly float thisTargetElementLocalPositionOnAxis;
		readonly IRealTimeCriticallyDampedSpringCalculator thisSpringCalculator;
		readonly float thisDiffThreshold;
		
		readonly float thisStopVelocity;
		float prevLocalPosOnAxis;
		float thisElapsedTime = 0f;
		protected float GetDeltaValue(float newValue, float deltaT){
			return Mathf.Abs(newValue - prevLocalPosOnAxis) / deltaT;
		}

		public override void UpdateProcess(float deltaT){
			thisElapsedTime += deltaT;
			float newElementLocalPosOnAxis = thisSpringCalculator.GetSpringValue(thisElapsedTime);

			SetScrollerElementLocalPosOnAxis(newElementLocalPosOnAxis);

			if(GetDeltaValue(newElementLocalPosOnAxis, deltaT) <= thisStopVelocity)
				Expire();

			prevLocalPosOnAxis = newElementLocalPosOnAxis;
		}
		public override void Expire(){
			SetScrollerElementLocalPosOnAxis(thisTargetElementLocalPositionOnAxis);
			base.Expire();
		}
	}	
}
