using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIconTAManagerState: ISwitchableState{

	}
	public abstract class AbsItemIconTAManagerState: IItemIconTAManagerState{
		public AbsItemIconTAManagerState(IItemIconTransactionManager tam){
			this.tam = tam;
		}
		protected readonly IItemIconTransactionManager tam;
		public abstract void OnEnter();
		public abstract void OnExit();
	}
	public interface IIITAMPickedState: IItemIconTAManagerState{
		void SetPickedII(IItemIcon itemIcon);
	}
	public class IITAMPickedState: AbsItemIconTAManagerState, IIITAMPickedState{
		public IITAMPickedState(IItemIconTransactionManager tam): base(tam){}
		public override void OnEnter(){
			tam.SetPickedII(pickedII);
			tam.CheckAndActivateHoverPads();
			tam.EvaluateHoverability();
			tam.HoverInitialPickUpReceiver();
		}
		public override void OnExit(){
			tam.ClearTAFields();
			tam.DeactivateHoverPads();
			tam.ClearHoverability();
			SetPickedII( null);
		}
		public void SetPickedII(IItemIcon pickedII){
			this.pickedII = pickedII;
		}
		IItemIcon pickedII;
	}
	public class EqpIITAMPickedState: IITAMPickedState{
		public EqpIITAMPickedState(IEquippableIITAManager eqpIITAM, IEquipTool eqpTool) :base(eqpIITAM){
			this.eqpTool = eqpTool;
		}
		readonly IEquipTool eqpTool;
		public override void OnExit(){
			base.OnExit();
			eqpTool.ResetMode();/* or, EvalueateMode ? */
		}
	}
	public class IITAMDefaultState: AbsItemIconTAManagerState{
		public IITAMDefaultState(IItemIconTransactionManager tam): base(tam){}
		public override void OnEnter(){
			tam.EvaluatePickability();
		}
		public override void OnExit(){}
	}
	public interface IItemIcomTAManagerStateEngine: ISwitchableStateEngine<IItemIconTAManagerState>, IIITAMStateHandler{
	}
	public class ItemIconTAManagerStateEngine: AbsSwitchableStateEngine<IItemIconTAManagerState>, IItemIcomTAManagerStateEngine{
		public ItemIconTAManagerStateEngine(){
			/* init states here */
			/* set to init state here */
		}
		public void SetToPickedState(IItemIcon pickedII){
			pickedState.SetPickedII(pickedII);
			this.TrySwitchState(pickedState);
		}
		readonly IITAMPickedState pickedState;
		public void SetToDefaultState(){
			this.TrySwitchState(defaultState);
		}
		readonly IITAMDefaultState defaultState;
	}
}

