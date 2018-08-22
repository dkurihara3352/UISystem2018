using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IVisualPickednessState: DKUtility.ISwitchableState, IVisualPickednessHandler{
		void SetPickableUIImage(IPickableUIImage pickableUIImage);
	}
	public abstract class AbsVisualPickednessState: IVisualPickednessState{
		public AbsVisualPickednessState(IVisualPickednessStateEngine stateEngine){
			thisStateEngine = stateEngine;
		}
		protected readonly IVisualPickednessStateEngine thisStateEngine;
		public void SetPickableUIImage(IPickableUIImage pickableUIImage){
			thisPickableUIImage = pickableUIImage;
		}
		protected IPickableUIImage thisPickableUIImage;
		public abstract void OnEnter();
		public abstract void OnExit();
		public abstract void BecomeVisuallyPickedUp();
		public abstract void BecomeVisuallyUnpicked();
	}
	public interface IVisuallyUnpickedState: IVisualPickednessState{}
	public class VisuallyUnpickedState: AbsVisualPickednessState, IVisuallyUnpickedState{
		public VisuallyUnpickedState(IVisualPickednessStateEngine stateEngine): base(stateEngine){}
		public override void OnEnter(){
			thisPickableUIImage.SetVisualPickedness(0f);
			/*	0 => unpicked, 1 => picked 
			*/
		}
		public override void OnExit(){}
		public override void BecomeVisuallyPickedUp(){
			thisStateEngine.SetToBecomingVisuallyPickedUpState();
		}
		public override void BecomeVisuallyUnpicked(){
			throw new System.InvalidOperationException("this is alreadly unpicked");
		}
	}
	public interface IChangingVisualPickednessState: IVisualPickednessState{}
	public interface IBecomingVisuallyPickedUpState: IChangingVisualPickednessState{}
	public class BecomingVisuallyPickedUpState: AbsVisualPickednessState, IBecomingVisuallyPickedUpState{
		public BecomingVisuallyPickedUpState(IVisualPickednessStateEngine stateEngine, IPickUpSystemProcessFactory pickUpSystemProcessFactory): base(stateEngine){
			thisProcessFactory = pickUpSystemProcessFactory;
		}
		IVisualPickednessProcess thisProcess;
		readonly IPickUpSystemProcessFactory thisProcessFactory;
		public override void OnEnter(){
			thisProcess = thisProcessFactory.CreateVisualPickednessProcess(
				thisPickableUIImage, 
				1f,
				thisStateEngine,
				true
			);
			thisProcess.Run();
		}
		public override void OnExit(){
			StopAndClearProcess();
		}
		public void ExpireProcess(){
			StopAndClearProcess();
		}
		public override void BecomeVisuallyPickedUp(){
			throw new System.InvalidOperationException("this is already being becomeing visually picked up");
		}
		public override void BecomeVisuallyUnpicked(){
			thisStateEngine.SetToBecomingVisuallyUnpickedState();
		}
		void StopAndClearProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
	}
	public interface IVisuallyPickedUpState: IVisualPickednessState{}
	public class VisuallyPickedUpState: AbsVisualPickednessState, IVisuallyPickedUpState{
		public VisuallyPickedUpState(IVisualPickednessStateEngine stateEngine): base(stateEngine){
		}
		public override void OnEnter(){
			thisPickableUIImage.SetVisualPickedness(1f);
			/*  0f => unpicked
				1f => picked
			*/
		}
		public override void OnExit(){}
		public override void BecomeVisuallyPickedUp(){
			throw new System.InvalidOperationException("this is already picked up");
		}
		public override void BecomeVisuallyUnpicked(){
			thisStateEngine.SetToBecomingVisuallyUnpickedState();
		}
	}
	public interface IBecomingVisuallyUnpickedState: IChangingVisualPickednessState{}
	public class BecomingVisuallyUnpickedState: AbsVisualPickednessState, IBecomingVisuallyUnpickedState{
		public BecomingVisuallyUnpickedState(IVisualPickednessStateEngine stateEngine, IPickUpSystemProcessFactory pickUpSystemProcessFactory): base(stateEngine){
			thisProcessFactory = pickUpSystemProcessFactory;
		}
		IVisualPickednessProcess thisProcess;
		readonly IPickUpSystemProcessFactory thisProcessFactory;
		public override void OnEnter(){
			float sourcePickedness = thisPickableUIImage.GetVisualPickedness();
			thisProcess = thisProcessFactory.CreateVisualPickednessProcess(
				thisPickableUIImage, 
				0f,
				thisStateEngine,
				false
			);
			thisProcess.Run();
		}
		public override void OnExit(){
			StopAndClearProcess();
		}
		public void ExpireProcess(){
			StopAndClearProcess();
		}
		public override void BecomeVisuallyPickedUp(){
			thisStateEngine.SetToBecomingVisuallyPickedUpState();
		}
		public override void BecomeVisuallyUnpicked(){
			throw new System.InvalidOperationException("this is already becoming visually unpicked");
		}
		void StopAndClearProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
			thisProcess = null;
		}
	}
}
