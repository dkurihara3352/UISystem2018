using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolPanel: IIconPanel, IEquipToolElementUIE{
		void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII);
		void HoverDefaultTransactionTargetEqpII(IEquippableItemIcon pickedEqpII);
		void CheckAndRemoveEmptyEqpIIs();
	}
	public abstract class AbsEquipToolPanel: AbsIconPanel, IEquipToolPanel{
		public AbsEquipToolPanel(IEquipToolPanelConstArg arg): base(arg){
			thisEqpIITAM = arg.eqpIITAM;
			thisEqpTool = arg.eqpTool;
			thisPanelTransactionStateEngine = arg.panelTransactionStateEngine;
		}
		readonly protected IEquippableIITAManager thisEqpIITAM;
		readonly protected IEquipTool thisEqpTool;
		public override void CheckForHover(){
			thisEqpIITAM.TrySwitchHoveredEqpToolPanel(this);
		}
		public void HoverDefaultTransactionTargetEqpII(IEquippableItemIcon pickedEqpII){
			IEquipToolIG relevantIG = this.GetRelevantEqpToolIG(pickedEqpII);
			IEquippableItemIcon defaultTATargetEqpII = relevantIG.GetDefaultTATargetEqpII(pickedEqpII);
			if(defaultTATargetEqpII != null)
				defaultTATargetEqpII.CheckForHover();
		}
		protected abstract IEquipToolIG GetRelevantEqpToolIG(IEquippableItemIcon eqpII);
		public abstract void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII);
		public abstract void CheckAndRemoveEmptyEqpIIs();
	}
	public interface IEquipToolEquippedItemsPanel: IEquipToolPanel{}
	public class EquipToolEquippedItemsPanel: AbsEquipToolPanel, IEquipToolEquippedItemsPanel{
		public EquipToolEquippedItemsPanel(IEquipToolPanelConstArg arg) :base(arg){
		}
		protected override bool IsEligibleForHover(IItemIcon pickedII){
			if(pickedII is IEquippableItemIcon){
				IEquippableItemIcon pickedEqpII = pickedII as IEquippableItemIcon;
				IUIItem pickedItem = pickedEqpII.GetUIItem();
				IItemTemplate pickedItemTemp = pickedItem.GetItemTemplate();
				if(pickedEqpII.IsBowOrWearItemIcon())// always swapped
					return true;
				else{
					if(pickedEqpII.IsInEqpIG()){
						return true;//always revertable
					}else{// pickd from pool
						if(pickedEqpII.IsEquipped()){//always has the same partially picked item
							return true;
						}else{
							IEquipToolIG relevantEqpIG = thisEqpIITAM.GetRelevantEquipIG(pickedEqpII);
							if(relevantEqpIG.GetSize() == 1){//swap target is deduced
								return true;
							}else{
								if(relevantEqpIG.HasSlotSpace())//add target is deduced
									return true;
								else
									return false;
							}
						}
					}
				}
			}else
				throw new System.ArgumentException("pickedII must be of type IEquippableItemIcon");
		}
		protected override IEquipToolIG GetRelevantEqpToolIG(IEquippableItemIcon pickedEqpII){
			return thisEqpIITAM.GetRelevantEquipIG(pickedEqpII);
		}
		public override void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII){
			if(this.IsEligibleForEmptyAddTargetAddition(pickedEqpII)){
				IEquipToolEquippedCarriedGearsIG eqpCGIG = thisEqpIITAM.GetRelevantEquippedCarriedGearsIG();
				eqpCGIG.AddEmptyAddTarget((pickedEqpII.GetEquippableItem()));
			}
		}
		public override void CheckAndRemoveEmptyEqpIIs(){
			IEquipToolEquipIG relevantEqpIG = thisEqpIITAM.GetRelevantEquipIG(thisEqpIITAM.GetPickedEqpII());
			if(relevantEqpIG is IEquipToolEquippedCarriedGearsIG)
				relevantEqpIG.RemoveEmptyIIs();

		}
		bool IsEligibleForEmptyAddTargetAddition(IEquippableItemIcon pickedEqpII){
			if(pickedEqpII.IsBowOrWearItemIcon())
				return false;
			else{
				IEquipToolEquippedCarriedGearsIG eqpCGIG = thisEqpIITAM.GetRelevantEquippedCarriedGearsIG();
				IEquippableItemIcon sameItemEqpII = (IEquippableItemIcon)eqpCGIG.GetItemIconFromItem(pickedEqpII.GetEquippableItem());
				if(sameItemEqpII != null)
					return false;
				else
					return true;
			}
		}
	}
	public interface IEquipToolPoolItemsPanel: IEquipToolPanel{}
	public class EqpToolPoolItemsPanel: AbsEquipToolPanel, IEquipToolPoolItemsPanel{
		public EqpToolPoolItemsPanel(IEquipToolPanelConstArg arg) :base(arg){}
		protected override bool IsEligibleForHover(IItemIcon pickedII){
			if(pickedII is IEquippableItemIcon){
				IEquippableItemIcon pickedEqpII = (IEquippableItemIcon)pickedII;
				if(pickedEqpII.IsBowOrWearItemIcon())
					return false;//can't specify target
				else
					return true;//always slot out from equipIG
			}else
				throw new System.ArgumentException("pickedII must be of type IEquippableItemIcon");
		}
		protected override IEquipToolIG GetRelevantEqpToolIG(IEquippableItemIcon pickedEqpII){
			return thisEqpIITAM.GetRelevantPoolIG();
		}
		public override void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII){
			return;
		}
		public override void CheckAndRemoveEmptyEqpIIs(){
			return;
		}
	}
	/* const  */
	public interface IEquipToolPanelConstArg: IUIElementConstArg{
		IEquippableIITAManager eqpIITAM{get;}
		IEquipTool eqpTool{get;}
		IPanelTransactionStateEngine panelTransactionStateEngine{get;}
	}
	public class EquipToolPanelConstArg: UIElementConstArg ,IEquipToolPanelConstArg{
		public EquipToolPanelConstArg(
			IUIManager uim, 
			IPickUpSystemProcessFactory pickUpSystemProcessFactory, 
			IEquipToolUIEFactory equipToolUIEFactory, 
			IUIAdaptor uia, 
			IUIImage image, 


			IEquipTool eqpTool, 
			IEquippableIITAManager eqpIITAM, 
			IPanelTransactionStateEngine engine
		):base(
			uim, 
			pickUpSystemProcessFactory, 
			equipToolUIEFactory, 
			uia, 
			image,
			ActivationMode.None
		){
			thisEqpIITAM = eqpIITAM;
			thisPanelTransactionStateEngine = engine;
			thisEquipTool = eqpTool;
		}
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableIITAManager eqpIITAM{get{return thisEqpIITAM;}}
		readonly IEquipTool thisEquipTool;
		public IEquipTool eqpTool{get{return thisEquipTool;}}
		readonly IPanelTransactionStateEngine thisPanelTransactionStateEngine;
		public IPanelTransactionStateEngine panelTransactionStateEngine{get{return thisPanelTransactionStateEngine;}}
	}
}
