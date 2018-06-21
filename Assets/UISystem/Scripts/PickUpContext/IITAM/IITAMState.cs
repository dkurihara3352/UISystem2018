using System.Collections;
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
	}
	public abstract class AbsItemIconTAManagerStateEngine: AbsSwitchableStateEngine<IItemIconTAManagerState>, IItemIconTAManagerStateEngine{
		public void SetToPickedState(IItemIcon pickedII){
			pickedState.SetPickedII(pickedII);
			this.TrySwitchState(pickedState);
		}
		protected IIITAMPickedState pickedState;
		public void SetToDefaultState(){
			this.TrySwitchState(defaultState);
		}
		protected IIITAMDefaultState defaultState;
		public bool IsInPickedUpState(){
			return thisCurState is IIITAMPickedState;
		}
		public bool IsInDefaultState(){
			return thisCurState is IIITAMDefaultState;
		}
	}
	public interface IEqpIITAMStateEngine: IItemIconTAManagerStateEngine{}
	public class EqpIITAMStateEngine: AbsItemIconTAManagerStateEngine, IEqpIITAMStateEngine{
		public EqpIITAMStateEngine(IEquippableIITAManager eqpIITAM, IEquipTool eqpTool){
			this.pickedState = new EqpIITAMPickedState(eqpIITAM, eqpTool);
			this.defaultState = new IITAMDefaultState(eqpIITAM);
		}
	}

	public interface IItemIconTAManagerState: ISwitchableState{}
	public interface IEqpIITAMState: IItemIconTAManagerState{}
	public abstract class AbsItemIconTAManagerState: IItemIconTAManagerState{
		public AbsItemIconTAManagerState(IItemIconTransactionManager iiTAM){
			this.iiTAM = iiTAM;
		}
		protected readonly IItemIconTransactionManager iiTAM;
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	/* picked */
		public interface IIITAMPickedState: IItemIconTAManagerState{
			void SetPickedII(IItemIcon itemIcon);
		}
		public class IITAMPickedState: AbsItemIconTAManagerState, IIITAMPickedState{
			public IITAMPickedState(IItemIconTransactionManager tam): base(tam){}
			public override void OnEnter(){
				iiTAM.SetPickedII(pickedII);
				iiTAM.CheckAndActivateHoverPads();
				iiTAM.EvaluateHoverability();
				iiTAM.HoverInitialPickUpReceiver();
			}
			public override void OnExit(){
				iiTAM.ClearTAFields();
				iiTAM.DeactivateHoverPads();
				iiTAM.ResetHoverability();
				SetPickedII( null);
			}
			public void SetPickedII(IItemIcon pickedII){
				this.pickedII = pickedII;
			}
			IItemIcon pickedII;
		}
		public interface IEqpIITAMPickedState: IIITAMPickedState, IEqpIITAMState{}
		public class EqpIITAMPickedState: IITAMPickedState, IEqpIITAMPickedState{
			public EqpIITAMPickedState(IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(eqpIITAM){
				this.eqpTool = eqpTool;
			}
			readonly IEquipTool eqpTool;
			public override void OnExit(){
				base.OnExit();
				eqpTool.ResetMode();/* or, EvalueateMode ? */
			}
		}
	/* default */
		public interface IIITAMDefaultState: IItemIconTAManagerState{}
		public class IITAMDefaultState: AbsItemIconTAManagerState, IIITAMDefaultState{
			public IITAMDefaultState(IItemIconTransactionManager tam): base(tam){}
			public override void OnEnter(){
				iiTAM.EvaluatePickability();
			}
			public override void OnExit(){}
		}
}

