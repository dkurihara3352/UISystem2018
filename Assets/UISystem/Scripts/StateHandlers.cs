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
			_uiImage = image;
		}
		IUIImage _uiImage;
		float counter;
		public override void Run(){
			float initVal = _uiImage.GetDarkness();
			float tarVal = _uiImage.GetDefaultDarkness();
			float totalT = CalcTotalIrperT(initVal, tarVal);
			irper = new ImageDarknessIrper(_uiImage, initVal, tarVal, totalT);
			SetIrper(irper);
			irper.Interpolate(0f);
			base.Run();
		}
		public override void UpdateProcess(float deltaT){
			float irperT = CalcIrperT(deltaT);
			GetIrper().Interpolate(irperT);
			if(irperT > 1f)
				this.Expire();
		}
		public override void Reset(){
			SetIrper(null);
			counter = 0f;
		}
		/* What's the difference b/w Stop() and Expire()?
			Is stopped process able to be resumed?
			What's going to happen when UpdateProcess(deltaT) is called in the same
			frame this is init'ed, with deltaT big enough to soon expire the process?
		 */
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

