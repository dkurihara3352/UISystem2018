﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITAMStateHandler{
		void SetToPickedState(IItemIcon pickedII);
		void SetToDefaultState();
		bool IsInPickedUpState();
		bool IsInDefaultState();
	}
	public interface IItemIconTAManagerStateEngine: ISwitchableStateEngine<IItemIconTAManagerState>, IIITAMStateHandler{
		void SetIITAM(IItemIconTransactionManager iiTAM);
	}
	public abstract class AbsItemIconTAManagerStateEngine: AbsSwitchableStateEngine<IItemIconTAManagerState>, IItemIconTAManagerStateEngine{
		public void SetIITAM(IItemIconTransactionManager iiTAM){
			thisPickedState.SetIITAM(iiTAM);
			thisDefaultState.SetIITAM(iiTAM);
		}
		public void SetToPickedState(IItemIcon pickedII){
			thisPickedState.SetPickedII(pickedII);
			this.TrySwitchState(thisPickedState);
		}
		protected IIITAMPickedState thisPickedState;
		public void SetToDefaultState(){
			this.TrySwitchState(thisDefaultState);
		}
		protected IIITAMDefaultState thisDefaultState;
		public bool IsInPickedUpState(){
			return thisCurState is IIITAMPickedState;
		}
		public bool IsInDefaultState(){
			return thisCurState is IIITAMDefaultState;
		}
	}
	public interface IEqpIITAMStateEngine: IItemIconTAManagerStateEngine{}
	public class EqpIITAMStateEngine: AbsItemIconTAManagerStateEngine, IEqpIITAMStateEngine{
		public EqpIITAMStateEngine(IEquipTool eqpTool){
			thisPickedState = new EqpIITAMPickedState(eqpTool);
			thisDefaultState = new IITAMDefaultState();
		}
	}

	public interface IItemIconTAManagerState: ISwitchableState{
		void SetIITAM(IItemIconTransactionManager iiTAM);
	}
	public interface IEqpIITAMState: IItemIconTAManagerState{}
	public abstract class AbsItemIconTAManagerState: IItemIconTAManagerState{
		public void SetIITAM(IItemIconTransactionManager iiTAM){
			thisIITAM = iiTAM;
		}
		protected IItemIconTransactionManager thisIITAM;
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	/* picked */
		public interface IIITAMPickedState: IItemIconTAManagerState{
			void SetPickedII(IItemIcon itemIcon);
		}
		public class IITAMPickedState: AbsItemIconTAManagerState, IIITAMPickedState{
			public override void OnEnter(){
				thisIITAM.SetPickedII(thisPickedII);
				thisIITAM.CheckAndActivateHoverPads();
				thisIITAM.EvaluateHoverability();
				thisIITAM.HoverInitialPickUpReceiver();
			}
			public override void OnExit(){
				thisIITAM.ClearTAFields();
				thisIITAM.DeactivateHoverPads();
				thisIITAM.ResetHoverability();
				SetPickedII( null);
			}
			public void SetPickedII(IItemIcon pickedII){
				thisPickedII = pickedII;
			}
			IItemIcon thisPickedII;
		}
		public interface IEqpIITAMPickedState: IIITAMPickedState, IEqpIITAMState{}
		public class EqpIITAMPickedState: IITAMPickedState, IEqpIITAMPickedState{
			public EqpIITAMPickedState(IEquipTool eqpTool){
				thisEqpTool = eqpTool;
			}
			readonly IEquipTool thisEqpTool;
			public override void OnExit(){
				base.OnExit();
				thisEqpTool.ResetMode();/* or, EvalueateMode ? */
			}
		}
	/* default */
		public interface IIITAMDefaultState: IItemIconTAManagerState{}
		public class IITAMDefaultState: AbsItemIconTAManagerState, IIITAMDefaultState{
			public override void OnEnter(){
				thisIITAM.EvaluatePickability();
			}
			public override void OnExit(){}
		}
}
