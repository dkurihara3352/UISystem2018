using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquippableIITAManager: IItemIconTransactionManager{
		IEqpToolPoolIG GetRelevantEqpToolPoolIG();
		IEqpToolEqpCarriedGearsIG GetRelevantEqpCGearsIG();
		IEqpToolEqpIG GetRelevantEquipIG(IEquippableItemIcon pickedEqpII);
		void TrySwitchHoveredEqpII(IEquippableItemIcon eqpII);
		void TrySwitchHoveredEqpToolPanel(IEquipToolPanel panel);
		void SetEqpIIToEquip(IEquippableItemIcon eqpII);
		void SetEqpIIToUnequip(IEquippableItemIcon eqpII);
		IEquippableItemIcon GetPickedEqpII();
		IEquippableItemIcon GetHoveredEqpII();
		IEquipToolPanel GetHoveredEqpToolPanel();
		void ClearHoveredEqpII();
		void ClearEqpIIsToEquipAndUnequip();
		void UpdateEquippedItems();
	}

	public class EquippableIITAManager: AbsItemIconTransactionManager, IEquippableIITAManager{
		public EquippableIITAManager(IEqpIITAMStateEngine eqpIITAMStateEngine, IIconPanel equippedItemsPanel, IIconPanel poolItemsPanel, IEquipTool eqpTool): base(eqpIITAMStateEngine){
			thisEquippedItemsPanel = equippedItemsPanel;
			thisPoolItemsPanel = poolItemsPanel;
			thishoveredEqpIISwitch = new PickUpReceiverSwitch<IEquippableItemIcon>();
			thisHoveredPanelSwitch = new PickUpReceiverSwitch<IEquipToolPanel>();
		}
		public override IItemIcon CreateItemIcon(IUIItem item){
			return null;
		}
		/* TA fields */
			readonly IPickUpContextUIE eqpToolUIE;
			readonly IIconPanel thisEquippedItemsPanel;
			readonly IIconPanel thisPoolItemsPanel;
			IEquippableItemIcon thisEIIToEquip;
			IEquippableItemIcon thisEIIToUnequip;
			public void SetEqpIIToEquip(IEquippableItemIcon eqpII){
				thisEIIToEquip = eqpII;
			}
			public void SetEqpIIToUnequip(IEquippableItemIcon eqpII){
				thisEIIToUnequip = eqpII;
			}
			public override void ClearTAFields(){
				base.ClearTAFields();
				thisEIIToEquip = null;
				thisEIIToUnequip = null;
			}
			public void ClearEqpIIsToEquipAndUnequip(){
				thisEIIToEquip = null;
				thisEIIToUnequip = null;
			}
		/* getting igs */
			public override List<IIconGroup> GetAllRelevantIGs(IItemIcon pickedII){
				List<IIconGroup> result = new List<IIconGroup>();
				IEqpToolPoolIG relevPoolIG = GetRelevantEqpToolPoolIG();
				result.Add(relevPoolIG);
				if(pickedEqpII != null){
					IEqpToolEqpIG relevEqpIG = GetRelevantEquipIG((IEquippableItemIcon)pickedII);
					result.Add(relevEqpIG);
				}else{
					List<IEqpToolEqpIG> allRelevantEqpIGs = GetAllRelevantEqpIGs();
					foreach(IEqpToolEqpIG eqpIG in allRelevantEqpIGs)
						result.Add(eqpIG);
				}
				return result;
			}
			public IEqpToolPoolIG GetRelevantEqpToolPoolIG(){
				return null;
			}
			public IEqpToolEqpCarriedGearsIG GetRelevantEqpCGearsIG(){
				return null;
			}
			IEqpToolEqpBowIG GetRelevantEqpBowIG(){
				return null;
			}
			IEqpToolEqpWearIG GetRelevantEqpWearIG(){
				return null;
			}
			public IEqpToolEqpIG GetRelevantEquipIG(IEquippableItemIcon eqpII){
				IItemTemplate itemTemp = eqpII.GetItemTemplate();
				if(itemTemp is IBowTemplate)
					return GetRelevantEqpBowIG();
				else if(itemTemp is IWearTemplate)
					return GetRelevantEqpWearIG();
				else
					return GetRelevantEqpCGearsIG();
			}
			List<IEqpToolEqpIG> GetAllRelevantEqpIGs(){
				List<IEqpToolEqpIG> result = new List<IEqpToolEqpIG>();
				result.Add(GetRelevantEqpBowIG());
				result.Add(GetRelevantEqpWearIG());
				result.Add(GetRelevantEqpCGearsIG());
				return result;
			}
		/* PUM */
			public IEquippableItemIcon GetPickedEqpII(){
				return pickedEqpII;
			}
			IEquippableItemIcon pickedEqpII{
				get{
					IEquippableItemIcon pickedEqpII = thisPickedUIE as IEquippableItemIcon;
					if(pickedEqpII != null)
						return pickedEqpII;
					else
						throw new System.InvalidOperationException("this.GetPickedII() must return instance of IEquippableItemIcon");
				}
			}
			public override IPickUpContextUIE GetPickUpContextUIE(){
				return eqpToolUIE;
			}
		/*  */
			public override void EvaluateHoverability(){
				this.thisEquippedItemsPanel.EvaluateHoverability(pickedEqpII);
				this.thisPoolItemsPanel.EvaluateHoverability(pickedEqpII);
				foreach(IIconGroup ig in this.GetAllRelevantIGs(pickedEqpII))
					ig.EvaluateAllIIsHoverability(pickedEqpII);
			}
			public override void ResetHoverability(){
				thisEquippedItemsPanel.WaitForPickUp();
				thisPoolItemsPanel.WaitForPickUp();
			}
			public override void ExecuteTransaction(){
				if(thisEIIToUnequip != null){
					thisEIIToUnequip.Unequip();
					if(thisEIIToUnequip == pickedEqpII)
						thisEIIToUnequip.Immigrate(GetRelevantEqpToolPoolIG());
					else
						thisEIIToUnequip.Transfer(GetRelevantEqpToolPoolIG());
				}
				if(thisEIIToEquip != null){
					thisEIIToEquip.Equip();
					if(thisEIIToEquip == pickedEqpII)
						thisEIIToEquip.Immigrate(GetRelevantEquipIG(thisEIIToEquip));
					else
						thisEIIToEquip.Transfer(GetRelevantEquipIG(thisEIIToEquip));
				}
			}
		/* hover switch */
			public override void HoverInitialPickUpReceiver(){
				/*  EqpII to hover is infered
				*/
				if(pickedEqpII.IsInEqpIG())
					thisEquippedItemsPanel.CheckForHover();
				else
					thisPoolItemsPanel.CheckForHover();
			}
			readonly IPickUpReceiverSwitch<IEquipToolPanel> thisHoveredPanelSwitch;
			public IEquipToolPanel GetHoveredEqpToolPanel(){
				return thisHoveredPanelSwitch.GetHoveredPUReceiver();
			}
			readonly IPickUpReceiverSwitch<IEquippableItemIcon> thishoveredEqpIISwitch;
			public IEquippableItemIcon GetHoveredEqpII(){
				return thishoveredEqpIISwitch.GetHoveredPUReceiver();
			}
			public void TrySwitchHoveredEqpToolPanel(IEquipToolPanel panel){
				thisHoveredPanelSwitch.TrySwitchHoveredPUReceiver(panel);
			}
			public void TrySwitchHoveredEqpII(IEquippableItemIcon eqpII){
				thishoveredEqpIISwitch.TrySwitchHoveredPUReceiver(eqpII);
			}
			public void ClearHoveredEqpII(){
				TrySwitchHoveredEqpII(null);
			}
			protected override void ClearHoverFields(){
				ClearHoveredEqpII();
				ClearHoveredEqpPanel();
			}
			void ClearHoveredEqpPanel(){
				TrySwitchHoveredEqpII(null);
			}
		/*  */
			public void UpdateEquippedItems(){}
		/*  */
	}
}
