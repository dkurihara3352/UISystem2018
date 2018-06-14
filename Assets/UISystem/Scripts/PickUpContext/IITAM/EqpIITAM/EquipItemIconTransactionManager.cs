using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquippableIITAManager: IItemIconTransactionManager{
		IEqpToolPoolIG GetRelevantEqpToolPoolIG();
		IEqpToolEqpIG<ICarriedGearTemplate> GetRelevantEqpCGearsIG();
		IEqpToolEqpIG<IItemTemplate> GetRelevantEquipIG(IEquippableItemIcon pickedEqpII);
		void TrySwitchHoveredEqpII(IEquippableItemIcon eqpII);
		void TrySwitchHoveredEqpToolPanel(IEquipToolPanel panel);
		void SetEqpIIToEquip(IEquippableItemIcon eqpII);
		void SetEqpIIToUnequip(IEquippableItemIcon eqpII);
		IEquippableItemIcon GetPickedEqpII();
		void ClearHoveredEqpII();
		void ClearEqpIIsToEquipAndUnequip();
	}

	public class EquippableIITAManager: AbsItemIconTransactionManager, IEquippableIITAManager{
		public EquippableIITAManager(IIconPanel equippedItemsPanel, IIconPanel poolItemsPanel, IEquipTool eqpTool){
			this.equippedItemsPanel = equippedItemsPanel;
			this.poolItemsPanel = poolItemsPanel;
			this.hoveredEqpIISwitch = new PickUpReceiverSwitch<IEquippableItemIcon>();
			this.hoveredPanelSwitch = new PickUpReceiverSwitch<IEquipToolPanel>();
		}
		/* TA fields */
			readonly IPickUpContextUIE eqpToolUIE;
			readonly IIconPanel equippedItemsPanel;
			readonly IIconPanel poolItemsPanel;
			IEquippableItemIcon eiiToEquip;
			IEquippableItemIcon eiiToUnequip;
			IEquippableItemIcon pickedEqpII{
				get{
					IEquippableItemIcon pickedEqpII = this.GetPickedEqpII() as IEquippableItemIcon;
					if(pickedEqpII != null)
						return pickedEqpII;
					else
						throw new System.InvalidOperationException("this.GetPickedII() must return instance of IEquippableItemIcon");
				}
			}
			public IEquippableItemIcon GetPickedEqpII(){
				return pickedEqpII;
			}
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
			public override void ClearHoverability(){
				equippedItemsPanel.WaitForPickUp();
				poolItemsPanel.WaitForPickUp();
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
					IEqpToolEqpIG<IItemTemplate> relevEqpIG = GetRelevantEquipIG((IEquippableItemIcon)pickedII);
					result.Add(relevEqpIG);
				}else{
					List<IEqpToolEqpIG<IItemTemplate>> allRelevantEqpIGs = GetAllRelevantEqpIGs();
					foreach(IEqpToolEqpIG<IItemTemplate> eqpIG in allRelevantEqpIGs)
						result.Add(eqpIG);
				}
				return result;
			}
			public IEqpToolPoolIG GetRelevantEqpToolPoolIG(){
				return null;
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
			public IEqpToolEqpIG<IItemTemplate> GetRelevantEquipIG(IEquippableItemIcon eqpII){
				IItemTemplate itemTemp = eqpII.GetItemTemplate();
				if(itemTemp is IBowTemplate)
					return GetRelevantEqpBowIG() as IEqpToolEqpIG<IItemTemplate>;
				else if(itemTemp is IWearTemplate)
					return GetRelevantEqpWearIG() as IEqpToolEqpIG<IItemTemplate>;
				else
					return GetRelevantEqpCGearsIG() as IEqpToolEqpIG<IItemTemplate>;
			}
			List<IEqpToolEqpIG<IItemTemplate>> GetAllRelevantEqpIGs(){
				List<IEqpToolEqpIG<IItemTemplate>> result = new List<IEqpToolEqpIG<IItemTemplate>>();
				result.Add((IEqpToolEqpIG<IItemTemplate>)GetRelevantEqpBowIG());
				result.Add((IEqpToolEqpIG<IItemTemplate>)GetRelevantEqpWearIG());
				result.Add((IEqpToolEqpIG<IItemTemplate>)GetRelevantEqpCGearsIG());
				return result;
			}
		/*  */
			public override IPickUpContextUIE GetPickUpContextUIE(){
				return eqpToolUIE;
			}
			public override void EvaluateHoverability(){
				this.equippedItemsPanel.EvaluateHoverability(pickedEqpII);
				this.poolItemsPanel.EvaluateHoverability(pickedEqpII);
				foreach(IIconGroup ig in this.GetAllRelevantIGs(pickedEqpII))
					ig.EvaluateAllIIsHoverability(pickedEqpII);
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
			readonly IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch;
			public void TrySwitchHoveredEqpToolPanel(IEquipToolPanel panel){
				hoveredPanelSwitch.TrySwitchHoveredPUReceiver(panel);
			}
			public void TrySwitchHoveredEqpII(IEquippableItemIcon eqpII){
				hoveredEqpIISwitch.TrySwitchHoveredPUReceiver(eqpII);
			}
			public void ClearHoveredEqpII(){
				TrySwitchHoveredEqpII(null);
			}
		/*  */
	}
}
