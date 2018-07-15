﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUISystemProcessFactory: IProcessFactory{
		ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
		IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
		IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
	}
	public class UISystemProcessFactory: AbsProcessFactory, IUISystemProcessFactory{
		public UISystemProcessFactory(IProcessManager procManager, IUIManager uim): base(procManager){
			if(uim != null)
				thisUIManager = uim;
			else
				throw new System.ArgumentNullException("uim", "ProcessFactory does not operate without a uim");

		}
		protected readonly IUIManager thisUIManager;
		public ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness){
			ITurnImageDarknessProcess process = new TurnImageDarknessProcess(thisProcessManager, ProcessConstraint.rateOfChange, 1f, .05f, image, targetDarkness, false);
			return process;
		}
		public IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime){
			IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(thisProcessManager, state, waitTime);
			return process;
		}
		public IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity){
			IncrementalQuantityAnimationProcess process = new IncrementalQuantityAnimationProcess(quantityRoller, targetQuantity, thisProcessManager, ProcessConstraint.expireTime, thisProcessManager.GetQuantityAnimationProcessExpireTime(), null, 0f, true);
			return process;
		}
		public IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity){
			OneshotQuantityAnimationProcess process = new OneshotQuantityAnimationProcess(quantityRoller, targetQuantity, thisProcessManager, ProcessConstraint.expireTime, thisProcessManager.GetQuantityAnimationProcessExpireTime(), null, 0f, true);
			return process;
		}
	}
}