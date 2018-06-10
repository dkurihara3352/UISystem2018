using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IUIItem{
		IItemTemplate GetItemTemplate();
		int GetQuantity();
		void SetQuantity(int q);
	}
	public interface IEquippableUIItem: IUIItem, IEquipStateHandler{
		int GetMaxEquippableQuantity();
	}
	public interface IItemTemplate{
		bool IsSameAs(IItemTemplate other);
		int GetPickUpStepQuantity();
	}
	public interface INonStackableItemTemplate: IItemTemplate{}
	public interface IStackableItemTemplate: IItemTemplate{}
	public interface IBowTemplate: INonStackableItemTemplate{}
	public interface IWearTemplate: INonStackableItemTemplate{}
	public interface ICarriedGearTemplate: IItemTemplate{}
}

