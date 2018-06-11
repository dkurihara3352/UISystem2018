using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquippableIITAManager: IItemIconTransactionManager{
		IEqpToolEqpIG<ICarriedGearTemplate> GetRelevantEqpCGearsIG();
		IEqpToolEqpIG<IItemTemplate> GetRelevantEquipIG(IItemTemplate itemTemp);
	}

	public class EquippableIITAManager: AbsItemIconTransactionManager, IEquippableIITAManager{
		public EquippableIITAManager(IIconPanel equippedItemsPanel, IIconPanel poolItemsPanel, IEquipTool eqpTool){
			this.equippedItemsPanel = equippedItemsPanel;
			this.poolItemsPanel = poolItemsPanel;
		}
		readonly IIconPanel equippedItemsPanel;
		readonly IIconPanel poolItemsPanel;
		IEquippableItemIcon eiiToEquip;
		IEquippableItemIcon eiiToUnequip;
		readonly IPickUpContextUIE eqpToolUIE;
		public override void ClearTAFields(){
			base.ClearTAFields();
			this.eiiToEquip = null;
			this.eiiToUnequip = null;
		}
		public void ClearHoverability(){
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
		public IEqpToolEqpIG<ICarriedGearTemplate> GetRelevantEqpCGearsIG(){
			return null;
		}
		IEqpToolEqpIG<IBowTemplate> GetRelevantEqpBowIG(){
			return null;
		}
		IEqpToolEqpIG<IWearTemplate> GetRelevantEqpWearIG(){
			return null;
		}
		public IEqpToolEqpIG<IItemTemplate> GetRelevantEquipIG(IItemTemplate itemTemp){
			if(itemTemp is IBowTemplate)
				return GetRelevantEqpBowIG() as IEqpToolEqpIG<IItemTemplate>;
			else if(itemTemp is IWearTemplate)
				return GetRelevantEqpWearIG() as IEqpToolEqpIG<IItemTemplate>;
			else
				return GetRelevantEqpCGearsIG() as IEqpToolEqpIG<IItemTemplate>;
		}
		public override IPickUpContextUIE GetPickUpContextUIE(){
			return eqpToolUIE;
		}
		public override void EvaluateHoverability(){
			IEquippableItemIcon pickedEqpII = this.pickedUIE as IEquippableItemIcon;
			this.equippedItemsPanel.EvaluateHoverability(pickedEqpII);
			this.poolItemsPanel.EvaluateHoverability(pickedEqpII);
			foreach(IIconGroup ig in this.GetAllRelevantIGs())
				ig.EvaluateAllIIsHoverability(pickedEqpII);
		}
	}
}
