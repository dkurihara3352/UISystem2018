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
	}
	public class EquippableItemIcon: AbsItemIcon, IEquippableItemIcon{
		public EquippableItemIcon(IUIManager uim, IUIAdaptor uia, IUIImage image, IEquippableUIItem eqpItem, IEquippableIITAManager eqpIITAM, IEquipTool tool) :base(uim, uia, image, eqpItem, eqpIITAM){
			this.eqpTool = tool;
		}
		readonly IEquipTool eqpTool;
		IEquippableIITAManager eqpIITAM{
			get{
				return (IEquippableIITAManager)this.iiTAM;
			}
		}
		IEquippableUIItem eqpItem{
			get{
				return this.item as IEquippableUIItem;
			}
		}
		int GetMaxEquippableQuantity(){
			return this.eqpItem.GetMaxEquippableQuantity();
		}
		protected override int GetMaxTransferableQuantity(){
			IItemTemplate thisItemTemp = this.eqpItem.GetItemTemplate();
			int thisQuantity = this.GetQuantity();
			if(thisItemTemp is IBowTemplate || thisItemTemp is IWearTemplate){
				if(thisQuantity != 0)
					return 1;
				else
					return 0;
			}else{/* itemTemp is CGearsTemplate */
				if(thisItemTemp is INonStackableItemTemplate)
					return thisQuantity;
				else{
					IIconGroup relevantEqpCGIG = eqpIITAM.GetRelevantEqpCGearsIG();
					int itemQuantityInEqpCGIG = relevantEqpCGIG.GetItemQuantity(thisItemTemp);
					return this.GetMaxEquippableQuantity() - itemQuantityInEqpCGIG;
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
		/*  */
		public bool IsInEqpIG(){
			return this.iconGroup is IEqpToolEqpIG<IItemTemplate>;
		}
		public override bool LeavesGhost(){
			return !this.IsInEqpIG();
		}
	}
}
