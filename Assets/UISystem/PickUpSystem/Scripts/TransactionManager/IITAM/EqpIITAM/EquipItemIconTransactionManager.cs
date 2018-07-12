using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquippableIITAManager: IItemIconTransactionManager, IEquipToolIGHandler{
		void SetEqpTool(IEquipTool eqpTool);
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

	public class EquippableItemIconTransactionManager: AbsItemIconTransactionManager, IEquippableIITAManager{
		public EquippableItemIconTransactionManager(IEqpIITAMConstArg arg): base(arg.eqpIITAMStateEngine){
			thisEquippedItemsPanel = arg.equippedItemsPanel;
			thisPoolItemsPanel = arg.poolItemsPanel;
			thishoveredEqpIISwitch = arg.hoveredEqpIISwitch;
			thisHoveredPanelSwitch = arg.hoveredEqpToolPanelSwitch;
			thisEqpToolIGManager = arg.eqpToolIGManager;
		}
		IEqpIITAMStateEngine thisEqpIITAMStateEngine{get{return (IEqpIITAMStateEngine)thisStateEngine;}}
		public void SetEqpTool(IEquipTool eqpTool){
			thisEqpIITAMStateEngine.SetEqpTool(eqpTool);
			thisEquipTool = eqpTool;
		}
		IEquipTool thisEquipTool;
		/* TA fields */
			readonly IPickUpContextUIE thisEqpToolUIE;
			readonly IIconPanel thisEquippedItemsPanel;
			readonly IIconPanel thisPoolItemsPanel;
			protected IEquippableItemIcon thisEIIToEquip;
			protected IEquippableItemIcon thisEIIToUnequip;
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
			protected IEquipToolIGManager thisEqpToolIGManager;
			public override List<IIconGroup> GetAllRelevantIGs(IItemIcon pickedII){
				List<IIconGroup> result = new List<IIconGroup>();
				IEquipToolPoolIG relevPoolIG = GetRelevantPoolIG();
				result.Add(relevPoolIG);
				if(pickedII != null){
					IEquipToolEquipIG relevEqpIG = GetRelevantEquipIG((IEquippableItemIcon)pickedII);
					result.Add(relevEqpIG);
				}else{
					List<IEquipToolEquipIG> allRelevantEqpIGs = GetAllRelevantEquipIGs();
					foreach(IEquipToolEquipIG eqpIG in allRelevantEqpIGs)
						result.Add(eqpIG);
				}
				return result;
			}
			public IEquipToolPoolIG GetRelevantPoolIG(){
				return thisEqpToolIGManager.GetRelevantPoolIG();
			}
			public IEquipToolEquipIG GetRelevantEquipIG(IEquippableItemIcon pickedEqpII){
				return thisEqpToolIGManager.GetRelevantEquipIG(pickedEqpII);
			}
			public IEquipToolEquippedBowIG GetRelevantEquippedBowIG(){
				return thisEqpToolIGManager.GetRelevantEquippedBowIG();
			}
			public IEquipToolEquippedWearIG GetRelevantEquippedWearIG(){
				return thisEqpToolIGManager.GetRelevantEquippedWearIG();
			}
			public IEquipToolEquippedCarriedGearsIG GetRelevantEquippedCarriedGearsIG(){
				return thisEqpToolIGManager.GetRelevantEquippedCarriedGearsIG();
			}
			public List<IEquipToolEquipIG> GetAllRelevantEquipIGs(){
				return thisEqpToolIGManager.GetAllRelevantEquipIGs();
			}
		/* PUM */
			public IEquippableItemIcon GetPickedEqpII(){
				return thisPickedEqpII;
			}
			IEquippableItemIcon thisPickedEqpII{get{return (IEquippableItemIcon)thisPickedUIE;}}
			public override IPickUpContextUIE GetPickUpContextUIE(){
				return thisEqpToolUIE;
			}
		/*  */
			public override void EvaluateHoverability(){
				this.thisEquippedItemsPanel.EvaluateHoverability(thisPickedEqpII);
				this.thisPoolItemsPanel.EvaluateHoverability(thisPickedEqpII);
				foreach(IIconGroup ig in this.GetAllRelevantIGs(thisPickedEqpII))
					ig.EvaluateAllIIsHoverability(thisPickedEqpII);
			}
			public override void ResetHoverability(){
				thisEquippedItemsPanel.WaitForPickUp();
				thisPoolItemsPanel.WaitForPickUp();
			}
			public override void ExecuteTransaction(){
				if(thisEIIToUnequip != null){
					thisEIIToUnequip.Unequip();
					if(thisEIIToUnequip == thisPickedEqpII)
						thisEIIToUnequip.TravelTransfer(GetRelevantPoolIG());
					else
						thisEIIToUnequip.SpotTransfer(GetRelevantPoolIG());
				}
				if(thisEIIToEquip != null){
					thisEIIToEquip.Equip();
					if(thisEIIToEquip == thisPickedEqpII)
						thisEIIToEquip.TravelTransfer(GetRelevantEquipIG(thisEIIToEquip));
					else
						thisEIIToEquip.SpotTransfer(GetRelevantEquipIG(thisEIIToEquip));
				}
			}
		/* hover switch */
			public override void HoverInitialPickUpReceiver(){
				/*  EqpII to hover is infered
				*/
				if(thisPickedEqpII.IsInEqpIG())
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
	/* Const */
	public interface IEqpIITAMConstArg{
		IEqpIITAMStateEngine eqpIITAMStateEngine{get;}
		IEquipToolPanel equippedItemsPanel{get;}
		IEquipToolPanel poolItemsPanel{get;}
		IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch{get;}
		IPickUpReceiverSwitch<IEquipToolPanel> hoveredEqpToolPanelSwitch{get;}
		IEquipToolIGManager eqpToolIGManager{get;}
	}
	public class EqpIITAMConstArg: IEqpIITAMConstArg{
		public EqpIITAMConstArg(IEqpIITAMStateEngine eqpIITAMStateEngine,IEquipToolPanel equippedItemsPanel,IEquipToolPanel poolItemsPanel, IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch,IPickUpReceiverSwitch<IEquipToolPanel> hoveredEqpToolPanelSwitch, IEquipToolIGManager eqpToolIGManager){
			thisEqpIITAMStateEngine = eqpIITAMStateEngine;
			thisEquippedItemsPanel = equippedItemsPanel;
			thisPoolItemsPanel = poolItemsPanel;
			thisHoveredEqpIISwitch = hoveredEqpIISwitch;
			thisHoveredEqpToolPanelSwitch = hoveredEqpToolPanelSwitch;
			thisEqpToolIGManager = eqpToolIGManager;
		}
		public IEqpIITAMStateEngine eqpIITAMStateEngine{get{return thisEqpIITAMStateEngine;}}
		readonly IEqpIITAMStateEngine thisEqpIITAMStateEngine;
		public IEquipToolPanel equippedItemsPanel{get{return thisEquippedItemsPanel;}}
		readonly IEquipToolPanel thisEquippedItemsPanel;
		public IEquipToolPanel poolItemsPanel{get{return thisPoolItemsPanel;}}
		readonly IEquipToolPanel thisPoolItemsPanel;
		public IPickUpReceiverSwitch<IEquippableItemIcon> hoveredEqpIISwitch{get{return thisHoveredEqpIISwitch;}}
		readonly IPickUpReceiverSwitch<IEquippableItemIcon> thisHoveredEqpIISwitch;
		public IPickUpReceiverSwitch<IEquipToolPanel> hoveredEqpToolPanelSwitch{get{return thisHoveredEqpToolPanelSwitch;}}
		readonly IPickUpReceiverSwitch<IEquipToolPanel> thisHoveredEqpToolPanelSwitch;
		public IEquipToolIGManager eqpToolIGManager{get{return thisEqpToolIGManager;}}
		readonly IEquipToolIGManager thisEqpToolIGManager;
	}
}
