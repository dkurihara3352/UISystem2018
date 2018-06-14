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
}
