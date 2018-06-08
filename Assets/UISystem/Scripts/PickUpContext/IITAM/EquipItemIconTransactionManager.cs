using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquippableIITAManager: IItemIconTransactionManager{
		IIconGroup GetRelevantEqpCGearsIG();
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
		public IIconGroup GetRelevantEqpCGearsIG(){
			return null;
		}
		public override IPickUpContextUIE GetPickUpContextUIE(){
			return eqpToolUIE;
		}
	}
}
