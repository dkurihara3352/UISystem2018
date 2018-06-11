using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IIconPanel: IPickUpReceiver, IUIElement{
		IIconGroup GetRelevantIG();
		void EvaluateHoverability(IItemIcon pickedII);
	}
	public abstract class AbsIconPanel: AbsUIElement, IIconPanel{
		public AbsIconPanel(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
		protected override void ActivateImple(){
			base.ActivateImple();
			WaitForPickUp();
		}
		public void CheckForHover(){}
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
	}
	public class EquippedItemsPanel: AbsIconPanel, IEquipToolElementUIE{
		public EquippedItemsPanel(IUIManager uim, IEquipToolElementUIA uia, IUIImage image, IEquipToolActivationData activationData) :base(uim, uia, image){
			this.equipTool = activationData.eqpTool;
			this.equipIITAM = activationData.eqpIITAM;
		}
		readonly IEquipTool equipTool;
		readonly IEquippableIITAManager equipIITAM;
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
							IEqpToolIG relevantEqpIG = equipIITAM.GetRelevantEquipIG(pickedItemTemp);
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
	public class PoolItemsPanel: AbsIconPanel, IEquipToolElementUIE{
		public PoolItemsPanel(IUIManager uim, IUIAdaptor uia, IUIImage image) :base(uim, uia, image){}
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
}
