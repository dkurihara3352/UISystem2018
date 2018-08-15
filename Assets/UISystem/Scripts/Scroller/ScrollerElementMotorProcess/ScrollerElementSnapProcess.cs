using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementSnapProcess: IScrollerElementMotorProcess{}
	public class ScrollerElementSnapProcess: AbsScrollerElementMotorProcess, IScrollerElementSnapProcess{
		public ScrollerElementSnapProcess(float targetElementLocalPositionOnAxis, float initialVelOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension, float diffThreshold, float stopVelocity, IProcessManager processManager): base(scroller, dimension, processManager){

			float initialElementLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisTargetElementLocalPositionOnAxis = targetElementLocalPositionOnAxis;

			thisDiffThreshold = MakeSureDiffThresholdIsInRange(diffThreshold);
			ExpireIfValueDifferenceIsSmallEnough(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, diffThreshold);

			float springCoefficient = processManager.GetScrollerElementSnapSpringCoefficient();
			thisSpringCalculator = new RealTimeCriticallyDampedSpringCalculator(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, initialVelOnAxis, springCoefficient);
			
			prevLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisInitVel = initialVelOnAxis;
		}
		void ExpireIfValueDifferenceIsSmallEnough(float initValue, float termValue, float diffThreshold){
			if(Mathf.Abs(initValue - termValue) <= diffThreshold){
				Expire();
				return;
			}
		}
		protected float MakeSureDiffThresholdIsInRange(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("source threshold must be greater than 0");
			else{
				if(source < thisMinDiffThreshold)
					return thisMinDiffThreshold;
				else
					return source;
			}
		}
		protected float MakeSureThresholdIsGreaterThanZero(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("source threshold must be greater than 0");
			else return source;
		}
		readonly float thisTargetElementLocalPositionOnAxis;
		readonly IRealTimeCriticallyDampedSpringCalculator thisSpringCalculator;
		readonly float thisDiffThreshold;
		const float thisMinDiffThreshold = 1f;
		
		float prevLocalPosOnAxis;
		float thisElapsedTime = 0f;
		readonly float thisInitVel;
		float GetVelocity(float newValue, float deltaT){
			if(deltaT == 0f)
				return thisInitVel;
			return (newValue - prevLocalPosOnAxis)/ deltaT;
		}

		public override void UpdateProcess(float deltaT){
			thisElapsedTime += deltaT;
			float newElementLocalPosOnAxis = thisSpringCalculator.GetSpringValue(thisElapsedTime);

			thisScroller.SetScrollerElementLocalPosOnAxis(newElementLocalPosOnAxis, thisDimension);
			
			float velocity = GetVelocity(newElementLocalPosOnAxis, deltaT);
			thisScroller.UpdateVelocity(velocity, thisDimension);
			
			prevLocalPosOnAxis = newElementLocalPosOnAxis;
			
			if(ScrollerElementLocalPosIsCloseEnoughToTarget()){
				Expire();
			}

		}
		bool ScrollerElementLocalPosIsCloseEnoughToTarget(){
			float diff = prevLocalPosOnAxis - thisTargetElementLocalPositionOnAxis;
			return Mathf.Abs(diff) <= thisDiffThreshold;
		}
		public override void Expire(){
			thisScroller.SetScrollerElementLocalPosOnAxis(thisTargetElementLocalPositionOnAxis, thisDimension);
			thisScroller.UpdateVelocity(0f, thisDimension);
			base.Expire();
		}
	}	
}
