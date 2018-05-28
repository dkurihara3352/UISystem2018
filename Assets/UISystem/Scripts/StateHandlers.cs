using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface ISelectabilityStateHandler{
		void BecomeSelectable();
		void BecomeUnselectable();
		void BecomeSelected();
		bool IsSelectable();
		bool IsSelected();
	}
	public interface ISelectabilityStateEngine: ISelectabilityStateHandler{

	}
	public class SelectabilityStateEngine: ISelectabilityStateEngine{
		ISelectabilityState curState;
		public SelectabilityStateEngine(IUIElement uie, IProcessFactory procFac){
			this.InitializeStates(uie, procFac);
			this.SetToInitialState();
		}
		public void BecomeSelectable(){
			TrySwitchState(selectableState);
		}
		public void BecomeUnselectable(){
			TrySwitchState(unselectableState);
		}
		public void BecomeSelected(){
			if(this.IsSelectable())
				TrySwitchState(selectedState);
		}
		public bool IsSelectable(){
			return curState is SelectableState;
		}
		public bool IsSelected(){
			return curState is SelectedState;
		}
		void TrySwitchState(ISelectabilityState state){
			if(state != null){
				if(curState != null){
					if(curState != state){
						curState.OnExit();
						curState = state;
						state.OnEnter();
					}else{//state no change
						return;
					}
				}else{// curstate null
					curState = state;
					state.OnEnter();
				}
			}else{
				throw new System.ArgumentNullException("state", "target state must not be null");
			}
		}
		SelectableState selectableState;
		UnselectableState unselectableState;
		SelectedState selectedState;
		void InitializeStates(IUIElement uie, IProcessFactory procFac){
			IUIImage image = uie.GetUIImage();
			TurnImageDarknessProcess turnToDefaultProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDefaultDarkness());
			TurnImageDarknessProcess turnToDarkenedProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDarkenedDarkness());

			selectableState = new SelectableState(turnToDefaultProcess);
			unselectableState = new UnselectableState(turnToDarkenedProcess);
			selectedState = new SelectedState(uie);

			MakeSureStatesAreSet();
		}
		void MakeSureStatesAreSet(){
			if(selectableState != null && unselectableState != null && selectedState != null)
				return;
			else
				throw new System.InvalidOperationException("any of the states not correctly set");
		}
		void SetToInitialState(){
			BecomeSelectable();
		}
	}
	public interface ISelectabilityState{
		void OnEnter();
		void OnExit();
	}
	public abstract class TurnImageDarknessState: ISelectabilityState{
		protected TurnImageDarknessProcess process;
		public TurnImageDarknessState(TurnImageDarknessProcess process){
			this.process = process;
		}
		public void OnEnter(){
			StartTurningImageDarkness();
		}
		public void OnExit(){
			if(process.IsRunning())
				process.Stop();
		}
		void StartTurningImageDarkness(){
			process.Run();
		}
	}
	public class SelectableState: TurnImageDarknessState{
		public SelectableState(TurnImageDarknessProcess process): base(process){}
	}
	public class UnselectableState: TurnImageDarknessState{
		public UnselectableState(TurnImageDarknessProcess process): base(process){}
	}
	public class TurnImageDarknessProcess: AbsProcess{
		public TurnImageDarknessProcess(IProcessManager procManager, IUIImage image, float targetDarkness): base(procManager){
			this.image = image;
			this.targetDarkness = targetDarkness;
		}
		IUIImage image;
		float targetDarkness;
		float elapsedT;
		float rateOfChange = 1f;
		float diffThreshold = .05f;
		ImageDarknessIrper GetIrper(){return irper;}
		void SetIrper(ImageDarknessIrper irper){this.irper = irper;}
		ImageDarknessIrper irper;
		public override void Run(){
			elapsedT = 0f;
			float initDarkness = image.GetCurrentDarkness();
			if(DifferenceIsBigEnough(targetDarkness - initDarkness)){
				ImageDarknessIrper newIrper = new ImageDarknessIrper(image,initDarkness, targetDarkness);
				SetIrper(newIrper);
				newIrper.Interpolate(0f);
				base.Run();
			}else{
				image.SetDarkness(targetDarkness);
				return;
			}
		}
		bool DifferenceIsBigEnough(float diff){
			if(diff >= 0f)
				return diff > diffThreshold;
			else
				return diff < -diffThreshold;
		}
		public override void UpdateProcess(float deltaT){
			elapsedT += deltaT;
			float irperT = elapsedT/ rateOfChange;
			GetIrper().Interpolate(irperT);
			if(irperT >= 1f)
				this.Expire();
		}
		public override void Reset(){
			SetIrper(null);
			elapsedT = 0f;
		}
	}
	public class ImageDarknessIrper: AbsInterpolater{
		IUIImage image;
		float initDarkness;
		float targetDarkness;
		public ImageDarknessIrper(IUIImage image, float sourceDarkness, float targetDarkness){
			this.image = image;
			this.initDarkness = sourceDarkness;
			this.targetDarkness = targetDarkness;
		}
		public override void InterpolateImple(float zeroToOne){
			float newDarkness = Mathf.Lerp(initDarkness, targetDarkness, zeroToOne);
			image.SetDarkness(newDarkness);
		}
		public override void Terminate(){return;}
	}
	public class SelectedState: ISelectabilityState{
		public SelectedState(IUIElement uie){
			// no process required.
		}
		public void OnEnter(){
			// CursorManager.MoveCursor(this.image);
		}
		public void OnExit(){}
	}
}

