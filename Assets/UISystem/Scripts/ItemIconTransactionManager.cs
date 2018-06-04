using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIITAMStateHandler{
		void SetToPickedState(IItemIcon pickedII);
		void SetToDefaultState();
	}
	public interface IItemIconTransactionManager: IPickUpManager ,IIITAMStateHandler{
		void SetPickedII(IItemIcon pickedII);
		IItemIcon GetPickedII();
		void Activate();
		void CheckAndActivateHoverPads();
		void DeactivateHoverPads();
		void EvaluateHoverability();
		void ClearHoverability();
		void HoverInitialPickUpReceiver();
		void EvaluatePickability();
		List<IIconGroup> GetAllRelevantIGs();
	}
	public abstract class AbsItemIconTransactionManager: AbsPickUpManager, IItemIconTransactionManager{
		public AbsItemIconTransactionManager(){
			this.stateEngine = new ItemIconTAManagerStateEngine();
		}
		readonly IItemIcomTAManagerStateEngine stateEngine;
		public void Activate(){
			SetToDefaultState();
		}
		public void SetToPickedState(IItemIcon pickedII){
			this.stateEngine.SetToPickedState(pickedII);
		}
		public void SetToDefaultState(){
			this.stateEngine.SetToDefaultState();
		}
		public void SetPickedII(IItemIcon pickedII){
			this.pickedUIE = pickedII;
		}
		public IItemIcon GetPickedII(){
			IPickableUIE pickedUIE = this.GetPickedUIE();
			if(pickedUIE != null){
				if(pickedUIE is IItemIcon)
					return pickedUIE as IItemIcon;
				else
					throw new System.InvalidCastException("pickedUIE is not of type IItemIcon");
			}
			return null;
		}
		IIconPanel hoveredPanel;
		IItemIcon hoveredII;
		public void CheckAndActivateHoverPads(){}
		public void DeactivateHoverPads(){}
		public void EvaluateHoverability(){}
		public virtual void ClearHoverability(){}
		public void HoverInitialPickUpReceiver(){}
		public override void ClearTAFields(){
			base.ClearTAFields();
			this.hoveredPanel = null;
			this.hoveredII = null;
		}
		public void EvaluatePickability(){
			foreach(IIconGroup ig in GetAllRelevantIGs()){
				ig.EvaluatePickability();
			}
		}
		public abstract List<IIconGroup> GetAllRelevantIGs();
	}
	public interface IEquippableIITAManager: IItemIconTransactionManager{
		IIconGroup GetRelevantEqpCGearsIG();
	}
	public class EquippableIITAManager: AbsItemIconTransactionManager, IEquippableIITAManager{
		public EquippableIITAManager(IIconPanel equippedItemsPanel, IIconPanel poolItemsPanel){
			this.equippedItemsPanel = equippedItemsPanel;
			this.poolItemsPanel = poolItemsPanel;
		}
		readonly IIconPanel equippedItemsPanel;
		readonly IIconPanel poolItemsPanel;
		IEquippableItemIcon eiiToEquip;
		IEquippableItemIcon eiiToUnequip;
		public override void ClearTAFields(){
			base.ClearTAFields();
			this.eiiToEquip = null;
			this.eiiToUnequip = null;
		}
		public override void ClearHoverability(){
			equippedItemsPanel.WaitForPickUp();
			poolItemsPanel.WaitForPickUp();
		}
		public override List<IIconGroup> GetAllRelevantIGs(){
			List<IIconGroup> result = new List<IIconGroup>();
			IIconGroup relevPoolIG = poolItemsPanel.GetRelevantIG();
			IIconGroup relevEqpIG = equippedItemsPanel.GetRelevantIG();
			result.Add(relevEqpIG);
			result.Add(relevPoolIG);
			return result;
		}
		public IIconGroup GetRelevantEqpCGearsIG(){
			return null;
		}
	}
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

