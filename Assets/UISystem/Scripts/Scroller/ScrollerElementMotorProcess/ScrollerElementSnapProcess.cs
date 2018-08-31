using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
using DKUtility.CurveUtility;

namespace UISystem{
	public interface IScrollerElementSnapProcess: IScrollerElementMotorProcess{}
	public class ScrollerElementSnapProcess: AbsScrollerElementMotorProcess, IScrollerElementSnapProcess{
		public ScrollerElementSnapProcess(
			IScrollerElementSnapProcessConstArg arg
		): base(
			arg
		){
			IUIElement scrollerElement = arg.scrollerElement;
			int dimension = arg.dimension;
			float targetElementLocalPositionOnAxis = arg.targetElementLocalPositionOnAxis;
			float initialVelOnAxis = arg.initialVelocityOnAxis;

			float initialElementLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisTargetElementLocalPositionOnAxis = targetElementLocalPositionOnAxis;

			float springCoefficient = thisProcessManager.GetScrollerElementSnapSpringCoefficient();
			thisSpringCalculator = new RealTimeCriticallyDampedSpringCalculator(initialElementLocalPosOnAxis, targetElementLocalPositionOnAxis, initialVelOnAxis, springCoefficient);
			
			prevLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			thisInitVel = initialVelOnAxis;
		}
		readonly float thisTargetElementLocalPositionOnAxis;
		readonly IRealTimeCriticallyDampedSpringCalculator thisSpringCalculator;
		protected readonly float thisDiffThreshold = 1f;
		
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
		protected override void ExpireImple(){
			base.ExpireImple();
			thisScroller.SetScrollerElementLocalPosOnAxis(thisTargetElementLocalPositionOnAxis, thisDimension);
			thisScroller.UpdateVelocity(0f, thisDimension);
		}
	}
	public interface IScrollerElementSnapProcessConstArg: IScrollerElementMotorProcessConstArg{
		float targetElementLocalPositionOnAxis{get;}
		float initialVelocityOnAxis{get;}
		float stopVelocity{get;}
	}
	public class ScrollerElementSnapProcessConstArg: ScrollerElementMotorProcessConstArg, IScrollerElementSnapProcessConstArg{
		public ScrollerElementSnapProcessConstArg(
			IProcessManager processManager,
			IUIElement scrollerElement,
			IScroller scroller,
			int dimension,

			float targetElementLocalPositionOnAxis,
			float initialVelocityOnAxis,
			float stopVelocity
		): base(
			processManager,
			scroller,
			scrollerElement,
			dimension
		){
			thisTargetElementLocalPositionOnAxis = targetElementLocalPositionOnAxis;
			thisInitialVelocityOnAxis = initialVelocityOnAxis;
			thisStopVelocity = stopVelocity;
		}
		readonly float thisTargetElementLocalPositionOnAxis;
		public float targetElementLocalPositionOnAxis{get{return thisTargetElementLocalPositionOnAxis;}}
		readonly float thisInitialVelocityOnAxis;
		public float initialVelocityOnAxis{get{return thisInitialVelocityOnAxis;}}
		readonly float thisStopVelocity;
		public float stopVelocity{get{return thisStopVelocity;}}
	}
}
