using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIItem{
		/*  is this Scriptable Object?
		*/
		IItemTemplate GetItemTemplate();
		int GetQuantity();
		void SetQuantity(int q);
		bool IsSameAs(IUIItem other);
		int GetItemID();
	}
	public abstract class AbsUIItem: IUIItem{
		public AbsUIItem(IItemTemplate itemTemp, int quantity, int itemID){
			this.itemTemp = itemTemp;
			this.quantity = quantity;
			this.itemID = itemID;
		}
		readonly IItemTemplate itemTemp;
		readonly int itemID;/* item instance ID, as opposed to templateID */
		public int GetItemID(){
			return itemID;
		}
		int quantity;
		public IItemTemplate GetItemTemplate(){
			return itemTemp;
		}
		public int GetQuantity(){
			return quantity;
		}
		public void SetQuantity(int q){
			this.quantity = q;
		}
		public bool IsSameAs(IUIItem other){
			/*  Ghost is treated as same, sharing the same item instance with picked
			*/
			if(this.itemTemp.IsStackable())
				return this.itemTemp == other.GetItemTemplate();
			else/* non stackable */
				return this.itemID == other.GetItemID();
		}
	}
	public interface IEquippableUIItem: IUIItem, IEquipStateHandler{
		int GetMaxEquippableQuantity();
	}
	public class EquippableUIItem: AbsUIItem, IEquippableUIItem{
		public EquippableUIItem(IItemTemplate itemTemp, int quantity, int itemID, bool isEquipped): base(itemTemp, quantity, itemID){
			this.isEquipped = isEquipped;
		}
		public int GetMaxEquippableQuantity(){
			return GetItemTemplate().GetMaxEquippableQuantity();
		}
		bool isEquipped;
		public void Equip(){
			this.isEquipped = true;
		}
		public void Unequip(){
			this.isEquipped = false;
		}
		public bool IsEquipped(){
			return this.isEquipped;
		}
	}
}

