using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipToolPanel: IIconPanel, IEquipToolElementUIE{
		void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII);
		void HoverDefaultTransactionTargetEqpII(IEquippableItemIcon pickedEqpII);
		void CheckAndRemoveEmptyEqpIIs();
	}
	public abstract class AbsEquipToolPanel: AbsIconPanel, IEquipToolPanel{
		public AbsEquipToolPanel(IEquipToolPanelConstArg arg): base(arg){
			this.eqpIITAM = arg.eqpIITAM;
			this.eqpTool = arg.eqpTool;
			this.panelTransactionStateEngine = arg.panelTransactionStateEngine;
		}
		readonly protected IEquippableIITAManager eqpIITAM;
		readonly protected IEquipTool eqpTool;
		public override void CheckForHover(){
			eqpIITAM.TrySwitchHoveredEqpToolPanel(this);
		}
		public void HoverDefaultTransactionTargetEqpII(IEquippableItemIcon pickedEqpII){
			IEqpToolIG relevantIG = this.GetRelevantEqpToolIG(pickedEqpII);
			IEquippableItemIcon defaultTATargetEqpII = relevantIG.GetDefaultTATargetEqpII(pickedEqpII);
			if(defaultTATargetEqpII != null)
				defaultTATargetEqpII.CheckForHover();
		}
		protected abstract IEqpToolIG GetRelevantEqpToolIG(IEquippableItemIcon eqpII);
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
				if(pickedItemTemp is IBowTemplate || pickedItemTemp is IWearTemplate)// always swapped
					return true;
				else{
					if(pickedEqpII.IsInEqpIG()){
						return true;//always revertable
					}else{// pickd from pool
						if(pickedEqpII.IsEquipped()){//always has the same partially picked item
							return true;
						}else{
							IEqpToolIG relevantEqpIG = eqpIITAM.GetRelevantEquipIG(pickedEqpII);
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
		protected override IEqpToolIG GetRelevantEqpToolIG(IEquippableItemIcon pickedEqpII){
			return eqpIITAM.GetRelevantEquipIG(pickedEqpII);
		}
		public override void CheckAndAddEmptyAddTarget(IEquippableItemIcon pickedEqpII){
			if(this.IsEligibleForEmptyAddTargetAddition(pickedEqpII)){
				IEqpToolEqpIG<ICarriedGearTemplate> eqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
				eqpCGIG.AddEmptyAddTarget((pickedEqpII.GetEquippableItem()));
			}
		}
		public override void CheckAndRemoveEmptyEqpIIs(){
			IEqpToolEqpIG<IItemTemplate> relevantEqpIG = eqpIITAM.GetRelevantEquipIG(eqpIITAM.GetPickedEqpII());
			if(relevantEqpIG is IEqpToolEqpIG<ICarriedGearTemplate>)
				relevantEqpIG.RemoveEmptyIIs();

		}
		bool IsEligibleForEmptyAddTargetAddition(IEquippableItemIcon pickedEqpII){
			if(pickedEqpII.IsBowOrWearItemIcon())
				return false;
			else{
				IEqpToolEqpIG<ICarriedGearTemplate> eqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
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
				IEquippableUIItem eqpItem = ((IEquippableItemIcon)pickedII).GetUIItem() as IEquippableUIItem;
				IItemTemplate itemTemp = eqpItem.GetItemTemplate();
				if(itemTemp is IBowTemplate || itemTemp is IWearTemplate)
					return false;//can't specify target
				else
					return true;//always slot out from equipIG
			}else
				throw new System.ArgumentException("pickedII must be of type IEquippableItemIcon");
		}
		protected override IEqpToolIG GetRelevantEqpToolIG(IEquippableItemIcon pickedEqpII){
			return eqpIITAM.GetRelevantEqpToolPoolIG();
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
		public EquipToolPanelConstArg(IUIManager uim, IUIAdaptor uia, IUIImage image, IEquippableIITAManager eqpIITAM, IEquipTool eqpTool, IPanelTransactionStateEngine engine): base(uim, uia, image){
			thisEqpIITAM = eqpIITAM;
			thisEqpTool = eqpTool;
			thisPanelTransactionStateEngine = engine;
		}
		readonly IEquippableIITAManager thisEqpIITAM;
		public IEquippableIITAManager eqpIITAM{get{return thisEqpIITAM;}}
		readonly IEquipTool thisEqpTool;
		public IEquipTool eqpTool{get{return thisEqpTool;}}
		readonly IPanelTransactionStateEngine thisPanelTransactionStateEngine;
		public IPanelTransactionStateEngine panelTransactionStateEngine{get{return thisPanelTransactionStateEngine;}}
	}
}
