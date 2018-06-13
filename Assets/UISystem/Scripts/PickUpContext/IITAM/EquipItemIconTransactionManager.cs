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
		IEquippableItemIcon pickedEqpII{
			get{return (IEquippableItemIcon)this.GetPickedII();}
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
		/* getting igs */
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
			IEquippableItemIcon pickedEqpII = this.pickedUIE as IEquippableItemIcon;
			this.equippedItemsPanel.EvaluateHoverability(pickedEqpII);
			this.poolItemsPanel.EvaluateHoverability(pickedEqpII);
			foreach(IIconGroup ig in this.GetAllRelevantIGs(pickedEqpII))
				ig.EvaluateAllIIsHoverability(pickedEqpII);
		}
		/* hover switch */
		public override void HoverInitialPickUpReceiver(){
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
	}
	public interface IPickUpReceiverSwitch<T> where T: class, IPickUpReceiver{
		void TrySwitchHoveredPUReceiver(T hovered);
		T GetHoveredPUReceiver();
	}
	public class PickUpReceiverSwitch<T>: IPickUpReceiverSwitch<T> where T: class, IPickUpReceiver{
		public PickUpReceiverSwitch(){
			currentHoveredPUReceiver = null;
		}
		T currentHoveredPUReceiver;
		public T GetHoveredPUReceiver(){return currentHoveredPUReceiver;}
		public void TrySwitchHoveredPUReceiver(T hoveredPURec){
			if(hoveredPURec == null){
				if(currentHoveredPUReceiver == null){
					return;
				}else{
					currentHoveredPUReceiver.BecomeHoverable();
					currentHoveredPUReceiver = null;
				}
			}else{
				if(hoveredPURec.IsHoverable()){
					if(currentHoveredPUReceiver == null){
						currentHoveredPUReceiver = hoveredPURec;
						hoveredPURec.BecomeHovered();
					}else{
						if(hoveredPURec == currentHoveredPUReceiver)
							return;
						else{
							currentHoveredPUReceiver.BecomeHoverable();
							currentHoveredPUReceiver = hoveredPURec;
							hoveredPURec.BecomeHovered();
						}
					}
				}
			}
		}
	}
}
