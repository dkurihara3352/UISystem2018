﻿using System.Collections;
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
		public EquippableIITAManager(IIconPanel equippedItemsPanel, IIconPanel poolItemsPanel, IEquipTool eqpTool){
			this.equippedItemsPanel = equippedItemsPanel;
			this.poolItemsPanel = poolItemsPanel;
			this.hoveredEqpIISwitch = new PickUpReceiverSwitch<IEquippableItemIcon>();
			this.hoveredPanelSwitch = new PickUpReceiverSwitch<IEquipToolPanel>();
		}
		public override IItemIcon CreateItemIcon(IUIItem item){
			return null;
		}
		/* TA fields */
			readonly IPickUpContextUIE eqpToolUIE;
			readonly IIconPanel equippedItemsPanel;
			readonly IIconPanel poolItemsPanel;
			IEquippableItemIcon eiiToEquip;
			IEquippableItemIcon eiiToUnequip;
			public void SetEqpIIToEquip(IEquippableItemIcon eqpII){
				this.eiiToEquip = eqpII;
			}
			public void SetEqpIIToUnequip(IEquippableItemIcon eqpII){
				this.eiiToUnequip = eqpII;
			}
			public override void ClearTAFields(){
				base.ClearTAFields();
				this.eiiToEquip = null;
				this.eiiToUnequip = null;
			}
			public void ClearEqpIIsToEquipAndUnequip(){
				eiiToEquip = null;
				eiiToUnequip = null;
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
				this.equippedItemsPanel.EvaluateHoverability(pickedEqpII);
				this.poolItemsPanel.EvaluateHoverability(pickedEqpII);
				foreach(IIconGroup ig in this.GetAllRelevantIGs(pickedEqpII))
					ig.EvaluateAllIIsHoverability(pickedEqpII);
			}
			public override void ResetHoverability(){
				equippedItemsPanel.WaitForPickUp();
				poolItemsPanel.WaitForPickUp();
			}
			public override void ExecuteTransaction(){
				if(eiiToUnequip != null){
					eiiToUnequip.Unequip();
					if(eiiToUnequip == pickedEqpII)
						eiiToUnequip.Immigrate(GetRelevantEqpToolPoolIG());
					else
						eiiToUnequip.Transfer(GetRelevantEqpToolPoolIG());
				}
				if(eiiToEquip != null){
					eiiToEquip.Equip();
					if(eiiToEquip == pickedEqpII)
						eiiToEquip.Immigrate(GetRelevantEquipIG(eiiToEquip));
					else
						eiiToEquip.Transfer(GetRelevantEquipIG(eiiToEquip));
				}
			}
		/* hover switch */
			public override void HoverInitialPickUpReceiver(){
				/*  EqpII to hover is infered
				*/
				if(pickedEqpII.IsInEqpIG())
					equippedItemsPanel.CheckForHover();
				else
					poolItemsPanel.CheckForHover();
			}
			readonly IPickUpReceiverSwitch<IEquipToolPanel> hoveredPanelSwitch;
			public IEquipToolPanel GetHoveredEqpToolPanel(){
				return hoveredPanelSwitch.GetHoveredPUReceiver();
			}
			readonly IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch;
			public IEquippableItemIcon GetHoveredEqpII(){
				return hoveredEqpIISwitch.GetHoveredPUReceiver();
			}
			public void TrySwitchHoveredEqpToolPanel(IEquipToolPanel panel){
				hoveredPanelSwitch.TrySwitchHoveredPUReceiver(panel);
			}
			public void TrySwitchHoveredEqpII(IEquippableItemIcon eqpII){
				hoveredEqpIISwitch.TrySwitchHoveredPUReceiver(eqpII);
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
