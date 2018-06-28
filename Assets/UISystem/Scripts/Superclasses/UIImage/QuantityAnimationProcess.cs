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
		public IncrementalQuantityAnimationProcess(IProcessManager procMan, IUIImage image, int sourceQuantity, int targetQuantity): base(procMan, image, sourceQuantity, targetQuantity){}
		float totalTime;
		public override void UpdateProcess(float deltaT){
			
		}
		public override void Reset(){}
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
