using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IInertialScrollProcess: IScrollerElementMotorProcess{}
	public class InertialScrollProcess: AbsScrollerElementMotorProcess, IInertialScrollProcess{
		public InertialScrollProcess(float initialDeltaPosOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension, IProcessManager processManager): base(scroller, scrollerElement, dimension, processManager){
			thisInitialVelocity = initialDeltaPosOnAxis;
			thisPrevVelocity = thisInitialVelocity;
			thisPrevLocalPosOnAxis = scrollerElement.GetLocalPosition()[dimension];
			float deceleration = processManager.GetInertialScrollDeceleration();
			thisDeceleration = MakeSureDecelerationIsGreaterThanZero(deceleration);

			thisExpireTime = Mathf.Abs(thisInitialVelocity / thisDeceleration);
		}
		readonly float thisInitialVelocity;
		readonly protected float thisDeceleration;
		float MakeSureDecelerationIsGreaterThanZero(float source){
			if(source <= 0f)
				throw new System.InvalidOperationException("deceleration must be greater than zero");
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
			// UpdateScrollerElementLocalPosOnAxis(deltaPosOnAxis);
			SetScrollerElementLocalPosOnAxis(newLocalPosOnAxis);
			thisPrevVelocity = newVelocity;
			thisPrevLocalPosOnAxis = newLocalPosOnAxis;
			thisScroller.CheckForDynamicBoundarySnap(deltaPosOnAxis, thisDimension);

			if(thisElapsedTime >= thisExpireTime)
				Expire();
		}
	}
}

