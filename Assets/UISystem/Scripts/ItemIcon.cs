using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IItemIcon: IPickableUIElement{
	}
	public class ItemIcon : AbsUIElement, IItemIcon {
		public ItemIcon(IUIManager uim, IUIAdaptor uia): base(uim, uia){

		}
		public override IUIImage CreateUIImage(){
			return null;
		}
		public void PickUp(){}
		public void CheckForPickUp(){}
	}
	public interface IEquippableUIE: IUIElement{
		void Equip();
		void Unequip();
		bool IsEquipped();
	}
	public interface IEquippableItemIcon: IItemIcon, IEquippableUIE{}
	
}
