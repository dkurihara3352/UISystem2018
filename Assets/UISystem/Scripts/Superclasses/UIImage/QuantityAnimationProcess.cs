using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityAnimationProcess: IProcess{
	}
	public abstract class AbsQuantityAnimationProcess: AbsProcess, IQuantityAnimationProcess{
		public AbsQuantityAnimationProcess(IProcessManager procMan, IUIImage image, int sourceQuantity, int targetQuantity): base(procMan){
			thisImage = image;
			thisSourceQuantity = sourceQuantity;
			thisTargetQuantity = targetQuantity;
		}
		protected IUIImage thisImage;
		protected int thisSourceQuantity;
		protected int thisTargetQuantity;
	}
	public interface IIncrementalQuantityAnimationProcess: IQuantityAnimationProcess{

	}
	public class IncrementalQuantityAnimationProcess: AbsQuantityAnimationProcess, IIncrementalQuantityAnimationProcess{
		public IncrementalQuantityAnimationProcess(IProcessManager procMan, IUIImage image, int sourceQuantity, int targetQuantity): base(procMan, image, sourceQuantity, targetQuantity){
			totalTime = procMan.GetIncrementalQuantityAnimationProcessTotalTime();
		}
		readonly float totalTime;
		float elapsedTime = 0f;
		readonly IQuantityRoller thisQuantityRoller;
		public override void UpdateProcess(float deltaT){
			elapsedTime += deltaT;
			float normalizedT = elapsedTime/ totalTime;
			float springT = CalcSpringT(normalizedT);
			float rollerTargetValue = Mathf.Lerp(thisSourceQuantity/1f, thisTargetQuantity/1f, springT);
			thisQuantityRoller.Roll(rollerTargetValue);
			if(elapsedTime >= totalTime)
				this.Expire();
		}
		public override void Reset(){
			elapsedTime = 0f;
		}
		public override void Expire(){
			base.Expire();
			thisQuantityRoller.Roll(thisTargetQuantity/1f);
		}
	}
	public interface IOneshotQuantityAnimationProcess: IQuantityAnimationProcess{
	}
	public class OneshotQuantityAnimationProcess: AbsQuantityAnimationProcess, IOneshotQuantityAnimationProcess{
		public OneshotQuantityAnimationProcess(IProcessManager procMan, IUIImage image, int sourceQuantity, int targetQuantity): base(procMan, image, sourceQuantity, targetQuantity){}
		public override void UpdateProcess(float deltaT){}
		public override void Reset(){

		}
	}
}
