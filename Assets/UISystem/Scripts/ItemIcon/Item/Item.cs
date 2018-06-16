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
		bool IsStackable();
	}
	public abstract class AbsUIItem: IUIItem{
		public AbsUIItem(IItemTemplate itemTemp, int quantity, int itemID){
			this.itemTemp = itemTemp;
			this.quantity = quantity;
			this.itemID = itemID;
		}
		public int GetItemID(){
			return itemID;
		}
		readonly int itemID;/* item instance ID, as opposed to templateID */
		public IItemTemplate GetItemTemplate(){
			return itemTemp;
		}
		readonly IItemTemplate itemTemp;
		public int GetQuantity(){
			return quantity;
		}
		public void SetQuantity(int q){
			this.quantity = q;
		}
		int quantity;
		public bool IsSameAs(IUIItem other){
			/*  Ghost is treated as same, sharing the same item instance with picked
			*/
			if(this.itemTemp.IsStackable())
				return this.itemTemp == other.GetItemTemplate();
			else/* non stackable */
				return this.itemID == other.GetItemID();
		}
		public bool IsStackable(){
			return this.itemTemp.IsStackable();
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
		public void Equip(){
			this.isEquipped = true;
		}
		public void Unequip(){
			this.isEquipped = false;
		}
		bool isEquipped;
		public bool IsEquipped(){
			return this.isEquipped;
		}
	}
}

