using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolItemFilterEngine: ISwitchableStateEngine<IEquipToolItemFilter>{}
	public class EquipToolItemFilterEngine: AbsSwitchableStateEngine<IEquipToolItemFilter>, IEquipToolItemFilterEngine{}
	public interface IEquipToolItemFilter: ISwitchableState{
		void FilterItems(List<IEquippableUIItem> items);
	}
	public interface IEquipToolNoneFilter: IEquipToolItemFilter{}
	public class EquipToolNoneFilter: IEquipToolNoneFilter{
		public void OnEnter(){}
		public void OnExit(){}
		public void FilterItems(List<IEquippableUIItem> items){}
	}
	public interface IEquipToolBowFilter: IEquipToolItemFilter{}
	public class EquipToolBowFilter: IEquipToolBowFilter{
		public void OnEnter(){}
		public void OnExit(){}
		public void FilterItems(List<IEquippableUIItem> items){}
	}
	public interface IEquipToolWearFilter: IEquipToolItemFilter{}
	public class EquipToolWearFilter: IEquipToolWearFilter{
		public void OnEnter(){}
		public void OnExit(){}
		public void FilterItems(List<IEquippableUIItem> items){}
	}
	public interface IEquipToolCarriedGearFilter: IEquipToolItemFilter{}
	public class EquipToolCarriedGearFilter: IEquipToolCarriedGearFilter{
		public void OnEnter(){}
		public void OnExit(){}
		public void FilterItems(List<IEquippableUIItem> items){}
	}
}

