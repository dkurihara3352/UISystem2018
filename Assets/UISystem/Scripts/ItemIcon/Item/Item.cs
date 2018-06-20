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
		public AbsUIItem(IUIItemConstArg arg){
			thisItemTemp = arg.itemTemp;
			thisQuantity = arg.quantity;
			thisItemID = arg.itemID;
		}
		public int GetItemID(){
			return thisItemID;
		}
		readonly int thisItemID;/* item instance ID, as opposed to templateID */
		public IItemTemplate GetItemTemplate(){
			return thisItemTemp;
		}
		readonly IItemTemplate thisItemTemp;
		public int GetQuantity(){
			return thisQuantity;
		}
		public void SetQuantity(int q){
			thisQuantity = q;
		}
		int thisQuantity;
		public bool IsSameAs(IUIItem other){
			/*  Ghost is treated as same, sharing the same item instance with picked
			*/
			if(thisItemTemp.IsStackable())
				return thisItemTemp == other.GetItemTemplate();
			else/* non stackable */
				return thisItemID == other.GetItemID();
		}
		public bool IsStackable(){
			return thisItemTemp.IsStackable();
		}
	}
	public interface IEquippableUIItem: IUIItem, IEquipStateHandler{
		int GetMaxEquippableQuantity();
	}
	public class EquippableUIItem: AbsUIItem, IEquippableUIItem{
		public EquippableUIItem(IEquippableUIItemConstArg arg): base(arg){
			thisIsEquipped = arg.isEquipped;
		}
		public int GetMaxEquippableQuantity(){
			return GetItemTemplate().GetMaxEquippableQuantity();
		}
		public void Equip(){
			thisIsEquipped = true;
		}
		public void Unequip(){
			thisIsEquipped = false;
		}
		bool thisIsEquipped;
		public bool IsEquipped(){
			return thisIsEquipped;
		}
	}
	/* const */
	public interface IUIItemConstArg{
		IItemTemplate itemTemp{get;}
		int quantity{get;}
		int itemID{get;}
	}
	public interface IEquippableUIItemConstArg: IUIItemConstArg{
		bool isEquipped{get;}
	}
}

