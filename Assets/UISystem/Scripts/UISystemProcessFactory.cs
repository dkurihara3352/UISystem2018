using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem{
	public interface IUISystemProcessFactory: IProcessFactory{
		IUIAWaitForTapProcess CreateUIAWaitForTapProcess(
			IWaitingForTapState state,
			IUIAdaptorInputStateEngine engine
		);
		IUIAWaitForReleaseProcess CreateUIAWaitForReleaseProcess(
			IWaitingForReleaseState state,
			IUIAdaptorInputStateEngine engine
		);
		IUIAWaitForNextTouchProcess CreateUIAWaitForNextTouchProcess(
			IWaitingForNextTouchState state,
			IUIAdaptorInputStateEngine engine
		);
		IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(
			IQuantityRoller quantityRoller, 
			int targetQuantity
		);
		IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(
			IQuantityRoller quantityRoller, 
			int targetQuantity
		);
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
		IInertialScrollProcess CreateInertialScrollProcess(
			float deltaPosOnAxis, 
			float decelerationOnAxis, 
			IScroller scroller, 
			IUIElement scrollerElement, 
			int dimension
		);
		IImageColorTurnProcess CreateGenericImageColorTurnProcess(
			IUIImage uiImage, 
			Color targetColor
		);
		IImageColorTurnProcess CreateFalshColorProcess(
			IUIImage uiImage, 
			Color targetColor
		);
		IAlphaPopUpProcess CreateAlphaPopUpProcess(
			IPopUp popUp,
			IPopUpStateEngine engine,
			bool hides
		);
	}
	public class UISystemProcessFactory: AbsProcessFactory, IUISystemProcessFactory{
		public UISystemProcessFactory(
			IProcessManager procManager, 
			IUIManager uim
		): base(
			procManager
		){
			if(uim != null)
				thisUIManager = uim;
			else
				throw new System.ArgumentNullException("uim", "ProcessFactory does not operate without a uim");

		}
		protected readonly IUIManager thisUIManager;

		public IUIAWaitForTapProcess CreateUIAWaitForTapProcess(
			IWaitingForTapState state,
			IUIAdaptorInputStateEngine engine
		){
			IUIAdaptorInputProcessConstArg arg = new UIADaptorInputProcessConstArg(
				thisProcessManager,
				ProcessConstraint.ExpireTime,
				thisProcessManager.GetUIAWaitForTapProcessExpireTime(),
				state,
				engine
			);
			return new UIAWaitForTapProcess(arg);
		}
		public IUIAWaitForReleaseProcess CreateUIAWaitForReleaseProcess(
			IWaitingForReleaseState state,
			IUIAdaptorInputStateEngine engine
		){
			IUIAdaptorInputProcessConstArg arg = new UIADaptorInputProcessConstArg(
				thisProcessManager,
				ProcessConstraint.none,
				1f,
				state,
				engine
			);
			return new UIAWaitForReleaseProcess(arg);
		}
		public IUIAWaitForNextTouchProcess CreateUIAWaitForNextTouchProcess(
			IWaitingForNextTouchState state,
			IUIAdaptorInputStateEngine engine
		){
			IUIAdaptorInputProcessConstArg arg = new UIADaptorInputProcessConstArg(
				thisProcessManager,
				ProcessConstraint.ExpireTime,
				thisProcessManager.GetUIAWaitForNextTouchProcessExpireTime(),
				state,
				engine
			);
			return new UIAWaitForNextTouchProcess(arg);
		}
		public IIncrementalQuantityAnimationProcess CreateIncrementalQuantityAnimationProcess(
			IQuantityRoller quantityRoller, 
			int targetQuantity
		){
			IQuantityAnimationProcessConstArg arg = new QuantityAnimationProcessConstArg(
				thisProcessManager,
				ProcessConstraint.ExpireTime,
				thisProcessManager.GetQuantityAnimationProcessExpireTime(),
				true,
				targetQuantity,
				quantityRoller
			);
			IncrementalQuantityAnimationProcess process = new IncrementalQuantityAnimationProcess(arg);
			return process;
		}
		public IOneshotQuantityAnimationProcess CreateOneshotQuantityAnimationProcess(
			IQuantityRoller quantityRoller, 
			int targetQuantity
		){
			IQuantityAnimationProcessConstArg arg = new QuantityAnimationProcessConstArg(
				thisProcessManager,
				ProcessConstraint.ExpireTime,
				thisProcessManager.GetQuantityAnimationProcessExpireTime(),
				true,
				targetQuantity,
				quantityRoller
			);
			OneshotQuantityAnimationProcess process = new OneshotQuantityAnimationProcess(arg);
			return process;
		}
		public IAlphaActivatorUIEActivationProcess CreateAlphaActivatorUIEActivationProcess(
			IUIElement uiElement, 
			IUIEActivationStateEngine engine, 
			bool doesActivate
		){
			IAlphaActivatorUIEActivationProcessConstArg arg = new AlphaActivatorUIEActivationProcessConstArg(
				thisProcessManager, 
				thisProcessManager.GetAlphaActivatorUIEActivationProcessExpireT(), 

				engine,
				uiElement, 
				doesActivate
			);
			IAlphaActivatorUIEActivationProcess process = new AlphaActivatorUIEActivationProcess(
				arg
			);
			return process;
		}
		public INonActivatorUIEActivationProcess CreateNonActivatorUIEActivationProcess(
			IUIEActivationStateEngine engine, 
			bool doesActivate
		){
			INonActivatorUIEActivationProcessConstArg arg = new NonActivatorUIEActivationProcessConstArg(
				thisProcessManager, 
				thisProcessManager.GetNonActivatorUIEActivationProcessExpireT(),
				engine,
				doesActivate
			);
			INonActivatorUIEActivationProcess process = new NonActivatorUIEActivationProcess(
				arg
			);
			return process;
		}
		public IScrollerElementSnapProcess CreateScrollerElementSnapProcess(
			IScroller scroller, 
			IUIElement scrollerElement, 
			float targetElementLocalPosOnAxis, 
			float initialVelOnAxis, 
			int dimension
		){
			float diffThreshold = thisProcessManager.GetScrollerElementSnapProcessDiffThreshold();
			float stopDelta = thisProcessManager.GetScrollerElementSnapProcessStopDelta();
			IScrollerElementSnapProcessConstArg arg = new ScrollerElementSnapProcessConstArg(
				thisProcessManager,
				scrollerElement,
				scroller,
				dimension,
				targetElementLocalPosOnAxis,
				initialVelOnAxis,
				stopDelta	
			);
			return new ScrollerElementSnapProcess(arg);
		}
		public IInertialScrollProcess CreateInertialScrollProcess(
			float deltaPosOnAxis, 
			float decelerationAxisFactor, 
			IScroller scroller, 
			IUIElement scrollerElement, 
			int dimension
		){
			float deceleration = thisProcessManager.GetInertialScrollDeceleration();
			IInertialScrollProcessConstArg arg = new InertialScrollProcessConstArg(
				thisProcessManager,
				scroller,
				scrollerElement,
				dimension,
				deltaPosOnAxis,
				deceleration,
				decelerationAxisFactor
			);
			return new InertialScrollProcess(arg);
		}
		public IImageColorTurnProcess CreateGenericImageColorTurnProcess(
			IUIImage uiImage, 
			Color targetColor
		){
			IImageColorTurnProcessConstArg arg = new ImageColorTurnProcessConstArg(
				thisProcessManager,
				thisProcessManager.GetImageColorTurnProcessExpireTime(),
				uiImage,
				targetColor,
				false
			);
			return new GenericImageColorTurnProcess(arg);
		}
		public IImageColorTurnProcess CreateFalshColorProcess(IUIImage uiImage, Color targetColor){
			IImageColorTurnProcessConstArg arg = new ImageColorTurnProcessConstArg(
				thisProcessManager,
				thisProcessManager.GetImageColorTurnProcessExpireTime(),
				uiImage,
				targetColor,
				true
			);
			return new GenericImageColorTurnProcess(arg);
		}
		public IAlphaPopUpProcess CreateAlphaPopUpProcess(
			IPopUp popUp,
			IPopUpStateEngine engine,
			bool hides
		){
			IAlphaPopUpProcessConstArg arg = new AlphaPopUpProcessConstArg(
				thisProcessManager,
				thisProcessManager.GetAlphaPopUpExpireTime(),
				engine,
				popUp,
				hides
			);
			IAlphaPopUpProcess process = new AlphaPopUpProcess(arg);
			return process;
		}
	}
}
