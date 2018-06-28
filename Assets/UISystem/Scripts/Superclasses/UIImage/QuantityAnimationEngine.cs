using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityAnimationHandler{
		void AnimateQuantityImageIncrementally(int sourceQuantity, int targetQuantity);
		void AnimateQuantityImageAtOnce(int sourceQuantity, int targetQuantity);
		void SetUIImage(IUIImage image);
	}
	public interface IQuantityAnimationEngine: IQuantityAnimationHandler{
	}
	public class QuantityAnimationEngine: IQuantityAnimationEngine{
		public QuantityAnimationEngine(IProcessFactory processFactory){
			thisProcessFactory = processFactory;
		}
		readonly IProcessFactory thisProcessFactory;
		IUIImage thisImage;
		public void SetUIImage(IUIImage image){
			thisImage = image;
		}
		public void AnimateQuantityImageIncrementally(int sourceQuantity, int targetQuantity){
			StopRunningQuantityAnimationProcess();
			StartIncrementalQuantityAnimationProcess(sourceQuantity, targetQuantity);
		}
		public void AnimateQuantityImageAtOnce(int sourceQuantity, int targetQuantity){
			StopRunningQuantityAnimationProcess();
			StartOneshotQuantityAnimationProcess(sourceQuantity, targetQuantity);
		}
		/* process management */
		IQuantityAnimationProcess thisRunningQuantityAnimationProcess;
		void StartIncrementalQuantityAnimationProcess(int sourceQuantity, int targetQuantity){
			StopRunningQuantityAnimationProcess();
			IIncrementalQuantityAnimationProcess process = thisProcessFactory.CreateIncrementalQuantityAnimationProcess(thisImage, sourceQuantity, targetQuantity);
			process.Run();
			thisRunningQuantityAnimationProcess = process;
		}
		void StartOneshotQuantityAnimationProcess(int sourceQuantity, int targetQuantity){
			StopRunningQuantityAnimationProcess();
			IOneshotQuantityAnimationProcess process = thisProcessFactory.CreateOneshotQuantityAnimationProcess (thisImage, sourceQuantity, targetQuantity);
			process.Run();
			thisRunningQuantityAnimationProcess = process;
		}
		public void StopRunningQuantityAnimationProcess(){
			thisRunningQuantityAnimationProcess.Stop();
		}
	}
}

