using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolIGHandler{
		IEquipToolPoolIG GetRelevantPoolIG();
		IEquipToolEquipIG GetRelevantEquipIG(IEquippableItemIcon pickedEqpII);
		IEquipToolEquippedBowIG GetRelevantEquippedBowIG();
		IEquipToolEquippedWearIG GetRelevantEquippedWearIG();
		IEquipToolEquippedCarriedGearsIG GetRelevantEquippedCarriedGearsIG();
		List<IEquipToolEquipIG> GetAllRelevantEquipIGs();
	}
	public interface IEquipToolIGManager: IEquipToolIGHandler{}
	public class EquipToolIGManager: IEquipToolIGManager{
		public virtual IEquipToolPoolIG GetRelevantPoolIG(){return null;}
		public IEquipToolEquipIG GetRelevantEquipIG(IEquippableItemIcon pickedEqpII){
			IItemTemplate itemTemp = pickedEqpII.GetItemTemplate();
			if(itemTemp is IBowTemplate)
				return GetRelevantEquippedBowIG();
			else if(itemTemp is IWearTemplate)
				return GetRelevantEquippedWearIG();
			else
				return GetRelevantEquippedCarriedGearsIG();
		}
		public virtual IEquipToolEquippedBowIG GetRelevantEquippedBowIG(){return null;}
		public virtual IEquipToolEquippedWearIG GetRelevantEquippedWearIG(){return null;}
		public virtual IEquipToolEquippedCarriedGearsIG GetRelevantEquippedCarriedGearsIG(){return null;}
		public List<IEquipToolEquipIG> GetAllRelevantEquipIGs(){
			List<IEquipToolEquipIG> result = new List<IEquipToolEquipIG>();
			result.Add(GetRelevantEquippedBowIG());
			result.Add(GetRelevantEquippedWearIG());
			result.Add(GetRelevantEquippedCarriedGearsIG());
			return result;
		}
	}
}
