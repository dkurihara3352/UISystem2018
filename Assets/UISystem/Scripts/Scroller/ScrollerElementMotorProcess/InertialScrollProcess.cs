using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IInertialScrollProcess: IScrollerElementMotorProcess{}
	public class InertialScrollProcess: AbsScrollerElementMotorProcess, IInertialScrollProcess{
		public InertialScrollProcess(float deltaPosOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension, IProcessManager processManager): base(scroller, scrollerElement, dimension, processManager){
			thisInitialVelocity = deltaPosOnAxis;
			float deceleration = processManager.GetInertialScrollDeceleration();
			thisDeceleration = MakeSureDecelerationIsGreaterThanZero(deceleration);

			thisExpireTime = Mathf.Abs(thisInitialVelocity / thisDeceleration);
		}
		readonly float thisInitialVelocity;
		readonly float thisDeceleration;
		float MakeSureDecelerationIsGreaterThanZero(float source){
			if(source < 0f)
				throw new System.InvalidOperationException("deceleration must be greater than zero");
			return source;
		}
		readonly float thisStopThresholdDeltaVelocity;
		readonly float thisExpireTime;
		bool thisIsScrolledInPositiveDirection{
			get{return thisInitialVelocity > 0f;}
		}

		float thisPrevVelocity = 0f;
		float thisElapsedTime = 0f;
		public override void UpdateProcess(float deltaT){
			thisElapsedTime += deltaT;

			float deltaV = - (deltaT * thisDeceleration);
			float newVelocity = thisPrevVelocity + deltaV;
			float deltaPosOnAxis = newVelocity * deltaT;
			UpdateScrollerElementLocalPosOnAxis(deltaPosOnAxis);
			thisPrevVelocity = newVelocity;
			thisScroller.CheckForDynamicBoundarySnap(deltaPosOnAxis, thisDimension);

			if(thisElapsedTime >= thisExpireTime)
				Expire();
		}
	}
}

