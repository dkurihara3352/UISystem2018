using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;
namespace UISystem.PickUpUISystem{
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
	public interface IEqpIITAMStateEngine: IItemIconTAManagerStateEngine{
		void SetEqpTool(IEquipTool eqpTool);
	}
	public class EqpIITAMStateEngine: AbsItemIconTAManagerStateEngine, IEqpIITAMStateEngine{
		public EqpIITAMStateEngine(){
			thisPickedState = new EqpIITAMPickedState();
			thisDefaultState = new IITAMDefaultState();
		}
		public void SetEqpTool(IEquipTool eqpTool){
			((IEqpIITAMPickedState)thisPickedState).SetEqpTool(eqpTool);
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
		public interface IEqpIITAMPickedState: IIITAMPickedState, IEqpIITAMState{
			void SetEqpTool(IEquipTool eqpTool);
		}
		public class EqpIITAMPickedState: IITAMPickedState, IEqpIITAMPickedState{
			public void SetEqpTool(IEquipTool eqpTool){
				thisEqpTool = eqpTool;
			}
			IEquipTool thisEqpTool;
			public override void OnExit(){
				base.OnExit();
				thisEqpTool.ResetMode();/* or, EvaluateMode ? */
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

