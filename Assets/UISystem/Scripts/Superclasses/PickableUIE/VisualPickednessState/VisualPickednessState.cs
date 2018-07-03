using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IVisualPickednessState: ISwitchableState, IVisualPickednessHandler{
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
	public interface IBecomingVisuallyPickedUpState: IVisualPickednessState, IWaitAndExpireProcessState{}
	public class BecomingVisuallyPickedUpState: AbsVisualPickednessState, IBecomingVisuallyPickedUpState{
		public BecomingVisuallyPickedUpState(IVisualPickednessStateEngine stateEngine, IProcessFactory processFactory): base(stateEngine){
			float sourcePickedness = thisPickableUIImage.GetVisualPickedness();
			float targetPickedness = 1f;
			thisProcess = processFactory.CreateVisualPickednessProcess(this, thisPickableUIImage, sourcePickedness, targetPickedness);
		}
		readonly IVisualPickednessProcess thisProcess;
		public override void OnEnter(){
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
		}
		public void ExpireProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Expire();
		}
		public void OnProcessUpdate(float deltaT){return;}
		public void OnProcessExpire(){
			thisStateEngine.SetToVisuallyPickedUpState();
		}
		public override void BecomeVisuallyPickedUp(){
			throw new System.InvalidOperationException("this is already being becomeing visually picked up");
		}
		public override void BecomeVisuallyUnpicked(){
			thisStateEngine.SetToBecomingVisuallyUnpickedState();
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
	public interface IBecomingVisuallyUnpickedState: IVisualPickednessState, IWaitAndExpireProcessState{}
	public class BecomingVisuallyUnpickedState: AbsVisualPickednessState, IBecomingVisuallyUnpickedState{
		public BecomingVisuallyUnpickedState(IVisualPickednessStateEngine stateEngine, IProcessFactory processFactory): base(stateEngine){
			float sourcePickedness = thisPickableUIImage.GetVisualPickedness();
			float targetPickedness = 0f;
			thisProcess = processFactory.CreateVisualPickednessProcess(this, thisPickableUIImage, sourcePickedness, targetPickedness);
		}
		readonly IVisualPickednessProcess thisProcess;
		public override void OnEnter(){
			thisProcess.Run();
		}
		public override void OnExit(){
			if(thisProcess.IsRunning())
				thisProcess.Stop();
		}
		public void ExpireProcess(){
			if(thisProcess.IsRunning())
				thisProcess.Expire();
		}
		public void OnProcessUpdate(float deltaT){return;}
		public void OnProcessExpire(){
			thisStateEngine.SetToVisuallyUnpickedState();
		}
		public override void BecomeVisuallyPickedUp(){
			thisStateEngine.SetToBecomingVisuallyPickedUpState();
		}
		public override void BecomeVisuallyUnpicked(){
			throw new System.InvalidOperationException("this is already becoming visually unpicked");
		}
	}
	public interface IPickableUIImage: IUIImage{
		void SetVisualPickedness(float pickedness);
		float GetVisualPickedness();
	}
}
