using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUISystemProcessFactory: IProcessFactory{
		ITurnImageDarknessProcess CreateTurnImageDarknessProcess(IUIImage image, float targetDarkness);
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
		IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
		IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
		IAlphaActivatorUIEActivationProcess CreateAlphaActivatorUIEActivationProcess(IUIEActivationProcessState state, bool doesActivate, IAlphaActivatorUIElement alphaActivatorUIElement);
		INonActivatorUIEActivationProcess CreateNonActivatorUIEActivationProcess(IUIEActivationProcessState state, bool doesActivate/* , INonActivatorUIElement nonActivatorUIElement */);
		IScrollerElementSnapProcess CreateScrollerElementSnapProcess(IScroller scroller, IUIElement scrollerElement, float targetElementLocalPosOnAxis, float initialVelOnAxis, int dimension);
		IInertialScrollProcess CreateInertialScrollProcess(float deltaPosOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension);
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
			IWaitAndExpireProcess process = new GenericWaitAndExpireProcess(thisProcessManager, waitTime, state);
			return process;
		}
		public IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity){
			IncrementalQuantityAnimationProcess process = new IncrementalQuantityAnimationProcess(quantityRoller, targetQuantity, thisProcessManager, ProcessConstraint.expireTime, thisProcessManager.GetQuantityAnimationProcessExpireTime(), 0f, true);
			return process;
		}
		public IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity){
			OneshotQuantityAnimationProcess process = new OneshotQuantityAnimationProcess(quantityRoller, targetQuantity, thisProcessManager, ProcessConstraint.expireTime, thisProcessManager.GetQuantityAnimationProcessExpireTime(), 0f, true);
			return process;
		}
		public IAlphaActivatorUIEActivationProcess CreateAlphaActivatorUIEActivationProcess(IUIEActivationProcessState state, bool doesActivate, IAlphaActivatorUIElement alphaActivatorUIElement){
			IAlphaActivatorUIEActivationProcess process = new AlphaActivatorUIEActivationProcess(thisProcessManager, thisProcessManager.GetAlphaActivatorUIEActivationProcessExpireT(), doesActivate, state, alphaActivatorUIElement);
			return process;
		}
		public INonActivatorUIEActivationProcess CreateNonActivatorUIEActivationProcess(IUIEActivationProcessState state, bool doesActivate/* , INonActivatorUIElement nonActivatorUIElement */){
			INonActivatorUIEActivationProcess process = new NonActivatorUIEActivationProcess(thisProcessManager, thisProcessManager.GetNonActivatorUIEActivationProcessExpireT(), state);
			return process;
		}
		public IScrollerElementSnapProcess CreateScrollerElementSnapProcess(IScroller scroller, IUIElement scrollerElement, float targetElementLocalPosOnAxis, float initialVelOnAxis, int dimension){
			float diffThreshold = thisProcessManager.GetScrollerElementSnapProcessDiffThreshold();
			float stopDelta = thisProcessManager.GetScrollerElementSnapProcessStopDelta();
			IScrollerElementSnapProcess process = new ScrollerElementSnapProcess(targetElementLocalPosOnAxis, initialVelOnAxis, scroller, scrollerElement, dimension, diffThreshold, stopDelta, thisProcessManager);

			return process;
		}
		public IInertialScrollProcess CreateInertialScrollProcess(float deltaPosOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension){
			IInertialScrollProcess process = new InertialScrollProcess(deltaPosOnAxis, scroller, scrollerElement, dimension, thisProcessManager);

			return process;
		}
	}
}
