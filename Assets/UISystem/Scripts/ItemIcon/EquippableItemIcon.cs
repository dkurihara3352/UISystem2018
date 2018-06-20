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
			this.eqpTool = arg.tool;
		}
		/*  */
			readonly IEquipTool eqpTool;
			IEquippableIITAManager eqpIITAM{
				get{
					return (IEquippableIITAManager)this.iiTAM;//safe
				}
			}
			IEquippableUIItem eqpItem{
				get{
					return this.thisItem as IEquippableUIItem;//safe
				}
			}
			public IEquippableUIItem GetEquippableItem(){
				return eqpItem;
			}
			IEqpToolIG eqpToolIG{
				get{
					if(this.thisIG is IEqpToolIG)
						return this.thisIG as IEqpToolIG;
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
				IItemTemplate thisItemTemp = this.eqpItem.GetItemTemplate();
				int thisQuantity = this.GetItemQuantity();
				if(this.IsBowOrWearItemIcon()){
					if(thisQuantity != 0)
						return 1;
					else
						return 0;
				}else{
					if(this.IsInEqpIG())
						return thisQuantity;
					else{
						if(this.thisItemTemp.IsStackable())
							return thisQuantity;
						else{
							IIconGroup relevantEqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
							int itemQuantityInEqpCGIG = relevantEqpCGIG.GetItemQuantity(this.eqpItem);
							return this.eqpItem.GetMaxEquippableQuantity() - itemQuantityInEqpCGIG;
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
			public override void CheckForDragPickUp(Vector2 pos, Vector2 deltaP){
				return;
			}
			void CheckForPickUp(){
				if(this.IsPicked())
					return;
				else{
					if(this.IsPickable())
						this.PickUp();
					else
						this.DeclinePickUp();
				}
			}
			bool IsEligibleForQuickDrop(){
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
				if(this.GetIconGroup() == pickedII.GetIconGroup()){
					return this.IsEligibleForHoverAsSourceIGEqpII(pickedII as IEquippableItemIcon);
				}else{
					return this.IsEligibleForHoverAsDestIGEqpII(pickedII as IEquippableItemIcon);
				}
			}
			bool IsEligibleForHoverAsSourceIGEqpII(IEquippableItemIcon pickedEqpII){
				/*  true on when transfer is ok, not when only reorder is allowed
				*/
				if(this.IsEmpty())
					return true;
				else{
					if(this.eqpItem.IsSameAs(pickedEqpII.GetUIItem()))
						return true;
					else
						return false;
				}
			}
			bool IsEligibleForHoverAsDestIGEqpII(IEquippableItemIcon pickedEqpII){
				if(this.IsEmpty())
					return true;
				else{
					if(this.ItemTempFamilyIsSameAs(pickedEqpII.GetItemTemplate())){
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
		/*  */
			public bool IsInEqpIG(){
				return this.eqpToolIG is IEqpToolEqpIG;
			}
			public bool IsInPoolIG(){
				return this.eqpToolIG is IEqpToolPoolIG;
			}
			public bool IsBowOrWearItemIcon(){
				return this.thisItemTemp is IBowTemplate || this.thisItemTemp is IWearTemplate;
			}
			public override bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp){
				if(
					(this.thisItemTemp is IBowTemplate && itemTemp is IBowTemplate)
					||(this.thisItemTemp is IWearTemplate && itemTemp is IWearTemplate)
					||(this.thisItemTemp is ICarriedGearTemplate && itemTemp is ICarriedGearTemplate)
				)
					return true;
				else
					return false;
			}
			public override bool LeavesGhost(){
				return !this.IsInEqpIG();
			}
			public override bool HasSameItem(IItemIcon other){
				IEquippableItemIcon otherEqpII = other as IEquippableItemIcon;
				if(otherEqpII != null){
					return this.GetEquippableItem().IsSameAs(otherEqpII.GetEquippableItem());
				}else
					throw new System.InvalidOperationException("other must be of type IEquippableItemIcon");
			}
		/*  */
	}
	public interface IEquippableItemIconConstArg: IItemIconConstArg{
		IEquipTool tool{get;}
	}
	public class EquippableItemIconConstArg: ItemIconConstArg, IEquippableItemIconConstArg{
		public EquippableItemIconConstArg(IUIManager uim, IEquippableItemIconUIA uia, IUIImage image, IEquippableIITAManager eqpIITAM, IEquippableUIItem item, IEqpIITransactionStateEngine eqpIITAStateEngine, IItemIconPickUpImplementor pickUpImplementor, IItemIconEmptinessStateEngine emptinessStateEngine, IEquipTool tool): base(uim, uia, image, eqpIITAM, item, eqpIITAStateEngine, pickUpImplementor, emptinessStateEngine){
			thisTool = tool;
		}
		readonly IEquipTool thisTool;
		public IEquipTool tool{get{return this.thisTool;}}
	}
}
