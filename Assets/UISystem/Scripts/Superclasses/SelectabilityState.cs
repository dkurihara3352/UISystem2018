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
	public class SelectabilityStateEngine: AbsSwitchableStateEngine<ISelectabilityState>, ISelectabilityStateEngine{
		public SelectabilityStateEngine(IUIElement uie, IProcessFactory procFac){
			IUIImage image = uie.GetUIImage();
			ITurnImageDarknessProcess turnToDefaultProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDefaultDarkness());
			ITurnImageDarknessProcess turnToDarkenedProcess = procFac.CreateTurnImageDarknessProcess(image, image.GetDarkenedDarkness());

			selectableState = new SelectableState(turnToDefaultProcess);
			unselectableState = new UnselectableState(turnToDarkenedProcess);
			selectedState = new SelectedState(uie);

			MakeSureStatesAreSet();

			this.SetToInitialState();
		}
		protected readonly SelectableState selectableState;
		protected readonly UnselectableState unselectableState;
		protected readonly SelectedState selectedState;
		void MakeSureStatesAreSet(){
			if(selectableState != null && unselectableState != null && selectedState != null)
				return;
			else
				throw new System.InvalidOperationException("any of the states not correctly set");
		}
		void SetToInitialState(){
			BecomeSelectable();
		}
		/* SelStateHandler */
			public void BecomeSelectable(){
				TrySwitchState(selectableState);
			}
			public void BecomeUnselectable(){
				TrySwitchState(unselectableState);
			}
			public void BecomeSelected(){
				if(this.IsSelectable() || this.IsSelected())
					TrySwitchState(selectedState);
				else
					throw new System.InvalidOperationException("This method should not be called while this is not selectable");
			}
			public bool IsSelectable(){
				return curState is SelectableState;
			}
			public bool IsSelected(){
				return curState is SelectedState;
			}
	}
	public interface ISelectabilityState: ISwitchableState{
	}
	public abstract class TurnImageDarknessState: ISelectabilityState{
		protected ITurnImageDarknessProcess process;
		public TurnImageDarknessState(ITurnImageDarknessProcess process){
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
		public SelectableState(ITurnImageDarknessProcess process): base(process){}
	}
	public class UnselectableState: TurnImageDarknessState{
		public UnselectableState(ITurnImageDarknessProcess process): base(process){}
	}
	public interface ITurnImageDarknessProcess: IProcess{}
	public class TurnImageDarknessProcess: AbsProcess, ITurnImageDarknessProcess{
		public TurnImageDarknessProcess(IProcessManager procManager, IUIImage image, float targetDarkness): base(procManager){
			this.image = image;
			targetDarkness = CheckAndMakeDarknessValueIsInRange(targetDarkness);
			this.targetDarkness = targetDarkness;
		}
		readonly IUIImage image;
		readonly float targetDarkness;
		readonly float rateOfChange = 1f;
			/*  time it takes from complete darkness 0f to zero darkness 1f
				it's set to 1sec here
			*/
		readonly float diffThreshold = .05f;
		float elapsedT;
		float totalReqTime;
			/* actual time it takes from init to tarDarkness
				,this is computed in CalcAndSetComputationFields
			 */
		ImageDarknessIrper irper;
		float CheckAndMakeDarknessValueIsInRange(float darkness){
			/* clamp the given value within 0 - 1 range */
			if(darkness < 0f)
				return 0f;
			else if( darkness > 1f)
				return 1f;
			else
				return darkness;
		}
		public override void Run(){
			ResetFields();
			float initDarkness = image.GetCurrentDarkness();
			initDarkness = CheckAndMakeDarknessValueIsInRange(initDarkness);
			if(DifferenceIsBigEnough(targetDarkness - initDarkness)){
				ImageDarknessIrper newIrper = new ImageDarknessIrper(image, initDarkness, targetDarkness);
				CalcAndSetComputationFields(initDarkness, targetDarkness);
				irper = newIrper;
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
		void CalcAndSetComputationFields(float initDarkness, float targetDarkness){
			/*	Comp detail
				Dx (max possible diff: constant)
				Tx (max possible total time: constant)
				r (rate of Change: constant)
				{
					Dx/Tx = r
					Dx = 1f
					Tx = 1f sec
					r = 1f
				}
				Da (actual diff)
				Ta(time it takes to irp actual diff)
				{
					Da/Ta = r
					Ta = Da/r
				}
				Ti (irperT)
				Te (elapsedT)
				{
					Ti = Te/Ta
				}
			*/
			float actualDiff = targetDarkness - initDarkness;
			if(actualDiff < 0f)
				actualDiff *= -1f;
			totalReqTime = actualDiff/rateOfChange;
		}
		void ResetFields(){
			irper = null;
			elapsedT = 0f;
			totalReqTime = 0f;
		}
		public override void UpdateProcess(float deltaT){
			/* see above for comp detail */
			elapsedT += deltaT;
			float irperT = elapsedT/ totalReqTime;
			if(irperT < 1f)
				irper.Interpolate(irperT);
			else{
				irper.Interpolate(1f);
				this.Expire();
			}
		}
		public override void Reset(){
			ResetFields();
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

