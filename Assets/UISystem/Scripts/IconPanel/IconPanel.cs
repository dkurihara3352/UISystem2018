using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver, IUIElement{
		IIconGroup GetRelevantIG();
		void EvaluateHoverability(IItemIcon pickedII);
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIElementConstArg arg) :base(arg){}
		protected override void ActivateImple(){
			base.ActivateImple();
			WaitForPickUp();
		}
		public abstract void CheckForHover();
		public abstract IIconGroup GetRelevantIG();
		/* panel transaction state handling */
		readonly IPanelTransactionStateEngine panTAStateEngine;
		public void EvaluateHoverability(IItemIcon pickedII){
			if(this.IsEligibleForHover(pickedII))
				BecomeHoverable();
			else
				BecomeUnhoverable();
		}
		protected abstract bool IsEligibleForHover(IItemIcon pickedII);
		public void WaitForPickUp(){
			panTAStateEngine.WaitForPickUp();
		}
		public void BecomeHoverable(){
			panTAStateEngine.BecomeHoverable();
		}
		public void BecomeUnhoverable(){
			panTAStateEngine.BecomeUnhoverable();
		}
		public void BecomeHovered(){
			panTAStateEngine.BecomeHovered();
		}
		public bool IsHoverable(){
			return panTAStateEngine.IsHoverable();
		}
		public bool IsHovered(){
			return panTAStateEngine.IsHovered();
		}
	}
	public interface IEquipToolPanel: IIconPanel, IEquipToolElementUIE{}
	public abstract class AbsEquipToolPanel: AbsIconPanel, IEquipToolPanel{
		public AbsEquipToolPanel(IEquipToolPanelConstArg arg): base(arg){
			this.eqpIITAM = arg.eqpIITAM;
			this.eqpTool = arg.eqpTool;
		}
		readonly protected IEquippableIITAManager eqpIITAM;
		readonly protected IEquipTool eqpTool;
		public override void CheckForHover(){
			eqpIITAM.TrySwitchHoveredEqpToolPanel(this);
		}
	}
	public class EquippedItemsPanel: AbsEquipToolPanel{
		public EquippedItemsPanel(IEquipToolPanelConstArg arg) :base(arg){
		}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
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
							IEqpToolIG relevantEqpIG = eqpIITAM.GetRelevantEquipIG(pickedItemTemp);
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
	}
	public class PoolItemsPanel: AbsEquipToolPanel{
		public PoolItemsPanel(IEquipToolPanelConstArg arg) :base(arg){}
		public override IIconGroup GetRelevantIG(){
			/* impled later when building Scrollers */
			return null;
		}
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
	}
	public interface IPanelTransactionStateEngine: IHoverabilityStateHandler{}
	public interface IEquipToolPanelConstArg: IUIElementConstArg{
		IEquippableIITAManager eqpIITAM{get;}
		IEquipTool eqpTool{get;}
	}
}
