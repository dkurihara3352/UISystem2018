using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquippableItemIcon: IItemIcon, IEquippableUIE{
		bool IsInEqpIG();
		IEquippableUIItem GetEquippableItem();
		bool IsBowOrWearItemIcon();
	}
	public class EquippableItemIcon: AbsItemIcon, IEquippableItemIcon{
		public EquippableItemIcon(IEquippableItemIconConstArg arg) :base(arg){
		}
		/*  */
			IEquippableIITAManager eqpIITAM{get{return (IEquippableIITAManager)this.iiTAM;}}
			IEquippableUIItem thisEqpItem{get{return thisItem as IEquippableUIItem;}}
			public IEquippableUIItem GetEquippableItem(){return thisEqpItem;}
			IEquipToolIG eqpToolIG{
				get{
					if(thisIG is IEquipToolIG)
						return (IEquipToolIG)thisIG;
					else 
						throw new System.InvalidCastException("this.iconGroup must be of type IEqpToolIG");
				}
			}
			void CheckPassedItemIconTypeValidity(IItemIcon itemIcon){
				if(!(itemIcon is IEquippableItemIcon))
					throw new System.InvalidCastException("passed itemIcon must be of type IEquippableItemIcon");
			}
		/* pick up imple */
			protected override int GetMaxTransferableQuantity(){
				IItemTemplate thisItemTemp = thisEqpItem.GetItemTemplate();
				int thisQuantity = GetItemQuantity();
				if(this.IsBowOrWearItemIcon()){
					if(thisQuantity != 0)
						return 1;
					else
						return 0;
				}else{
					if(this.IsInEqpIG())
						return thisQuantity;
					else{
						if(thisItemTemp.IsStackable())
							return thisQuantity;
						else{
							IEquipToolEquippedCarriedGearsIG relevantEqpCGIG = eqpIITAM.GetRelevantEquippedCarriedGearsIG();
							int equippedQuantity = relevantEqpCGIG.GetItemQuantity(thisEqpItem);
							int spaceInEqpIG = thisEqpItem.GetMaxEquippableQuantity() - equippedQuantity;
							return Mathf.Min(spaceInEqpIG, thisQuantity);
						}
					}
				}
			}
			public override void CheckForImmediatePickUp(){
				return;
			}
			public override void CheckForDelayedPickUp(){
				this.CheckForPickUp();
			}
			public override void CheckForSecondTouchPickUp(){
				this.CheckForPickUp();
			}
			public override void CheckForDragPickUp(ICustomEventData eventData){
				return;
			}
			protected void CheckForPickUp(){
				if(this.IsPicked())
					return;
				else{
					if(this.IsPickable())
						this.PickUp();
					else
						this.DeclinePickUp();
				}
			}
			protected bool IsEligibleForQuickDrop(){
				IEquippableItemIcon hoveredEqpII = eqpIITAM.GetHoveredEqpII();
				if(thisItem.IsStackable() && this.HasSameItem(hoveredEqpII))
					return false;
				else
					return true;
			}
			public override void CheckForQuickDrop(){
				if(this.IsEligibleForQuickDrop())
					Drop();
			}
			public override void CheckForDelayedDrop(){
				if(!this.IsEligibleForQuickDrop())
					Drop();
			}
		/* Equip imple */
			public void Equip(){}
			public void Unequip(){}
			public bool IsEquipped(){return false;}
		/* hoverability */
			public override void CheckForHover(){
				eqpIITAM.TrySwitchHoveredEqpII(this);
			}
			protected override bool IsEligibleForHover(IItemIcon pickedII){
				CheckPassedItemIconTypeValidity(pickedII);
				IEquippableItemIcon pickedEqpII = (IEquippableItemIcon)pickedII;
				if(this.IsInSourceIG(pickedEqpII)){
					return this.IsEligibleForHoverAsSourceIGEqpII(pickedEqpII);
				}else{
					return this.IsEligibleForHoverAsDestIGEqpII(pickedEqpII);
				}
			}
			bool IsInSourceIG(IEquippableItemIcon pickedEqpII){
				return this.GetIconGroup() == pickedEqpII.GetIconGroup();
			}
			bool IsEligibleForHoverAsSourceIGEqpII(IEquippableItemIcon pickedEqpII){
				/*  true on when transfer is ok, not when only reorder is allowed
				*/
				if(this.IsEmpty())//revert when no ghost
					return true;
				else{
					if(this.HasSameItem(pickedEqpII))//revert when ghost
						return true;
					else
						return false;
				}
			}
			bool IsEligibleForHoverAsDestIGEqpII(IEquippableItemIcon pickedEqpII){
				if(this.IsEmpty())
					return true;
				else{
					if(this.HasSameItemTemp(pickedEqpII)){
						if(this.IsInPoolIG()){//picked from equipIG to pool
							if(this.IsTransferable())
								return true;
							else
								return false;
						}else//this.IsInEqpIG(), picked from pool
							return true;
					}else//diff family
						return false;
				}
			}
			bool HasSameItemTemp(IEquippableItemIcon otherEqpII){
				return this.GetItemTemplate().GetType() == otherEqpII.GetItemTemplate().GetType();
			}
		/*  */
			public bool IsInEqpIG(){
				return this.eqpToolIG is IEquipToolEquipIG;
			}
			public bool IsInPoolIG(){
				return this.eqpToolIG is IEquipToolPoolIG;
			}
			public bool IsBowOrWearItemIcon(){
				return thisItemTemp is IBowTemplate || thisItemTemp is IWearTemplate;
			}
			public override bool LeavesGhost(){
				return !this.IsInEqpIG();
			}
			public override bool HasSameItem(IItemIcon other){
				IEquippableItemIcon otherEqpII = (IEquippableItemIcon)other;
				return this.HasSameItem(otherEqpII.GetEquippableItem());
			}
			public override bool HasSameItem(IUIItem item){
				return thisEqpItem.IsSameAs(item);
			}
		/*  */
	}
	public interface IEquippableItemIconConstArg: IItemIconConstArg{
	}
	public class EquippableItemIconConstArg: ItemIconConstArg, IEquippableItemIconConstArg{
		public EquippableItemIconConstArg(IUIManager uim, IEquippableItemIconAdaptor uia, IUIImage image, IEquipTool tool, IDragImageImplementor dragImageImplementor , IEquippableIITAManager eqpIITAM, IEquippableUIItem item, IEqpIITransactionStateEngine eqpIITAStateEngine, IItemIconPickUpImplementor pickUpImplementor, IItemIconEmptinessStateEngine emptinessStateEngine): base(uim, uia, image, tool, dragImageImplementor, eqpIITAM, item, eqpIITAStateEngine, pickUpImplementor, emptinessStateEngine){
		}
	}
}
