using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UISystem{
	public interface IPlayerCharacterConfigurationTool{

	}
	public interface IEquipTool: IPlayerCharacterConfigurationTool{
		void ResetMode();
		void TrySwitchItemMode(IItemTemplate itemTemp);
		void TrySwitchItemFilter(IItemTemplate itemTemp);
	}
}

