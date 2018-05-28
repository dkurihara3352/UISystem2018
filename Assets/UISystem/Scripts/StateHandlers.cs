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
		ISelectabilityState _curState;
		public SelectabilityStateEngine(IUIElement uie){
			this.InitializeStates(uie);
			this.SetToInitialState();
		}
		public void BecomeSelectable(){
			TrySwitchState(_selectableState);
		}
		public void BecomeUnselectable(){
			TrySwitchState(_unselectableState);
		}
		public void BecomeSelected(){
			if(this.IsSelectable())
				TrySwitchState(_selectedState);
		}
		public bool IsSelectable(){
			return _curState is SelectableState;
		}
		public bool IsSelected(){
			return _curState is SelectedState;
		}
		void TrySwitchState(ISelectabilityState state){
			if(state != null){
				if(_curState != null){
					if(_curState != state){
						_curState.OnExit();
						_curState = state;
						state.OnEnter();
					}else{//state no change
						return;
					}
				}else{// curstate null
					_curState = state;
					state.OnEnter();
				}
			}else{
				throw new System.ArgumentNullException("state", "target state must not be null");
			}
		}
		SelectableState _selectableState;
		UnselectableState _unselectableState;
		SelectedState _selectedState;
		void InitializeStates(IUIElement uie){
			_selectableState = new SelectableState(uie);
			_unselectableState = new UnselectableState(uie);
			_selectedState = new SelectedState(uie);
			MakeSureStatesAreSet();
		}
		void MakeSureStatesAreSet(){
			if(_selectableState != null && _unselectableState != null && _selectedState != null)
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
	public abstract class AbsSelectabilityState: ISelectabilityState{
		protected IUIImage _uiImage;
		public AbsSelectabilityState(IUIElement uie){
			this._uiImage = uie.GetUIImage();
		}
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	public class SelectableState: AbsSelectabilityState{
		public SelectableState(IUIElement uie) :base(uie){
		}
		public override void OnEnter(){
			TryStartTurningDarknessToDefault();
		}
		public override void OnExit(){
		}
		void TryStartTurningDarknessToDefault(){

		}
	}
	public class TurnDarknessToDefaultProcess: AbsProcess{
		public TurnDarknessToDefaultProcess(IProcessManager procManager, IUIImage image): base(procManager){
			this.image = image;
		}
		IUIImage image;
		float elapsedT;
		float rateOfChange = 1f;
		ImageDarknessIrper GetIrper(){return irper;}
		void SetIrper(ImageDarknessIrper irper){this.irper = irper;}
		ImageDarknessIrper irper;
		public override void Run(){
			elapsedT = 0f;
			float tarVal = image.GetDefaultDarkness();
			float initVal = image.GetCurrentDarkness();
			ImageDarknessIrper newIrper = new ImageDarknessIrper(image,initVal, tarVal);
			SetIrper(newIrper);
			newIrper.Interpolate(0f);
			base.Run();
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
	public class UnselectableState: AbsSelectabilityState{
		public UnselectableState(IUIElement uie) :base(uie){
		}
		public override void OnEnter(){}
		public override void OnExit(){}
	}
	public class SelectedState: AbsSelectabilityState{
		public SelectedState(IUIElement uie) :base(uie){
		}
		public override void OnEnter(){}
		public override void OnExit(){}
	}
}

