using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUISystemProcessFactory: IProcessFactory{
		IWaitAndExpireProcess CreateWaitAndExpireProcess(IWaitAndExpireProcessState state, float waitTime);
		IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
		IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(IQuantityRoller quantityRoller, int targetQuantity);
		IAlphaActivatorUIEActivationProcess CreateAlphaActivatorUIEActivationProcess(
			IUIElement uiElement, 
			IUIEActivationStateEngine engine, 
			bool doesActivate
		);
		INonActivatorUIEActivationProcess CreateNonActivatorUIEActivationProcess(
			IUIEActivationStateEngine engine, 
			bool doesActivate
		);
		IScrollerElementSnapProcess CreateScrollerElementSnapProcess(
			IScroller scroller, 
			IUIElement scrollerElement, 
			float targetElementLocalPosOnAxis, 
			float initialVelOnAxis, 
			int dimension
		);
		IInertialScrollProcess CreateInertialScrollProcess(float deltaPosOnAxis, float decelerationOnAxis, IScroller scroller, IUIElement scrollerElement, int dimension);
		IImageColorTurnProcess CreateGenericImageColorTurnProcess(IUIImage uiImage, Color targetColor);
		IImageColorTurnProcess CreateFalshColorProcess(IUIImage uiImage, Color targetColor);
		IAlphaPopUpProcess CreateAlphaPopUpProcess(
			IPopUp popUp,
			IPopUpStateEngine engine,
			bool hides
		);
	}
	public class UISystemProcessFactory: AbsProcessFactory, IUISystemProcessFactory{
		public UISystemProcessFactory(IProcessManager procManager, IUIManager uim): base(procManager){
			if(uim != null)
				thisUIManager = uim;
			else
				throw new System.ArgumentNullException("uim", "ProcessFactory does not operate without a uim");

		}
		protected readonly IUIManager thisUIManager;
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
		public IAlphaActivatorUIEActivationProcess CreateAlphaActivatorUIEActivationProcess(
			IUIElement uiElement, 
			IUIEActivationStateEngine engine, 
			bool doesActivate
		){
			IAlphaActivatorUIEActivationProcess process = new AlphaActivatorUIEActivationProcess(
				thisProcessManager, 
				thisProcessManager.GetAlphaActivatorUIEActivationProcessExpireT(), 
				engine,
				uiElement, 
				doesActivate
			);
			return process;
		}
		public INonActivatorUIEActivationProcess CreateNonActivatorUIEActivationProcess(
			IUIEActivationStateEngine engine, 
			bool doesActivate
		){
			INonActivatorUIEActivationProcess process = new NonActivatorUIEActivationProcess(
				thisProcessManager, 
				thisProcessManager.GetNonActivatorUIEActivationProcessExpireT(),
				engine,
				doesActivate
			);
			return process;
		}
		public IScrollerElementSnapProcess CreateScrollerElementSnapProcess(IScroller scroller, IUIElement scrollerElement, float targetElementLocalPosOnAxis, float initialVelOnAxis, int dimension){
			float diffThreshold = thisProcessManager.GetScrollerElementSnapProcessDiffThreshold();
			float stopDelta = thisProcessManager.GetScrollerElementSnapProcessStopDelta();
			IScrollerElementSnapProcess process = new ScrollerElementSnapProcess(targetElementLocalPosOnAxis, initialVelOnAxis, scroller, scrollerElement, dimension, diffThreshold, stopDelta, thisProcessManager);

			return process;
		}
		public IInertialScrollProcess CreateInertialScrollProcess(float deltaPosOnAxis, float decelerationAxisFactor, IScroller scroller, IUIElement scrollerElement, int dimension){
			float deceleration = thisProcessManager.GetInertialScrollDeceleration();
			IInertialScrollProcess process = new InertialScrollProcess(deltaPosOnAxis, deceleration, decelerationAxisFactor, scroller, scrollerElement, dimension, thisProcessManager);

			return process;
		}
		public IImageColorTurnProcess CreateGenericImageColorTurnProcess(IUIImage uiImage, Color targetColor){
			return new GenericImageColorTurnProcess(thisProcessManager, thisProcessManager.GetImageColorTurnProcessExpireTime(), uiImage, targetColor, false);
		}
		public IImageColorTurnProcess CreateFalshColorProcess(IUIImage uiImage, Color targetColor){
			return new GenericImageColorTurnProcess(thisProcessManager, thisProcessManager.GetImageFlashTime(), uiImage, targetColor, true);
		}
		public IAlphaPopUpProcess CreateAlphaPopUpProcess(
			IPopUp popUp,
			IPopUpStateEngine engine,
			bool hides
		){
			IAlphaPopUpProcess process = new AlphaPopUpProcess(
				thisProcessManager,
				thisProcessManager.GetAlphaPopUpExpireTime(),
				popUp,
				engine,
				hides
			);
			return process;
		}
	}
}
