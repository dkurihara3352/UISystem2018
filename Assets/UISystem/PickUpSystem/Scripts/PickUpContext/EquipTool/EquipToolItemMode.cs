using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DKUtility;

namespace UISystem.PickUpUISystem{
	public interface IEquipToolItemModeEngine: ISwitchableStateEngine<IEquipToolItemMode>{}
	public class EquipToolItemModeEngine: AbsSwitchableStateEngine<IEquipToolItemMode>, IEquipToolItemModeEngine{

	}
	public interface IEquipToolItemMode: ISwitchableState{}
	public interface IEquipToolAllItemMode: IEquipToolItemMode{}
	public class EquipToolAllItemMode: IEquipToolAllItemMode{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IEquipToolBowMode: IEquipToolItemMode{}
	public class EquipToolBowMode: IEquipToolBowMode{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IEquipToolWearMode: IEquipToolItemMode{}
	public class EquipToolWearMode: IEquipToolWearMode{
		public void OnEnter(){}
		public void OnExit(){}
	}
	public interface IEquipToolCarriedGearMode: IEquipToolItemMode{}
	public class EquipToolCarriedGearMode: IEquipToolCarriedGearMode{
		public void OnEnter(){}
		public void OnExit(){}
	}	
}
