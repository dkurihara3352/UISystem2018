using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IEquipStateHandler{
		void Equip();
		void Unequip();
		bool IsEquipped();
	}
	public interface IEquippableUIE: IUIElement, IEquipStateHandler{
	}
	public interface IEquippableItemIcon: IItemIcon, IEquippableUIE{
		bool IsInEqpIG();
		bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp);
	}
	public class EquippableItemIcon: AbsItemIcon, IEquippableItemIcon{
		public EquippableItemIcon(IEquippableItemIconConstArg arg) :base(arg){
			this.eqpTool = arg.tool;
		}
		readonly IEquipTool eqpTool;
		IEquippableIITAManager eqpIITAM{
			get{
				return (IEquippableIITAManager)this.iiTAM;//safe
			}
		}
		IEquippableUIItem eqpItem{
			get{
				return this.item as IEquippableUIItem;//safe
			}
		}
		IEqpToolIG eqpToolIG{
			get{
				if(this.iconGroup is IEqpToolIG)
					return this.iconGroup as IEqpToolIG;
				else 
					throw new System.InvalidCastException("this.iconGroup must be of type IEqpToolIG");
			}
		}
		void CheckPassedItemIconTypeValidity(IItemIcon itemIcon){
			if(!(itemIcon is IEquippableItemIcon))
				throw new System.InvalidCastException("passed itemIcon must be of type IEquippableItemIcon");
		}
		protected override int GetMaxTransferableQuantity(){
			IItemTemplate thisItemTemp = this.eqpItem.GetItemTemplate();
			int thisQuantity = this.GetQuantity();
			if(this.IsBowOrWearItem()){
				if(thisQuantity != 0)
					return 1;
				else
					return 0;
			}else{
				if(this.IsInEqpIG())
					return thisQuantity;
				else{
					if(this.itemTemp.IsStackable())
						return thisQuantity;
					else{
						IIconGroup relevantEqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
						int itemQuantityInEqpCGIG = relevantEqpCGIG.GetItemQuantity(this.eqpItem);
						return this.eqpItem.GetMaxEquippableQuantity() - itemQuantityInEqpCGIG;
					}
				}
			}
		}
		/* pick up imple */
			public override void CheckForImmediatePickUp(){
				return;
			}
			public override void CheckForDelayedPickUp(){
				this.CheckForPickUp();
			}
			public override void CheckForSecondTouchPickUp(){
				this.CheckForPickUp();
			}
			public override void CheckForDragPickUp(){
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
							if(this.IsBowOrWearItem()){
								if(this.eqpItem.IsSameAs(pickedEqpII.GetUIItem()))
									return false;
								else
									return true;//swapping equipped bow/wear
							}else{
								if(this.eqpItem.IsSameAs(pickedEqpII.GetUIItem()))
									return true;// unequipping
								else{
									if(this.GetQuantity() <= 0){
										return false;//can't swap with zero item
									}else{
									// true only when eqpIG can accomodate this
										IEqpToolEqpIG<ICarriedGearTemplate> eqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
										if(eqpCGIG.HasItemSpace(this.eqpItem)){
											return true;
										}else
											return false;
									}
								}
							}
						}else//this.IsInEqpIG(), picked from pool
							return true;
					}else//diff family
						return false;
				}
			}
		/*  */
		public bool IsInEqpIG(){
			return this.eqpToolIG is IEqpToolEqpIG<IItemTemplate>;
		}
		public bool IsInPoolIG(){
			return this.eqpToolIG is IEqpToolPoolIG;
		}
		public bool IsBowOrWearItem(){
			return this.itemTemp is IBowTemplate || this.itemTemp is IWearTemplate;
		}
		public bool ItemTempFamilyIsSameAs(IItemTemplate itemTemp){
			if(
				(this.itemTemp is IBowTemplate && itemTemp is IBowTemplate)
				||(this.itemTemp is IWearTemplate && itemTemp is IWearTemplate)
				||(this.itemTemp is ICarriedGearTemplate && itemTemp is ICarriedGearTemplate)
			)
				return true;
			else
				return false;
		}
		public override bool LeavesGhost(){
			return !this.IsInEqpIG();
		}
	}
	public interface IEquippableItemIconConstArg: IItemIconConstArg{
		IEquipTool tool{get;}
	}
	public class EquippableItemIconConstArg: IEquippableItemIconConstArg{
		public EquippableItemIconConstArg(IUIManager uim, IEquippableItemIconUIA uia, IUIImage image, IEquippableIITAManager eqpIITAM, IEquippableUIItem item, IEquipTool tool){

		}
		readonly IUIManager _uim;
		readonly IEquippableItemIconUIA _eqpIIUIA;
		readonly IUIImage _image;
		readonly IEquippableIITAManager _eqpIITAM;
		readonly IEquippableUIItem _eqpItem;
		readonly IEquipTool _tool;
		public IUIManager uim{get{return this._uim;}}
		public IUIAdaptor uia{get{return this._eqpIIUIA;}}
		public IUIImage image{get{return this._image;}}
		public IItemIconTransactionManager iiTAM{get{return this._eqpIITAM;}}
		public IUIItem item{get{return this._eqpItem;}}
		public IEquipTool tool{get{return this._tool;}}
	}
}
