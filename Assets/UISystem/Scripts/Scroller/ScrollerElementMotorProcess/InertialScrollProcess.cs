using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IInertialScrollProcess: IScrollerElementMotorProcess{}
	public class InertialScrollProcess: AbsScrollerElementMotorProcess, IInertialScrollProcess{
		public InertialScrollProcess(
			IInertialScrollProcessConstArg arg
		): base(
			arg
		){
			thisInitialVelocity = arg.initialVelocity;
			thisPrevVelocity = thisInitialVelocity;
			thisPrevLocalPosOnAxis = thisScrollerElement.GetLocalPosition()[thisDimension];
			thisDeceleration = MakeSureDecelerationIsGreaterThanZero(arg.deceleration) * MakeSureDecelAxisCompMultiplierIsNotLessThanZero(arg.decelerationAxisComponentMultiplier);
			if(thisDeceleration == 0f || thisInitialVelocity == 0f)
				thisExpireTime = 0f;
			else
				thisExpireTime = Mathf.Abs(thisInitialVelocity / thisDeceleration);
			thisScroller.UpdateVelocity(thisInitialVelocity, thisDimension);
		}
		readonly float thisInitialVelocity;
		readonly protected float thisDeceleration;
		float MakeSureDecelerationIsGreaterThanZero(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("deceleration must be greater than zero");
			return source;
		}
		float MakeSureDecelAxisCompMultiplierIsNotLessThanZero(float source){
			if(source < 0f)
				throw new System.InvalidOperationException("deceleration axis component multiplier must not be less than zero");
			else
				return source;
		}
		readonly protected float thisExpireTime;

		protected float thisPrevVelocity;
		protected float thisElapsedTime = 0f;
		float thisPrevLocalPosOnAxis;
		public override void UpdateProcess(float deltaT){
			thisElapsedTime += deltaT;

			float deltaV = - deltaT * thisDeceleration;
			if(thisInitialVelocity < 0f)
				deltaV *= -1f;

			float newVelocity = thisPrevVelocity + deltaV;
			float deltaPosOnAxis = newVelocity * deltaT;
			float newLocalPosOnAxis = thisPrevLocalPosOnAxis + deltaPosOnAxis;
			thisScroller.SetScrollerElementLocalPosOnAxis(newLocalPosOnAxis, thisDimension);
			thisPrevVelocity = newVelocity;
			thisPrevLocalPosOnAxis = newLocalPosOnAxis;
			thisScroller.UpdateVelocity(newVelocity, thisDimension);
			thisScroller.CheckAndPerformDynamicBoundarySnapOnAxis(deltaPosOnAxis, newVelocity, thisDimension);

			if(thisElapsedTime >= thisExpireTime)
				Expire();
		}
		protected override void ExpireImple(){
			base.ExpireImple();
			thisScroller.UpdateVelocity(0f, thisDimension);
		}
	}
	public interface IInertialScrollProcessConstArg: IScrollerElementMotorProcessConstArg{
		float initialVelocity{get;}
		float deceleration{get;}
		float decelerationAxisComponentMultiplier{get;}
	}
	public class InertialScrollProcessConstArg: ScrollerElementMotorProcessConstArg, IInertialScrollProcessConstArg{
		public InertialScrollProcessConstArg(
			IProcessManager processManager,
			IScroller scroller,
			IUIElement scrollerElement,
			int dimension,

			float initialVelocity,
			float deceleration,
			float decelerationAxisComponentMultiplier
		): base(
			processManager,
			scroller,
			scrollerElement,
			dimension
		){
			thisInitialVelocity = initialVelocity;
			thisDeceleration = deceleration;
			thisDecelerationAxisComponentMultiplier = decelerationAxisComponentMultiplier;
		}
		readonly float thisInitialVelocity;
		public float initialVelocity{get{return thisInitialVelocity;}}
		readonly float thisDeceleration;
		public float deceleration{get{return thisDeceleration;}}
		readonly float thisDecelerationAxisComponentMultiplier;
		public float decelerationAxisComponentMultiplier{get{return thisDecelerationAxisComponentMultiplier;}}
	}
}

