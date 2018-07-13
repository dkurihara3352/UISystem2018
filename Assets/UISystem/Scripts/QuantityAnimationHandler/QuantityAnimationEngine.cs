using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IQuantityAnimationHandler{
		void AnimateQuantityImageIncrementally(int targetQuantity);
		void AnimateQuantityImageAtOnce(int targetQuantity);
		void SetQuantityImageInstantlyTo(int targetQuantity);
	}
	public interface IQuantityAnimationEngine: IQuantityAnimationHandler{
		void SetQuantityRoller(IQuantityRoller quantityRoller);
	}
	public class QuantityAnimationEngine: IQuantityAnimationEngine{
		public QuantityAnimationEngine(IUISystemProcessFactory processFactory){
			thisProcessFactory = processFactory;
		}
		readonly IUISystemProcessFactory thisProcessFactory;
		IQuantityRoller thisQuantityRoller;
		public void SetQuantityRoller(IQuantityRoller quantityRoller){
			thisQuantityRoller = quantityRoller;
		}
		public void AnimateQuantityImageIncrementally(int targetQuantity){
			StopRunningQuantityAnimationProcess();
			StartIncrementalQuantityAnimationProcess(targetQuantity);
		}
		public void AnimateQuantityImageAtOnce(int targetQuantity){
			StopRunningQuantityAnimationProcess();
			StartOneshotQuantityAnimationProcess(targetQuantity);
		}
		/* process management */
		IQuantityAnimationProcess thisRunningQuantityAnimationProcess;
		void StartIncrementalQuantityAnimationProcess(int targetQuantity){
			StopRunningQuantityAnimationProcess();
			IIncrementalQuantityAnimationProcess process = thisProcessFactory.CreateIncrementalQuantityAnimationProcess(thisQuantityRoller, targetQuantity);
			process.Run();
			thisRunningQuantityAnimationProcess = process;
		}
		void StartOneshotQuantityAnimationProcess(int targetQuantity){
			StopRunningQuantityAnimationProcess();
			IOneshotQuantityAnimationProcess process = thisProcessFactory.CreateOneshotQuantityAnimationProcess (thisQuantityRoller, targetQuantity);
			process.Run();
			thisRunningQuantityAnimationProcess = process;
		}
		public void StopRunningQuantityAnimationProcess(){
			thisRunningQuantityAnimationProcess.Stop();
		}
		public void SetQuantityImageInstantlyTo(int targetQuantity){
			thisQuantityRoller.SetRollerValue(targetQuantity/ 1f);
		}
	}
}

